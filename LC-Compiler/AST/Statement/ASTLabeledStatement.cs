using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTLabeled : ASTStmt, IEquatable<ASTLabeled> {

        public ASTLabeled(ASTId identifier, ASTStmt statement) {
            this.identifier = identifier;
            this.statement = statement;
        }

        public override Position Pos => identifier.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTLabeled);
        }

        public bool Equals(ASTLabeled x) {
            return x != null
                && x.identifier.Equals(identifier)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return identifier.GetHashCode();
        }

        public readonly ASTId identifier;
        public readonly ASTStmt statement;
    }

    public sealed class ASTCase : ASTStmt, IEquatable<ASTCase> {

        public ASTCase(ASTExpr expr, ASTStmt statement) {
            this.expr = expr;
            this.statement = statement;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTCase);
        }

        public bool Equals(ASTCase x) {
            return x != null
                && x.expr.Equals(expr)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public readonly ASTExpr expr;
        public readonly ASTStmt statement;
    }

    public sealed class ASTDefault : ASTStmt, IEquatable<ASTDefault> {

        public ASTDefault(ASTStmt statement) {
            this.statement = statement;
        }

        public override Position Pos => statement.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTDefault);
        }

        public bool Equals(ASTDefault x) {
            return x != null && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return statement.GetHashCode();
        }

        public readonly ASTStmt statement;
    }
}
