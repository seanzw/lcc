using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeArray : UnqualifiedType {

        public TypeArray(Type element, int n) {
            this.element = element;
            this.n = n;

        }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override bool Completed {
            get {
                return true;
            }
        }

        public override string ToString() {
            return string.Format("{0}[{1}]", element.ToString(), n);
        }

        public readonly Type element;
        public readonly int n;
    }
}
