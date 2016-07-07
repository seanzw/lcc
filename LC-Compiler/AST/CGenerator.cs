using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {

    public enum Seg {
        TEXT,
        DATA
    }

    public sealed class X86Gen {

        public X86Gen() {
            text = new StringBuilder();
            data = new StringBuilder();
            text.Append("\t.text\n");
            data.Append("\t.data\n");
        }

        public override string ToString() {
            var all = new StringBuilder(data.ToString());
            all.Append(text.ToString());
            return all.ToString();
        }

        public abstract class Operator {
            public abstract string Intel { get; }
        }

        #region Register
        public sealed class Reg : Operator {
            private readonly string s;
            public Reg(string s) { this.s = s; }
            public override string Intel => s;
        }

        public static readonly Reg eax = new Reg("eax");
        public static readonly Reg ebp = new Reg("ebx");
        public static readonly Reg esp = new Reg("esp");
        #endregion

        #region Instruction
        public sealed class Inst {
            private readonly string i;
            public Inst(string i) { this.i = i; }
            public override string ToString() {
                return i;
            }
            public string l() {
                return i + "l";
            }
        }

        public static readonly Inst mov = new Inst("mov");
        #endregion

        public void label(Seg seg, string label, bool isGlobal = false) {
            StringBuilder sb = seg == Seg.DATA ? data : text;
            if (isGlobal)
                sb.Append(string.Format("\tglobal {0}\n", label));
            sb.Append(string.Format("{0}:\n", label));
        }

        private readonly StringBuilder text;
        private readonly StringBuilder data;
    }
}
