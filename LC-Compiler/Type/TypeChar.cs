using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {

    /// <summary>
    /// Built-in type char.
    /// </summary>
    public sealed class TypeChar : IntegerType {

        private static readonly TypeChar instance = new TypeChar();

        public static TypeChar Instance {
            get {
                return instance;
            }
        }

        private TypeChar()
            : base(1) {
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "char";
        }

        public override BigInteger MAX => TypeSignedChar.Instance.MAX;
        public override BigInteger MIN => TypeSignedChar.Instance.MIN;
    }

    public sealed class TypeUnsignedChar : IntegerType {

        private static readonly TypeUnsignedChar instance = new TypeUnsignedChar(false);

        public static TypeUnsignedChar Instance => instance;
        private TypeUnsignedChar(bool isConstant) : base(1) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned char";
        }

        public override BigInteger MAX => 255;
        public override BigInteger MIN => 0;
    }

    public sealed class TypeSignedChar : IntegerType {

        private static readonly TypeSignedChar instance = new TypeSignedChar();

        public static TypeSignedChar Instance => instance;

        private TypeSignedChar() : base(1) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "signed char";
        }

        public override BigInteger MAX => 127;
        public override BigInteger MIN => -128;
    }
}
