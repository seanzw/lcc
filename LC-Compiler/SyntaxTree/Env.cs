using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.SyntaxTree {

    public enum ScopeKind {
        BLOCK,
        FUNC,
        FILE,
        PARAM,
    }

    public abstract class SymbolEntry {
        public enum Kind {
            PARAMETER,
            OBJECT,
            TYPEDEF,
            FUNCTION
        }
        public readonly Kind kind;
        public readonly T type;
        public SymbolEntry(Kind kind, T type) {
            this.kind = kind;
            this.type = type;
        }
        public abstract Position Pos { get; }
    }

    public sealed class ObjEntry : SymbolEntry {
        public enum Store {
            NONE,
            AUTO,
            EXTERN,
            STATIC,
            REGISTER,
        }
        public readonly Declaration declaration;
        public readonly Store store;
        public ObjEntry(T type, Declaration declaration, Store store) : base(Kind.OBJECT, type) {
            this.declaration = declaration;
            this.store = store;
        }
        public override Position Pos => declaration.Pos;
    }

    public sealed class ParamEntry : SymbolEntry {
        public readonly Param declaration;
        public ParamEntry(T type, Param declaration) : base(Kind.PARAMETER, type) {
            this.declaration = declaration;
        }
        public override Position Pos => declaration.Pos;
    }

    public sealed class TypeEntry : SymbolEntry {
        public readonly Declaration declaration;
        public TypeEntry(T type, Declaration declaration) : base(Kind.TYPEDEF, type) {
            this.declaration = declaration;
        }
        public override Position Pos => declaration.Pos;
    }

    public sealed class FuncEntry : SymbolEntry {
        public readonly Declaration declaration;
        public FuncEntry(T type, Declaration declaration) : base(Kind.FUNCTION, type) {
            this.declaration = declaration;
        }
        public override Position Pos => declaration.Pos;
    }

    public struct TagEntry {
        public TUnqualified type;
        public TypeUserSpec node;
        public TagEntry(TUnqualified type, TypeUserSpec node) {
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
                symbols = new Dictionary<string, SymbolEntry>();
                tags = new Dictionary<string, TagEntry>();
            }
            public void AddSymbol(string symbol, SymbolEntry signature) {
                symbols.Add(symbol, signature);
            }
            public void AddTag(string tag, TagEntry signature) {
                tags.Add(tag, signature);
            }

            /// <summary>
            /// Get the information of a symbol, null if undeclared.
            /// </summary>
            /// <param name="symbol"></param>
            /// <returns></returns>
            public SymbolEntry GetSymbol(string symbol) {
                if (symbols.ContainsKey(symbol)) return symbols[symbol];
                else return null;
            }

            /// <summary>
            /// Get the information of a tag, null if undeclaraed.
            /// </summary>
            /// <param name="tag"></param>
            /// <returns></returns>
            public TagEntry? GetTag(string tag) {
                if (tags.ContainsKey(tag)) return tags[tag];
                else return null;
            }

            public readonly ScopeKind kind;
            private readonly Dictionary<string, SymbolEntry> symbols;
            private readonly Dictionary<string, TagEntry> tags;
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
        public void AddObj(string symbol, T type, Declaration declaration, ObjEntry.Store store) {
            scopes.Peek().AddSymbol(symbol, new ObjEntry(type, declaration, store));
        }

        public void AddParam(string symbol, T type, Param declaration) {
            scopes.Peek().AddSymbol(symbol, new ParamEntry(type, declaration));
        }

        public void AddTypeDef(string symbol, T type, Declaration declaration) {
            scopes.Peek().AddSymbol(symbol, new TypeEntry(type, declaration));
        }

        public void AddFunc(string symbol, T type, Declaration declaration) {
            scopes.Peek().AddSymbol(symbol, new FuncEntry(type, declaration));
        }

        /// <summary>
        /// Add a tag to the current scope.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="t"></param>
        /// <param name="definition"></param>
        public TagEntry AddTag(string tag, TUnqualified type, TypeUserSpec node) {
            var entry = new TagEntry(type, node);
            scopes.Peek().AddTag(tag, entry);
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
        public TagEntry? GetTag(string tag, bool here = false) {
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

        public bool IsFuncParam;
        public bool IsFuncDef;

        private Stack<Scope> scopes;
    }
}
