using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {

    /// <summary>
    /// Environment used for semantic analysis.
    /// </summary>
    public sealed class ASTEnv {

        public enum ScopeKind {
            BLOCK,
            FUNC,
            FILE
        }

        /// <summary>
        /// Scope contains two dictionary.
        /// One for symbols, another for tags.
        /// </summary>
        private sealed class Scope {
            public Scope(ScopeKind kind) {
                this.kind = kind;
                symbols = new Dictionary<string, Tuple<T, ASTDeclaration>>();
                tags = new Dictionary<string, Tuple<TUnqualified, ASTTypeUserSpec>>();
                typedefs = new Dictionary<string, Tuple<T, ASTDeclaration>>();
            }
            public void AddSymbol(string symbol, T t, ASTDeclaration declaration) {
                symbols.Add(symbol, new Tuple<T, ASTDeclaration>(t, declaration));
            }
            public void AddTag(string tag, TUnqualified t, ASTTypeUserSpec definition) {
                tags.Add(tag, new Tuple<TUnqualified, ASTTypeUserSpec>(t, definition));
            }
            public void AddTypedef(string typedef, T t, ASTDeclaration declaration) {
                typedefs.Add(typedef, new Tuple<T, ASTDeclaration>(t, declaration));
            }

            /// <summary>
            /// Get the type of a symbol, null if undeclared.
            /// </summary>
            /// <param name="symbol"></param>
            /// <returns></returns>
            public T TSymbol(string symbol) {
                if (symbols.ContainsKey(symbol)) return symbols[symbol].Item1;
                else return null;
            }
            /// <summary>
            /// Get the declaration of a symbol, null if undeclaraed.
            /// </summary>
            /// <param name="symbol"></param>
            /// <returns></returns>
            public ASTDeclaration DSymbol(string symbol) {
                if (symbols.ContainsKey(symbol)) return symbols[symbol].Item2;
                else return null;
            }
            /// <summary>
            /// Get the unqualified type of the tag.
            /// </summary>
            /// <param name="tag"></param>
            /// <returns></returns>
            public TUnqualified TTag(string tag) {
                if (tags.ContainsKey(tag)) return tags[tag].Item1;
                else return null;
            }
            /// <summary>
            /// Get the definition of the tag.
            /// </summary>
            /// <param name="tag"></param>
            /// <returns></returns>
            public ASTTypeUserSpec DTag(string tag) {
                if (tags.ContainsKey(tag)) return tags[tag].Item2;
                else return null;
            }
            /// <summary>
            /// Get the type of a typedef name.
            /// </summary>
            /// <param name="typedef"></param>
            /// <returns></returns>
            public T TTypedef(string typedef) {
                if (typedefs.ContainsKey(typedef)) return typedefs[typedef].Item1;
                else return null;
            }
            /// <summary>
            /// Get the declaration of a typedef name.
            /// </summary>
            /// <param name="typedef"></param>
            /// <returns></returns>
            public ASTDeclaration DTypedef(string typedef) {
                if (typedefs.ContainsKey(typedef)) return typedefs[typedef].Item2;
                else return null;
            }
            public readonly ScopeKind kind;
            private readonly Dictionary<string, Tuple<T, ASTDeclaration>> symbols;
            private readonly Dictionary<string, Tuple<TUnqualified, ASTTypeUserSpec>> tags;
            private readonly Dictionary<string, Tuple<T, ASTDeclaration>> typedefs;
        }

        /// <summary>
        /// Initialize the environment with the file scope.
        /// </summary>
        public ASTEnv() {
            scopes = new Stack<Scope>();
            PushScope(ScopeKind.FILE);
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
        /// Add a symbol to the current scope.
        /// Note: 
        ///     This method won't check if the symbol is redefined,
        ///     although in this situation Add() will throw an exception.
        ///     Caller should call ContainsSymbolInCurrentScope() first.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="line"></param>
        public void AddSymbol(string symbol, T t, ASTDeclaration declaration) {
            scopes.Peek().AddSymbol(symbol, t, declaration);
        }
        /// <summary>
        /// Add a tag to the current scope.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="t"></param>
        /// <param name="definition"></param>
        public void AddTag(string tag, TUnqualified t, ASTTypeUserSpec definition) {
            scopes.Peek().AddTag(tag, t, definition);
        }
        /// <summary>
        /// Add a typdef name to the current scope.
        /// </summary>
        /// <param name="typedef"></param>
        /// <param name="t"></param>
        /// <param name="declaration"></param>
        public void AddTypedef(string typedef, T t, ASTDeclaration declaration) {
            scopes.Peek().AddTypedef(typedef, t, declaration);
        }

        /// <summary>
        /// Find the type of a symbol.
        /// </summary>
        /// <param name="symbol"> The name of the symbol. </param>
        /// <param name="here"> Restrict the search area to the current scope. </param>
        /// <returns></returns>
        public T TSymbol(string symbol, bool here = false) {
            if (here) return scopes.Peek().TSymbol(symbol);
            else {
                foreach (var scope in scopes) {
                    T t = scope.TSymbol(symbol);
                    if (t != null) return t;
                }
                return null;
            }
        }

        /// <summary>
        /// Find the declaration of a symbol.
        /// </summary>
        /// <param name="symbol"> The name of the symbol. </param>
        /// <param name="here"> Restrict the search area to the current scope. </param>
        /// <returns></returns>
        public ASTDeclaration DSymbol(string symbol, bool here = false) {
            if (here) return scopes.Peek().DSymbol(symbol);
            else {
                foreach (var scope in scopes) {
                    var d = scope.DSymbol(symbol);
                    if (d != null) return d;
                }
                return null;
            }
        }

        /// <summary>
        /// Get the unqualified type of the tag.
        /// </summary>
        /// <param name="tag"> Name of the tag. </param>
        /// <param name="here"> Retrict the search area to the current scope. </param>
        /// <returns></returns>
        public TUnqualified TTag(string tag, bool here = false) {
            if (here) return scopes.Peek().TTag(tag);
            else {
                foreach (var scope in scopes) {
                    var t = scope.TTag(tag);
                    if (t != null) return t;
                }
                return null;
            }
        }

        /// <summary>
        /// Find the definition of a tag.
        /// </summary>
        /// <param name="symbol"> The name of the tag. </param>
        /// <param name="here"> Restrict the search area to the current scope. </param>
        /// <returns></returns>
        public ASTTypeUserSpec DTag(string tag, bool here = false) {
            if (here) return scopes.Peek().DTag(tag);
            else {
                foreach (var scope in scopes) {
                    var d = scope.DTag(tag);
                    if (d != null) return d;
                }
                return null;
            }
        }

        /// <summary>
        /// Find the type of a typedef name.
        /// </summary>
        /// <param name="symbol"> The name of the typedef. </param>
        /// <param name="here"> Restrict the search area to the current scope. </param>
        /// <returns></returns>
        public T TTypedef(string typedef, bool here = false) {
            if (here) return scopes.Peek().TTypedef(typedef);
            else {
                foreach (var scope in scopes) {
                    T t = scope.TTypedef(typedef);
                    if (t != null) return t;
                }
                return null;
            }
        }

        /// <summary>
        /// Find the declaration of a typedef name.
        /// </summary>
        /// <param name="symbol"> The name of the typedef. </param>
        /// <param name="here"> Restrict the search area to the current scope. </param>
        /// <returns></returns>
        public ASTDeclaration DTypedef(string typedef, bool here = false) {
            if (here) return scopes.Peek().DTypedef(typedef);
            else {
                foreach (var scope in scopes) {
                    var d = scope.DTypedef(typedef);
                    if (d != null) return d;
                }
                return null;
            }
        }

        private Stack<Scope> scopes;
    }
}
