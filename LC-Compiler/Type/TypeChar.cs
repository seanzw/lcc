using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {

    /// <summary>
    /// Built-in type char.
    /// </summary>
    public sealed class TypeChar : TypeBuiltIn {

        private static readonly TypeChar Var = new TypeChar(false);
        private static readonly TypeChar Const = new TypeChar(true);

        public static TypeChar Variable {
            get {
                return Var;
            }
        }

        public static TypeChar Constant {
            get {
                return Const;
            }
        }

        private TypeChar(bool isConstant)
            : base(isConstant, 1) {
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "char ";
        }
    }

    public sealed class TypeUnsignedChar : TypeBuiltIn {

        private static readonly TypeUnsignedChar Var = new TypeUnsignedChar(false);
        private static readonly TypeUnsignedChar Const = new TypeUnsignedChar(true);

        public static TypeUnsignedChar Variable {
            get {
                return Var;
            }
        }

        public static TypeUnsignedChar Constant {
            get {
                return Const;
            }
        }

        private TypeUnsignedChar(bool isConstant)
            : base(isConstant, 1) {
        }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "unsigned char ";
        }
    }

    public sealed class TypeSignedChar : TypeBuiltIn {

        private static readonly TypeSignedChar Var = new TypeSignedChar(false);
        private static readonly TypeSignedChar Const = new TypeSignedChar(true);

        public static TypeSignedChar Variable {
            get {
                return Var;
            }
        }

        public static TypeSignedChar Constant {
            get {
                return Const;
            }
        }

        private TypeSignedChar(bool isConstant)
            : base(isConstant, 1) {
        }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return base.ToString() + "signed char ";
        }
    }
}
