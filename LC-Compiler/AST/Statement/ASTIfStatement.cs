using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTIfStmt : ASTStmt, IEquatable<ASTIfStmt> {


        public ASTIfStmt(
            int line,
            ASTExpr expr,
            ASTStmt then,
            ASTStmt other
            ) {
            this.pos = new Position { line = line };
            this.expr = expr;
            this.then = then;
            this.other = other;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTIfStmt);
        }

        public bool Equals(ASTIfStmt x) {
            return x != null
                && x.pos.Equals(pos)
                && x.expr.Equals(expr)
                && x.then.Equals(then)
                && x.other == null ? other == null : x.other.Equals(other);
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        private readonly Position pos;
        public readonly ASTExpr expr;
        public readonly ASTStmt then;
        public readonly ASTStmt other;
    }
}
