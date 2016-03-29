using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeInt : ArithmeticType {

        private static readonly TypeInt instance = new TypeInt();

        public static TypeInt Instance {
            get {
                return instance;
            }
        }
        private TypeInt() : base(4) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "int";
        }
    }

    public sealed class TypeUnsignedInt : ArithmeticType {

        private static readonly TypeUnsignedInt instance = new TypeUnsignedInt();

        public static TypeUnsignedInt Instance {
            get {
                return instance;
            }
        }
        private TypeUnsignedInt() : base(4) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned int";
        }
    }
}
