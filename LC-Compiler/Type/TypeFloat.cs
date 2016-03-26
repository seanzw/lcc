using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeFloat : TypeBuiltIn {

        private static readonly TypeFloat Const = new TypeFloat(true);
        private static readonly TypeFloat Var = new TypeFloat(false);

        public static TypeFloat Constant {
            get {
                return Const;
            }
        }

        public static TypeFloat Variable {
            get {
                return Var;
            }
        }

        private TypeFloat(bool isConstant) : base(isConstant, 4) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "float ";
        }
    }

    public sealed class TypeDouble : TypeBuiltIn {

        private static readonly TypeDouble Const = new TypeDouble(true);
        private static readonly TypeDouble Var = new TypeDouble(false);

        public static TypeDouble Constant {
            get {
                return Const;
            }
        }

        public static TypeDouble Variable {
            get {
                return Var;
            }
        }

        private TypeDouble(bool isConstant) : base(isConstant, 8) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "double ";
        }
    }

    public sealed class TypeLongDouble : TypeBuiltIn {

        private static readonly TypeLongDouble Const = new TypeLongDouble(true);
        private static readonly TypeLongDouble Var = new TypeLongDouble(false);

        public static TypeLongDouble Constant {
            get {
                return Const;
            }
        }

        public static TypeLongDouble Variable {
            get {
                return Var;
            }
        }

        private TypeLongDouble(bool isConstant) : base(isConstant, 8) { }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "long double ";
        }
    }
}
