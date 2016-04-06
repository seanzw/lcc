using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lcc.TypeSystem;

namespace lcc.AST {

    /// <summary>
    /// ++i
    /// --i
    /// </summary>
    public sealed class ASTPreStep : ASTExpr, IEquatable<ASTPreStep> {

        public enum Kind {
            INC,
            DEC
        }

        public ASTPreStep(ASTExpr expr, Kind kind) {
            this.expr = expr;
            this.kind = kind;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTPreStep);
        }

        public bool Equals(ASTPreStep x) {
            return x != null && x.expr.Equals(expr) && x.kind == kind;
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public override T TypeCheck(ASTEnv env) {
            T t = expr.TypeCheck(env);
            if (!t.IsArithmetic && !t.IsPointer) {
                throw new ASTException(expr.Pos, string.Format("{0} on type {1}", kind, t));
            }
            if (!t.IsModifiable) {
                throw new ASTException(expr.Pos, string.Format("cannot assign to type {0}", t));
            }
            return t.R();
        }

        public readonly ASTExpr expr;
        public readonly Kind kind;
    }

    /// <summary>
    /// & * + - ~ !
    /// </summary>
    public sealed class ASTUnaryOp : ASTExpr, IEquatable<ASTUnaryOp> {

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

        public ASTUnaryOp(ASTExpr expr, Op op) {
            this.expr = expr;
            this.op = op;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTUnaryOp);
        }

        public bool Equals(ASTUnaryOp x) {
            return x != null && x.expr.Equals(expr) && x.op == op;
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public override T TypeCheck(ASTEnv env) {
            T t = expr.TypeCheck(env);
            switch (op) {
                case Op.REF:
                    if (t.IsRValue)
                        throw new ASTException(expr.Pos, string.Format("cannot take address of rvalue of {0}", t));
                    return t.Ptr().R();
                case Op.STAR:
                    if (!t.IsPointer) 
                        throw new ASTException(expr.Pos, string.Format("indirection requires pointer type ({0} provided)", t));
                    return (t.baseType as TPointer).element;
                case Op.PLUS:
                case Op.MINUS:
                    if (!t.IsArithmetic)
                        throw new ASTException(expr.Pos, string.Format("invalid argument type '{0}' to unary expression", t));
                    return t.IntPromote().R();
                case Op.REVERSE:
                    if (!t.IsInteger) 
                        throw new ASTException(expr.Pos, string.Format("invalid argument type '{0}' to unary expression", t));
                    return t.IntPromote().R();
                case Op.NOT:
                    if (!t.IsScalar)
                        throw new ASTException(expr.Pos, string.Format("invalid argument type '{0}' to unary expression", t));
                    return TInt.Instance.None(T.LR.R);
                default:
                    throw new InvalidOperationException("Unrecognized unary operator");
            }
        }

        public readonly ASTExpr expr;
        public readonly Op op;
    }

}
