using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;
using lcc.Token;
using lcc.AST;

namespace lcc.SyntaxTree {

    /// <summary>
    /// The base class for all expression.
    /// </summary>
    public abstract class Expr : Stmt {

        public sealed override AST.Node ToAST(Env env) {
            return GetASTExpr(env);
        }
        public abstract AST.Expr GetASTExpr(Env env);
    }

    public sealed class CommaExpr : Expr, IEquatable<CommaExpr> {

        public CommaExpr(IEnumerable<Expr> exprs) {
            this.exprs = exprs;
        }

        public override Position Pos => exprs.First().Pos;

        public override bool Equals(object obj) {
            return Equals(obj as CommaExpr);
        }

        public bool Equals(CommaExpr x) {
            return x != null && exprs.SequenceEqual(x.exprs);
        }

        public override int GetHashCode() {
            return exprs.Aggregate(0, (acc, expr) => acc ^ expr.GetHashCode());
        }

        public override AST.Expr GetASTExpr(Env env) {
            IEnumerable<AST.Expr> es = exprs.Select(e => e.GetASTExpr(env).ValueTransform());
            return new AST.CommaExpr(es.Last().Type, env.ASTEnv, es);
        }

        public readonly IEnumerable<Expr> exprs;
    }

    public sealed class Assign : Expr, IEquatable<Assign> {

        public enum Op {
            ASSIGN,
            MULEQ,
            DIVEQ,
            MODEQ,
            PLUSEQ,
            MINUSEQ,
            SHIFTLEQ,
            SHIFTREQ,
            BITANDEQ,
            BITXOREQ,
            BITOREQ
        }

        public Assign(Expr lhs, Expr rhs, Op op) {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }

