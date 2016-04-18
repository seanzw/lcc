using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.SyntaxTree {

    /// <summary>
    /// Position in the source code.
    /// </summary>
    public struct Position : IEquatable<Position> {
        public int line;
        public override string ToString() {
            return string.Format("line {0}", line);
        }
        public bool Equals(Position x) {
            return x.line == line; 
        }
        public override int GetHashCode() {
            return line;
        }
    }

    public abstract class Node {

        public abstract Position Pos { get; }

        /// <summary>
        /// Returns true if x equals y or both values are null.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool NullableEquals(Node x, Node y) {
            return x == null ? y == null : x.Equals(y);
        }

        /// <summary>
        /// Returns true if xs sequence equals ys or both values are null.
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <returns></returns>
        public static bool NullableEquals(IEnumerable<Node> xs, IEnumerable<Node> ys) {
            return xs == null ? ys == null : xs.SequenceEqual(ys);
        }
    }


}
