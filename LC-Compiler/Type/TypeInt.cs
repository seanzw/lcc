using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeInt : IntegerType {

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

        public override BigInteger MAX {
            get { return int.MaxValue; }
        }
        public override BigInteger MIN {
            get { return int.MinValue; }
        }
    }

    public sealed class TypeUnsignedInt : IntegerType {

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

        public override BigInteger MAX {
            get { return uint.MaxValue; }
        }
        public override BigInteger MIN {
            get { return uint.MinValue; }
        }
    }
}
