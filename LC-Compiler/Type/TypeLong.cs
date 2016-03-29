using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeLong : IntegerType {

        private static readonly TypeLong instance = new TypeLong();

        public static TypeLong Instance => instance;

        private TypeLong() : base(4) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "long";
        }

        public override BigInteger MAX => TypeInt.Instance.MAX;
        public override BigInteger MIN => TypeInt.Instance.MIN;
    }

    public sealed class TypeUnsignedLong : IntegerType {

        private static readonly TypeUnsignedLong instance = new TypeUnsignedLong();

        public static TypeUnsignedLong Instance => instance;

        private TypeUnsignedLong() : base(4) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned long";
        }

        public override BigInteger MAX => TypeUnsignedInt.Instance.MAX;
        public override BigInteger MIN => TypeUnsignedInt.Instance.MIN;
    }
}
