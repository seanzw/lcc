using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeShort : ArithmeticType {

        private static readonly TypeShort instance = new TypeShort();

        public static TypeShort Instance {
            get {
                return instance;
            }
        }

        private TypeShort() : base(2) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "short";
        }
    }

    public sealed class TypeUnsignedShort : ArithmeticType {

        private static readonly TypeUnsignedShort instance = new TypeUnsignedShort();

        public static TypeUnsignedShort Instance {
            get {
                return instance;
            }
        }

        private TypeUnsignedShort() : base(2) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned short";
        }
    }
}
