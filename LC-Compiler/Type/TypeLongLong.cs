using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeLongLong : IntegerType {

        private static readonly TypeLongLong instance = new TypeLongLong();

        public static TypeLongLong Instance {
            get {
                return instance;
            }
        }
        private TypeLongLong() : base(8) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "long long";
        }

        public override BigInteger MAX {
            get { return long.MaxValue; }
        }
        public override BigInteger MIN {
            get { return long.MinValue; }
        }
    }

    public sealed class TypeUnsignedLongLong : IntegerType {

        private static readonly TypeUnsignedLongLong instance = new TypeUnsignedLongLong();

        public static TypeUnsignedLongLong Instance {
            get {
                return instance;
            }
        }
        private TypeUnsignedLongLong() : base(4) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned long long";
        }

        public override BigInteger MAX {
            get { return ulong.MaxValue; }
        }
        public override BigInteger MIN {
            get { return ulong.MinValue; }
        }
    }
}
