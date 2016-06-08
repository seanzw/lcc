using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {
    public sealed class Declaraion : Stmt {
        public readonly IEnumerable<AST.Stmt> inits;
        public Declaraion(IEnumerable<AST.Stmt> inits) {
            this.inits = inits;
        }
    }
}
