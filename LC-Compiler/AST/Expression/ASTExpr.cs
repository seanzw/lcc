using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {

    /// <summary>
    /// The base class for all expression.
    /// </summary>
    abstract class ASTExpr : ASTNode {

        public abstract override int GetLine();
    }
}
