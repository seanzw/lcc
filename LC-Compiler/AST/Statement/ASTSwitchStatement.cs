using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTSwitchStatement : ASTStatement {

        public ASTSwitchStatement(
            int line,
            ASTExpr expr,
            ASTStatement statement
            ) {
            this.line = line;
            this.expr = expr;
            this.statement = statement;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTSwitchStatement x = obj as ASTSwitchStatement;
            return Equals(x);
        }

        public bool Equals(ASTSwitchStatement x) {
            return x == null ? false : base.Equals(x)
                && x.line == line
                && x.expr.Equals(expr)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public readonly int line;
        public readonly ASTExpr expr;
        public readonly ASTStatement statement;
    }
}
