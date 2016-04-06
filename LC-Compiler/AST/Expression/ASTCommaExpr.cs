using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTCommaExpr : ASTExpr, IEquatable<ASTCommaExpr> {

        public ASTCommaExpr(IEnumerable<ASTExpr> exprs) {
            this.exprs = exprs;
        }

        public override Position Pos => exprs.First().Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTCommaExpr);
        }

        public bool Equals(ASTCommaExpr x) {
            return x != null && exprs.SequenceEqual(x.exprs);
        }

        public override int GetHashCode() {
            return exprs.Aggregate(0, (acc, expr) => acc ^ expr.GetHashCode());
        }

        public readonly IEnumerable<ASTExpr> exprs;
    }
}
