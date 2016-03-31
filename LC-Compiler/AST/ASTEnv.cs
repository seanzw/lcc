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

        /// <summary>
        /// Signature records the type of a symbol and where it is declared.
        /// </summary>
        public struct Signature {
            public T type;
            public readonly int line;
            public Signature(T type, int line) {
                this.type = type;
                this.line = line;
            }
        }

        /// <summary>
        /// Scope contains two dictionary.
        /// One for symbols, another for tags.
        /// </summary>
        private sealed class Scope {
            public Scope() {
                symbols = new Dictionary<string, Signature>();
                tags = new Dictionary<string, T>();
            }
            public readonly Dictionary<string, Signature> symbols;
            public readonly Dictionary<string, T> tags;
        }

        /// <summary>
        /// Initialize the environment with the global scope.
        /// </summary>
        public ASTEnv() {
            scopes = new Stack<Scope>();
            PushScope();
        }

        /// <summary>
        /// Push a nested scope into the environment.
        /// </summary>
        public void PushScope() {
            scopes.Push(new Scope());
        }

        /// <summary>
        /// Exit this scope.
        /// </summary>
        public void PopScope() {
            scopes.Pop();
        }

        /// <summary>
        /// Check if this symbol is defined.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool ContainsSymbol(string symbol) {
            foreach (var scope in scopes) {
                if (scope.symbols.ContainsKey(symbol)) return true;
            }
            return false;
        }

        /// <summary>
        /// Check if this symbol is defined in the current scope.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool ContainsSymbolInCurrentScope(string symbol) {
            return scopes.Peek().symbols.ContainsKey(symbol);
        }

        /// <summary>
        /// Get the declaration for this symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public int GetDeclaration(string symbol) {
            foreach (var scope in scopes) {
                if (scope.symbols.ContainsKey(symbol)) {
                    return scope.symbols[symbol].line;
                }
            }
            return -1;
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
        public void AddSymbol(string symbol, T type, int line) {
            scopes.Peek().symbols.Add(symbol, new Signature(type, line));
        }

        /// <summary>
        /// Get the symbol from the environment.
        /// Return null if this symbol doesn't exist.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Signature? GetSymbol(string symbol) {
            foreach (var scope in scopes) {
                if (scope.symbols.ContainsKey(symbol)) return scope.symbols[symbol];
            }
            return null;
        }

        /// <summary>
        /// Get the type of the symbol.
        /// Return null if the symbol is undefined.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public T GetType(string symbol) {
            foreach (var scope in scopes) {
                if (scope.symbols.ContainsKey(symbol)) return scope.symbols[symbol].type;
            }
            return null;
        }

        private Stack<Scope> scopes;
    }
}
