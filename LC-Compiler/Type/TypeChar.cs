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
    public sealed class TypeChar : ArithmeticType {

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

        public static BigInteger MAX = 127;
        public static BigInteger MIN = -128;
    }

    public sealed class TypeUnsignedChar : ArithmeticType {

        private static readonly TypeUnsignedChar instance = new TypeUnsignedChar(false);

        public static TypeUnsignedChar Instance {
            get {
                return instance;
            }
        }
        private TypeUnsignedChar(bool isConstant) : base(1) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned char";
        }

        public static BigInteger MIN = 0;
        public static BigInteger MAX = 255;
    }

    public sealed class TypeSignedChar : ArithmeticType {

        private static readonly TypeSignedChar instance = new TypeSignedChar();

        public static TypeSignedChar Instance {
            get {
                return instance;
            }
        }

        private TypeSignedChar()
            : base(1) {
        }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "signed char";
        }
    }
}
