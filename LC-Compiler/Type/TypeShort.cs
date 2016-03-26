using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeShort : TypeBuiltIn {

        private static readonly TypeShort Const = new TypeShort(true);
        private static readonly TypeShort Var = new TypeShort(false);

        public static TypeShort Constant {
            get {
                return Const;
            }
        }

        public static TypeShort Variable {
            get {
                return Var;
            }
        }

        private TypeShort(bool isConstant) : base(isConstant, 2) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "short ";
        }
    }

    public sealed class TypeUnsignedShort : TypeBuiltIn {

        private static readonly TypeUnsignedShort Const = new TypeUnsignedShort(true);
        private static readonly TypeUnsignedShort Var = new TypeUnsignedShort(false);

        public static TypeUnsignedShort Constant {
            get {
                return Const;
            }
        }

        public static TypeUnsignedShort Variable {
            get {
                return Var;
            }
        }

        private TypeUnsignedShort(bool isConstant) : base(isConstant, 2) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "unsigned short ";
        }
    }
}
