using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public sealed class TSingle : TRealFloat {
        private TSingle() : base(TKind.SINGLE) { }
        private static readonly TSingle instance = new TSingle();
        public static TSingle Instance => instance;
        public override int Rank => 6;
        public override int Bits => 32;
        public override TComplex Complex => TCSingle.Instance;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "float";
        }
    }

    public sealed class TCSingle : TComplex {
        private TCSingle() : base(TKind.CSINGLE) { }
        private static readonly TCSingle instance = new TCSingle();
        public static TCSingle Instance => instance;
        public override int Rank => 6;
        public override int Bits => 64;
        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "float _Complex";
        }
    }

    public sealed class TDouble : TRealFloat {
        private TDouble() : base(TKind.DOUBLE) { }
        private static readonly TDouble instance = new TDouble();

        public static TDouble Instance => instance;
        public override int Rank => 7;
        public override int Bits => 64;
        public override TComplex Complex => TCDouble.Instance;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "double";
        }
    }

    public sealed class TCDouble : TComplex {
        private TCDouble() : base(TKind.CDOUBLE) { }
        private static readonly TCDouble instance = new TCDouble();

        public static TCDouble Instance => instance;
        public override int Rank => 7;
        public override int Bits => 128;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "double _Complex";
        }
    }

    public sealed class TLDouble : TRealFloat {
        private TLDouble() : base(TKind.LDOUBLE) { }
        private static readonly TLDouble instance = new TLDouble();
        public static TLDouble Instance => instance;
        public override int Rank => 8;
        public override int Bits => 64;
        public override TComplex Complex => TCLDouble.Instance;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "long double";
        }
    }

    public sealed class TCLDouble : TComplex {
        private TCLDouble() : base(TKind.CLDOUBLE) { }
        private static readonly TCLDouble instance = new TCLDouble();

        public static TCLDouble Instance => instance;
        public override int Rank => 8;
        public override int Bits => 128;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "long double _Complex";
        }
    }
}
