using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {
    public sealed class Declaraion : Node {
        public readonly IEnumerable<AST.Node> inits;
        public Declaraion(IEnumerable<AST.Node> inits) {
            this.inits = inits;
        }
    }
}
