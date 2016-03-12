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

        public override bool Equals(object obj) {
            ASTConditionalExpr expr = obj as ASTConditionalExpr;
            return expr == null ? false : base.Equals(expr)
                && expr.predicator.Equals(predicator)
                && expr.trueExpr.Equals(trueExpr)
                && expr.falseExpr.Equals(falseExpr);
        }

        public bool Equals(ASTConditionalExpr expr) {
            return base.Equals(expr)
                && expr.predicator.Equals(predicator)
                && expr.trueExpr.Equals(trueExpr)
                && expr.falseExpr.Equals(falseExpr);
        }

        public override int GetHashCode() {
            return predicator.GetHashCode() ^ trueExpr.GetHashCode() ^ falseExpr.GetHashCode();
        }

        public readonly ASTExpr predicator;
        public readonly ASTExpr trueExpr;
        public readonly ASTExpr falseExpr;
    }
}
