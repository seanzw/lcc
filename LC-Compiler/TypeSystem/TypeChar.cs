using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {

    /// <summary>
    /// Built-in type char.
    /// </summary>
    public sealed class TChar : TInteger {

        private static readonly TChar instance = new TChar();

        public static TChar Instance => instance;
        public override int RANK => 1;

        private TChar()
            : base(1) {
        }


        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "char";
        }

        public override BigInteger MAX => TSChar.Instance.MAX;
        public override BigInteger MIN => TSChar.Instance.MIN;
    }

    public sealed class TUChar : TInteger {

        private static readonly TUChar instance = new TUChar(false);

        public static TUChar Instance => instance;
        public override int RANK => 1;
        private TUChar(bool isConstant) : base(1) { }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned char";
        }

        public override BigInteger MAX => 255;
        public override BigInteger MIN => 0;
    }

    public sealed class TSChar : TInteger {

        private static readonly TSChar instance = new TSChar();

        public static TSChar Instance => instance;
        public override int RANK => 1;

        private TSChar() : base(1) { }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "signed char";
        }

        public override BigInteger MAX => 127;
        public override BigInteger MIN => -128;
    }
}
