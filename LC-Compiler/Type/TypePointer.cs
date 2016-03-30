using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public class TypePointer : ObjectType {

        public TypePointer(Type element) {
            this.element = element;
        }

        public override bool Completed => true;
        public override string ToString() {
            return string.Format("pointer to ({0})", element.ToString());
        }
        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public readonly Type element;
    }

    public sealed class TypeArray : TypePointer {

        public TypeArray(Type element, int n) : base(element) {
            this.n = n;
        }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return string.Format("{0}[{1}]", element.ToString(), n);
        }

        public readonly int n;
    }
}
