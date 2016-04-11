using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.SyntaxTree;

namespace lcc.Parser {
    /// <summary>
    /// Tiny environment to sovle typedef / variable ambiguity during parsing.
    /// </summary>
    public static class Env {

        private sealed class Scope {

            public Scope() {
                typedefs = new HashSet<string>();
            }

            public bool IsTypedefName(string name) {
                return typedefs.Contains(name);
            }

            public void AddTypedefName(string name) {
                typedefs.Add(name);
            }

            private HashSet<string> typedefs;
        }

        /// <summary>
        /// Static initializer.
        /// </summary>
        static Env() {
            scopes = new Stack<Scope>();
            PushScope();
        }

        /// <summary>
        /// Push in a new scope.
        /// </summary>
        public static void PushScope() {
            scopes.Push(new Scope());
        }

        /// <summary>
        /// Pop the top scope.
        /// </summary>
        public static void PopScope() {
            scopes.Pop();
        }

        /// <summary>
        /// Check if this is a typedef name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsTypedefName(string name) {
            foreach (var scope in scopes) 
                if (scope.IsTypedefName(name)) return true;
            return false;
        }

        /// <summary>
        /// Check if the typedef name has already been defined in current scope.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsRefefined(string name) {
            return scopes.Peek().IsTypedefName(name);
        }

        /// <summary>
        /// Add the typedef name to the current scope.
        /// Throw exception if the typedef name has already been defined in the current scope.
        /// </summary>
        /// <param name="name"></param>
        public static void AddTypedefName(int line, string name) {
            if (scopes.Peek().IsTypedefName(name)) throw new TypedefRedefined(line, name);
            else scopes.Peek().AddTypedefName(name);
        }

        private static Stack<Scope> scopes;
    }
}
