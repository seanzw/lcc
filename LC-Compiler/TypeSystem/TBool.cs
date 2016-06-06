using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public sealed class TBool : TInteger {
        private TBool() : base(TKind.BOOL) { }
        private static readonly TBool instance = new TBool();
        public static TBool Instance => instance;
        public override int Rank => 0;
        public override int Bits => 8;
        public override BigInteger MIN => 0;
        public override BigInteger MAX => 1;
        public override bool IsSigned {
            get {
                throw new InvalidOperationException("IsSigned method is invalid on bool type");
            }
        }
        public override TInteger Signed {
            get {
                throw new InvalidOperationException("No corresponding signed integer type on Bool");
            }
        }

        public override TInteger Unsigned {
            get {
                throw new InvalidOperationException("No corresponding signed integer type on Bool");
            }
        }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "_Bool";
        }
    }
}
