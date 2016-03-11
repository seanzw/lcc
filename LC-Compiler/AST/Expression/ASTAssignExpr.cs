using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTAssignExpr : ASTExpr {

        public enum Op {
            ASSIGN,
            MULEQ,
            DIVEQ,
            MODEQ,
            PLUSEQ,
            MINUSEQ,
            SHIFTLEQ,
            SHIFTREQ,
            BITANDEQ,
            BITXOREQ,
            BITOREQ
        }

        public ASTAssignExpr(ASTExpr lexpr, ASTExpr rexpr, Op op) {
            this.lexpr = lexpr;
            this.rexpr = rexpr;
            this.op = op;
        }

        public override int GetLine() {
            return lexpr.GetLine();
        }

        public readonly ASTExpr lexpr;
        public readonly ASTExpr rexpr;
        public readonly Op op;
    }
}
