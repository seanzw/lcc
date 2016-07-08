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

        /// <summary>
        /// Intel (true) or AT&T (false) syntax.
        /// </summary>
        private readonly bool IntelOrATT;

        public X86Gen(bool IntelOrATT = true) {
            if (!IntelOrATT) {
                throw new NotImplementedException("donot support AT&T syntax");
            }
            this.IntelOrATT = IntelOrATT;
            text = new StringBuilder();
            data = new StringBuilder();
            text.Append("\t.text\n");
            data.Append("\t.data\n");
            if (IntelOrATT) {
                // To support Intel syntax.
                text.Append("\t.intel_syntax noprefix\n");
            }
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
        public static readonly Reg ebp = new Reg("ebp");
        public static readonly Reg esp = new Reg("esp");
        #endregion

        #region Instruction
        public sealed class Inst {
            private readonly string i;
            public Inst(string i) { this.i = i; }
            public string Intel => i;
            public string l => i + "l";
        }

        public static readonly Inst mov = new Inst("mov");
        public static readonly Inst push = new Inst("push");
        public static readonly Inst pop = new Inst("pop");
        public static readonly Inst ret = new Inst("ret");
        #endregion

        public void inst(Inst i) {
            if (IntelOrATT)
                text.Append(string.Format("\t{0}\n", i.Intel));
        }

        public void inst(Inst i, Operator o) {
            if (IntelOrATT)
                text.Append(string.Format("\t{0, -6} {1}\n", i.Intel, o.Intel));
        }

        public void inst(Inst i, Operator dst, Operator src) {
            if (IntelOrATT)
                text.Append(string.Format("\t{0, -6} {1}, {2}\n", i.Intel, dst.Intel, src.Intel));
        }

        public void label(Seg seg, string label, bool isGlobal = false) {
            StringBuilder sb = seg == Seg.DATA ? data : text;
            if (isGlobal)
                sb.Append(string.Format("\t.globl {0}\n", label));
            sb.Append(string.Format("{0}:\n", label));
        }

        private readonly StringBuilder text;
        private readonly StringBuilder data;
    }
}
