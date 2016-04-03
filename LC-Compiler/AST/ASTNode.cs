using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public abstract class ASTNode {
        protected ASTNode() { }

        public abstract int GetLine();

        public override bool Equals(object obj) {
            return obj is ASTNode;
        }

        public bool Equals(ASTNode node) {
            return true;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns true if x equals y or both values are null.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool NullableEquals(ASTNode x, ASTNode y) {
            return x == null ? y == null : x.Equals(y);
        }

        /// <summary>
        /// Returns true if xs sequence equals ys or both values are null.
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <returns></returns>
        public static bool NullableEquals(IEnumerable<ASTNode> xs, IEnumerable<ASTNode> ys) {
            return xs == null ? ys == null : xs.SequenceEqual(ys);
        }
    }


}
