using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeInt : TypeBuiltIn {

        private static readonly TypeInt Const = new TypeInt(true);
        private static readonly TypeInt Var = new TypeInt(false);

        public static TypeInt Constant {
            get {
                return Const;
            }
        }

        public static TypeInt Variable {
            get {
                return Var;
            }
        }

        private TypeInt(bool isConstant) : base(isConstant, 4) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "int ";
        }
    }

    public sealed class TypeUnsignedInt : TypeBuiltIn {

        private static readonly TypeUnsignedInt Const = new TypeUnsignedInt(true);
        private static readonly TypeUnsignedInt Var = new TypeUnsignedInt(false);

        public static TypeUnsignedInt Constant {
            get {
                return Const;
            }
        }

        public static TypeUnsignedInt Variable {
            get {
                return Var;
            }
        }

        private TypeUnsignedInt(bool isConstant) : base(isConstant, 4) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "unsigned int ";
        }
    }
}
