using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTBinaryExpr : ASTExpr, IEquatable<ASTBinaryExpr> {

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

        public override Position Pos => lhs.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTBinaryExpr);
        }

        public bool Equals(ASTBinaryExpr x) {
            return x != null
                && x.lhs.Equals(lhs)
                && x.rhs.Equals(rhs)
                && x.op.Equals(op);
        }

        public override int GetHashCode() {
            return lhs.GetHashCode() ^ rhs.GetHashCode() ^ op.GetHashCode();
        }

        public readonly ASTExpr lhs;
        public readonly ASTExpr rhs;
        public readonly Op op;
    }
}
