using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public sealed class TLLong : TInteger {

        private static readonly TLLong instance = new TLLong();

        public static TLLong Instance => instance;
        private TLLong() : base(8) { }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "long long";
        }

        public override BigInteger MAX => long.MaxValue;
        public override BigInteger MIN => long.MinValue;
    }

    public sealed class TULLong : TInteger {

        private static readonly TULLong instance = new TULLong();

        public static TULLong Instance => instance;
        private TULLong() : base(4) { }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned long long";
        }

        public override BigInteger MAX => ulong.MaxValue;
        public override BigInteger MIN => ulong.MinValue;
    }
}
