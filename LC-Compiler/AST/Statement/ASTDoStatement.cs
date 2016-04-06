using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTDo : ASTStmt, IEquatable<ASTDo> {

        public ASTDo(ASTExpr expr, ASTStmt statement) {
            this.expr = expr;
            this.statement = statement;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            ASTDo x = obj as ASTDo;
            return Equals(x);
        }

        public bool Equals(ASTDo x) {
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
