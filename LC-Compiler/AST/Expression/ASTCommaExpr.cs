using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTCommaExpr : ASTExpr {

        public ASTCommaExpr(LinkedList<ASTExpr> exprs) {
            this.exprs = exprs;
        }

        public override int GetLine() {
            return exprs.First().GetLine();
        }

        public override bool Equals(object obj) {
            ASTCommaExpr expr = obj as ASTCommaExpr;
            return expr == null ? false : base.Equals(expr)
                && exprs.SequenceEqual(expr.exprs);
        }

        public bool Equals(ASTCommaExpr expr) {
            return base.Equals(expr)
                && exprs.SequenceEqual(expr.exprs);
        }

        public override int GetHashCode() {
            return exprs.Aggregate(0, (acc, expr) => acc ^ expr.GetHashCode());
        }

        public readonly LinkedList<ASTExpr> exprs;
    }
}
