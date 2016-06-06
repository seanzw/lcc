using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public class TPtr : TScalar {

        public TPtr(T element) : base(TKind.PTR) {
            this.element = element;
        }

        public override bool IsPtr => true;
        public override bool IsComplete => true;
        public override bool IsDefined => true;
        public override int Bits => 32;

        public override string ToString() {
            return string.Format("({0}) *", element.ToString());
        }
        public override bool Equals(object obj) {
            return Equals(obj as TPtr);
        }
        public bool Equals(TPtr t) {
            return t != null && t.element.Equals(element);
        }
        public override int GetHashCode() {
            return element.GetHashCode();
        }

        /// <summary>
        /// For two pointers to be compatible, both shall be identically qualified
        /// and both shall be pointers to compatible types.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Compatible(TUnqualified other) {
            return other.IsPtr && element.Compatible((other as TPtr).element);
        }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public readonly T element;
    }

    
}
