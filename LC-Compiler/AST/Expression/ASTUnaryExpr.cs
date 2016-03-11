using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTPreInc : ASTExpr {

        public ASTPreInc(ASTExpr expr) {
            this.expr = expr;
        }

        public override int GetLine() {
            return expr.GetLine();
        }

        public readonly ASTExpr expr;
    }

    public sealed class ASTPreDec : ASTExpr {

        public ASTPreDec(ASTExpr expr) {
            this.expr = expr;
        }

        public override int GetLine() {
            return expr.GetLine();
        }

        public readonly ASTExpr expr;
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

        public readonly ASTExpr expr;
        public readonly Op op;
    }

}
