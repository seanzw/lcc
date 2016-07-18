using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {
    public abstract class Expr : Node {
        public T Type => type;
        public Env Envrionment => env;
        public virtual bool IsLValue => false;

        public virtual bool IsConstZero => false;
        public virtual bool IsNullPtr => false;

        protected readonly T type;
        protected readonly Env env;

        public Expr(T type, Env env) {
            this.type = type;
            this.env = env;
        }

        /// <summary>
        /// Performs integer promotion by explicitly using cast operator.
        /// </summary>
        /// <returns></returns>
        public Expr IntPromote() {
            T type = this.type.IntPromote();
            return type.Equals(this.type) ? this : new Cast(type.nake, env, this);
        }

        /// <summary>
        /// Perform usual arithmetic conversion by explicitly using cast operator.
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        public static Tuple<T, Expr, Expr> UsualArithConvert(Expr e1, Expr e2) {
            T type = e1.Type.UsualArithConversion(e2.Type);
            Expr c1 = type.Equals(e1.Type) ? e1 : new Cast(type.nake, e1.Envrionment, e1);
            Expr c2 = type.Equals(e2.type) ? e2 : new Cast(type.nake, e2.Envrionment, e2);
            return new Tuple<T, Expr, Expr>(type, c1, c2);
        }

        /// <summary>
        /// Whether this type can be implicitly convertible to target without generating an warning.
        /// Returns null if it cannot be implicitly converted to the target.
        /// 
        /// One of the following situation. All other situations are either illegal or requiring explicit cast.
        /// 
        /// 0. The same types.
        /// 
        /// 1. Compatible types.
        /// 
        /// 2. Boolean conversion.
        ///    A value of any scalar type can be implicitly converted to _Bool.
        ///    
        /// 3. Arithmetic conversion.
        ///    A value of arithmetic type can be implicitly converted to another arithmetic type without warning.
        ///    No matter real or complex.
        ///    
        /// 4. Pointer conversion.
        ///    a. A pointer to void can be implicitly converted to and from a pointer to any incomplete or object type.
        ///    NOTE: I checked clang and void* can be implicitly converted to a pointer to function type and vice verse.
        ///    Here I stick with the standard.
        ///    
        ///    b. For any qualifier q, a pointer to a non-q-qualified type may be converted to a pointer to the q-qualified
        ///    version of the type.
        ///    
        ///    c. An integer constant expression with value 0 can be converted to a null pointer to any type.
        ///    
        ///    d. A null pointer can be converted another null pointer to any type.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Expr ImplicitConvert(T target) {
            if (Type.Equals(target)) {
                return this;
            }
            if (Type.Compatible(target)) {
                return new Cast(target.nake, env, this);
            }
            if (Type.IsScalar && target.Kind == TKind.BOOL) {
                return new Cast(target.nake, env, this);
            }
            if (Type.IsArithmetic && target.IsArithmetic) {
                return new Cast(target.nake, env, this);
            }
            if (Type.IsPtr) {
                T e1 = (Type.nake as TPtr).element;
                if (target.IsPtr) {
                    T e2 = (target.nake as TPtr).element;
                    if (e1.IsVoid && !e2.IsFunc) {
                        return new Cast(target.nake, env, this);
                    }
                    if (e2.IsVoid && !e1.IsFunc) {
                        return new Cast(target.nake, env, this);
                    }
                    if ((e1.qualifiers | e2.qualifiers).Equals(e2.qualifiers)) {
                        return new Cast(target.nake, env, this);
                    }
                }
            }
            if ((IsNullPtr || IsConstZero) && target.IsPtr) {
                return new ConstNullPtr((target.nake as TPtr).element, env);
            }

            return null;
        }

        /// <summary>
        /// Performs all the three value transforms by explicitly using cast operator.
        /// Notice that these three implicit conversions will be perfromed automatically and
        /// there is only one possible conversion result.
        /// </summary>
        /// <returns></returns>
        public Expr ValueTransform() {
            return VTFunc().VTLValue().VTArr();
        }

        /// <summary>
        /// 1. An lvalue that does not have array type is converted to the value stored in the designated
        ///    object (and is no longer an lvalue).
        ///    If the lvalue has qualified type, the values has unqualified version of the type of the lvalue.
        ///    Otherwise, the value has the type of the lvalue.
        ///    EXCEPT: sizeof, unary &, ++, --, left operand of ., left operand of assignment operator.
        /// </summary>
        /// <returns></returns>
        public Expr VTLValue() {
            return (IsLValue && !type.IsArray) ? new LValueCast(env, this) : this;
        }

        /// <summary>
        /// 2. An expression that has type "array of type" is converted to an expression with type "pointer to type"
        ///    that points to the initial element of the array object and is not an lvalue.
        ///    If the array object has register storage class, the behavior is undefined.
        ///    EXCEPT: sizeof, unary &, a string literal used to initialize an array.
        /// </summary>
        /// <returns></returns>
        public Expr VTArr() {
            return type.IsArray ? new ArrCast(type.nake as TArr, env, this) : this;
        }

        /// <summary>
        /// 3. A function designator is an expression that has function type. A function designator with type
        ///    "function returning type" is converted to an expression that has type "pointer to function returning type".
        ///    EXCEPT: sizeof, unary &.
        /// </summary>
        /// <returns></returns>
        public Expr VTFunc() {
            return type.IsFunc ? new FuncCast(type, env, this) : this;
        }

        public virtual X86Gen.Ret ToX86Expr(X86Gen gen) {
            throw new NotImplementedException();
        }

        public sealed override void ToX86(X86Gen gen) {
            ToX86Expr(gen);
        }

    }

    public sealed class CommaExpr : Expr {
        public readonly IEnumerable<Expr> exprs;
        public CommaExpr(T type, Env env, IEnumerable<Expr> exprs) : base(type, env) {
            this.exprs = exprs;
        }
    }

    public sealed class Assign : Expr {
        public readonly Expr lhs;
        public readonly Expr rhs;
        public readonly SyntaxTree.Assign.Op op;
        public Assign(T type, Env env, Expr lhs, Expr rhs, SyntaxTree.Assign.Op op) : base(type, env) {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }
        public override string ToString() {
            return string.Format("{0} {1} {2}", lhs, SyntaxTree.Assign.OpToString(op), rhs);
        }
        public override X86Gen.Ret ToX86Expr(X86Gen gen) {
            /// The order of evaluation of the opoerands is unspecified.
            /// Here I choose to evaluate rhs first.
            gen.Comment(X86Gen.Seg.TEXT, ToString());
            var rhsRet = rhs.ToX86Expr(gen);
            switch (op) {
                case SyntaxTree.Assign.Op.ASSIGN: {
                        switch (lhs.Type.Kind) {
                            case TKind.INT: {
                                    gen.Inst(X86Gen.push, X86Gen.eax);
                                    var lhsRet = lhs.ToX86Expr(gen);
                                    Debug.Assert(lhsRet == X86Gen.Ret.PTR);
                                    gen.Inst(X86Gen.mov, X86Gen.ebx, X86Gen.eax);
                                    gen.Inst(X86Gen.pop, X86Gen.eax);
                                    if (rhsRet == X86Gen.Ret.PTR) {
                                        gen.Inst(X86Gen.mov, X86Gen.eax, X86Gen.eax.Addr());
                                    }
                                    gen.Inst(X86Gen.mov, X86Gen.ebx.Addr(), X86Gen.eax);
                                    return X86Gen.Ret.REG;
                                }                                
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public sealed class CondExpr : Expr {
        public readonly Expr p;
        public readonly Expr t;
        public readonly Expr f;
        public CondExpr(T type, Env env, Expr p, Expr t, Expr f) : base(type, env) {
            this.p = p;
            this.t = t;
            this.f = f;
        }
    }

    public sealed class BiExpr : Expr {
        /// <summary>
        /// Used in branch for logical operator.
        /// </summary>
        public readonly string logicalShortCutLabel;
        /// <summary>
        /// Used in branch for logical operator.
        /// </summary>
        public readonly string logicalEndLabel;
        public readonly Expr lhs;
        public readonly Expr rhs;
        public readonly SyntaxTree.BiExpr.Op op;
        public BiExpr(
            T type,
            Env env,
            Expr lhs, 
            Expr rhs, 
            SyntaxTree.BiExpr.Op op
            ) : base(type, env) {
            Debug.Assert(op != SyntaxTree.BiExpr.Op.LOGAND && op != SyntaxTree.BiExpr.Op.LOGOR);
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
            this.logicalShortCutLabel = null;
            this.logicalEndLabel = null;
        }

        /// <summary>
        /// Constructor for logical operator.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="env"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="op"></param>
        /// <param name="logicalShortCutLabel"></param>
        /// <param name="logicalEndLabel"></param>
        public BiExpr(
            T type,
            Env env, 
            Expr lhs,
            Expr rhs,
            SyntaxTree.BiExpr.Op op,
            string logicalShortCutLabel,
            string logicalEndLabel
            ) : base(type, env) {
            Debug.Assert(op == SyntaxTree.BiExpr.Op.LOGAND || op == SyntaxTree.BiExpr.Op.LOGOR);
            Debug.Assert(logicalEndLabel != null && logicalShortCutLabel != null);
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
            this.logicalShortCutLabel = logicalShortCutLabel;
            this.logicalEndLabel = logicalEndLabel;
        }
        public override string ToString() {
            return string.Format("{0} ({1}) ({2})", SyntaxTree.BiExpr.OpToString(op), lhs, rhs);
        }
        public override X86Gen.Ret ToX86Expr(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, ToString());
            switch (op) {
                case SyntaxTree.BiExpr.Op.MULT:
                case SyntaxTree.BiExpr.Op.DIV:
                case SyntaxTree.BiExpr.Op.MOD:
                    Multiplicative(gen);
                    break;
                case SyntaxTree.BiExpr.Op.PLUS:
                case SyntaxTree.BiExpr.Op.MINUS:
                    Additive(gen);
                    break;
                case SyntaxTree.BiExpr.Op.LEFT:
                case SyntaxTree.BiExpr.Op.RIGHT:
                    throw new NotImplementedException();
                case SyntaxTree.BiExpr.Op.LE:
                case SyntaxTree.BiExpr.Op.GE:
                case SyntaxTree.BiExpr.Op.LT:
                case SyntaxTree.BiExpr.Op.GT:
                case SyntaxTree.BiExpr.Op.EQ:
                case SyntaxTree.BiExpr.Op.NEQ:
                    RelationalEquality(gen);
                    break;
                case SyntaxTree.BiExpr.Op.AND:
                case SyntaxTree.BiExpr.Op.XOR:
                case SyntaxTree.BiExpr.Op.OR:
                    throw new NotImplementedException();
                case SyntaxTree.BiExpr.Op.LOGAND:
                case SyntaxTree.BiExpr.Op.LOGOR:
                    Logical(gen);
                    break;
            }
            return X86Gen.Ret.REG;
        }

        /// <summary>
        /// Handle multiplicative ooperator.
        /// </summary>
        /// <param name="gen"></param>
        private void Multiplicative(X86Gen gen) {
            Debug.Assert(lhs.Type.Kind == rhs.Type.Kind);
            Debug.Assert(type.Kind == lhs.Type.Kind);
            Debug.Assert(op == SyntaxTree.BiExpr.Op.MULT || op == SyntaxTree.BiExpr.Op.DIV || op == SyntaxTree.BiExpr.Op.MOD);

            gen.Push(lhs.Type, lhs.ToX86Expr(gen));
            switch (type.Kind) {
                case TKind.INT:
                    // Generate code for rhs and move the result to ebx.
                    gen.Mov(rhs.Type, rhs.ToX86Expr(gen), X86Gen.ebx);
                    gen.Inst(X86Gen.pop, X86Gen.eax);
                    switch (op) {
                        case SyntaxTree.BiExpr.Op.MOD:
                            throw new NotImplementedException();
                        case SyntaxTree.BiExpr.Op.MULT:
                            gen.Inst(X86Gen.imul, X86Gen.eax, X86Gen.ebx);
                            break;
                        case SyntaxTree.BiExpr.Op.DIV:
                            gen.Inst(X86Gen.cdq);
                            gen.Inst(X86Gen.idiv, X86Gen.ebx);
                            break;
                    }
                    break;
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Handle additive opeator.
        /// </summary>
        /// <param name="gen"></param>
        private void Additive(X86Gen gen) {
            Debug.Assert(lhs.Type.Kind == rhs.Type.Kind);
            Debug.Assert(type.Kind == lhs.Type.Kind);
            Debug.Assert(op == SyntaxTree.BiExpr.Op.PLUS || op == SyntaxTree.BiExpr.Op.MINUS);

            gen.Push(lhs.Type, lhs.ToX86Expr(gen));
            switch (type.Kind) {
                case TKind.INT:
                    // Generate code for rhs and move the result to ebx.
                    gen.Mov(rhs.Type, rhs.ToX86Expr(gen), X86Gen.ebx);
                    gen.Inst(X86Gen.pop, X86Gen.eax);
                    switch (op) {
                        case SyntaxTree.BiExpr.Op.PLUS:     gen.Inst(X86Gen.add, X86Gen.eax, X86Gen.ebx); break;
                        case SyntaxTree.BiExpr.Op.MINUS:    gen.Inst(X86Gen.sub, X86Gen.eax, X86Gen.ebx); break;
                    }
                    break;
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Handle relational and equality operator.
        /// </summary>
        /// <param name="gen"></param>
        private void RelationalEquality(X86Gen gen) {
            Debug.Assert(type.Kind == TKind.INT);
            Debug.Assert(op == SyntaxTree.BiExpr.Op.LE ||
                op == SyntaxTree.BiExpr.Op.LT ||
                op == SyntaxTree.BiExpr.Op.GE ||
                op == SyntaxTree.BiExpr.Op.GT ||
                op == SyntaxTree.BiExpr.Op.EQ ||
                op == SyntaxTree.BiExpr.Op.NEQ);
            Debug.Assert(lhs.Type.Kind == rhs.Type.Kind);

            gen.Push(lhs.Type, lhs.ToX86Expr(gen));

            /// Both lhs and rhs should have the same type (either pointer or result from usual arithmetic conversion).
            switch (lhs.Type.Kind) {
                case TKind.INT:
                    // Generate code for rhs and move the result to ebx.
                    gen.Mov(rhs.Type, rhs.ToX86Expr(gen), X86Gen.ebx);

                    // Pop the result of lhs to eax.
                    gen.Inst(X86Gen.pop, X86Gen.eax);
                    gen.Inst(X86Gen.cmp, X86Gen.eax, X86Gen.ebx);
                    switch (op) {
                        case SyntaxTree.BiExpr.Op.LE: gen.Inst(X86Gen.setle, X86Gen.al); break;
                        case SyntaxTree.BiExpr.Op.LT: gen.Inst(X86Gen.setl, X86Gen.al); break;
                        case SyntaxTree.BiExpr.Op.GE: gen.Inst(X86Gen.setge, X86Gen.al); break;
                        case SyntaxTree.BiExpr.Op.GT: gen.Inst(X86Gen.setg, X86Gen.al); break;
                        case SyntaxTree.BiExpr.Op.EQ: gen.Inst(X86Gen.sete, X86Gen.al); break;
                        case SyntaxTree.BiExpr.Op.NEQ: gen.Inst(X86Gen.setne, X86Gen.al); break;
                    }
                    gen.Inst(X86Gen.and, X86Gen.al, 1);
                    gen.Inst(X86Gen.movzx, X86Gen.eax, X86Gen.al);
                    break;
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Handle logical operator.
        /// </summary>
        /// <param name="gen"></param>
        private void Logical(X86Gen gen) {
            Debug.Assert(type.Kind == TKind.INT);
            Debug.Assert(op == SyntaxTree.BiExpr.Op.LOGAND || op == SyntaxTree.BiExpr.Op.LOGOR);
            Debug.Assert(logicalEndLabel != null && logicalShortCutLabel != null);

            switch (op) {
                case SyntaxTree.BiExpr.Op.LOGAND:
                    gen.BranchFalse(lhs, logicalShortCutLabel);
                    gen.BranchFalse(rhs, logicalShortCutLabel);
                    gen.Inst(X86Gen.mov, X86Gen.eax, 1);
                    gen.Inst(X86Gen.jmp, logicalEndLabel);
                    gen.Tag(X86Gen.Seg.TEXT, logicalShortCutLabel);
                    gen.Inst(X86Gen.mov, X86Gen.eax, 0);
                    break;
                case SyntaxTree.BiExpr.Op.LOGOR:
                    gen.BranchTrue(lhs, logicalShortCutLabel);
                    gen.BranchTrue(rhs, logicalShortCutLabel);
                    gen.Inst(X86Gen.mov, X86Gen.eax, 0);
                    gen.Inst(X86Gen.jmp, logicalEndLabel);
                    gen.Tag(X86Gen.Seg.TEXT, logicalShortCutLabel);
                    gen.Inst(X86Gen.mov, X86Gen.eax, 1);
                    break;
            }

            gen.Tag(X86Gen.Seg.TEXT, logicalEndLabel);
        }

        //private void X86Int(X86Gen gen, X86Gen.Ret lhsRet) {
        //    /// Store the lhs operand.
        //    if (lhsRet == X86Gen.Ret.PTR) {
        //        gen.Inst(X86Gen.push, X86Gen.eax.Addr());
        //    } else {
        //        gen.Inst(X86Gen.push, X86Gen.eax);
        //    }

        //    /// Generate the second operand and mov it to ebx.
        //    var rhsRet = rhs.ToX86Expr(gen);
        //    if (rhsRet == X86Gen.Ret.PTR) {
        //        gen.Inst(X86Gen.mov, X86Gen.ebx, X86Gen.eax.Addr());
        //    } else {
        //        gen.Inst(X86Gen.mov, X86Gen.ebx, X86Gen.eax);
        //    }
        //    /// Pop the lhs operand and calcuate the result.
        //    gen.Inst(X86Gen.pop, X86Gen.eax);
        //    switch (op) {
        //        case SyntaxTree.BiExpr.Op.PLUS:     gen.Inst(X86Gen.add, X86Gen.eax, X86Gen.ebx); break;
        //        case SyntaxTree.BiExpr.Op.MINUS:    gen.Inst(X86Gen.sub, X86Gen.eax, X86Gen.ebx); break;
        //        case SyntaxTree.BiExpr.Op.MULT:     gen.Inst(X86Gen.imul, X86Gen.eax, X86Gen.ebx); break;
        //        case SyntaxTree.BiExpr.Op.DIV:
        //            gen.Inst(X86Gen.cdq);
        //            gen.Inst(X86Gen.idiv, X86Gen.ebx);
        //            break;
        //        case SyntaxTree.BiExpr.Op.LT:
        //        case SyntaxTree.BiExpr.Op.LE:
        //        case SyntaxTree.BiExpr.Op.GT:
        //        case SyntaxTree.BiExpr.Op.GE:
        //        case SyntaxTree.BiExpr.Op.EQ:
        //        case SyntaxTree.BiExpr.Op.NEQ:
        //            gen.Inst(X86Gen.cmp, X86Gen.eax, X86Gen.ebx);
        //            switch (op) {
        //                case SyntaxTree.BiExpr.Op.LE:   gen.Inst(X86Gen.setle, X86Gen.al); break;
        //                case SyntaxTree.BiExpr.Op.LT:   gen.Inst(X86Gen.setl, X86Gen.al); break;
        //                case SyntaxTree.BiExpr.Op.GE:   gen.Inst(X86Gen.setge, X86Gen.al); break;
        //                case SyntaxTree.BiExpr.Op.GT:   gen.Inst(X86Gen.setg, X86Gen.al); break;
        //                case SyntaxTree.BiExpr.Op.EQ:   gen.Inst(X86Gen.sete, X86Gen.al); break;
        //                case SyntaxTree.BiExpr.Op.NEQ:  gen.Inst(X86Gen.setne, X86Gen.al); break;
        //            }
                    
        //            gen.Inst(X86Gen.and, X86Gen.al, 1);
        //            gen.Inst(X86Gen.movzx, X86Gen.eax, X86Gen.al);
        //            break;
        //        default: throw new NotImplementedException();
        //    }
        //}
    }

    public class Cast : Expr {
        public readonly Expr expr;
        public Cast(TUnqualified type, Env env, Expr expr) : base(type.None(), env) {
            this.expr = expr;
        }
        public sealed override string ToString() {
            return string.Format("({0})({1})", type, expr);
        }
        public override X86Gen.Ret ToX86Expr(X86Gen gen) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represent lvalue to rvalue value transformation.
    /// </summary>
    public sealed class LValueCast : Cast {
        public LValueCast(Env env, Expr expr) : base(expr.Type.nake, env, expr) { }
        /// <summary>
        /// LValue transform, simply do nothing.
        /// </summary>
        /// <param name="gen"></param>
        /// <returns></returns>
        public override X86Gen.Ret ToX86Expr(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, ToString());
            return expr.ToX86Expr(gen);
        }
    }

    public sealed class ArrCast : Cast {
        public ArrCast(TArr arr, Env env, Expr expr) : base(arr.element.Ptr().nake, env, expr) { }
        /// <summary>
        /// Array to pointer value transformation.
        /// Assert the ret is ptr, and simply return eax.
        /// </summary>
        /// <param name="gen"></param>
        /// <returns></returns>
        public override X86Gen.Ret ToX86Expr(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, ToString());
            var ret = expr.ToX86Expr(gen);
            Debug.Assert(ret == X86Gen.Ret.PTR);
            return X86Gen.Ret.REG;
        }
    }

    public sealed class FuncCast : Cast {
        public FuncCast(T type, Env env, Expr expr) : base(type.Ptr().nake, env, expr) { }
        /// <summary>
        /// Function designator to function pointer cast.
        /// Simply return reg.
        /// </summary>
        /// <param name="gen"></param>
        /// <returns></returns>
        public override X86Gen.Ret ToX86Expr(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, ToString());
            var ret = expr.ToX86Expr(gen);
            Debug.Assert(ret == X86Gen.Ret.PTR);
            return X86Gen.Ret.REG;
        }
    }

    public sealed class UnaryOp : Expr {
        public readonly Expr expr;
        public readonly SyntaxTree.UnaryOp.Op op;
        public UnaryOp(T type, Env env, Expr expr, SyntaxTree.UnaryOp.Op op) : base(type, env) {
            this.expr = expr;
            this.op = op;
        }
        public override string ToString() {
            return string.Format("{0}({1})", SyntaxTree.UnaryOp.OpToString(op), expr);
        }
        public override X86Gen.Ret ToX86Expr(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, ToString());
            var ret = expr.ToX86Expr(gen);
            switch (op) {
                case SyntaxTree.UnaryOp.Op.REF:
                    Debug.Assert(ret == X86Gen.Ret.PTR);
                    return X86Gen.Ret.REG;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public sealed class ArrSub : Expr {
        public readonly Expr arr;
        public readonly Expr idx;
        public readonly T element;
        public override bool IsLValue => true;
        public ArrSub(T type, Env env, Expr arr, Expr idx, T element) : base(type, env) {
            this.arr = arr;
            this.idx = idx;
            this.element = element;
        }
        public override string ToString() {
            return string.Format("({0})[{1}]", arr, idx);
        }
        public override X86Gen.Ret ToX86Expr(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, ToString());

            /// Evaluate arr.
            var arrRet = arr.ToX86Expr(gen);
            gen.Push(arr.Type, arrRet);


            /// Evaluate the offset.
            var idxRet = idx.ToX86Expr(gen);
            switch (idx.Type.Kind) {
                case TKind.INT:
                    if (idxRet == X86Gen.Ret.PTR) gen.Inst(X86Gen.imul, X86Gen.ebx, X86Gen.eax.Addr(), element.Size);
                    else gen.Inst(X86Gen.imul, X86Gen.ebx, X86Gen.eax, element.Size);
                    break;
                default:
                    throw new NotImplementedException();
            }

            /// Compute the new address.
            gen.Inst(X86Gen.pop, X86Gen.eax);
            gen.Inst(X86Gen.add, X86Gen.eax, X86Gen.ebx);

            return X86Gen.Ret.PTR;
        }
    }

    public sealed class Access : Expr {
        public readonly Expr agg;
        public readonly string field;
        public readonly SyntaxTree.Access.Kind kind;
        public readonly TStructUnion aggType;
        public override bool IsLValue => kind == SyntaxTree.Access.Kind.DOT ? agg.IsLValue : true;
        public Access(T type, Env env, Expr agg, string field, SyntaxTree.Access.Kind kind, TStructUnion aggType)
            : base(type, env) {
            this.agg = agg;
            this.field = field;
            this.kind = kind;
            this.aggType = aggType;
        }
        public override string ToString() {
            return string.Format("({0}){1}{2}", agg, kind == SyntaxTree.Access.Kind.DOT ? "." : "->", field);
        }
        public override X86Gen.Ret ToX86Expr(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, ToString());
            var ret = agg.ToX86Expr(gen);
            var f = aggType.GetField(field);
            if (kind == SyntaxTree.Access.Kind.PTR) {
                if (ret == X86Gen.Ret.REG) {
                    gen.Inst(X86Gen.add, X86Gen.eax, f.Value.offset / 8);
                    return X86Gen.Ret.PTR;
                } else {
                    throw new NotImplementedException();
                }
            } else {
                throw new NotImplementedException();
            }
        }
    }

    public sealed class PostStep : Expr {
        public readonly Expr expr;
        public readonly SyntaxTree.PostStep.Op op;
        public PostStep(T type, Env env, Expr expr, SyntaxTree.PostStep.Op op) : base(type, env) {
            this.expr = expr;
            this.op = op;
        }
        public override string ToString() {
            return string.Format("{0}{1}", expr, SyntaxTree.PostStep.OpToString(op));
        }
        public override X86Gen.Ret ToX86Expr(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, ToString());
            var ret = expr.ToX86Expr(gen);
            Debug.Assert(ret == X86Gen.Ret.PTR);
            switch (type.Kind) {
                case TKind.INT:
                    gen.Inst(X86Gen.mov, X86Gen.ebx, X86Gen.eax.Addr());
                    gen.Inst(op == SyntaxTree.PostStep.Op.DEC ? X86Gen.dec : X86Gen.inc, X86Gen.eax.Addr());
                    gen.Inst(X86Gen.mov, X86Gen.eax, X86Gen.ebx);
                    return X86Gen.Ret.REG;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public sealed class FuncCall : Expr {
        public readonly Expr f;
        public readonly IEnumerable<Expr> args;
        public FuncCall(T type, Env env, Expr f, IEnumerable<Expr> args) : base(type, env) {
            this.f = f;
            this.args = args;
        }
        public override string ToString() {
            return string.Format("{0}({1})", f, args.Aggregate("", (acc, arg) => acc == "" ? arg.ToString() : acc + ", " + arg.ToString()));
        }
        public override X86Gen.Ret ToX86Expr(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, ToString());

            /// Reversely evaluate all the arguments.
            int paramSize = args.Aggregate(0, (acc, arg) => acc + arg.Type.AlignByte);
            foreach (var arg in args.Reverse()) {
                var ret = arg.ToX86Expr(gen);
                gen.Push(arg.Type, ret);
            }

            /// Generate code for function.
            var fRet = f.ToX86Expr(gen);
            gen.Inst(X86Gen.call, fRet == X86Gen.Ret.REG ? X86Gen.eax as X86Gen.Operand : X86Gen.eax.Addr());

            /// Pop out the paramenters.
            gen.Inst(X86Gen.add, X86Gen.esp, paramSize);
            return X86Gen.Ret.REG;
        }
    }

    public sealed class FuncDesignator : Expr {
        public readonly string name;
        public override bool IsLValue => true;
        public FuncDesignator(T type, Env env, string name) : base(type, env) {
            this.name = name;
        }
        public override string ToString() {
            return name;
        }
        public override X86Gen.Ret ToX86Expr(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, name);
            gen.Inst(X86Gen.lea, X86Gen.eax, (new X86Gen.Label("_" + name)).Addr());
            return X86Gen.Ret.PTR;
        }
    }

    /// <summary>
    /// Represent an object.
    /// </summary>
    public sealed class DynamicObjExpr : Expr {
        public readonly string uid;
        public readonly string symbol;
        public override bool IsLValue => true;
        public DynamicObjExpr(T type, Env env, string uid, string symbol) : base(type, env) {
            this.uid = uid;
            this.symbol = symbol;
        }
        public override string ToString() {
            return symbol;
        }
        public override X86Gen.Ret ToX86Expr(X86Gen gen) {
            /// Get the offset to ebp.
            int ebp = env.GetEBP(uid);
            gen.Comment(X86Gen.Seg.TEXT, ToString());
            gen.Inst(X86Gen.lea, X86Gen.eax, X86Gen.ebp.Addr(ebp));
            return X86Gen.Ret.PTR;
        }
    }

    public abstract class ConstExpr : Expr {
        public ConstExpr(TUnqualified type, Env env) : base(type.None(), env) { }
    }

    /// <summary>
    /// Arithmetic constant expression
    ///     - integer constant expression
    ///     - float constant expression
    /// </summary>
    public abstract class ConstArithExpr : ConstExpr {
        public ConstArithExpr(TUnqualified type, Env env) : base(type, env) { }
    }

    /// <summary>
    /// Integer constant expression.
    /// </summary>
    public sealed class ConstIntExpr : ConstArithExpr {
        public readonly TInteger t;
        public readonly BigInteger value;
        public override bool IsConstZero => value == 0;
        public ConstIntExpr(TInteger t, BigInteger value, Env env) : base(t, env) {
            this.t = t;
            this.value = value;
        }
        public override string ToString() {
            return value.ToString();
        }
        public override X86Gen.Ret ToX86Expr(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, ToString());
            if (t.Kind == TKind.INT) {
                gen.Inst(X86Gen.mov, X86Gen.eax, value);
                return X86Gen.Ret.REG;
            }


            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Float constant expression.
    /// </summary>
    public sealed class ConstFloatExpr : ConstArithExpr {
        public readonly TArithmetic t;
        public readonly double value;
        public ConstFloatExpr(TArithmetic t, double value, Env env) : base(t, env) {
            this.t = t;
            this.value = value;
        }
    }

    /// <summary>
    /// Null pointer constant.
    /// </summary>
    public sealed class ConstNullPtr : ConstExpr {
        public override bool IsNullPtr => false;
        public ConstNullPtr(T element, Env env) : base(new TPtr(element), env) { }
    }

    ///// <summary>
    ///// An address constant is a null pointer, a pointer to an lvalue designating an object of static storage duration,
    ///// or a pointer to a function.
    ///// </summary>
    //public abstract class ConstAddrExpr : ConstExpr {
    //}

    ///// <summary>
    ///// A pointer to an lvalue designating an object of static storage duration.
    ///// </summary>
    //public sealed class ConstAddrObj : ConstAddrExpr {
    //    public readonly Obj entry;
    //    public ConstAddrObj(Obj entry) {
    //        this.entry = entry;
    //    }
    //    public override TUnqualified Type => t;
    //}
}
