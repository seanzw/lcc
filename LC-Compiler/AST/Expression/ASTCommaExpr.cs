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

        public readonly LinkedList<ASTExpr> exprs;
    }
}
