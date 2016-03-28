using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeLong : ArithmeticType {

        private static readonly TypeLong instance = new TypeLong();

        public static TypeLong Instance {
            get {
                return instance;
            }
        }

        private TypeLong() : base(4) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "long ";
        }
    }

    public sealed class TypeUnsignedLong : ArithmeticType {

        private static readonly TypeUnsignedLong instance = new TypeUnsignedLong();

        public static TypeUnsignedLong Instance {
            get {
                return instance;
            }
        }

        private TypeUnsignedLong() : base(4) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned long ";
        }
    }
}
