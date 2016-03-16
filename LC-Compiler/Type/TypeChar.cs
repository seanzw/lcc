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
        public TypeChar(bool isConstant)
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

        public TypeUnsignedChar(bool isConstant)
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

        public TypeSignedChar(bool isConstant)
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
