using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public abstract class ASTDeclaration : ASTNode {

        public abstract override int GetLine();
    }
}
