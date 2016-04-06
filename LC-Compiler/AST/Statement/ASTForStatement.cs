using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTForStmt : ASTStmt, IEquatable<ASTForStmt> {


        public ASTForStmt(
            int line,
            ASTExpr init,
            ASTExpr pred,
            ASTExpr iter, 
            ASTStmt statement) {
            this.pos = new Position { line = line };
            this.init = init;
            this.pred = pred;
            this.iter = iter;
            this.statement = statement;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTForStmt);
        }

        public bool Equals(ASTForStmt x) {
            return x != null
                && x.pos.Equals(pos)
                && NullableEquals(x.init, init)
                && NullableEquals(x.pred, pred)
                && NullableEquals(x.iter, iter)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return statement.GetHashCode();
        }

        private readonly Position pos;
        public readonly ASTExpr init;
        public readonly ASTExpr pred;
        public readonly ASTExpr iter;
        public readonly ASTStmt statement;
    }
}
