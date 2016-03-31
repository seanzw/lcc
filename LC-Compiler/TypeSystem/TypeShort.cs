using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public sealed class TShort : TInteger {

        private static readonly TShort instance = new TShort();

        public static TShort Instance => instance;
        public override int RANK => 2;

        private TShort() : base(2) { }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "short";
        }

        public override BigInteger MAX => short.MaxValue;
        public override BigInteger MIN => short.MinValue;
    }

    public sealed class TUShort : TInteger {

        private static readonly TUShort instance = new TUShort();

        public static TUShort Instance => instance;
        public override int RANK => 2;

        private TUShort() : base(2) { }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned short";
        }

        public override BigInteger MAX => ushort.MaxValue;
        public override BigInteger MIN => ushort.MinValue;
    }
}
