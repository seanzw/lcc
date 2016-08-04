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
            /// For double, the result is in xmm0.
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
            public Mem Addr(Size size) {
                return new MemReg(this, 0, size);
            }
        }

        /// <summary>
        /// General register.
        /// </summary>
        public static readonly Reg eax = new Reg("eax");
        public static readonly Reg ax = new Reg("ax");
        public static readonly Reg al = new Reg("al");

        /// <summary>
        /// Special attention!
        /// This is used like ebp to point to the base of a block.
        /// </summary>
        public static readonly Reg ebx = new Reg("ebx");

        public static readonly Reg ecx = new Reg("ecx");
        public static readonly Reg cl = new Reg("cl");

        public static readonly Reg edx = new Reg("edx");
        public static readonly Reg dx = new Reg("dx");
        public static readonly Reg dl = new Reg("dl");

        public static readonly Reg ebp = new Reg("ebp");
        public static readonly Reg esp = new Reg("esp");

        /// <summary>
        /// For float point.
        /// </summary>
        public static readonly Reg xmm0 = new Reg("xmm0");
        public static readonly Reg xmm1 = new Reg("xmm1");

        

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
            public Mem Addr(Size size) {
                return new MemLabel(this, 0, size);
            }
        }

        #region Instruction
        public sealed class Operator {
            private readonly string i;
            public Operator(string i) { this.i = i; }
            public string Intel => i;
            public string l => i + "l";
        }

        public static readonly Operator mov = new Operator("mov");
        public static readonly Operator movzx = new Operator("movzx");
        public static readonly Operator movsx = new Operator("movsx");

        public static readonly Operator lea = new Operator("lea");
        public static readonly Operator push = new Operator("push");
        public static readonly Operator pop = new Operator("pop");
        public static readonly Operator ret = new Operator("ret");
        public static readonly Operator call = new Operator("call");

        public static readonly Operator sub = new Operator("sub");
        public static readonly Operator add = new Operator("add");
        public static readonly Operator inc = new Operator("inc");
        public static readonly Operator dec = new Operator("dec");
        public static readonly Operator imul = new Operator("imul");
        public static readonly Operator mul = new Operator("mul");
        public static readonly Operator mulsd = new Operator("mulsd");
        public static readonly Operator idiv = new Operator("idiv");
        public static readonly Operator div = new Operator("div");
        public static readonly Operator divsd = new Operator("divsd");
        public static readonly Operator cdq = new Operator("cdq");

        public static readonly Operator shl = new Operator("shl");
        public static readonly Operator shr = new Operator("shr");

        public static readonly Operator cmp = new Operator("cmp");
        public static readonly Operator setle = new Operator("setle");
        public static readonly Operator setl = new Operator("setl");
        public static readonly Operator setge = new Operator("setge");
        public static readonly Operator setg = new Operator("setg");
        public static readonly Operator setbe = new Operator("setbe");
        public static readonly Operator setb = new Operator("setb");
        public static readonly Operator setae = new Operator("setae");
        public static readonly Operator seta = new Operator("seta");
        public static readonly Operator sete = new Operator("sete");
        public static readonly Operator setne = new Operator("setne");

        public static readonly Operator and = new Operator("and");
        public static readonly Operator xor = new Operator("xor");
        public static readonly Operator or = new Operator("or");

        public static readonly Operator jmp = new Operator("jmp");
        public static readonly Operator je = new Operator("je");
        public static readonly Operator jne = new Operator("jne");

        public static readonly Operator cvtsi2sd = new Operator("cvtsi2sd");
        public static readonly Operator movsd = new Operator("movsd");
        public static readonly Operator fld = new Operator("fld");
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

        public void Ascii(string value) {
            data.AppendFormat("\t.ascii \"{0}\"\n", value);
        }

        public void Data(Size size, string data) {
            switch (size) {
                case Size.QWORD: this.data.AppendFormat("\t.quad {0}\n", data); break;
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Push the result into the stack.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ret"></param>
        public void Push(Expr expr) {
            var ret = expr.ToX86Expr(this);
            switch (expr.Type.Kind) {
                case TKind.PTR:
                case TKind.ENUM:
                case TKind.ULONG:
                case TKind.LONG:
                case TKind.UINT:
                case TKind.INT:
                    Inst(push, ret == Ret.PTR ? eax.Addr() as Operand : eax);
                    break;
                case TKind.DOUBLE:
                    Inst(sub, esp, 8);
                    if (ret == Ret.PTR) Inst(movsd, xmm0, eax.Addr(Size.DWORD));
                    Inst(movsd, esp.Addr(Size.QWORD), xmm0);
                    break;
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Evaluate the expr and branch to "which".
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="label"></param>
        /// <param name="which"></param>
        public void Branch(Expr expr, string label, bool which) {
            switch (expr.Type.Kind) {
                case TKind.CHAR:
                case TKind.UCHAR:
                case TKind.SCHAR:
                    if (expr.ToX86Expr(this) == Ret.PTR) {
                        Inst(mov, al, eax.Addr(Size.BYTE));
                    }
                    Inst(cmp, al, 0);
                    Inst(which ? jne : je, label);
                    break;
                case TKind.PTR:
                case TKind.LONG:
                case TKind.ULONG:
                case TKind.UINT:
                case TKind.INT:
                    if (expr.ToX86Expr(this) == Ret.PTR) {
                        Inst(mov, eax, eax.Addr());
                    }
                    Inst(cmp, eax, 0);
                    Inst(which ? jne : je, label);
                    break;
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Evaluate and cast to type.
        /// 
        /// Refs:
        /// https://msdn.microsoft.com/en-us/library/k630sk6z.aspx
        /// 
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Ret Cast(Expr expr, T type) {
            var ret = expr.ToX86Expr(this);
            if (type.Kind == expr.Type.Kind) {
                return ret;
            }

            switch (expr.Type.Kind) {
                case TKind.CHAR:
                    if (ret == Ret.PTR) Inst(mov, al, eax.Addr(Size.BYTE));
                    switch (type.Kind) {
                        case TKind.CHAR:
                        case TKind.SCHAR:
                        case TKind.UCHAR:
                            /// Preserve bit pattern.
                            return Ret.REG;
                        case TKind.SHORT:
                        case TKind.USHORT:
                            /// Sign extend.
                            Inst(movsx, ax, al);
                            return Ret.REG;
                        case TKind.INT:
                        case TKind.UINT:
                        case TKind.LONG:
                        case TKind.ULONG:
                            /// Sign extend.
                            Inst(movsx, eax, al);
                            return Ret.REG;
                    }
                    break;
                case TKind.UCHAR:
                    if (ret == Ret.PTR) Inst(mov, al, eax.Addr(Size.BYTE));
                    switch (type.Kind) {
                        case TKind.CHAR:
                        case TKind.SCHAR:
                            /// Preserve bit pattern; high-order bit becomes sign bit.
                            return Ret.REG;
                        case TKind.SHORT:
                        case TKind.USHORT:
                            /// Zero extended.
                            Inst(movzx, ax, al);
                            return Ret.REG;
                        case TKind.INT:
                        case TKind.LONG:
                        case TKind.UINT:
                        case TKind.ULONG:
                            Inst(movzx, eax, al);
                            return Ret.REG;
                    }
                    break;
                case TKind.USHORT:
                case TKind.SHORT:
                    if (ret == Ret.PTR) Inst(mov, ax, eax.Addr(Size.WORD));
                    switch (type.Kind) {
                        case TKind.CHAR:
                        case TKind.SCHAR:
                        case TKind.UCHAR:
                        case TKind.USHORT:
                            return Ret.REG;
                        case TKind.INT:
                        case TKind.UINT:
                        case TKind.LONG:
                        case TKind.ULONG:
                            /// Sign extension.
                            Inst(expr.Type.Kind == TKind.SHORT ? movsx : movzx, eax, ax);
                            return Ret.REG;
                    }
                    break;
                case TKind.UINT:
                case TKind.ULONG:
                    if (ret == Ret.PTR) Inst(mov, eax, eax.Addr());
                    switch (type.Kind) {
                        case TKind.CHAR:
                        case TKind.SCHAR:
                        case TKind.UCHAR:
                        case TKind.SHORT:
                        case TKind.USHORT:
                        case TKind.INT:
                        case TKind.UINT:
                        case TKind.LONG:
                        case TKind.ULONG:
                        case TKind.PTR:
                            /// Preserve bit pattern; high-order bit becomes sign bit.
                            return Ret.REG;
                    }
                    break;
                case TKind.INT:
                case TKind.LONG:
                    if (ret == Ret.PTR) Inst(mov, eax, eax.Addr());
                    switch (type.Kind) {
                        case TKind.UCHAR:
                        case TKind.CHAR:
                        case TKind.SCHAR:
                        case TKind.SHORT:
                        case TKind.USHORT:
                        case TKind.INT:
                        case TKind.UINT:
                        case TKind.LONG:
                        case TKind.ULONG:
                        case TKind.PTR:
                            return Ret.REG;
                        case TKind.DOUBLE:
                            Inst(cvtsi2sd, xmm0, eax);
                            return Ret.REG;
                    }
                    break;
                case TKind.PTR:
                    /// Pointer behaves like unsigned int:
                    /// A pointer value can also be converted to an integral value. 
                    /// The conversion path depends on the size of the pointer and 
                    /// the size of the integral type, according to the following rules:
                    /// 
                    ///     If the size of the pointer is greater than or equal to the
                    ///   size of the integral type, the pointer behaves like an unsigned 
                    ///   value in the conversion, except that it cannot be converted to 
                    ///   a floating value.
                    ///   
                    ///     If the pointer is smaller than the integral type, the pointer is 
                    ///   first converted to a pointer with the same size as the integral type,
                    ///   then converted to the integral type.
                    if (ret == Ret.PTR) Inst(mov, eax, eax.Addr());
                    switch (type.Kind) {
                        case TKind.CHAR:
                        case TKind.SCHAR:
                        case TKind.UCHAR:
                        case TKind.SHORT:
                        case TKind.USHORT:
                        case TKind.INT:
                        case TKind.UINT:
                        case TKind.LONG:
                        case TKind.ULONG:
                            /// Preserve lower-order word.
                            /// Trick: do not mov al/ax eax.
                            return Ret.REG;
                    }
                    break;
            }

            throw new NotImplementedException();
        }

        private readonly StringBuilder text;
        private readonly StringBuilder data;
        private readonly StringBuilder bss;
    }
}
