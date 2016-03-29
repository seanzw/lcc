using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeVoid : ArithmeticType {

        private static readonly TypeVoid instance = new TypeVoid();

        public static TypeVoid Instance {
            get {
                return instance;
            }
        }

        private TypeVoid() : base(1) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "void";
        }
    }
}
