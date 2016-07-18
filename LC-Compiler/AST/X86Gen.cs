using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;

using lcc.TypeSystem;

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
            /// The result is in register, maybe al, ax, eax, or edx:eax.
            /// </summary>
            REG,
            /// <summary>
            /// The result is a pointer in eax.
            /// </summary>
            PTR
        }

        /// <summary>
        /// Represent operation size.
        /// </summary>
        public enum Size {
            BYTE,
            WORD,
            DWORD,
            QWORD
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
            bss = new StringBuilder();
            text.Append("\t.text\n");
            data.Append("\t.data\n");
            bss.Append("\t.bss\n");
            if (IntelOrATT) {
                // To support Intel syntax.
                text.Append("\t.intel_syntax noprefix\n");
            }
        }

        public override string ToString() {
            var all = new StringBuilder(data.ToString());
            all.Append(bss.ToString());
            all.Append(text.ToString());
            return all.ToString();
        }

        public abstract class Operand {
            public abstract string Intel { get; }
            public static implicit operator Operand(int i) { return new Immediate(i); }
            public static implicit operator Operand(BigInteger i) { return new Immediate(i); }
            public static implicit operator Operand(string label) { return new Label(label); }
        }

        #region Effective Address
        public abstract class Mem : Operand {
            public readonly int offset;
            public readonly Size size;
            public Mem(int offset, Size size) {
                this.offset = offset;
                this.size = size;
            }
            protected string SizeIntel(Size size) {
                switch (size) {
                    case Size.BYTE: return "byte";
                    case Size.WORD: return "word";
                    case Size.DWORD: return "dword";
                    case Size.QWORD: return "qword";
                    default: throw new ArgumentException("illegal operation size");
                }
            }
        }

        private sealed class MemReg : Mem {
            public readonly Reg b;
            public MemReg(Reg b, int offset, Size size) : base(offset, size) {
                this.b = b;
            }
            public override string Intel => string.Format("{2} ptr [{0}{1}]", b.Intel, offset.ToString(" + 0; - #"), SizeIntel(size));
        }

        private sealed class MemLabel : Mem {
            public readonly Label b;
            public MemLabel(Label b, int offset, Size size) : base(offset, size) {
                this.b = b;
            }
            public override string Intel => string.Format("{2} ptr [{0}{1}]", b.Intel, offset.ToString(" + 0; - #"), SizeIntel(size));
        }
        #endregion

        #region Immediate
        public sealed class Immediate : Operand {
            private readonly BigInteger i;
            public Immediate(BigInteger i) { this.i = i; }
            public override string Intel => i.ToString();
        }
        #endregion

        #region Register
        public sealed class Reg : Operand {
            private readonly string s;
            public Reg(string s) { this.s = s; }
            public override string Intel => s;
            public Mem Addr(int offset = 0, Size size = Size.DWORD) {
                return new MemReg(this, offset, size);
            }
        }

        /// <summary>
        /// General register.
        /// </summary>
        public static readonly Reg eax = new Reg("eax");
        public static readonly Reg al = new Reg("al");

        public static readonly Reg ebx = new Reg("ebx");

        /// <summary>
        /// Special attention!
        /// This is used like ebp to point to the base of a block.
        /// </summary>
        public static readonly Reg ecx = new Reg("ecx");
        public static readonly Reg edx = new Reg("edx");
        public static readonly Reg ebp = new Reg("ebp");
        public static readonly Reg esp = new Reg("esp");
        #endregion

        public sealed class Label : Operand {
            public readonly string label;
            public Label(string label) {
                this.label = label;
            }
            public override string Intel => label;
            public Mem Addr(int offset = 0, Size size = Size.DWORD) {
                return new MemLabel(this, offset, size);
            }
        }

        #region Instruction
        public sealed class Operator {
            private readonly string i;
            public Operator(string i) { this.i = i; }
            public string Intel => i;
            public string l => i + "l";
        }

        public static readonly Operator mov     = new Operator("mov");
        public static readonly Operator movzx   = new Operator("movzx");

        public static readonly Operator lea     = new Operator("lea");
        public static readonly Operator push    = new Operator("push");
        public static readonly Operator pop     = new Operator("pop");
        public static readonly Operator ret     = new Operator("ret");
        public static readonly Operator call    = new Operator("call");

        public static readonly Operator sub     = new Operator("sub");
        public static readonly Operator add     = new Operator("add");
        public static readonly Operator inc     = new Operator("inc");
        public static readonly Operator dec     = new Operator("dec");
        public static readonly Operator imul    = new Operator("imul");
        public static readonly Operator idiv    = new Operator("idiv");
        public static readonly Operator cdq     = new Operator("cdq");

        public static readonly Operator cmp     = new Operator("cmp");
        public static readonly Operator setle   = new Operator("setle");
        public static readonly Operator setl    = new Operator("setl");
        public static readonly Operator setge   = new Operator("setge");
        public static readonly Operator setg    = new Operator("setg");
        public static readonly Operator sete    = new Operator("sete");
        public static readonly Operator setne   = new Operator("setne");

        public static readonly Operator and     = new Operator("and");

        public static readonly Operator jmp     = new Operator("jmp");
        public static readonly Operator je      = new Operator("je");
        public static readonly Operator jne     = new Operator("jne");
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

        public void Inst(Operator i, Operand dst, Operand src1, Operand src2) {
            if (IntelOrATT)
                text.Append(string.Format("\t{0, -6} {1}, {2}, {3}\n", i.Intel, dst.Intel, src1.Intel, src2.Intel));
        }

        /// <summary>
        /// Lay down a label.
        /// </summary>
        /// <param name="seg"></param>
        /// <param name="label"></param>
        /// <param name="isGlobal"></param>
        public void Tag(Seg seg, string label, bool isGlobal = false) {
            StringBuilder sb = seg == Seg.DATA ? data : text;
            if (isGlobal)
                sb.Append(string.Format("\t.globl {0}\n", label));
            sb.Append(string.Format("{0}:\n", label));
        }

        public void Comment(Seg seg, string cmt) {
            StringBuilder sb = seg == Seg.DATA ? data : text;
            sb.Append(string.Format("\t# {0}\n", cmt));
        }

        /// <summary>
        /// Push the result into the stack.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ret"></param>
        public void Push(T type, Ret ret) {
            switch (type.Kind) {
                case TKind.PTR:
                case TKind.ULONG:
                case TKind.LONG:
                case TKind.UINT:
                case TKind.INT:
                    Inst(push, ret == Ret.PTR ? eax.Addr() as Operand : eax);
                    break;
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Mov the result in eax to dst.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ret"></param>
        /// <param name="dst"></param>
        public void Mov(T type, Ret ret, Operand dst) {
            switch (type.Kind) {
                case TKind.PTR:
                case TKind.ULONG:
                case TKind.LONG:
                case TKind.UINT:
                case TKind.INT:
                    Inst(mov, dst, ret == Ret.PTR ? eax.Addr() as Operand : eax);
                    break;
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Evaluate the expr and branch to label if the result is 0.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="label"></param>
        public void BranchFalse(Expr expr, string label) {
            switch (expr.Type.Kind) {
                case TKind.INT:
                    Mov(expr.Type, expr.ToX86Expr(this), eax);
                    Inst(cmp, eax, 0);
                    Inst(je, label);
                    break;
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Evaluate the expr and brance to label if the result is not 0.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="label"></param>
        public void BranchTrue(Expr expr, string label) {
            switch (expr.Type.Kind) {
                case TKind.INT:
                    Mov(expr.Type, expr.ToX86Expr(this), eax);
                    Inst(cmp, eax, 0);
                    Inst(jne, label);
                    break;
                default: throw new NotImplementedException();
            }
        }

        private readonly StringBuilder text;
        private readonly StringBuilder data;
        private readonly StringBuilder bss;
    }
}
