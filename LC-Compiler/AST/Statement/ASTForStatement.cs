using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTForStatement : ASTStatement {


        public ASTForStatement(
            int line,
            ASTExpr init,
            ASTExpr pred,
            ASTExpr iter, 
            ASTStatement statement) {
            this.line = line;
            this.init = init;
            this.pred = pred;
            this.iter = iter;
            this.statement = statement;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTForStatement x = obj as ASTForStatement;
            return Equals(x);
        }

        public bool Equals(ASTForStatement x) {
            return x == null ? false : base.Equals(x)
                && x.line == line
                && x.init == null ? init == null : x.init.Equals(init)
                && x.pred == null ? pred == null : x.pred.Equals(pred)
                && x.pred == null ? pred == null : x.pred.Equals(iter)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return statement.GetHashCode();
        }

        public readonly int line;
        public readonly ASTExpr init;
        public readonly ASTExpr pred;
        public readonly ASTExpr iter;
        public readonly ASTStatement statement;
    }
}
