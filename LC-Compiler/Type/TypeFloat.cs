using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public sealed class TypeFloat : ArithmeticType {

        private static readonly TypeFloat instance = new TypeFloat();

        public static TypeFloat Instance {
            get {
                return instance;
            }
        }

        private TypeFloat() : base(4) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "float";
        }
    }

    public sealed class TypeDouble : ArithmeticType {

        private static readonly TypeDouble instance = new TypeDouble();

        public static TypeDouble Instance {
            get {
                return instance;
            }
        }
        private TypeDouble() : base(8) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "double";
        }
    }

    public sealed class TypeLongDouble : ArithmeticType {

        private static readonly TypeLongDouble instance = new TypeLongDouble();

        public static TypeLongDouble Instance {
            get {
                return instance;
            }
        }
        private TypeLongDouble() : base(8) { }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "long double";
        }
    }
}
