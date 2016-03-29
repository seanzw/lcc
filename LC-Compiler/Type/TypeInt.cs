using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeInt : IntegerType {

        private static readonly TypeInt instance = new TypeInt();

        public static TypeInt Instance => instance;
        private TypeInt() : base(4) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "int";
        }

        public override BigInteger MAX => int.MaxValue;
        public override BigInteger MIN => int.MinValue;
    }

    public sealed class TypeUnsignedInt : IntegerType {

        private static readonly TypeUnsignedInt instance = new TypeUnsignedInt();

        public static TypeUnsignedInt Instance => instance;
        private TypeUnsignedInt() : base(4) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned int";
        }

        public override BigInteger MAX => uint.MaxValue;
        public override BigInteger MIN => uint.MinValue;
    }
}
