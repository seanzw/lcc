using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public abstract class ASTStatement : ASTNode {

        public override bool Equals(object obj) {
            return obj is ASTStatement;
        }

        public bool Equals(ASTStatement expr) {
            return true;
        }

        public override int GetHashCode() {
            return GetLine();
        }
    }
}
