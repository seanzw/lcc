using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public sealed class TypeVoid : TUnqualified {

        private static readonly TypeVoid instance = new TypeVoid();
        public static TypeVoid Instance => instance;
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
