using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {
    public sealed class Declaraion : Node {
        public readonly IEnumerable<Node> inits;
        public Declaraion(IEnumerable<Node> inits) {
            this.inits = inits;
        }
        public override void ToX86(X86Gen gen) {
            foreach (var init in inits) {
                init.ToX86(gen);
            }
        }
    }
}
