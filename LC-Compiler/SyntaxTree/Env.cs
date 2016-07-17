using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.SyntaxTree {

    public enum ScopeKind {
        BLOCK,
        STRUCT,
        FUNC,
        FILE,
        PARAM,
    }

    public abstract class SymbolEntry {
        public enum Kind {
            PARAM,
            OBJ,
            TYPEDEF,
            FUNC,
            FIELD,
            ENUM
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

    public sealed class EObj : SymbolEntry {
        public enum Storage {
            AUTO,
            EXTERNAL,
            STATIC,
            REGISTER,
        }
        public readonly string uid;
        public readonly Position pos;
        public readonly Storage storage;
        public EObj(string uid, string symbol, T type, Position pos, Link link, Storage storage)
            : base(symbol, Kind.OBJ, type, link) {
            this.uid = uid;
            this.pos = pos;
            this.storage = storage;
        }
        public override string ToString() {
            return string.Format("{0,-12} {1,-20} {2,-10} {3,-10}\n", symbol, type, link, storage);
        }
        public override Position Pos => pos;
    }

    public sealed class EField : SymbolEntry {
        public readonly StructDeclarator declarator;
        public EField(string symbol, T type, StructDeclarator declarator)
            : base(symbol, Kind.FIELD, type, Link.NONE) {
            this.declarator = declarator;
        }
        public override string ToString() {
            return string.Format("{0,-12} {1,-20}\n", symbol, type);
        }
        public override Position Pos => declarator.Pos;
    }

    public sealed class EEnum : SymbolEntry {
        public readonly Enum declarator;
        public readonly AST.ConstIntExpr expr;
        public EEnum(string symbol, TEnum type, AST.ConstIntExpr expr, Enum declarator)
            : base(symbol, Kind.ENUM, type.Const(), Link.NONE) {
            this.expr = expr;
            this.declarator = declarator;
        }
        public override string ToString() {
            return string.Format("{0,-12} {1,-20} {2, -20}\n", symbol, type, expr.value);
        }
        public override Position Pos => declarator.Pos;
    }

    /// <summary>
    /// Represent a parameter.
    /// An identifier declared to be a function parameter has no linkage.
    /// </summary>
    public sealed class EParam : SymbolEntry {
        public readonly Param declaration;
        public EParam(string symbol, T type, Param declaration)
            : base(symbol, Kind.PARAM, type, Link.NONE) {
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
    public sealed class ETypeDef : SymbolEntry {
        public readonly Declaration declaration;
        public ETypeDef(string symbol, T type, Declaration declaration)
            : base(symbol, Kind.TYPEDEF, type, Link.NONE) {
            this.declaration = declaration;
        }
        public override string ToString() {
            return string.Format("{0,-12} {1,-20}\n", symbol, type);
        }
        public override Position Pos => declaration.Pos;
    }

    public sealed class EFunc : SymbolEntry {
        public readonly Position pos;
        public EFunc(string symbol, T type, Position pos, Link link)
            : base(symbol, Kind.FUNC, type, link) {
            if (link == Link.NONE) throw new ArgumentException("Linkage for a function can only be internal or external.");
            this.pos = pos;
        }
        public override string ToString() {
            return string.Format("{0,-12} {1,-20} {2,-10}\n", symbol, type, link);
        }
        public override Position Pos => pos;
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
        private class Scope {
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

            public bool HasVARR() {
                foreach (var s in symbols) 
                    if (s.kind == SymbolEntry.Kind.OBJ && s.type.Kind == TKind.VARR) return true;
                return false;
            }

            #region Interface for function scope.

            public virtual TFunc FuncType {
                get {
                    throw new InvalidOperationException("can not get function type in non-function scope");
                }
            }

            /// <summary>
            /// Add a label, return the transformed label.
            /// </summary>
            /// <param name="label"></param>
            /// <returns></returns>
            public virtual string AddLabel(string label, Scope scope) {
                throw new InvalidOperationException("can not add label in non-function scope");
            }

            /// <summary>
            /// Get the transformed label. Returns null if the label is undefined.
            /// </summary>
            /// <param name="label"></param>
            /// <returns></returns>
            public virtual string GetLabel(string label) {
                throw new InvalidOperationException("can not get label in non-function scope");
            }

            /// <summary>
            /// Is this label defined within a scope that has VLA?
            /// </summary>
            /// <param name="label"></param>
            /// <returns></returns>
            public virtual bool IsLabelWithVARR(string label) {
                throw new InvalidOperationException("can not get label in non-function scope");
            }

            #endregion

            public void Dump(int tab, StringBuilder builder) {
                StringBuilder sb = new StringBuilder();
                string tt = "";
                while (tab > 0) { tt += "  "; tab--; }
                sb.Append(tt + "ScopeKind: " + kind.ToString() + "\n");
                Action<SymbolEntry.Kind> dumpSymbol = (kind) => {
                    sb.Append(tt + kind.ToString() + "\n");
                    foreach (var s in symbols)
                        if (s.kind == kind)
                            sb.Append(tt + s.ToString());
                };
                dumpSymbol(SymbolEntry.Kind.TYPEDEF);
                dumpSymbol(SymbolEntry.Kind.FUNC);
                dumpSymbol(SymbolEntry.Kind.PARAM);
                dumpSymbol(SymbolEntry.Kind.OBJ);
                dumpSymbol(SymbolEntry.Kind.FIELD);
                dumpSymbol(SymbolEntry.Kind.ENUM);
                builder.Insert(0, sb.ToString());
            }

            public readonly ScopeKind kind;
            private readonly LinkedList<SymbolEntry> symbols;
            private readonly LinkedList<TagEntry> tags;
        }

        private sealed class FuncScope : Scope {
            public readonly string name;
            public readonly string returnLabel;
            public readonly TFunc type;
            public override TFunc FuncType => type;
            public FuncScope(string name, string returnLabel, TFunc type) : base(ScopeKind.FUNC) {
                labels = new Dictionary<string, Tuple<string, Scope>>();
                this.name = name;
                this.returnLabel = returnLabel;
                this.type = type;
            }

            public override string AddLabel(string label, Scope scope) {
                string transformed = string.Format("__{0}_{1}", label, cnt++);
                labels.Add(label, new Tuple<string, Scope>(transformed, scope));
                return transformed;
            }

            public override string GetLabel(string label) {
                if (labels.ContainsKey(label)) return labels[label].Item1;
                else return null;
            }

            public override bool IsLabelWithVARR(string label) {
                if (labels.ContainsKey(label)) return labels[label].Item2.HasVARR();
                else throw new ArgumentException(string.Format("there is no such label as {0}", label));
            }

            private static int cnt = 0;
            private readonly Dictionary<string, Tuple<string, Scope>> labels;
        }

        /// <summary>
        /// Initialize the environment with the file scope.
        /// </summary>
        public Env() {
            scopes = new Stack<Scope>();
            scopes.Push(new Scope(ScopeKind.FILE));
            IsFuncDef = false;
            IsFuncParam = false;
            ASTEnv = new AST.Env();
            loopId = 0;
            ifId = 0;
            switchId = 0;
            caseId = 0;
            defaultId = 0;
            staticId = 0;
            dynamicId = 0;
            breakables = new Stack<Breakable>();
        }

        /// <summary>
        /// Push a block scope.
        /// </summary>
        public void PushBlockScope() {
            scopes.Push(new Scope(ScopeKind.BLOCK));
            ASTEnv.PushBlock();
        }

        /// <summary>
        /// Push a struct or union scope.
        /// </summary>
        public void PushStructScope() {
            scopes.Push(new Scope(ScopeKind.STRUCT));
        }

        /// <summary>
        /// Push a scope used for the parameter.
        /// </summary>
        public void PushParamScope() {
            scopes.Push(new Scope(ScopeKind.PARAM));
        }

        /// <summary>
        /// Push a function scope
        /// </summary>
        /// <param name="type"></param>
        public void PushFuncScope(string name, TFunc type, IEnumerable<Tuple<string, T>> parameters, Position pos) {
            // Clear the dynamic id.
            dynamicId = 0;
            scopes.Push(new FuncScope(name, string.Format("__{0}_return", name), type));
            ASTEnv = new AST.Env();
            /// Add all the parameter to the environment.
            foreach (var p in parameters) {
                scopes.Peek().AddSymbol(new EObj(dynamicId.ToString(), p.Item1, p.Item2, pos, SymbolEntry.Link.NONE, EObj.Storage.AUTO));
                ASTEnv.AddParam(dynamicId.ToString(), p.Item1, p.Item2);
                dynamicId++;
            }
        }

        /// <summary>
        /// Exit this scope.
        /// </summary>
        public void PopScope() {
            if (scopes.Peek().kind == ScopeKind.BLOCK) {
                ASTEnv.PopBlock();
            }
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
        public void AddObj(string symbol, T type, SymbolEntry.Link link, EObj.Storage storage, Position pos) {
            if (storage == EObj.Storage.AUTO || storage == EObj.Storage.REGISTER) {
                // This is a dynamic object.
                // In this implementation, register and auto are the same.
                scopes.Peek().AddSymbol(new EObj(dynamicId.ToString(), symbol, type, pos, link, storage));
                ASTEnv.AddLocal(dynamicId.ToString(), symbol, type);
                dynamicId++;
            } else {
                string uid = string.Format("__static_{0}_{1}", symbol, staticId++);
                if (storage == EObj.Storage.STATIC) {
                    // This is a static object, check its linkage (either external or internal).
                    scopes.Peek().AddSymbol(new EObj(uid, symbol, type, pos, link, storage));
                    AST.Env.AddStaticObj(uid, symbol, type, link == SymbolEntry.Link.EXTERNAL, false);
                } else {
                    // This is an external object, the linkage must be external.
                    scopes.Peek().AddSymbol(new EObj(uid, symbol, type, pos, link, storage));
                    AST.Env.AddStaticObj(uid, symbol, type, true, true);
                }
            }
        }

        public void AddMem(string symbol, T type, StructDeclarator declarator) {
            scopes.Peek().AddSymbol(new EField(symbol, type, declarator));
        }

        public void AddEnum(string symbol, TEnum type, AST.ConstIntExpr expr, Enum declarator) {
            scopes.Peek().AddSymbol(new EEnum(symbol, type, expr, declarator));
        }

        /// <summary>
        /// Used to add parameter to the paraemeter scope in function declaration.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="declaration"></param>
        public void AddParam(string symbol, T type, Param declaration) {
            scopes.Peek().AddSymbol(new EParam(symbol, type, declaration));
        }

        public void AddTypeDef(string symbol, T type, Declaration declaration) {
            scopes.Peek().AddSymbol(new ETypeDef(symbol, type, declaration));
        }

        public void AddFunc(string symbol, T type, SymbolEntry.Link link, Position pos) {
            scopes.Peek().AddSymbol(new EFunc(symbol, type, pos, link));
        }

        public string AddLabel(string label) {
            foreach (var scope in scopes)
                if (scope.kind == ScopeKind.FUNC) return scope.AddLabel(label, scopes.Peek());
            throw new InvalidOperationException("there is no upper function scope");
        }

        public string GetLable(string label) {
            foreach (var scope in scopes)
                if (scope.kind == ScopeKind.FUNC) return scope.GetLabel(label);
            throw new InvalidOperationException("there is no upper function scope");
        }

        public bool IsLabelWithVARR(string label) {
            foreach (var scope in scopes) 
                if (scope.kind == ScopeKind.FUNC) return scope.IsLabelWithVARR(label);
            throw new InvalidOperationException("there is no upper function scope");
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
        /// Push a switch statement to the environment.
        /// </summary>
        /// <param name="s"></param>
        public void PushSwitch(Switch s) {
            s.breakLabel = string.Format("__switch_break_{0}", switchId++);
            breakables.Push(s);
        }

        /// <summary>
        /// Get the enclosing switch statement, null if there are no switch.
        /// </summary>
        /// <returns></returns>
        public Switch GetSwitch() {
            foreach (var b in breakables) {
                Switch s = b as Switch;
                if (s != null) return s;
            }
            return null;
        }

        /// <summary>
        /// Push a loop statement to the environment.
        /// </summary>
        /// <param name="l"></param>
        public void PushLoop(Loop l) {
            l.secondPlusLabel = string.Format("__loop_second_plus_{0}", loopId);
            l.firstLabel = string.Format("__loop_first_{0}", loopId);
            l.breakLabel = string.Format("__loop_break_{0}", loopId);
            l.continueLabel = string.Format("__loop_continure{0}", loopId++);
            breakables.Push(l);
        }

        /// <summary>
        /// Get the enclosing loop statement.
        /// </summary>
        /// <returns></returns>
        public Loop GetLoop() {
            foreach (var b in breakables) {
                Loop l = b as Loop;
                if (l != null) return l;
            }
            return null;
        }

        /// <summary>
        /// Get the enclosing breakable statement.
        /// </summary>
        /// <returns></returns>
        public Breakable GetBreakable() {
            return breakables.Peek();
        }

        /// <summary>
        /// Pop a breakable.
        /// </summary>
        public void PopBreakable() {
            breakables.Pop();
        }

        /// <summary>
        /// Get the type of the current function.
        /// </summary>
        /// <returns></returns>
        public TFunc GetFuncType() {
            foreach (var s in scopes) {
                if (s.kind == ScopeKind.FUNC) {
                    return (s as FuncScope).type;
                }
            }
            throw new InvalidOperationException("there is no function scope");
        }

        /// <summary>
        /// Get the return label.
        /// </summary>
        /// <returns></returns>
        public string GetReturnLabel() {
            foreach (var s in scopes) {
                if (s.kind == ScopeKind.FUNC) {
                    return (s as FuncScope).returnLabel;
                }
            }
            throw new InvalidOperationException("there is no function scope");
        }

        /// <summary>
        /// Allocate a case label.
        /// </summary>
        /// <returns></returns>
        public string AllocCaseLabel() {
            return string.Format("__case_{0}", caseId++);
        }

        /// <summary>
        /// Allocate a default label.
        /// </summary>
        /// <returns></returns>
        public string AllocDefaultLabel() {
            return string.Format("__default_{0}", defaultId++);
        }

        /// <summary>
        /// Allocate a if label.
        /// Item1: elseLabel;
        /// Item2: endIfLabel;
        /// </summary>
        /// <returns></returns>
        public Tuple<string, string> AllocIfLabel() {
            return new Tuple<string, string>(string.Format("__else_block_{0}", ifId), string.Format("__endif_{0}", ifId++));
        }

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

        public AST.Env ASTEnv {
            get;
            private set;
        }

        private int loopId;
        private int ifId;
        private int switchId;
        private int caseId;
        private int defaultId;
        private int staticId;
        private int dynamicId;

        private Stack<Breakable> breakables;
        private Stack<Scope> scopes;

    }
}
