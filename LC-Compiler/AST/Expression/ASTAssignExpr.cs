using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTAssignExpr : ASTExpr, IEquatable<ASTAssignExpr> {

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

        public override Position Pos => lexpr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTAssignExpr);
        }

        public bool Equals(ASTAssignExpr expr) {
            return expr != null
                && expr.lexpr.Equals(lexpr)
                && expr.rexpr.Equals(rexpr)
                && expr.op == op;
        }

        public override int GetHashCode() {
            return lexpr.GetHashCode() ^ rexpr.GetHashCode() ^ op.GetHashCode();
        }

        public readonly ASTExpr lexpr;
        public readonly ASTExpr rexpr;
        public readonly Op op;
    }
}
