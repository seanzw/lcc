using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {
    public class CompoundStmt : Stmt {
        public readonly IEnumerable<Stmt> stmts;
        public CompoundStmt(IEnumerable<Stmt> stmts) {
            this.stmts = stmts;
        }
    }
}
