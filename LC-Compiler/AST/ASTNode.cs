using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public abstract class ASTNode {
        protected ASTNode() { }

        public abstract int GetLine();

        public override bool Equals(object obj) {
            return obj is ASTNode;
        }

        public bool Equals(ASTNode node) {
            return true;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

    }
}
