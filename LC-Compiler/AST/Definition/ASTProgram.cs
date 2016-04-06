using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTProgram : ASTNode, IEquatable<ASTProgram> {

        public ASTProgram(IEnumerable<ASTNode> nodes) {
            this.nodes = nodes;
        }

        public override Position Pos => nodes.First().Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTProgram);
        }

        public bool Equals(ASTProgram x) {
            return x.nodes.SequenceEqual(nodes);
        }

        public override int GetHashCode() {
            return nodes.Aggregate(0, (hash, node) => hash ^ node.GetHashCode());
        }

        public readonly IEnumerable<ASTNode> nodes;
    }
}
