using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public sealed class TLong : TInteger {

        private static readonly TLong instance = new TLong();

        public static TLong Instance => instance;
        public override int RANK => 4;

        private TLong() : base(4) { }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "long";
        }

        public override BigInteger MAX => TInt.Instance.MAX;
        public override BigInteger MIN => TInt.Instance.MIN;
    }

    public sealed class TULong : TInteger {

        private static readonly TULong instance = new TULong();

        public static TULong Instance => instance;
        public override int RANK => 4;

        private TULong() : base(4) { }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned long";
        }

        public override BigInteger MAX => TUInt.Instance.MAX;
        public override BigInteger MIN => TUInt.Instance.MIN;
    }
}
