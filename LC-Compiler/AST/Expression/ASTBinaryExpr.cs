using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTBinaryExpr : ASTExpr {

        public enum Op {
            MULT,
            DIV,
            MOD,
            PLUS,
            MINUS,
            LEFT,
            RIGHT,
            LT,
            GT,
            LE,
            GE,
            EQ,
            NEQ,
            AND,
            XOR,
            OR,
            LOGAND,
            LOGOR
        }

        public ASTBinaryExpr(ASTExpr lhs, ASTExpr rhs, Op op) {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }

        public override int GetLine() {
            return lhs.GetLine();
        }

        public readonly ASTExpr lhs;
        public readonly ASTExpr rhs;
        public readonly Op op;
    }
}
