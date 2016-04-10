using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public sealed class TFloat : TArithmetic {

        private static readonly TFloat instance = new TFloat();
        public static TFloat Instance => instance;
        public override int Rank => 6;
        public override int Size => 4;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "float";
        }
    }

    public sealed class TDouble : TArithmetic {

        private static readonly TDouble instance = new TDouble();

        public static TDouble Instance => instance;
        public override int Rank => 6;
        public override int Size => 8;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "double";
        }
    }

    public sealed class TLDouble : TArithmetic {

        private static readonly TLDouble instance = new TLDouble();
        public static TLDouble Instance => instance;
        public override int Rank => 7;
        public override int Size => 8;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "long double";
        }
    }
}
