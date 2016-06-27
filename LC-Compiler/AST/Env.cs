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
        public readonly int ebp;

        public DynamicObj(string symbol, T type, int ebp)
            : base(symbol, type) {
            this.ebp = ebp;
        }
    }

    public enum ScopeKind {
        BLOCK,
        FUNCTION
    }

    public sealed class Env {

        private sealed class Frame {

            /// <summary>
            /// The size of the frame.
            /// </summary>
            public int size {
                get;
                private set;
            }

            public readonly LinkedList<DynamicObj> objs;

            public Frame() {
                objs = new LinkedList<DynamicObj>();
                blocks = new Stack<int>();
                preEBP = 8;
                offEBP = 0;
                size = 0;
            }
            public void AddLocal(string symbol, T type) {
                offEBP -= type.AlignByte;
                size = Math.Max(size, -offEBP);
                objs.AddLast(new DynamicObj(symbol, type, offEBP));
            }

            public void AddParam(string symbol, T type) {
                objs.AddFirst(new DynamicObj(symbol, type, preEBP));
                preEBP += type.AlignByte;
            }

            public void PushBlock() {
                blocks.Push(offEBP);
            }

            public void PopBlock() {
                offEBP = blocks.Pop();
            }

            /// <summary>
            /// Offset to EBP for parameters.
            /// </summary>
            private int preEBP;

            /// <summary>
            /// Offset to EBP for local objects.
            /// </summary>
            private int offEBP;

            /// <summary>
            /// offEBP for blocks.
            /// </summary>
            private Stack<int> blocks;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Env() {
            frame = new Frame();
        }

        public void PushBlock() {
            frame.PushBlock();
        }

        public void PopBlock() {
            frame.PopBlock();
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

        public void AddLocal(string symbol, T type) {
            frame.AddLocal(symbol, type);
        }

        public void AddParam(string symbol, T type) {
            frame.AddParam(symbol, type);
        }

        private readonly Frame frame;

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
