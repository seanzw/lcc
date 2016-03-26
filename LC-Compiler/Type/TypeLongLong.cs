using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeLongLong : TypeBuiltIn {

        private static readonly TypeLongLong Const = new TypeLongLong(true);
        private static readonly TypeLongLong Var = new TypeLongLong(false);

        public static TypeLongLong Constant {
            get {
                return Const;
            }
        }

        public static TypeLongLong Variable {
            get {
                return Var;
            }
        }

        private TypeLongLong(bool isConstant) : base(isConstant, 8) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "long long ";
        }
    }

    public sealed class TypeUnsignedLongLong : TypeBuiltIn {

        private static readonly TypeUnsignedLongLong Const = new TypeUnsignedLongLong(true);
        private static readonly TypeUnsignedLongLong Var = new TypeUnsignedLongLong(false);

        public static TypeUnsignedLongLong Constant {
            get {
                return Const;
            }
        }

        public static TypeUnsignedLongLong Variable {
            get {
                return Var;
            }
        }

        private TypeUnsignedLongLong(bool isConstant) : base(isConstant, 4) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "unsigned long long ";
        }
    }
}
