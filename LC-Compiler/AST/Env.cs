using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {

    public struct Entry {
        public readonly string obj;
        public readonly T t;
        public Entry(string obj, T t) {
            this.obj = obj;
            this.t = t;
        }
    }

    public sealed class Env {

        private sealed class Scope {
            public readonly LinkedList<Entry> entries;
            public Scope() {
                entries = new LinkedList<Entry>();
            }
            public Scope(Scope s) {
                entries = new LinkedList<Entry>(s.entries);
            }
            public void AddObj(Entry entry) {
                entries.AddLast(entry);
            }
            public Entry? GetObj(string obj) {
                foreach (var entry in entries) {
                    if (entry.obj == obj) {
                        return entry;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Env() {
            scopes = new Stack<Scope>();
            PushScope();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="e"></param>
        public Env(Env e) {
            scopes = new Stack<Scope>(from scope in e.scopes select new Scope(scope));
        }

        public void PushScope() {
            scopes.Push(new Scope());
        }

        public void PopScope() {
            scopes.Pop();
        }

        public static Entry AddStaticObj(string obj, T t) {
            Entry entry = new Entry(string.Format("{0}_{1}", obj, static_id++), t);
            statics.AddLast(entry);
            return entry;
        }

        public static Entry? GetStaticObj(string obj) {
            foreach (var entry in statics) {
                if (entry.obj == obj) {
                    return entry;
                }
            }
            return null;
        }

        public Entry AddDynamicObj(string obj, T t) {
            Entry entry = new Entry(obj, t);
            scopes.Peek().AddObj(entry);
            return entry;
        }

        public Entry? GetDynamicObj(string obj, bool here = false) {
            if (here) return scopes.Peek().GetObj(obj);
            else {
                foreach (var scope in scopes) {
                    var entry = scope.GetObj(obj);
                    if (entry != null)
                        return entry;
                }
                return null;
            }
        }

        private readonly Stack<Scope> scopes;

        /// <summary>
        /// Static objects are global visible in AST, so use static.
        /// </summary>
        private static LinkedList<Entry> statics = new LinkedList<Entry>();

        /// <summary>
        /// Internal used to index all the static objects.
        /// </summary>
        private static int static_id = 0;
    }
}
