using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {

    /// <summary>
    /// The base class for all expression.
    /// </summary>
    public abstract class ASTExpr : ASTNode {

        public abstract override int GetLine();

        public override bool Equals(object obj) {
            return obj as ASTExpr != null;
        }

        public bool Equals(ASTExpr expr) {
            return true;
        }

        public override int GetHashCode() {
            return 0;
        }
    }
}
