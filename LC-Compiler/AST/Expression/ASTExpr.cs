using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {

    /// <summary>
    /// The base class for all expression.
    /// </summary>
    public abstract class ASTExpr : ASTStmt {

        public virtual T TypeCheck(ASTEnv env) {
            throw new NotImplementedException();
        }
    }
}
