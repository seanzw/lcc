using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTBinaryExpr : ASTExpr {

        public enum Op {
            MULT,   // *
            DIV,    // /
            MOD,    // %
            PLUS,   // +
            MINUS,  // -
            LEFT,   // <<
            RIGHT,  // >>
            LT,     // <
            GT,     // >
            LE,     // <=
            GE,     // >=
            EQ,     // ==
            NEQ,    // !=
            AND,    // &
            XOR,    // ^
            OR,     // |
            LOGAND, // &&
            LOGOR   // ||
        }

        public ASTBinaryExpr(ASTExpr lhs, ASTExpr rhs, Op op) {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }

        public override int GetLine() {
            return lhs.GetLine();
        }

        public override bool Equals(object obj) {
            ASTBinaryExpr expr = obj as ASTBinaryExpr;
            return expr == null ? false : base.Equals(expr)
                && expr.lhs.Equals(lhs)
                && expr.rhs.Equals(rhs)
                && expr.op.Equals(op);
        }

        public bool Equals(ASTBinaryExpr expr) {
            return base.Equals(expr)
                && expr.lhs.Equals(lhs)
                && expr.rhs.Equals(rhs)
                && expr.op.Equals(op);
        }

        public override int GetHashCode() {
            return lhs.GetHashCode() ^ rhs.GetHashCode() ^ op.GetHashCode();
        }

        public readonly ASTExpr lhs;
        public readonly ASTExpr rhs;
        public readonly Op op;
    }
}
