using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public sealed class TInt : TInteger {

        private static readonly TInt instance = new TInt();

        public static TInt Instance => instance;
        public override int RANK => 3;
        private TInt() : base(4) { }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "int";
        }

        public override BigInteger MAX => int.MaxValue;
        public override BigInteger MIN => int.MinValue;
    }

    public sealed class TUInt : TInteger {

        private static readonly TUInt instance = new TUInt();

        public static TUInt Instance => instance;
        public override int RANK => 3;
        private TUInt() : base(4) { }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned int";
        }

        public override BigInteger MAX => uint.MaxValue;
        public override BigInteger MIN => uint.MinValue;
    }
}
