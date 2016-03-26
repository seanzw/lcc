using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeLong : TypeBuiltIn {

        private static readonly TypeLong Const = new TypeLong(true);
        private static readonly TypeLong Var = new TypeLong(false);

        public static TypeLong Constant {
            get {
                return Const;
            }
        }

        public static TypeLong Variable {
            get {
                return Var;
            }
        }

        private TypeLong(bool isConstant) : base(isConstant, 4) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "long ";
        }
    }

    public sealed class TypeUnsignedLong : TypeBuiltIn {

        private static readonly TypeUnsignedLong Const = new TypeUnsignedLong(true);
        private static readonly TypeUnsignedLong Var = new TypeUnsignedLong(false);

        public static TypeUnsignedLong Constant {
            get {
                return Const;
            }
        }

        public static TypeUnsignedLong Variable {
            get {
                return Var;
            }
        }

        private TypeUnsignedLong(bool isConstant) : base(isConstant, 4) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "unsigned long ";
        }
    }
}
