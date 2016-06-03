using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public sealed class TSingle : TFloat {

        private static readonly TSingle instance = new TSingle();
        public static TSingle Instance => instance;
        public override int Rank => 6;
        public override int Bits => 32;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "float";
        }
    }

    public sealed class TDouble : TFloat {

        private static readonly TDouble instance = new TDouble();

        public static TDouble Instance => instance;
        public override int Rank => 6;
        public override int Bits => 64;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "double";
        }
    }

    public sealed class TLDouble : TFloat {

        private static readonly TLDouble instance = new TLDouble();
        public static TLDouble Instance => instance;
        public override int Rank => 7;
        public override int Bits => 64;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "long double";
        }
    }
}
