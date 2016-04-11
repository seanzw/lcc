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

        public override bool IsPointer => true;
        public override bool IsComplete => true;
        public override bool IsDefined => true;
        public override int Size => 8;

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

    
}
