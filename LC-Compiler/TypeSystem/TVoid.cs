using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public sealed class TVoid : TUnqualified {

        private static readonly TVoid instance = new TVoid();
        public static TVoid Instance => instance;
        public override int Bits => 8;
        public override bool IsComplete => false;
        /// <summary>
        /// void is always defined.
        /// </summary>
        public override bool IsDefined => true;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "void";
        }
    }
}
