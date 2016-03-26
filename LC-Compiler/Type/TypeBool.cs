using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeBool : TypeBuiltIn {

        private static readonly TypeBool Const = new TypeBool(true);
        private static readonly TypeBool Var = new TypeBool(false);

        public static TypeBool Constant {
            get {
                return Const;
            }
        }

        public static TypeBool Variable {
            get {
                return Var;
            }
        }

        private TypeBool(bool isConstant) : base(isConstant, 1) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "_Bool ";
        }
    }
}