        public override Position Pos => lhs.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as Assign);
        }

        public bool Equals(Assign expr) {
            return expr != null
                && expr.lhs.Equals(lhs)
                && expr.rhs.Equals(rhs)
                && expr.op == op;
        }

        public override int GetHashCode() {
            return lhs.GetHashCode() ^ rhs.GetHashCode() ^ op.GetHashCode();
        }

        /// <summary>
        /// The left operand shall be a modifiable lvalue.
        /// The result shall have the type of the left operand with all the qualifiers omitted and is an rvalue.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr lExpr = lhs.GetASTExpr(env).VTArr().VTFunc();
            AST.Expr rExpr = rhs.GetASTExpr(env).ValueTransform();
            if (!lExpr.IsLValue || lExpr.Type.qualifiers.isConstant) {
                throw new ETypeError(Pos, "left operator of assign operator shall be a modifiable lvalue");
            }

            ETypeError typeError = new ETypeError(Pos, string.Format("illegal type: {0} {1} {2}", lExpr.Type, op, rExpr.Type));

            T resultType = rExpr.Type.nake.None();
            T lElement;

            Func<AST.Assign> Convert = () => {
                AST.Expr converted = rExpr.ImplicitConvert(resultType);
                if (converted == null) {
                    throw new ETypeError(Pos, string.Format("cannot implicitly convert from {0} to {1}", lExpr.Type, resultType));
                }
                return new AST.Assign(resultType, env.ASTEnv, lExpr, converted, op);
            };

            switch (op) {
                case Op.ASSIGN:
                    /// Simple assign. See SimpleAssignable.
                    if (SimpleAssignable(lExpr.Type, rExpr)) {
                        return Convert();
                    }
                    throw new ETypeError(Pos, string.Format("can not assign from {0} to {1}", rExpr.Type, lExpr.Type));
                /// Compound assignment: leave all the conversion to AST.
                case Op.MULEQ:
                case Op.DIVEQ:
                case Op.MODEQ:
                    /// Each of the operands shall have arithmetic type.
                    if (!lExpr.Type.IsArithmetic || !rExpr.Type.IsArithmetic) {
                        throw typeError;
                    }
                    /// The operands of the % operator shall have integer type.
                    if (op == Op.MODEQ) {
                        if (!lExpr.Type.IsInteger || !rExpr.Type.IsInteger) {
                            throw typeError;
                        }
                    }
                    return new AST.Assign(resultType, env.ASTEnv, lExpr, rExpr, op);
                case Op.PLUSEQ:
                case Op.MINUSEQ:
                    /// Either the left operand shall be a pointer to an object type and the right shall have integer type,
                    /// or the left operand shall have qualified or unqualified arithmetic type and the right shall have arithemetic type.
                    if (lExpr.Type.IsPtr) {
                        lElement = (lExpr.Type.nake as TPtr).element;
                        if (lElement.IsObject && rExpr.Type.IsInteger) {
                            return new AST.Assign(resultType, env.ASTEnv, lExpr, rExpr, op);
                        }
                    }
                    if (lExpr.Type.IsArithmetic && rExpr.Type.IsArithmetic) {
                        return new AST.Assign(resultType, env.ASTEnv, lExpr, rExpr, op);
                    }
                    throw typeError;
                case Op.SHIFTLEQ:
                case Op.SHIFTREQ:
                    if (lExpr.Type.IsInteger && rExpr.Type.IsInteger) {
                        return new AST.Assign(resultType, env.ASTEnv, lExpr, rExpr, op);
                    }
                    throw typeError;
                case Op.BITANDEQ:
                case Op.BITXOREQ:
                case Op.BITOREQ:
                    /// Each of the operands shall have integer type.
                    if (lExpr.Type.IsInteger && rExpr.Type.IsInteger) {
                        return new AST.Assign(resultType, env.ASTEnv, lExpr, rExpr, op);
                    }
                    throw typeError;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Check whether this is legal for simple assignment.
        /// </summary>
        /// <param name="lType"></param>
        /// <param name="rExpr"></param>
        /// <returns></returns>
        public static bool SimpleAssignable(T lType, AST.Expr rExpr) {

            /// Simple assignment.
            /// - the left operand has qualified or unqualified arithmetic type and the right has arithmetic type;
            /// - the left operand has a qualified or unqualified version of a structure or union type compatible
            ///   with the type of the right;
            /// - both operands are pointers to qualified or unqualified versions of compatible types, and the type
            ///   pointed to by the left has all the qualifiers of the type pointed to by the right;
            /// - one operand is a pointer to an object or incomplete type and the other is a pointer to a qualified
            ///   or unqualified version of void, and the type pointed to by the left has all the qualifiers of the 
            ///   type pointed to by the right;
            /// - the left operand is a pointer and the right is a null pointer constant;
            /// - the left operand has type _Bool and the right is a pointer.
            /// 
            /// The value of the right operand is converted to the type of the assignment expression.
            if (lType.IsArithmetic && rExpr.Type.IsArithmetic) {
                return true;
            }
            if ((lType.IsStruct || lType.IsUnion) && lType.nake.Compatible(rExpr.Type.nake)) {
                return true;
            }
            if (lType.IsPtr && rExpr.Type.IsPtr) {
                T lElement = (lType.nake as TPtr).element;
                T rElement = (rExpr.Type.nake as TPtr).element;
                if (lElement.nake.Compatible(rElement.nake) && (lElement.qualifiers | rElement.qualifiers).Equals(lElement.qualifiers)) {
                    return true;
                }
                if ((!lElement.IsFunc && rElement.IsVoid) ||
                    (!rElement.IsFunc && lElement.IsVoid)) {
                    if ((lElement.qualifiers | rElement.qualifiers).Equals(lElement.qualifiers)) {
                        return true;
                    }
                }
            }
            if (lType.IsPtr && (rExpr.IsNullPtr || rExpr.IsConstZero)) {
                return true;
            }
            if (lType.Kind == TKind.BOOL && rExpr.Type.IsPtr) {
                return true;
            }

            return false;
        }

        public readonly Expr lhs;
        public readonly Expr rhs;
        public readonly Op op;
    }

    public sealed class CondExpr : Expr, IEquatable<CondExpr> {

        public CondExpr(Expr predicator, Expr trueExpr, Expr falseExpr) {
            this.predicator = predicator;
            this.trueExpr = trueExpr;
            this.falseExpr = falseExpr;
        }

        public override Position Pos => predicator.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as CondExpr);
        }

        public bool Equals(CondExpr x) {
            return x != null && x.predicator.Equals(predicator)
                && x.trueExpr.Equals(trueExpr)
                && x.falseExpr.Equals(falseExpr);
        }

        public override int GetHashCode() {
            return predicator.GetHashCode() ^ trueExpr.GetHashCode() ^ falseExpr.GetHashCode();
        }

        /// <summary>
        /// The first operand shall have scalar type.
        /// One of the following shall hold for the second and the thrid operands:
        /// - both operands have arithmetic types;
        /// - both operands have the same structure or union type;
        /// - both operands have void type;
        /// - both operands are pointer to qualified or unqualified versions of compatible types;
        /// - one operand is a pointer and the other is a null pointer constant/constant integer with value 0;
        /// - one operand is a pointer to an object or incomplete type and the other is a pointer to a qualified
        ///   or unqualified version of void.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr p = predicator.GetASTExpr(env);
            AST.Expr t = trueExpr.GetASTExpr(env);
            AST.Expr f = trueExpr.GetASTExpr(env);
            if (!p.Type.IsScalar) {
                throw new ETypeError(Pos, "the predicator shall have scalar type");
            }
            if (t.Type.IsArithmetic && f.Type.IsArithmetic) {
                var uac = AST.Expr.UsualArithConvert(t, f);
                return new AST.CondExpr(uac.Item1, env.ASTEnv, p, uac.Item2, uac.Item3);
            }
            if ((t.Type.IsStruct || t.Type.IsUnion) && (f.Type.IsStruct || f.Type.IsUnion)) {
                if (t.Type.Equals(f.Type)) {
                    return new AST.CondExpr(t.Type, env.ASTEnv, p, t, f);
                }
            }
            if (t.Type.IsVoid && p.Type.IsVoid) {
                return new AST.CondExpr(t.Type, env.ASTEnv, p, t, f);
            }

            //if (t.Type.IsPtr && f.Type.IsPtr) {
            //    T tElement = (t.Type.nake as TPtr).element;
            //    T fElement = (f.Type.nake as TPtr).element;
            //    if (tElement.Compatible(fElement)) {
            //        return new AST.BiExpr(TInt.Instance.None(), env.ASTEnv, lExpr, rExpr, op);
            //    }
            //    if ((!lElement.IsFunc && rElement.IsVoid) || (!rElement.IsFunc && lElement.IsVoid)) {
            //        return new AST.BiExpr(TInt.Instance.None(), env.ASTEnv, lExpr, rExpr, op);
            //    }
            //}
            //if ((lExpr.Type.IsPtr && (rExpr.IsConstZero || rExpr.IsNullPtr)) ||
            //    (rExpr.Type.IsPtr && (lExpr.IsConstZero || lExpr.IsNullPtr))) {
            //    return new AST.BiExpr(TInt.Instance.None(), env.ASTEnv, lExpr, rExpr, op);
            //}
            throw new ETypeError(Pos, "sorry only partially support the conditional expression.");
        }

        public readonly Expr predicator;
        public readonly Expr trueExpr;
        public readonly Expr falseExpr;
    }

    public sealed class BiExpr : Expr, IEquatable<BiExpr> {

        public enum Op {
            MULT,   // *
            DIV,    // /
            MOD,    // %
            PLUS,   // +
            MINUS,  // -
            LEFT,   // <<
            RIGHT,  // >>
            LT,     // <
            GT,     // >
            LE,     // <=
            GE,     // >=
            EQ,     // ==
            NEQ,    // !=
            AND,    // &
            XOR,    // ^
            OR,     // |
            LOGAND, // &&
            LOGOR   // ||
        }

        public BiExpr(Expr lhs, Expr rhs, Op op) {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }

        public override Position Pos => lhs.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as BiExpr);
        }

        public bool Equals(BiExpr x) {
            return x != null
                && x.lhs.Equals(lhs)
                && x.rhs.Equals(rhs)
                && x.op.Equals(op);
        }

        public override int GetHashCode() {
            return lhs.GetHashCode() ^ rhs.GetHashCode() ^ op.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr lExpr = lhs.GetASTExpr(env).ValueTransform();
            AST.Expr rExpr = rhs.GetASTExpr(env).ValueTransform();

            var typeError = new ETypeError(Pos, string.Format("{0} {1} {2}", lExpr.Type, op, rExpr.Type));

            Tuple<T, AST.Expr, AST.Expr> uac;   // For usual arithmetic conversion.
            T lElement, rElement;
            switch (op) {
                case Op.MULT:
                case Op.DIV:
                case Op.MOD:
                    /// Multiplicative operators.
                    /// Each of the operands shall have arithmetic type.
                    if (!lExpr.Type.IsArithmetic || !rExpr.Type.IsArithmetic) {
                        throw typeError;
                    }
                    /// The operands of the % operator shall have integer type.
                    if (op == Op.MOD) {
                        if (!lExpr.Type.IsInteger || !rExpr.Type.IsInteger) {
                            throw typeError;
                        }
                    }
                    /// The usual arithmetic conversions are performed on the operands.
                    uac = AST.Expr.UsualArithConvert(lExpr, rExpr);
                    return new AST.BiExpr(uac.Item1, env.ASTEnv, uac.Item2, uac.Item3, op);
                case Op.PLUS:
                    /// + operator.
                    /// For addition, either both operands shall have arithmetic type
                    /// or one operand shall be a pointer to an object type and the other shall have integer type.
                    if (lExpr.Type.IsArithmetic && rExpr.Type.IsArithmetic) {
                        /// If both operands have arithmetic type, the usual arithmetic conversions are performed on them.
                        uac = AST.Expr.UsualArithConvert(lExpr, rExpr);
                        return new AST.BiExpr(uac.Item1, env.ASTEnv, uac.Item2, uac.Item3, Op.PLUS);
                    }
                    if ((lExpr.Type.IsPtr && (lExpr.Type.nake as TPtr).element.IsObject && rExpr.Type.IsInteger) ||
                        (rExpr.Type.IsPtr && (rExpr.Type.nake as TPtr).element.IsObject && lExpr.Type.IsInteger)) {
                        AST.Expr ptr = lExpr.Type.IsPtr ? lExpr : rExpr;
                        AST.Expr dif = lExpr.Type.IsPtr ? rExpr : lExpr;
                        /// The result has the type of the pointer operand.
                        /// Make sure that the first operand is the pointer operand in AST node.
                        return new AST.BiExpr(ptr.Type, env.ASTEnv, ptr, dif, Op.PLUS);
                    }
                    throw typeError;
                case Op.MINUS:
                    /// - operator.
                    /// For subtraction, one of the following shall hold:
                    /// 1. Both operands have arithmetic types.
                    /// 2. Both operands are pointers to qualified or unqualified versions of compatible object types.
                    ///    The result shall have type ptrdiff_t, which is int in this implementation.
                    /// 3. The left operand is a pointer to an object type and the right operand has integer type.
                    ///    
                    /// Do not worry about "one past the last element" requirements.
                    if (lExpr.Type.IsArithmetic && rExpr.Type.IsArithmetic) {
                        uac = AST.Expr.UsualArithConvert(lExpr, rExpr);
                        return new AST.BiExpr(uac.Item1, env.ASTEnv, uac.Item2, uac.Item3, Op.MINUS);
                    }
                    if (lExpr.Type.IsPtr) {
                        lElement = (lExpr.Type.nake as TPtr).element;
                        if (lElement.IsObject) {
                            if (rExpr.Type.IsInteger) {
                                return new AST.BiExpr(lExpr.Type, env.ASTEnv, lExpr, rExpr, Op.MINUS);
                            } else if (rExpr.Type.IsPtr) {
                                rElement = (rExpr.Type.nake as TPtr).element;
                                if (rElement.IsObject && lElement.nake.Compatible(rElement.nake)) {
                                    return new AST.BiExpr(TInt.Instance.None(), env.ASTEnv, lExpr, rExpr, Op.MINUS);
                                }
                            }
                        }
                    }
                    throw typeError;
                case Op.LEFT:
                case Op.RIGHT:
                    /// Bitwise shift operators.
                    /// Each of the operands shall have integer types.
                    /// The integer promotions are performed on each of the operands.
                    /// The type of the result is that of the promoted left operand.
                    if (lExpr.Type.IsInteger && rExpr.Type.IsInteger) {
                        AST.Expr lPromoted = lExpr.IntPromote();
                        AST.Expr rPromoted = rExpr.IntPromote();
                        return new AST.BiExpr(lPromoted.Type, env.ASTEnv, lPromoted, rPromoted, op);
                    }
                    throw typeError;
                case Op.LT:
                case Op.GT:
                case Op.LE:
                case Op.GE:
                    /// Relational operators.
                    /// One of the following shall hold:
                    /// 1. Both operands have real type.
                    /// 2. Both operands are pointers to qualified or unqualified versions of compatible object types.
                    /// 3. Both operands are pointers to qualified or unqualified versions of compatible incomplete types.
                    /// The result has type int.
                    if (lExpr.Type.IsReal && rExpr.Type.IsReal) {
                        /// The usual arithmetic conversions are performed.
                        uac = AST.Expr.UsualArithConvert(lExpr, rExpr);
                        return new AST.BiExpr(TInt.Instance.None(), env.ASTEnv, uac.Item2, uac.Item3, op);
                    }
                    if (lExpr.Type.IsPtr && rExpr.Type.IsPtr) {
                        lElement = (lExpr.Type.nake as TPtr).element;
                        rElement = (rExpr.Type.nake as TPtr).element;
                        if (lElement.IsObject && rElement.IsObject && lElement.nake.Compatible(rElement.nake)) {
                            return new AST.BiExpr(TInt.Instance.None(), env.ASTEnv, lExpr, rExpr, op);
                        }
                        if (!lElement.IsComplete && !rElement.IsComplete && lElement.nake.Compatible(rElement.nake)) {
                            return new AST.BiExpr(TInt.Instance.None(), env.ASTEnv, lExpr, rExpr, op);
                        }
                    }
                    throw typeError;
                case Op.EQ:
                case Op.NEQ:
                    /// Equality operators.
                    /// One of the following shall hold:
                    /// 1. Both operands have arithmetic type.
                    /// 2. Both operands are pointers to qualified or unqualified versions of compatible types.
                    /// 3. One operand is a pointer to an object or incomplete type and the other is a pointer to
                    ///    a qualified or unqualified version of void.
                    /// 4. One operand is a pointer and the other is a null pointer constant.
                    /// 5. One operand is a pointer and the other is a integer constant with value 0.
                    if (lExpr.Type.IsArithmetic && rExpr.Type.IsArithmetic) {
                        uac = AST.Expr.UsualArithConvert(lExpr, rExpr);
                        return new AST.BiExpr(TInt.Instance.None(), env.ASTEnv, uac.Item2, uac.Item3, op);
                    }
                    if (lExpr.Type.IsPtr && rExpr.Type.IsPtr) {
                        lElement = (lExpr.Type.nake as TPtr).element;
                        rElement = (rExpr.Type.nake as TPtr).element;
                        if (lElement.nake.Compatible(rElement.nake)) {
                            return new AST.BiExpr(TInt.Instance.None(), env.ASTEnv, lExpr, rExpr, op);
                        }
                        if ((!lElement.IsFunc && rElement.IsVoid) || (!rElement.IsFunc && lElement.IsVoid)) {
                            return new AST.BiExpr(TInt.Instance.None(), env.ASTEnv, lExpr, rExpr, op);
                        }
                    }
                    if ((lExpr.Type.IsPtr && (rExpr.IsConstZero || rExpr.IsNullPtr)) ||
                        (rExpr.Type.IsPtr && (lExpr.IsConstZero || lExpr.IsNullPtr))) {
                        return new AST.BiExpr(TInt.Instance.None(), env.ASTEnv, lExpr, rExpr, op);
                    }
                    throw typeError;
                case Op.AND:
                case Op.XOR:
                case Op.OR:
                    /// Bitwise &, ^, | operator.
                    /// Each of the operands shall have integer type.
                    if (lExpr.Type.IsInteger && rExpr.Type.IsInteger) {
                        uac = AST.Expr.UsualArithConvert(lExpr, rExpr);
                        return new AST.BiExpr(uac.Item1, env.ASTEnv, uac.Item2, uac.Item3, op);
                    }
                    throw typeError;
                case Op.LOGAND:
                case Op.LOGOR:
                    /// &&, || operators.
                    /// Each of the operands shall have scalar type.
                    /// The result has type int.
                    if (lExpr.Type.IsScalar && rExpr.Type.IsScalar) {
                        return new AST.BiExpr(TInt.Instance.None(), env.ASTEnv, lExpr, rExpr, op);
                    }
                    throw typeError;
                default:
                    throw new NotImplementedException();
            }
        }

        public readonly Expr lhs;
        public readonly Expr rhs;
        public readonly Op op;
    }


    /// <summary>
    /// ( type-name ) cast-expression
    /// </summary>
    public sealed class Cast : Expr, IEquatable<Cast> {

        public Cast(TypeName name, Expr expr) {
            this.name = name;
            this.expr = expr;
        }

        public override Position Pos => name.Pos;
        public bool Equals(Cast x) {
            return x != null && x.name.Equals(name) && x.expr.Equals(expr);
        }

        public override bool Equals(object obj) {
            return Equals(obj as Cast);
        }

        public override int GetHashCode() {
            return name.GetHashCode() ^ expr.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr e = expr.GetASTExpr(env).ValueTransform();
            if (e == null) {
                return null;
            }

            T type = name.GetT(env);

            /// Unless the type name specifies a void type, the type name shall specify qualified or unqualified
            /// scalar type and the operand shall have scalar type.
            if (!type.IsVoid) {
                if (!type.IsScalar) {
                    throw new Error(Pos, "the type name shall specify scalar type");
                }
                if (!e.Type.IsScalar) {
                    throw new Error(Pos, "the operand shall have scalar type");
                }
            }

            /// The following cast is illegal.
            /// 1. Conversions between pointers and floating types.
            /// 2. Conversions between pointers to functions and pointers to objects (including void*)
            /// 3. Conversions between pointers to functions and integers.
            /// http://en.cppreference.com/w/c/language/cast
            CheckIllegalCast(type.nake, e.Type.nake);
            CheckIllegalCast(e.Type.nake, type.nake);

            /// If the value of the expression is represented with greater precision or range than
            /// required by the type named by the cast, then the cast specifies a conversion even
            /// if the type of the expression is the same as the named type.
            /// NOTE: This is used for constant expression that has float type. In this implementation
            /// all constant float expression are represented in double.
            /// TODO: Support constant expression cast.

            /// A cast that specifies no conversion has no effect on the type or value of an expression.
            /// However, the result of a cast is always rvalue, therefore we still need to new VarExpr.
            /// Notice that since the result is rvalue, the qualifiers will be discarded.
            return new AST.Cast(type.nake, env.ASTEnv, e);
        }

        private void CheckIllegalCast(TUnqualified s, TUnqualified t) {
            TPtr p = s as TPtr;
            if (p != null && t.IsFloat) {
                throw new Error(Pos, "cannot cast between pointers and floating types");
            }
            if (p != null && p.element.IsFunc && t.IsInteger) {
                throw new Error(Pos, "cannot cast between pointers to functions and integers");
            }
            TPtr q = t as TPtr;
            if (p != null && p.element.IsFunc && q != null && q.element.IsObject) {
                throw new Error(Pos, "cannot cast between pointers to functions and pointers to objects");
            }
        }

        public readonly TypeName name;
        public readonly Expr expr;
    }

    /// <summary>
    /// ++i
    /// --i
    /// </summary>
    public sealed class PreStep : Expr, IEquatable<PreStep> {

        public enum Kind {
            INC,
            DEC
        }

        public PreStep(Expr expr, Kind kind) {
            this.expr = expr;
            this.kind = kind;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as PreStep);
        }

        public bool Equals(PreStep x) {
            return x != null && x.expr.Equals(expr) && x.kind == kind;
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        /// <summary>
        /// ++E is equivalent to E += 1.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr e = expr.GetASTExpr(env).VTArr().VTFunc();
            if (!e.Type.IsReal && !e.Type.IsPtr) {
                throw new Error(expr.Pos, string.Format("{0} on type {1}", kind, e.Type));
            }
            if (!e.IsLValue) {
                throw new Error(Pos, "cannot modify rvalue");
            }
            if (e.Type.qualifiers.isConstant) {
                throw new Error(Pos, "cannot modify constant value");
            }
            return new AST.Assign(e.Type.nake.None(), env.ASTEnv, e,
                new AST.ConstIntExpr(TInt.Instance, 0, env.ASTEnv), kind == Kind.INC ? Assign.Op.PLUSEQ : Assign.Op.MINUSEQ);
        }

        public readonly Expr expr;
        public readonly Kind kind;
    }

    /// <summary>
    /// & * + - ~ !
    /// </summary>
    public sealed class UnaryOp : Expr, IEquatable<UnaryOp> {

        public enum Op {
            /// <summary>
            /// &
            /// </summary>
            REF,
            /// <summary>
            /// *
            /// </summary>
            STAR,
            /// <summary>
            /// +
            /// </summary>
            PLUS,
            /// <summary>
            /// -
            /// </summary>
            MINUS,
            /// <summary>
            /// ~
            /// </summary>
            REVERSE,
            /// <summary>
            /// ！
            /// </summary>
            NOT
        }

        public UnaryOp(Expr expr, Op op) {
            this.expr = expr;
            this.op = op;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as UnaryOp);
        }

        public bool Equals(UnaryOp x) {
            return x != null && x.expr.Equals(expr) && x.op == op;
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr e = op == Op.REF ? expr.GetASTExpr(env) : expr.GetASTExpr(env).ValueTransform();
            switch (op) {
                case Op.REF:
                    /// The operand of the unary & operator shall be either a function designator, 
                    /// the result of a [] or unary * operator, or an lvalue that designates an object
                    /// that is not as bit-FieldAccessException and is not declared with the register
                    /// storage-class specifier.
                    if (e is AST.ArrSub) {
                        // The & operator is removed and the [] operator were changed to a + operator.
                        AST.ArrSub x = e as AST.ArrSub;
                        return new AST.BiExpr(x.arr.Type, env.ASTEnv, x.arr, x.idx, BiExpr.Op.PLUS);
                    }
                    if (e is AST.UnaryOp && (e as AST.UnaryOp).op == Op.STAR) {
                        // If the operand is the result of a unary * operator, neither that operator nor the &
                        // operator is evaluated and the result is as if both were omitted, except that the constraints
                        // on the opoerators still apply and the result is not an lvalue.
                        // To make sure the result is an rvalue, change the & and * operator to + operator with the second
                        // operand 0.
                        AST.UnaryOp x = e as AST.UnaryOp;
                        return new AST.BiExpr(x.expr.Type, env.ASTEnv, x.expr, new AST.ConstIntExpr(TInt.Instance, 0, env.ASTEnv), BiExpr.Op.PLUS);
                    }
                    if (!e.IsLValue) {
                        throw new Error(Pos, "cannot take address of rvalue");
                    }
                    if (e.Type.IsBitField) {
                        throw new Error(Pos, "cannot take address of bit-field");
                    }
                    // TODO: Check register storage-class specifier.
                    return new AST.UnaryOp(e.Type.Ptr(), env.ASTEnv, e, Op.REF);
                case Op.STAR:
                    /// The operand of the uanry * operator shall have pointer type.
                    if (!e.Type.IsPtr) {
                        throw new Error(Pos, "cannot deref none pointer type");
                    }
                    return new AST.UnaryOp((e.Type.nake as TPtr).element, env.ASTEnv, e, Op.STAR);
                case Op.PLUS:
                case Op.MINUS:
                    /// The operand of the unary + or - operator shall have arithmetic type.
                    /// The result has integer promoted type.
                    if (!e.Type.IsArithmetic) {
                        throw new Error(Pos, "the operand of the unary + or - operator shall have arithmetic type");
                    }
                    return new AST.UnaryOp(e.Type.IntPromote(), env.ASTEnv, e, op);
                case Op.REVERSE:
                    /// Of the ~ operator, integer type.
                    /// The result has integer promoted type.
                    if (!e.Type.IsInteger) {
                        throw new Error(Pos, "the operand of the unary ~ operator shall have integer type");
                    }
                    return new AST.UnaryOp(e.Type.IntPromote(), env.ASTEnv, e, Op.REVERSE);
                case Op.NOT:
                    /// Of the ! operator, scalar type.
                    /// The result has type int.
                    if (!e.Type.IsScalar) {
                        throw new Error(Pos, "the operand of the unary ! operator shall have scalar type");
                    }
                    return new AST.UnaryOp(TInt.Instance.None(), env.ASTEnv, e, Op.NOT);
                default:
                    throw new InvalidOperationException("Unknown unary opeartor");
            }
        }

        public readonly Expr expr;
        public readonly Op op;
    }


    /// <summary>
    /// sizeof unary-expression
    /// sizeof ( type-name )
    /// </summary>
    public sealed class SizeOf : Expr, IEquatable<SizeOf> {

        public SizeOf(Expr expr) {
            this.expr = expr;
        }

        public SizeOf(TypeName name) {
            this.name = name;
        }

        public override Position Pos => expr == null ? name.Pos : expr.Pos;


        public bool Equals(SizeOf x) {
            return x != null && NullableEquals(x.name, name) && NullableEquals(x.expr, expr);
        }

        public override bool Equals(object obj) {
            return Equals(obj as SizeOf);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            T type;
            AST.Expr e;
            if (name != null) {
                type = name.GetT(env);
            } else {
                e = expr.GetASTExpr(env);
                type = e.Type;
            }

            if (type.IsVarArray) {
                throw new Error(Pos, "do not support variable length array");
            }

            /// The sizeof operator shall not be applied to an expression that has function type or an incomplete type,
            /// to the parenthesized name of such a type, or to an expression that designates a bit-field member.
            if (type.IsFunc) {
                throw new Error(Pos, "cannot apply sizeof to function type");
            }
            if (!type.IsComplete) {
                throw new Error(Pos, "cannot apply sizeof to incomplete type");
            }
            if (type.IsBitField) {
                throw new Error(Pos, "cannot apply sizeof to bit-field member");
            }

            /// The sizeof operator yields the size (in bytes) of its operand.
            /// The result has type an unsigned integer type size_t, which is defined in "stddef.h".
            /// Here I take unsigned int.
            return new AST.ConstIntExpr(TUInt.Instance, type.Size, env.ASTEnv);

        }

        /// <summary>
        /// Nullable.
        /// </summary>
        public readonly Expr expr;

        /// <summary>
        /// Nullable.
        /// </summary>
        public readonly TypeName name;
    }

    /// <summary>
    /// arr[idx].
    /// </summary>
    public sealed class ArrSub : Expr, IEquatable<ArrSub> {

        public ArrSub(Expr arr, Expr idx) {
            this.arr = arr;
            this.idx = idx;
        }

        public override Position Pos => arr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ArrSub);
        }

        public bool Equals(ArrSub x) {
            return x != null && x.arr.Equals(arr) && x.idx.Equals(idx);
        }

        public override int GetHashCode() {
            return arr.GetHashCode() ^ idx.GetHashCode();
        }

        /// <summary>
        /// arr[idx] where
        ///     - arr -> pointer to object T
        ///     - idx -> integer type
        /// 
        /// Returns AST expr.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr arrExpr = arr.GetASTExpr(env).ValueTransform();
            if (!arrExpr.Type.IsPtr) {
                throw new Error(arr.Pos, "subscripting none pointer type");
            }

            TPtr ptr = arrExpr.Type.nake as TPtr;
            if (!ptr.element.IsObject) {
                throw new Error(arr.Pos, "subscripting pointer to function");
            }

            AST.Expr idxExpr = idx.GetASTExpr(env);
            if (!idxExpr.Type.IsInteger) {
                throw new Error(idx.Pos, "subscripting none integer type");
            }

            return new AST.ArrSub(ptr.element, env.ASTEnv, arrExpr, idxExpr);
        }

        public readonly Expr arr;
        public readonly Expr idx;
    }

    /// <summary>
    /// s.i
    /// p->i
    /// </summary>
    public sealed class Access : Expr, IEquatable<Access> {

        public enum Kind {
            DOT,
            PTR
        }

        public Access(Expr aggregation, T_IDENTIFIER token, Kind type) {
            this.agg = aggregation;
            this.field = token.name;
            this.kind = type;
        }

        public override Position Pos => agg.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as Access);
        }

        public bool Equals(Access x) {
            return x != null && x.agg.Equals(agg)
                && x.field.Equals(field)
                && x.kind == kind;
        }

        public override int GetHashCode() {
            return agg.GetHashCode() ^ field.GetHashCode();
        }

        /// <summary>
        /// 
        /// A postfix expression followed by the DOT operator and an identifier designates a member.
        /// The value of the member is an lvalue if the the first expression is an lvalue.
        /// The result is a so-qualified version of the type of the designated member.
        /// 
        /// A postfix expression followed by the -> operator and an identifier designates a member.
        /// The value of the member is always an lvalue!
        /// The result is a so-qualified version of the type of the designated member.
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr aggExpr = kind == Kind.DOT ? agg.GetASTExpr(env).VTArr().VTFunc() : agg.GetASTExpr(env).ValueTransform();
            T aggType = aggExpr.Type;

            if (kind == Kind.PTR) {
                if (!aggType.IsPtr)
                    throw new Error(agg.Pos, "member reference base type is not a pointer");
                aggType = (aggType.nake as TPtr).element;
            }

            if (!aggType.IsStruct && !aggType.IsUnion)
                throw new Error(agg.Pos, "member reference base type is not a struct or union");

            TStructUnion s = aggType.nake as TStructUnion;

            T m = s.GetType(field);
            if (m == null) {
                throw new Error(agg.Pos, string.Format("no member named {0} in {1}", field, s.ToString()));
            }

            return new AST.Access(aggType.Unnest(m), env.ASTEnv, aggExpr, field, kind);
        }

        public readonly Expr agg;
        public readonly string field;
        public readonly Kind kind;
    }

    /// <summary>
    /// i++
    /// i--
    /// </summary>
    public sealed class PostStep : Expr, IEquatable<PostStep> {

        public enum Kind {
            INC,
            DEC
        }

        public PostStep(Expr expr, Kind kind) {
            this.expr = expr;
            this.kind = kind;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as PostStep);
        }

        public bool Equals(PostStep x) {
            return x != null && x.expr.Equals(expr) && x.kind == kind;
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        /// <summary>
        /// From C99 standard:
        /// "
        ///   The operand of the postfix increment or decrement operator shall have qualified
        /// or unqualified real or pointer type and shall be a modifiable lvalue.
        /// "
        ///  
        /// However test with clang and gcc shows that complex type can also be incremented.
        /// 
        /// Returns the same type but as rvalue.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr e = expr.GetASTExpr(env).VTArr().VTFunc();
            if (!e.Type.IsPtr && !e.Type.IsReal) {
                throw new Error(expr.Pos, string.Format("{0} on type {1}", kind, e.Type));
            }

            if (!e.IsLValue) {
                throw new Error(expr.Pos, "cannot modify rvalue");
            }

            if (e.Type.qualifiers.isConstant) {
                throw new Error(expr.Pos, "cannot modify constant value");
            }

            return new AST.PostStep(e.Type, env.ASTEnv, e, kind);
        }

        public readonly Expr expr;
        public readonly Kind kind;
    }

    /// <summary>
    /// ( type-name ) { initializer-list [,] }
    /// </summary>
    public sealed class Compound : Expr, IEquatable<Compound> {

        public Compound(TypeName name, IEnumerable<STInitItem> inits) {
            this.name = name;
            this.inits = inits;
        }

        public bool Equals(Compound x) {
            return x != null && x.name.Equals(name) && x.inits.SequenceEqual(inits);
        }

        public override bool Equals(object obj) {
            return Equals(obj as Compound);
        }

        public override int GetHashCode() {
            return name.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            throw new NotImplementedException();
        }

        public override Position Pos => name.Pos;

        public readonly TypeName name;
        public readonly IEnumerable<STInitItem> inits;
    }

    /// <summary>
    /// postfix-expression ( argument-expression-list_opt )
    /// </summary>
    public sealed class FuncCall : Expr, IEquatable<FuncCall> {

        public FuncCall(Expr expr, IEnumerable<Expr> args) {
            this.expr = expr;
            this.args = args;
        }

        public override Position Pos => expr.Pos;

        public bool Equals(FuncCall x) {
            return x != null && x.expr.Equals(expr) && x.args.SequenceEqual(args);
        }

        public override bool Equals(object obj) {
            return Equals(obj as FuncCall);
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        /// <summary>
        /// The expression that denotes the called function shall have type pointer to function
        /// returning void or returning an object type other than an array type.
        /// 
        /// In this implementation, all function should have prototype.
        /// 
        /// If the function has a prototype, the number of arguments shall agree with the number of
        /// parameters. Each argument shall have a type such that its value may be assigned to an 
        /// object with the unqualified version of the type of its corresponding parameter.
        /// 
        /// The ellipsis notation in a function prototype declarator causes argument type conversion
        /// to stop after the last declared parameter. The default argument promotions are performed on
        /// tailing arguments.
        /// 
        /// Default argument promotion:
        /// - integer promotion;
        /// - float -> doubl.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr fExpr = expr.GetASTExpr(env).ValueTransform();
            if (fExpr.Type.IsPtr) {
                TFunc f = (fExpr.Type.nake as TPtr).element.nake as TFunc;
                if (f != null) {
                    if (!f.isEllipis) {
                        if (args.Count() != f.parameters.Count()) {
                            throw new ETypeError(Pos, string.Format("expect {0} parameters, {1} given", f.parameters.Count(), args.Count()));
                        }
                    } else {
                        if (args.Count() < f.parameters.Count()) {
                            throw new ETypeError(Pos, string.Format("expect at least {0} parameters, {1} given", f.parameters.Count(), args.Count()));
                        }
                    }
                    var p = f.parameters.GetEnumerator();
                    var a = args.GetEnumerator();
                    LinkedList<AST.Expr> aExprs = new LinkedList<AST.Expr>();
                    while (a.MoveNext()) {
                        AST.Expr aExpr = a.Current.GetASTExpr(env).ValueTransform();
                        if (!aExpr.Type.IsObject) {
                            throw new ETypeError(Pos, string.Format("expect object type for argument"));
                        }
                        if (p.MoveNext()) {
                            // Type check for the arguments.
                            AST.Expr converted = aExpr.ImplicitConvert(p.Current.nake.None());
                            if (converted == null) {
                                throw new ETypeError(Pos, string.Format("expect {0}, actual {1}", p.Current, aExpr.Type));
                            }
                            aExprs.AddLast(converted);
                        } else {
                            // Tailing arguments.
                            // Default argument promotion.
                            if (aExpr.Type.IsInteger) {
                                aExpr = aExpr.IntPromote();
                            } else if (aExpr.Type.Kind == TKind.SINGLE) {
                                aExpr = new AST.Cast(TDouble.Instance, aExpr.Envrionment, aExpr);
                            }
                            aExprs.AddLast(aExpr);
                        }
                    }
                    return new AST.FuncCall(f.ret, env.ASTEnv, fExpr, aExprs);
                }
            }
            throw new ETypeError(Pos, string.Format("called illegal type {0}", fExpr.Type));
        }
        public readonly Expr expr;
        public readonly IEnumerable<Expr> args;
    }

    /// <summary>
    /// Represents an identfier.
    /// </summary>
    public sealed class Id : Expr, IEquatable<Id> {

        public Id(T_IDENTIFIER token) {
            pos = new Position { line = token.line };
            symbol = token.name;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as Id);
        }

        public bool Equals(Id x) {
            return x != null && x.symbol.Equals(symbol) && x.pos.Equals(pos);
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            var entry = env.GetSymbol(symbol);
            if (entry == null) {
                // This is an undeclared identifier.
                throw new ErrUndefinedIdentifier(pos, symbol);
            } else {
                switch (entry.kind) {
                    case SymbolEntry.Kind.OBJ:
                        return new AST.ObjExpr(entry.type, env.ASTEnv, symbol);
                    case SymbolEntry.Kind.FUNC:
                        return new AST.FuncDesignator(entry.type, env.ASTEnv, symbol);
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public static bool IsReservedIdentifier(string symbol) {
            return symbol.Length > 2 && symbol.Substring(0, 2) == "__";
        }

        public readonly string symbol;

        private readonly Position pos;
    }

    public abstract class Constant : Expr {

        public Constant(Position pos) {
            this.pos = pos;
        }

        public override Position Pos => pos;

        protected static Func<char, bool> IsOctal = (char c) => c >= '0' && c <= '7';
        protected static Func<char, bool> IsDecimal = (char c) => c >= '0' && c <= '9';
        protected static Func<char, bool> IsHexadecimal = (char c) => IsDecimal(c) || char.ToLower(c) >= 'a' && char.ToLower(c) <= 'f';
        protected static Func<char, int> GetHexadecimal = (char c) => {
            if (IsDecimal(c)) return c - '0';
            else return char.ToLower(c) - 'a' + 10;
        };

        protected readonly Position pos;
    }

    /// <summary>
    /// Represent an integer constant with its value and its type (rvalue).
    /// The type of an integer constant is the first of the corresponding list in which its value can be represented.
    /// 
    /// Sufix               Decimal Constant                Octal or Hexadecimal Constant
    /// ------------------------------------------------------------------------------------------
    /// NONE                int                             int
    ///                     long int                        unsigned int
    ///                     long long int                   long int
    ///                                                     unsigned long int
    ///                                                     long long int
    ///                                                     unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// U                   unsigned int                    unsigned int
    ///                     unsigned long int               unsigned long int
    ///                     unsigned long long int          unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// L                   long int                        long int
    ///                     long long int                   unsigned long int
    ///                                                     long long int
    ///                                                     unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// UL                  unsigned long int               unsigned long int
    ///                     unsigned long long int          unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// LL                  long long int                   long long int
    ///                                                     unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// ULL                 unsigned long long int          unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// 
    /// </summary>
    public sealed class ConstInt : Constant, IEquatable<ConstInt> {

        public ConstInt(T_CONST_INT token) : base(new Position { line = token.line }) {
            value = Evaluate(token);

            // Select the proper type for this constant.
            switch (token.suffix) {
                case T_CONST_INT.Suffix.NONE:
                    if (token.n != 10)
                        // Octal or hexadecimal constant.
                        type = FitInType(pos, value,
                            TInt.Instance,
                            TUInt.Instance,
                            TLong.Instance,
                            TULong.Instance,
                            TLLong.Instance,
                            TULLong.Instance);
                    else
                        // Decimal constant.
                        type = FitInType(pos, value,
                            TInt.Instance,
                            TLong.Instance,
                            TLLong.Instance);
                    break;
                case T_CONST_INT.Suffix.U:
                    type = FitInType(pos, value,
                        TUInt.Instance,
                        TULong.Instance,
                        TULLong.Instance);
                    break;
                case T_CONST_INT.Suffix.L:
                    if (token.n != 10)
                        type = FitInType(pos, value,
                            TLong.Instance,
                            TULong.Instance,
                            TLLong.Instance,
                            TULLong.Instance);
                    else
                        type = FitInType(pos, value,
                            TLong.Instance,
                            TLLong.Instance);
                    break;
                case T_CONST_INT.Suffix.UL:
                    type = FitInType(pos, value,
                        TULong.Instance,
                        TULLong.Instance);
                    break;
                case T_CONST_INT.Suffix.LL:
                    if (token.n != 10)
                        type = FitInType(pos, value,
                            TLLong.Instance,
                            TULLong.Instance);
                    else
                        type = FitInType(pos, value, TLLong.Instance);
                    break;
                case T_CONST_INT.Suffix.ULL:
                    type = FitInType(pos, value, TULLong.Instance);
                    break;
            }
        }

        public override bool Equals(object obj) {
            return Equals(obj as ConstInt);
        }

        public bool Equals(ConstInt x) {
            return x != null && x.pos.Equals(pos)
                && x.value == value
                && x.type.Equals(type);
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            return new AST.ConstIntExpr(type, value, env.ASTEnv);
        }

        /// <summary>
        /// Evaluate the text and get the value.
        /// </summary>
        /// <param name="token"> Token to be evaluated. </param>
        /// <returns> BigInteger. </returns>
        private static BigInteger Evaluate(T_CONST_INT token) {

            BigInteger value = 0;

            foreach (var c in token.text) {
                value = value * token.n + GetHexadecimal(c);
            }

            return value;
        }

        private static TInteger FitInType(Position pos, BigInteger value, params TInteger[] types) {
            foreach (var type in types) {
                if (value >= type.MIN && value <= type.MAX) {
                    return type;
                }
            }
            throw new ErrIntegerLiteralOutOfRange(pos);
        }

        public readonly BigInteger value;
        public readonly TInteger type;
    }

    /// <summary>
    /// A character is either an octal sequence, a hexadecimal sequence or an ascii character.
    /// An octal sequence and a hexadecimal sequence shall be in the range of representable values for the type unsigned char.
    /// </summary>
    public sealed class ConstChar : Constant {

        public ConstChar(T_CONST_CHAR token) : base(new Position { line = token.line }) {

            // Do not support multi-character characer.
            if (token.prefix == T_CONST_CHAR.Prefix.L) {
                throw new EUnknownType(pos, "multi-character");
            }

            values = Evaluate(pos, token.text);

            // Do not support multi-character characer.
            if (values.Count() > 1) {
                throw new EUnknownType(pos, "multi-character");
            }
        }

        public override bool Equals(object obj) {
            return Equals(obj as ConstChar);
        }

        public bool Equals(ConstChar x) {
            return x != null && x.pos.Equals(pos)
                && x.values.SequenceEqual(values);
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            return new AST.ConstIntExpr(TChar.Instance, values.First(), env.ASTEnv);
        }

        /// <summary>
        /// Evaluate the text to a list of integer character constant.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<ushort> Evaluate(Position pos, string text) {

            BigInteger value = 0;
            LinkedList<ushort> values = new LinkedList<ushort>();

            // state 0: initial state.
            // state 1: \.
            // state 2: \x.
            // state 3: \o.
            // state 4: \oo.

            const int INITIAL = 0;
            const int ESCAPED = 1;
            const int HEXADEC = 2;
            const int OCTAL1 = 3;
            const int OCTAL2 = 4;

            int state = INITIAL;

            // Check the value is within the range of unsigned char
            // and push it into the list.
            Action<BigInteger> PushValue = v => {
                if (v < TUChar.Instance.MIN || v > TUChar.Instance.MAX) {
                    throw new ErrEscapedSequenceOutOfRange(pos, text);
                }
                values.AddLast((ushort)v);  // Store the current result.
            };

            for (int i = 0; i < text.Length; ++i) {

                switch (state) {
                    case INITIAL:
                        // Initial state.
                        if (text[i] == '\\') {
                            state = ESCAPED;
                        } else {
                            values.AddLast(text[i]);
                        }
                        break;
                    case ESCAPED:
                        // Seen one backslash.
                        if (text[i] == 'x') {
                            state = HEXADEC;
                        } else if (IsOctal(text[i])) {
                            value = value * 8 + (text[i] - '0');
                            state = OCTAL1;
                        } else {
                            state = INITIAL;
                            switch (text[i]) {
                                case 'a':
                                    values.AddLast(7);
                                    break;
                                case 'b':
                                    values.AddLast(8);
                                    break;
                                case 'f':
                                    values.AddLast(12);
                                    break;
                                case 'n':
                                    values.AddLast(10);
                                    break;
                                case 'r':
                                    values.AddLast(13);
                                    break;
                                case 't':
                                    values.AddLast(9);
                                    break;
                                case 'v':
                                    values.AddLast(11);
                                    break;
                                default:
                                    values.AddLast(text[i]);
                                    break;
                            }
                        }
                        break;
                    case HEXADEC:
                        if (IsHexadecimal(text[i])) {
                            value = value * 16 + GetHexadecimal(text[i]);
                        } else {
                            // Check the value is within the range of unsinged char.
                            PushValue(value);
                            value = 0;              // Reset the value to zero;
                            i--;                    // Backward one char.
                            state = INITIAL;        // Reset the state.
                        }
                        break;
                    case OCTAL1:
                        if (IsOctal(text[i])) {
                            value = value * 8 + (text[i] - '0');
                            state = OCTAL2;
                        } else {
                            // Check the value is within the range of unsinged char.
                            PushValue(value);
                            value = 0;
                            i--;
                            state = INITIAL;
                        }
                        break;
                    case OCTAL2:
                        if (IsOctal(text[i])) {
                            value = value * 8 + (text[i] - '0');
                        } else {
                            i--;
                        }
                        // Check the value is within the range of unsinged char.
                        PushValue(value);
                        state = INITIAL;
                        value = 0;
                        break;
                }
            }

            // Must in escaped sequence.
            if (state != INITIAL) {
                PushValue(value);
            }

            return values;
        }

        /// <summary>
        /// Store the values from Evaluate().
        /// </summary>
        public readonly IEnumerable<ushort> values;
    }

    /// <summary>
    /// A float constant is composed with
    /// 
    /// significant-part [eEpP] exponent-part
    /// 
    /// whole-part . fraction-part [eEpP] exponent-part.
    /// 
    /// Significant-part is processed as rational number with base 10 or 16.
    /// Exponent-part is processes as a decimal integer, to which 10 or 2 will be raised.
    /// 
    /// </summary>
    public sealed class ConstFloat : Constant, IEquatable<ConstFloat> {
        public ConstFloat(T_CONST_FLOAT token) : base(new Position { line = token.line }) {
            value = Evaluate(token);
            switch (token.suffix) {
                case T_CONST_FLOAT.Suffix.NONE:
                    t = TDouble.Instance;
                    break;
                case T_CONST_FLOAT.Suffix.F:
                    t = TSingle.Instance;
                    break;
                case T_CONST_FLOAT.Suffix.L:
                    t = TLDouble.Instance;
                    break;
            }
        }

        public override bool Equals(object obj) {
            return Equals(obj as ConstFloat);
        }

        public bool Equals(ConstFloat x) {
            return x != null && x.pos.Equals(pos)
                && Math.Abs(x.value - value) < 0.0001f
                && x.t.Equals(t);
        }

        public override AST.Expr GetASTExpr(Env env) {
            return new AST.ConstFloatExpr(t, value, env.ASTEnv);
        }

        private static double Evaluate(T_CONST_FLOAT token) {

            const int WHOLE = 0;
            const int FRACT = 1;
            const int PEXPO = 2;
            const int NEXPO = 3;

            int state = WHOLE;

            double value = 0;
            int fractCount = -1;
            int exponPart = 0;

            Func<char, bool> IsExpon = c => char.ToLower(c) == 'p' || char.ToLower(c) == 'e';

            foreach (var c in token.text) {
                switch (state) {
                    case WHOLE:
                        if (c == '.') state = FRACT;
                        else if (IsExpon(c)) state = PEXPO;
                        else value = value * token.n + GetHexadecimal(c);
                        break;
                    case FRACT:
                        if (IsExpon(c)) state = PEXPO;
                        else value += GetHexadecimal(c) * Math.Pow(token.n, fractCount--);
                        break;
                    case PEXPO:
                        if (c == '-') state = NEXPO;
                        else if (c == '+') state = PEXPO;
                        else exponPart = exponPart * 10 + c - '0';
                        break;
                    case NEXPO:
                        exponPart = exponPart * 10 - (c - '0');
                        break;
                }
            }

            value *= Math.Pow(token.n == 10 ? 10 : 2, exponPart);

            return value;
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        public readonly double value;
        public readonly TArithmetic t;
    }

    /// <summary>
    /// A string is variable sequence of char.
    /// Notice: string is array of char, lvalue. which means
    ///     "what"[0] = 'c';
    /// is totally legal.
    /// 
    /// Each char is the same as in constant character integer defined in ASTConstChar.
    /// </summary>
    public sealed class Str : Expr, IEquatable<Str> {
        public Str(LinkedList<T_STRING_LITERAL> tokens) {
            pos = new Position { line = tokens.First().line };
            values = Evaluate(tokens);
            type = TChar.Instance.None().Arr(values.Count());
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as Str);
        }

        public bool Equals(Str s) {
            return s != null && s.pos.Equals(pos) && s.values.SequenceEqual(values);
        }

        public override int GetHashCode() {
            return values.First();
        }

        public override AST.Expr GetASTExpr(Env env) {
            throw new NotImplementedException();
        }

        public static IEnumerable<ushort> Evaluate(LinkedList<T_STRING_LITERAL> tokens) {

            IEnumerable<ushort> values = new LinkedList<ushort>();

            foreach (var t in tokens) {
                Position pos = new Position { line = t.line };
                if (t.prefix == T_STRING_LITERAL.Prefix.L) {
                    throw new EUnknownType(pos, "multi-character");
                }
                values = values.Concat(ConstChar.Evaluate(pos, t.text));
            }
            return values;
        }

        public readonly Position pos;
        public readonly IEnumerable<ushort> values;
        public readonly T type;
    }
}
