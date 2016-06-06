using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {

    public abstract class TBitField : TObject {
        public TBitField(TKind Kind, int bits) : base(Kind) {
            this.bits = bits;
        }
        public override bool IsBitField => true;
        public override int Bits => bits;
        public override int Size {
            get {
                throw new InvalidOperationException("Can't take the size of a bit field type. ");
            }
        }
        protected readonly int bits;
    }

    public sealed class TBoolBit : TBitField {

        public TBoolBit(int bits) : base(TKind.BOOLBIT, bits) { }
        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }
        public bool Equals(TBoolBit x) {
            return x != null && x.bits == bits;
        }
        public override bool Equals(object obj) {
            return Equals(obj as TBoolBit);
        }
        public override int GetHashCode() {
            return bits;
        }
        public override string ToString() {
            return string.Format("_Bool({0})", bits);
        }
        public static TBoolBit New(int bits) {
            return new TBoolBit(bits);
        }
    }

    public sealed class TIntBit : TBitField {
        public TIntBit(int bits) : base(TKind.INTBIT, bits) { }
        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }
        public bool Equals(TIntBit x) {
            return x != null && x.bits == bits;
        }
        public override bool Equals(object obj) {
            return Equals(obj as TIntBit);
        }
        public override int GetHashCode() {
            return bits;
        }
        public override string ToString() {
            return string.Format("int({0})", bits);
        }
        public static TIntBit New(int bits) {
            return new TIntBit(bits);
        }
    }

    public sealed class TUIntBit : TBitField {
        public TUIntBit(int bits) : base(TKind.UINTBIT, bits) { }
        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }
        public bool Equals(TUIntBit x) {
            return x != null && x.bits == bits;
        }
        public override bool Equals(object obj) {
            return Equals(obj as TUIntBit);
        }
        public override int GetHashCode() {
            return bits;
        }
        public override string ToString() {
            return string.Format("unsigned int({0})", bits);
        }
        public static TUIntBit New(int bits) {
            return new TUIntBit(bits);
        }
    }
}
