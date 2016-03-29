using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeShort : IntegerType {

        private static readonly TypeShort instance = new TypeShort();

        public static TypeShort Instance => instance;

        private TypeShort() : base(2) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "short";
        }

        public override BigInteger MAX => short.MaxValue;
        public override BigInteger MIN => short.MinValue;
    }

    public sealed class TypeUnsignedShort : IntegerType {

        private static readonly TypeUnsignedShort instance = new TypeUnsignedShort();

        public static TypeUnsignedShort Instance => instance;

        private TypeUnsignedShort() : base(2) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned short";
        }

        public override BigInteger MAX => ushort.MaxValue;
        public override BigInteger MIN => ushort.MinValue;
    }
}
