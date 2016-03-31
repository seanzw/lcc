using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public class TPointer : TObject {

        public TPointer(T element) {
            this.element = element;
        }

        public override bool Completed => true;
        public override string ToString() {
            return string.Format("pointer to ({0})", element.ToString());
        }
        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public readonly T element;
    }

    public sealed class TArray : TPointer {

        public TArray(T element, int n) : base(element) {
            this.n = n;
        }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return string.Format("{0}[{1}]", element.ToString(), n);
        }

        public readonly int n;
    }
}
