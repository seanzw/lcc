using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTIfStatement : ASTStatement {


        public ASTIfStatement(
            int line,
            ASTExpr expr,
            ASTStatement then,
            ASTStatement other
            ) {
            this.line = line;
            this.expr = expr;
            this.then = then;
            this.other = other;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTIfStatement x = obj as ASTIfStatement;
            return Equals(x);
        }

        public bool Equals(ASTIfStatement x) {
            return x == null ? false : base.Equals(x)
                && x.line == line
                && x.expr.Equals(expr)
                && x.then.Equals(then)
                && x.other == null ? other == null : x.other.Equals(other);
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public readonly int line;
        public readonly ASTExpr expr;
        public readonly ASTStatement then;
        public readonly ASTStatement other;
    }
}
