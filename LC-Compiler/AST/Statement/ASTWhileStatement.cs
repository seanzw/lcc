using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTWhile : ASTStmt, IEquatable<ASTWhile> {

        public ASTWhile(ASTExpr expr, ASTStmt statement) {
            this.expr = expr;
            this.statement = statement;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTWhile);
        }

        public bool Equals(ASTWhile x) {
            return x != null
                && x.expr.Equals(expr)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return expr.GetHashCode() | statement.GetHashCode();
        }

        public readonly ASTExpr expr;
        public readonly ASTStmt statement;
    }
}
