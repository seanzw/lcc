using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTVoidStmt : ASTStmt {

        public ASTVoidStmt(int line) {
            this.pos = new Position { line = line };
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTVoidStmt);
        }

        public bool Equals(ASTVoidStmt x) {
            return x != null && x.pos.Equals(pos);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        private readonly Position pos;
    }
}
