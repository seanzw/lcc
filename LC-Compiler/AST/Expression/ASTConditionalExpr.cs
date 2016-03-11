using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTConditionalExpr : ASTExpr {

        public ASTConditionalExpr(ASTExpr predicator, ASTExpr trueExpr, ASTExpr falseExpr) {
            this.predicator = predicator;
            this.trueExpr = trueExpr;
            this.falseExpr = falseExpr;
        }

        public override int GetLine() {
            return predicator.GetLine();
        }

        public readonly ASTExpr predicator;
        public readonly ASTExpr trueExpr;
        public readonly ASTExpr falseExpr;
    }
}
