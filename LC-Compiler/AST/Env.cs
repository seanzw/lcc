using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {

    public abstract class Obj {
        public readonly string uid;
        public readonly string symbol;
        public readonly T type;
        public Obj(string uid, string symbol, T type) {
            this.uid = uid;
            this.symbol = symbol;
            this.type = type;
        }
    }

    public class StaticObj : Obj {
        /// <summary>
        /// True if the linkage is external.
        /// </summary>
        public readonly bool linkage;
        /// <summary>
        /// True if the storage is external.
        /// </summary>
        public readonly bool storage;
        public StaticObj(string uid, string symbol, T type, bool linkage, bool storage)
            : base(uid, symbol, type) {
            this.linkage = linkage;
            this.storage = storage;
        }
    }

    public class DynamicObj : Obj {

        /// <summary>
        /// Offset to ebx reg.
        /// </summary>
        public readonly int ebp;

        public DynamicObj(string uid, string symbol, T type, int ebp)
            : base(uid, symbol, type) {
            this.ebp = ebp;
        }
    }

    public sealed class Env {

        /// <summary>
        /// The size of the frame.
        /// </summary>
        public int size {
            get;
            private set;
        }

        public readonly LinkedList<DynamicObj> objs;

        public Env() {
            objs = new LinkedList<DynamicObj>();
            blocks = new Stack<int>();
            preEBP = 8;
            offEBP = 0;
            size = 0;
        }
        public void AddLocal(string uid, string symbol, T type) {
            offEBP -= type.AlignByte;
            size = Math.Max(size, -offEBP);
            objs.AddLast(new DynamicObj(uid, symbol, type, offEBP));
        }

        public void AddParam(string uid, string symbol, T type) {
            objs.AddFirst(new DynamicObj(uid, symbol, type, preEBP));
            preEBP += type.AlignByte;
        }

        /// <summary>
        /// Given uid, find its offset to ebp register.
        /// Throw ArgumentException if this is illegal uid.
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int GetEBP(string uid) {
            foreach (var o in objs) {
                if (uid == o.uid)
                    return o.ebp;
            }
            throw new ArgumentException("unkown uid.");
        }

        public void PushBlock() {
            blocks.Push(offEBP);
        }

        public void PopBlock() {
            offEBP = blocks.Pop();
        }

        public void Dump(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, string.Format("Frame Size: {0}", size));
            gen.Comment(X86Gen.Seg.TEXT, string.Format("{0, -10} {1, -5} {2, -10} {3, -20}",
                "EBP",
                "UID",
                "SYMBOL",
                "TYPE"
                ));
            foreach (var o in objs) {
                gen.Comment(X86Gen.Seg.TEXT, string.Format("{0, -10} {1, -5} {2, -10} {3, -20}",
                    o.ebp,
                    o.uid,
                    o.symbol,
                    o.type
                    ));
            }
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

        /// <summary>
        /// Add a static object to the environment.
        /// </summary>
        /// <param name="symbol"> The name of the symbol. </param>
        /// <param name="type"> The type of the symbol. </param>
        /// <param name="linkage"> True if the linkage is external. </param>
        /// <param name="storage"> True if the storage is external. </param>
        public static void AddStaticObj(string uid, string symbol, T type, bool linkage, bool storage) {
            StaticObj obj = new StaticObj(
                uid,                
                symbol,
                type,
                linkage,
                storage);
            statics.AddLast(obj);
        }

        /// <summary>
        /// Static objects are global visible in AST, so use static.
        /// </summary>
        private static LinkedList<StaticObj> statics = new LinkedList<StaticObj>();
    }
}
