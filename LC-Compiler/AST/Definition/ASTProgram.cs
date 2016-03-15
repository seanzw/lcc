using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTProgram : ASTNode {

        public ASTProgram(LinkedList<ASTNode> nodes) {
            this.nodes = nodes;
        }

        public override int GetLine() {
            return 1;
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTProgram);
        }

        public bool Equals(ASTProgram x) {
            return x.nodes.SequenceEqual(nodes);
        }

        public override int GetHashCode() {
            return nodes.GetHashCode();
        }

        public readonly LinkedList<ASTNode> nodes;
    }
}
