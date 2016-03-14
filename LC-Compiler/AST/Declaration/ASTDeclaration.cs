using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public abstract class ASTDeclaration : ASTNode {

        public abstract override int GetLine();

        public override bool Equals(object obj) {
            return obj is ASTDeclaration;
        }

        public bool Equals(ASTDeclaration decl) {
            return true;
        }

        public override int GetHashCode() {
            return 0;
        }
    }
}
