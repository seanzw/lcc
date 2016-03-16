using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeLongLong : TypeBuiltIn {

        public TypeLongLong(bool isConstant) : base(isConstant, 8) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "long long ";
        }
    }

    public sealed class TypeUnsignedLongLong : TypeBuiltIn {

        public TypeUnsignedLongLong(bool isConstant) : base(isConstant, 4) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "unsigned long long ";
        }
    }
}
