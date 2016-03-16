using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeShort : TypeBuiltIn {

        public TypeShort(bool isConstant) : base(isConstant, 2) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "short ";
        }
    }

    public sealed class TypeUnsignedShort : TypeBuiltIn {

        public TypeUnsignedShort(bool isConstant) : base(isConstant, 2) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "unsigned short ";
        }
    }
}
