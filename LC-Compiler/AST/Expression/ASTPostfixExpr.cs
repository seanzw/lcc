using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lcc.Token;

namespace lcc.AST {
    sealed class ASTArrSub : ASTExpr {

        public ASTArrSub(ASTExpr arr, ASTExpr idx) {
            this.arr = arr;
            this.idx = idx;
        }

        public override int GetLine() {
            return arr.GetLine();
        }

        public readonly ASTExpr arr;
        public readonly ASTExpr idx;
    }

    sealed class ASTDotAccess : ASTExpr {

        public ASTDotAccess(ASTExpr aggregation, T_IDENTIFIER token) {
            this.aggregation = aggregation;
            this.field = token.name;
        }

        public override int GetLine() {
            return aggregation.GetLine();
        }

        public readonly ASTExpr aggregation;
        public readonly string field;
    }

    sealed class ASTPtrAccess : ASTExpr {

        public ASTPtrAccess(ASTExpr aggregation, T_IDENTIFIER token) {
            this.aggregation = aggregation;
            this.field = token.name;
        }

        public override int GetLine() {
            return aggregation.GetLine();
        }

        public readonly ASTExpr aggregation;
        public readonly string field;
    }

    sealed class ASTPostInc : ASTExpr {

        public ASTPostInc(ASTExpr expr) {
            this.expr = expr;
        }

        public override int GetLine() {
            return expr.GetLine();
        }

        public readonly ASTExpr expr;
    }

    sealed class ASTPostDec : ASTExpr {

        public ASTPostDec(ASTExpr expr) {
            this.expr = expr;
        }

        public override int GetLine() {
            return expr.GetLine();
        }

        public readonly ASTExpr expr;
    }
}
