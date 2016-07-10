using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;

namespace lcc.AST {



    public sealed class X86Gen {

        public enum Seg {
            TEXT,
            DATA
        }

        /// <summary>
        /// The result of an expression.
        /// </summary>
        public enum Ret {
            /// <summary>
            /// The result is in eax.
            /// </summary>
            EAX,
            /// <summary>
            /// The result is a pointer in eax.
            /// </summary>
            PTR
        }

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

        public abstract class Operand {
            public abstract string Intel { get; }
            public static implicit operator Operand(int i) { return new Num(i); }
        }

        #region Effective Address
        public sealed class Address : Operand {
            public readonly Reg b;
            public readonly int offset;
            public Address(Reg b, int offset) {
                this.b = b;
                this.offset = offset;
            }
            public override string Intel => string.Format("[{0}{1}]", b.Intel, offset.ToString(" + 0; - #"));
        }
        #endregion

        #region Number
        public sealed class Num : Operand {
            private readonly BigInteger i;
            public Num(BigInteger i) { this.i = i; }
            public override string Intel => i.ToString();
        }
        #endregion

        #region Register
        public sealed class Reg : Operand {
            private readonly string s;
            public Reg(string s) { this.s = s; }
            public override string Intel => s;
        }

        public static readonly Reg eax = new Reg("eax");
        public static readonly Reg ebp = new Reg("ebp");
        public static readonly Reg esp = new Reg("esp");
        #endregion

        #region Instruction
        public sealed class Operator {
            private readonly string i;
            public Operator(string i) { this.i = i; }
            public string Intel => i;
            public string l => i + "l";
        }

        public static readonly Operator mov = new Operator("mov");
        public static readonly Operator lea = new Operator("lea");
        public static readonly Operator push = new Operator("push");
        public static readonly Operator pop = new Operator("pop");
        public static readonly Operator ret = new Operator("ret");
        public static readonly Operator sub = new Operator("sub");
        public static readonly Operator add = new Operator("add");
        #endregion

        public void Inst(Operator i) {
            if (IntelOrATT)
                text.Append(string.Format("\t{0}\n", i.Intel));
        }

        public void Inst(Operator i, Operand o) {
            if (IntelOrATT)
                text.Append(string.Format("\t{0, -6} {1}\n", i.Intel, o.Intel));
        }

        public void Inst(Operator i, Operand dst, Operand src) {
            if (IntelOrATT)
                text.Append(string.Format("\t{0, -6} {1}, {2}\n", i.Intel, dst.Intel, src.Intel));
        }

        public void Label(Seg seg, string label, bool isGlobal = false) {
            StringBuilder sb = seg == Seg.DATA ? data : text;
            if (isGlobal)
                sb.Append(string.Format("\t.globl {0}\n", label));
            sb.Append(string.Format("{0}:\n", label));
        }

        public void Comment(Seg seg, string cmt) {
            StringBuilder sb = seg == Seg.DATA ? data : text;
            sb.Append(string.Format("\t# {0}\n", cmt));
        }

        private readonly StringBuilder text;
        private readonly StringBuilder data;
    }
}
