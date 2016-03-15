using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTDoStatement : ASTStatement {

        public ASTDoStatement(ASTExpr expr, ASTStatement statement) {
            this.expr = expr;
            this.statement = statement;
        }

        public override int GetLine() {
            return expr.GetLine();
        }

        public override bool Equals(object obj) {
            ASTDoStatement x = obj as ASTDoStatement;
            return Equals(x);
        }

        public bool Equals(ASTDoStatement x) {
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
