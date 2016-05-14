using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.SyntaxTree {

    public enum ScopeKind {
        BLOCK,
        FUNCTION,
        FILE,
        PROTOTYPE,
    }

    public abstract class SymbolEntry {
        public enum Kind {
            PARAMETER,
            OBJECT,
            TYPEDEF,
            FUNCTION,
            MEMBER
        }
        public enum Link {
            NONE,
            INTERNAL,
            EXTERNAL
        }
        public readonly string symbol;
        public readonly Kind kind;
        public readonly T type;
        public readonly Link link;
        public SymbolEntry(string symbol, Kind kind, T type, Link link) {
            this.symbol = symbol;
            this.kind = kind;
            this.type = type;
            this.link = link;
        }
        public abstract Position Pos { get; }
    }

    public sealed class ObjEntry : SymbolEntry {
        public enum Storage {
            AUTO,
            EXTERNAL,
            STATIC,
            REGISTER,
        }
        public readonly Declaration declaration;
        public readonly Storage storage;
        public ObjEntry(string symbol, T type, Declaration declaration, Link link, Storage storage)
            : base(symbol, Kind.OBJECT, type, link) {
            this.declaration = declaration;
            this.storage = storage;
        }
        public override string ToString() {
            return string.Format("{0,-12} {1,-20} {2,-10} {3,-10}\n", symbol, type, link, storage);
        }
        public override Position Pos => declaration.Pos;
    }

    public sealed class MemEntry : SymbolEntry {
        public readonly StructDeclarator declarator;
        public MemEntry(string symbol, T type, StructDeclarator declarator)
            : base(symbol, Kind.MEMBER, type, Link.NONE) {
            this.declarator = declarator;
        }
        public override string ToString() {
            return string.Format("{0,-12} {1,-20} {2,-10}\n", symbol, type, link);
        }
        public override Position Pos => declarator.Pos;
    }

    /// <summary>
    /// Represent a parameter.
    /// An identifier declared to be a function parameter has no linkage.
    /// </summary>
    public sealed class ParamEntry : SymbolEntry {
        public readonly Param declaration;
        public ParamEntry(string symbol, T type, Param declaration)
            : base(symbol, Kind.PARAMETER, type, Link.NONE) {
            this.declaration = declaration;
        }
        public override string ToString() {
            return string.Format("{0,-12} {1,-20}\n", symbol, type);
        }
        public override Position Pos => declaration.Pos;
    }

    /// <summary>
    /// An identifier declared to be anything other than an object or a function has no linkage.
    /// </summary>
    public sealed class TypeEntry : SymbolEntry {
        public readonly Declaration declaration;
        public TypeEntry(string symbol, T type, Declaration declaration)
            : base(symbol, Kind.TYPEDEF, type, Link.NONE) {
            this.declaration = declaration;
        }
        public override string ToString() {
            return string.Format("{0,-12} {1,-20}\n", symbol, type);
        }
        public override Position Pos => declaration.Pos;
    }

    public sealed class FuncEntry : SymbolEntry {
        public readonly Declaration declaration;
        public FuncEntry(string symbol, T type, Declaration declaration, Link link)
            : base(symbol, Kind.FUNCTION, type, link) {
            if (link == Link.NONE) throw new ArgumentException("Linkage for a function can only be internal or external.");
            this.declaration = declaration;
        }
        public override string ToString() {
            return string.Format("{0,-12} {1,-20} {2,-10}\n", symbol, type, link);
        }
        public override Position Pos => declaration.Pos;
    }

    public class TagEntry {
        public readonly string tag;
        public readonly TUnqualified type;
        public TypeUserSpec node;
        public TagEntry(string tag, TUnqualified type, TypeUserSpec node) {
            this.tag = tag;
            this.type = type;
            this.node = node;
        }
    }

    /// <summary>
    /// Environment used for semantic analysis.
    /// </summary>
    public sealed class Env {

        /// <summary>
        /// Scope contains two dictionary.
        /// One for symbols, another for tags.
        /// </summary>
        private sealed class Scope {
            public Scope(ScopeKind kind) {
                this.kind = kind;
                symbols = new LinkedList<SymbolEntry>();
                tags = new LinkedList<TagEntry>();
            }
            public void AddSymbol(SymbolEntry entry) {
                symbols.AddLast(entry);
            }
            public void AddTag(TagEntry signature) {
                tags.AddLast(signature);
            }

            /// <summary>
            /// Get the information of a symbol, null if undeclared.
            /// </summary>
            /// <param name="symbol"></param>
            /// <returns></returns>
            public SymbolEntry GetSymbol(string symbol) {
                foreach (var s in symbols)
                    if (s.symbol == symbol) return s;
                return null;
            }

            /// <summary>
            /// Get the information of a tag, null if undeclaraed.
            /// </summary>
            /// <param name="tag"></param>
            /// <returns></returns>
            public TagEntry GetTag(string tag) {
                foreach (var t in tags)
                    if (t.tag == tag) return t;
                return null;
            }

            public void Dump(int tab, StringBuilder builder) {
                StringBuilder sb = new StringBuilder();
                string tt = "";
                while (tab > 0) { tt += "  "; tab--; }
                Action<SymbolEntry.Kind> dumpSymbol = (kind) => {
                    sb.Append(tt + kind.ToString() + "\n");
                    foreach (var s in symbols)
                        if (s.kind == kind)
                            sb.Append(tt + s.ToString());
                };
                dumpSymbol(SymbolEntry.Kind.TYPEDEF);
                dumpSymbol(SymbolEntry.Kind.FUNCTION);
                dumpSymbol(SymbolEntry.Kind.PARAMETER);
                dumpSymbol(SymbolEntry.Kind.OBJECT);
                dumpSymbol(SymbolEntry.Kind.MEMBER);
                builder.Insert(0, sb.ToString());
            }

            public readonly ScopeKind kind;
            private readonly LinkedList<SymbolEntry> symbols;
            private readonly LinkedList<TagEntry> tags;
        }

        /// <summary>
        /// Initialize the environment with the file scope.
        /// </summary>
        public Env() {
            scopes = new Stack<Scope>();
            PushScope(ScopeKind.FILE);
            IsFuncDef = false;
            IsFuncParam = false;
        }

        /// <summary>
        /// Push a nested scope into the environment.
        /// </summary>
        public void PushScope(ScopeKind kind) {
            scopes.Push(new Scope(kind));
        }

        /// <summary>
        /// Exit this scope.
        /// </summary>
        public void PopScope() {
            scopes.Pop();
        }

        /// <summary>
        /// Add an object symbol to the current scope.
        /// Note: 
        ///     This method won't check if the symbol is redefined,
        ///     although in this situation Add() will throw an exception.
        ///     Caller should call ContainsSymbolInCurrentScope() first.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="declaration"></param>
        public void AddObj(string symbol, T type, SymbolEntry.Link link, ObjEntry.Storage storage, Declaration declaration) {
            scopes.Peek().AddSymbol(new ObjEntry(symbol, type, declaration, link, storage));
        }

        public void AddMem(string symbol, T type, StructDeclarator declarator) {
            scopes.Peek().AddSymbol(new MemEntry(symbol, type, declarator));
        }

        public void AddParam(string symbol, T type, Param declaration) {
            scopes.Peek().AddSymbol(new ParamEntry(symbol, type, declaration));
        }

        public void AddTypeDef(string symbol, T type, Declaration declaration) {
            scopes.Peek().AddSymbol(new TypeEntry(symbol, type, declaration));
        }

        public void AddFunc(string symbol, T type, SymbolEntry.Link link, Declaration declaration) {
            scopes.Peek().AddSymbol(new FuncEntry(symbol, type, declaration, link));
        }

        /// <summary>
        /// Add a tag to the current scope.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="t"></param>
        /// <param name="definition"></param>
        public TagEntry AddTag(string tag, TUnqualified type, TypeUserSpec node) {
            var entry = new TagEntry(tag, type, node);
            scopes.Peek().AddTag(entry);
            return entry;
        }

        /// <summary>
        /// Find the information of a symbol.
        /// </summary>
        /// <param name="symbol"> The name of the symbol. </param>
        /// <param name="here"> Restrict the search area to the current scope. </param>
        /// <returns></returns>
        public SymbolEntry GetSymbol(string symbol, bool here = false) {
            if (here) return scopes.Peek().GetSymbol(symbol);
            else {
                foreach (var scope in scopes) {
                    var t = scope.GetSymbol(symbol);
                    if (t != null) return t;
                }
                return null;
            }
        }

        /// <summary>
        /// Get the information of the tag.
        /// </summary>
        /// <param name="tag"> Name of the tag. </param>
        /// <param name="here"> Retrict the search area to the current scope. </param>
        /// <returns></returns>
        public TagEntry GetTag(string tag, bool here = false) {
            if (here) return scopes.Peek().GetTag(tag);
            else {
                foreach (var scope in scopes) {
                    var t = scope.GetTag(tag);
                    if (t != null) return t;
                }
                return null;
            }
        }

        /// <summary>
        /// Get the current scope kind.
        /// </summary>
        public ScopeKind WhatScope => scopes.Peek().kind;

        /// <summary>
        /// Dump the environment.
        /// </summary>
        /// <returns></returns>
        public string Dump() {
            int n = scopes.Count - 1;
            StringBuilder builder = new StringBuilder();
            foreach (var scope in scopes) {
                scope.Dump(n--, builder);
            }
            return builder.ToString();
        }

        public bool IsFuncParam;
        public bool IsFuncDef;

        private Stack<Scope> scopes;
    }
}
