using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTSwitch : ASTStmt, IEquatable<ASTSwitch> {

        public ASTSwitch(
            int line,
            ASTExpr expr,
            ASTStmt statement
            ) {
            this.pos = new Position { line = line };
            this.expr = expr;
            this.statement = statement;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTSwitch);
        }

        public bool Equals(ASTSwitch x) {
            return x != null
                && x.pos.Equals(pos)
                && x.expr.Equals(expr)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        private readonly Position pos;
        public readonly ASTExpr expr;
        public readonly ASTStmt statement;
    }
}
