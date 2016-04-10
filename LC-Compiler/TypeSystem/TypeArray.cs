using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {

    /// <summary>
    /// Represent a complete array or incomplete array.
    /// </summary>
    public sealed class TArray : TObject, IEquatable<TArray> {

        public TArray(T element, int n = -1) {
            this.element = element;
            this.n = n;
        }
        public override bool IsArray => true;
        public override bool IsComplete => n != -1;
        public override int Size {
            get {
                if (n == -1) throw new InvalidOperationException("Can't take size of an incomplete array.");
                else return n * element.Size;
            }
        }

        /// <summary>
        /// Complete this array with length n.
        /// </summary>
        /// <param name="n"></param>
        public override void CompleteArr(int n) {
            if (this.n != -1) base.CompleteArr(n);
            else this.n = n;
        }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public bool Equals(TArray t) {
            return t != null && t.n == n && t.element.Equals(element);
        }
        public override bool Equals(object obj) {
            return Equals(obj as TArray);
        }
        public override int GetHashCode() {
            return element.GetHashCode();
        }

        public override string ToString() {
            if (IsComplete) return string.Format("{0}[{1}]", element, n);
            else return string.Format("{0}[]", element);
        }

        private int n;
        public readonly T element;
    }

    /// <summary>
    /// Represents a variable length array.
    /// </summary>
    public sealed class TVarArray : TObject, IEquatable<TVarArray> {

        public TVarArray(T element) {
            this.element = element;
        }
        public override bool IsArray => true;
        public override bool IsComplete => true;
        public override int Size {
            get {
                throw new InvalidOperationException("Can't take the size of a variable length array.");
            }
        }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public bool Equals(TVarArray x) {
            return x != null && x.element.Equals(element);
        }

        public override bool Equals(object obj) {
            return Equals(obj as TVarArray);
        }

        public override int GetHashCode() {
            return element.GetHashCode();
        }
        public override string ToString() {
            return string.Format("{0}[x]", element);
        }


        public readonly T element;
    }

}
