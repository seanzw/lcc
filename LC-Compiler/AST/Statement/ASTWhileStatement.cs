using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTWhileStatement : ASTStatement {

        public ASTWhileStatement(ASTExpr expr, ASTStatement statement) {
            this.expr = expr;
            this.statement = statement;
        }

        public override int GetLine() {
            return expr.GetLine();
        }

        public override bool Equals(object obj) {
            ASTWhileStatement x = obj as ASTWhileStatement;
            return Equals(x);
        }

        public bool Equals(ASTWhileStatement x) {
            return x == null ? false : base.Equals(x)
                && x.expr.Equals(expr)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return expr.GetHashCode() | statement.GetHashCode();
        }

        public readonly ASTExpr expr;
        public readonly ASTStatement statement;
    }
}
