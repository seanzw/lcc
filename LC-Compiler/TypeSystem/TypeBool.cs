using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public sealed class TBool : TArithmetic {

        private static readonly TBool instance = new TBool();

        public static TBool Instance {
            get {
                return instance;
            }
        }

        public override int RANK => 0;

        private TBool() : base(1) { }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "_Bool";
        }
    }
}
