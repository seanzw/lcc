using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public class TPointer : TScalar {

        public TPointer(T element) {
            this.element = element;
        }

        public override bool Completed => true;
        public override string ToString() {
            return string.Format("{0} *", element.ToString());
        }
        public override bool Equals(object obj) {
            return Equals(obj as TPointer);
        }
        public bool Equals(TPointer t) {
            return t != null && t.element.Equals(element);
        }
        public override int GetHashCode() {
            return element.GetHashCode();
        }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public readonly T element;
    }

    public sealed class TArray : TObject {

        public TArray(T element, int n) {
            this.element = element;
            this.n = n;
        }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj) {
            return Equals(obj as TArray);
        }
        public bool Equals(TArray t) {
            return t != null && t.n == n && t.element.Equals(element);
        }
        public override int GetHashCode() {
            return element.GetHashCode() ^ n;
        }

        public override string ToString() {
            return string.Format("{0}[{1}]", element.ToString(), n);
        }

        public readonly int n;
        public readonly T element;
    }
}
