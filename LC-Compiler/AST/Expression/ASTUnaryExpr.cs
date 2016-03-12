using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTPreStep : ASTExpr {

        public enum Type {
            INC,
            DEC
        }

        public ASTPreStep(ASTExpr expr, Type type) {
            this.expr = expr;
            this.type = type;
        }

        public override int GetLine() {
            return expr.GetLine();
        }

        public override bool Equals(object obj) {
            ASTPreStep x = obj as ASTPreStep;
            return x == null ? false : base.Equals(x)
                && x.expr.Equals(expr)
                && x.type == type;
        }

        public bool Equals(ASTPreStep x) {
            return base.Equals(x)
                && x.expr.Equals(expr)
                && x.type == type;
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public readonly ASTExpr expr;
        public readonly Type type;
    }

    public sealed class ASTUnaryOp : ASTExpr {

        public enum Op {
            REF,
            STAR,
            PLUS,
            MINUS,
            REVERSE,
            NOT
        }

        public ASTUnaryOp(ASTExpr expr, Op op) {
            this.expr = expr;
            this.op = op;
        }

        public override int GetLine() {
            return expr.GetLine();
        }

        public override bool Equals(object obj) {
            ASTUnaryOp x = obj as ASTUnaryOp;
            return x == null ? false : base.Equals(x)
                && x.expr.Equals(expr)
                && x.op == op;
        }

        public bool Equals(ASTUnaryOp x) {
            return base.Equals(x)
                && x.expr.Equals(expr)
                && x.op == op;
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public readonly ASTExpr expr;
        public readonly Op op;
    }

}
