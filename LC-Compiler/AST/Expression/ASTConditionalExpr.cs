using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTConditionalExpr : ASTExpr, IEquatable<ASTConditionalExpr> {

        public ASTConditionalExpr(ASTExpr predicator, ASTExpr trueExpr, ASTExpr falseExpr) {
            this.predicator = predicator;
            this.trueExpr = trueExpr;
            this.falseExpr = falseExpr;
        }

        public override Position Pos => predicator.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTConditionalExpr);
        }

        public bool Equals(ASTConditionalExpr x) {
            return x != null && x.predicator.Equals(predicator)
                && x.trueExpr.Equals(trueExpr)
                && x.falseExpr.Equals(falseExpr);
        }

        public override int GetHashCode() {
            return predicator.GetHashCode() ^ trueExpr.GetHashCode() ^ falseExpr.GetHashCode();
        }

        public readonly ASTExpr predicator;
        public readonly ASTExpr trueExpr;
        public readonly ASTExpr falseExpr;
    }
}
