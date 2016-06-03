using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {

    public abstract class Obj {
        public readonly string symbol;
        public readonly T type;
        public Obj(string symbol, T type) {
            this.symbol = symbol;
            this.type = type;
        }
    }

    public class StaticObj : Obj {
        public readonly string id;
        /// <summary>
        /// True if the linkage is external.
        /// </summary>
        public readonly bool linkage;
        /// <summary>
        /// True if the storage is external.
        /// </summary>
        public readonly bool storage;
        public StaticObj(string symbol, T type, string id, bool linkage, bool storage)
            : base(symbol, type) {
            this.id = id;
            this.linkage = linkage;
            this.storage = storage;
        }
    }

    public class DynamicObj : Obj {

        /// <summary>
        /// Offset to ebx reg.
        /// </summary>
        public readonly int ebx;

        public DynamicObj(string symbol, T type, int ebx)
            : base(symbol, type) {
            this.ebx = ebx;
        }
    }

    public enum ScopeKind {
        BLOCK,
        FUNCTION
    }

    public sealed class Env {

        private sealed class Scope {
            public ScopeKind kind;
            public bool isVarLength;
            public int ebx;
            public readonly LinkedList<DynamicObj> objs;

            public Scope(ScopeKind kind) {
                this.kind = kind;
                objs = new LinkedList<DynamicObj>();
                isVarLength = false;
                ebx = 0;
            }
            public void AddObj(string symbol, T type) {
                DynamicObj obj = new DynamicObj(symbol, type, ebx);
                objs.AddLast(obj);
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Env() {
            scopes = new Stack<Scope>();
        }

        /// <summary>
        /// Copy constructor.
        /// Only copy the stack.
        /// </summary>
        /// <param name="e"></param>
        public Env(Env e) {
            scopes = new Stack<Scope>(e.scopes);
        }

        public void PushScope() {
            if (scopes.Count == 0) {
                // This is the first scope, should be function scope.
                scopes.Push(new Scope(ScopeKind.FUNCTION));
            } else {
                // This is a block scope.
                scopes.Push(new Scope(ScopeKind.BLOCK));
            }
        }

        public void PopScope() {
            scopes.Pop();
        }

        /// <summary>
        /// Add a static object to the environment.
        /// </summary>
        /// <param name="symbol"> The name of the symbol. </param>
        /// <param name="type"> The type of the symbol. </param>
        /// <param name="linkage"> True if the linkage is external. </param>
        /// <param name="storage"> True if the storage is external. </param>
        public static void AddStaticObj(string symbol, T type, bool linkage, bool storage) {
            StaticObj obj = new StaticObj(symbol,
                type,
                string.Format("__{0}_{1}", symbol, static_id++),
                linkage,
                storage);
            statics.AddLast(obj);
        }

        public void AddDynamicObj(string symbol, T type) {
            scopes.Peek().AddObj(symbol, type);
        }

        private readonly Stack<Scope> scopes;

        /// <summary>
        /// Static objects are global visible in AST, so use static.
        /// </summary>
        private static LinkedList<StaticObj> statics = new LinkedList<StaticObj>();

        /// <summary>
        /// Internal used to index all the static objects.
        /// </summary>
        private static int static_id = 0;
    }
}
