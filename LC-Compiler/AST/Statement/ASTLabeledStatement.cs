using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTLabeledStatement : ASTStatement {

        public ASTLabeledStatement(ASTIdentifier identifier, ASTStatement statement) {
            this.identifier = identifier;
            this.statement = statement;
        }

        public override int GetLine() {
            return identifier.GetLine();
        }

        public override bool Equals(object obj) {
            ASTLabeledStatement x = obj as ASTLabeledStatement;
            return Equals(x);
        }

        public bool Equals(ASTLabeledStatement x) {
            return x == null ? false : base.Equals(x)
                && x.identifier.Equals(identifier)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return identifier.GetHashCode();
        }

        public readonly ASTIdentifier identifier;
        public readonly ASTStatement statement;
    }

    public sealed class ASTCaseStatement : ASTStatement {

        public ASTCaseStatement(ASTExpr expr, ASTStatement statement) {
            this.expr = expr;
            this.statement = statement;
        }

        public override int GetLine() {
            return expr.GetLine();
        }

        public override bool Equals(object obj) {
            ASTCaseStatement x = obj as ASTCaseStatement;
            return Equals(x);
        }

        public bool Equals(ASTCaseStatement x) {
            return x == null ? false : base.Equals(x)
                && x.expr.Equals(expr)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public readonly ASTExpr expr;
        public readonly ASTStatement statement;
    }

    public sealed class ASTDefaultStatement : ASTStatement {

        public ASTDefaultStatement(ASTStatement statement) {
            this.statement = statement;
        }

        public override int GetLine() {
            return statement.GetLine();
        }

        public override bool Equals(object obj) {
            ASTDefaultStatement x = obj as ASTDefaultStatement;
            return Equals(x);
        }

        public bool Equals(ASTDefaultStatement x) {
            return x == null ? false : base.Equals(x)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return statement.GetHashCode();
        }

        public readonly ASTStatement statement;
    }
}
