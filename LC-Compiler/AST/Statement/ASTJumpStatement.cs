using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTContinueStatement : ASTStatement {

        public ASTContinueStatement(
            int line
            ) {
            this.line = line;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTContinueStatement x = obj as ASTContinueStatement;
            return Equals(x);
        }

        public bool Equals(ASTContinueStatement x) {
            return x == null ? false : base.Equals(x)
                && x.line == line;
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly int line;
    }

    public sealed class ASTBreakStatement : ASTStatement {

        public ASTBreakStatement(
            int line
            ) {
            this.line = line;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTBreakStatement x = obj as ASTBreakStatement;
            return Equals(x);
        }

        public bool Equals(ASTBreakStatement x) {
            return x == null ? false : base.Equals(x)
                && x.line == line;
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly int line;
    }

    public sealed class ASTReturnStatement : ASTStatement {

        public ASTReturnStatement(
            int line,
            ASTExpr expr
            ) {
            this.line = line;
            this.expr = expr;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTReturnStatement x = obj as ASTReturnStatement;
            return Equals(x);
        }

        public bool Equals(ASTReturnStatement x) {
            return x == null ? false : base.Equals(x)
                && x.line == line
                && x.expr == null ? expr == null : x.expr.Equals(expr);
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly int line;
        public readonly ASTExpr expr;
    }

    public sealed class ASTGotoStatement : ASTStatement {

        public ASTGotoStatement(
            int line,
            ASTIdentifier label
            ) {
            this.line = line;
            this.label = label;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTGotoStatement x = obj as ASTGotoStatement;
            return Equals(x);
        }

        public bool Equals(ASTGotoStatement x) {
            return x == null ? false : base.Equals(x)
                && x.line == line
                && x.label.Equals(label);
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly int line;
        public readonly ASTIdentifier label;
    }
}
