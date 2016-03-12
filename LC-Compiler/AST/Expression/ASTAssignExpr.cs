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

        public override bool Equals(object obj) {
            ASTAssignExpr expr = obj as ASTAssignExpr;
            return expr == null ? false : base.Equals(expr)
                && expr.lexpr.Equals(lexpr)
                && expr.rexpr.Equals(rexpr)
                && expr.op.Equals(op);
        }

        public bool Equals(ASTAssignExpr expr) {
            return base.Equals(expr)
                && expr.lexpr.Equals(lexpr)
                && expr.rexpr.Equals(rexpr)
                && expr.op.Equals(op);
        }

        public override int GetHashCode() {
            return lexpr.GetHashCode() ^ rexpr.GetHashCode() ^ op.GetHashCode();
        }

        public readonly ASTExpr lexpr;
        public readonly ASTExpr rexpr;
        public readonly Op op;
    }
}
