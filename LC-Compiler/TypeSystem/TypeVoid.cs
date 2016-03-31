using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public sealed class TypeVoid : TUnqualified {

        private static readonly TypeVoid instance = new TypeVoid();

        public static TypeVoid Instance {
            get {
                return instance;
            }
        }

        public override bool Completed => false;

        //private TypeVoid() : base(1) { }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "void";
        }
    }
}
