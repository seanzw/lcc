using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeBool : ArithmeticType {

        private static readonly TypeBool instance = new TypeBool();

        public static TypeBool Instance {
            get {
                return instance;
            }
        }

        private TypeBool() : base(1) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "_Bool";
        }
    }
}
