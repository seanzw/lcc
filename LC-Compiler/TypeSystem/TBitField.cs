using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {

    public abstract class TBitField : TObject {
        public TBitField(int bits) {
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

        public TBoolBit(int bits) : base(bits) { }
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
    }

    public sealed class TIntBit : TBitField {
        public TIntBit(int bits) : base(bits) { }
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
    }

    public sealed class TUIntBit : TBitField {
        public TUIntBit(int bits) : base(bits) { }
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
    }
}
