using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeVoid : TypeBuiltIn {

        public TypeVoid(bool isConstant)
            : base(isConstant, 1) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "void ";
        }
    }
}
