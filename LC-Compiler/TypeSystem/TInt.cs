using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {

    /// <summary>
    /// Built-in type char.
    /// </summary>
    public sealed class TChar : TCharacter {

        private static readonly TChar instance = new TChar();
        public static TChar Instance => instance;
        public override int Rank => 1;
        public override BigInteger MAX => TSChar.Instance.MAX;
        public override BigInteger MIN => TSChar.Instance.MIN;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "char";
        }
    }

    public sealed class TUChar : TCharacter {

        private static readonly TUChar instance = new TUChar();
        public static TUChar Instance => instance;
        public override int Rank => 1;
        public override BigInteger MAX => 255;
        public override BigInteger MIN => 0;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned char";
        }
    }

    public sealed class TSChar : TCharacter {

        private static readonly TSChar instance = new TSChar();
        public static TSChar Instance => instance;
        public override int Rank => 1;
        public override BigInteger MAX => 127;
        public override BigInteger MIN => -128;


        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "signed char";
        }
    }

    public sealed class TShort : TInteger {

        private static readonly TShort instance = new TShort();
        public static TShort Instance => instance;
        public override int Rank => 2;
        public override int Bits => 16;
        public override BigInteger MAX => short.MaxValue;
        public override BigInteger MIN => short.MinValue;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "short";
        }
    }

    public sealed class TUShort : TInteger {

        private static readonly TUShort instance = new TUShort();
        public static TUShort Instance => instance;
        public override int Rank => 2;
        public override int Bits => 16;
        public override BigInteger MAX => ushort.MaxValue;
        public override BigInteger MIN => ushort.MinValue;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned short";
        }

        
    }

    public sealed class TInt : TInteger {

        private static readonly TInt instance = new TInt();

        public static TInt Instance => instance;
        public override int Rank => 3;
        public override int Bits => 32;
        public override BigInteger MAX => int.MaxValue;
        public override BigInteger MIN => int.MinValue;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "int";
        }
    }

    public sealed class TUInt : TInteger {

        private static readonly TUInt instance = new TUInt();

        public static TUInt Instance => instance;
        public override int Rank => 3;
        public override int Bits => 32;
        public override BigInteger MAX => uint.MaxValue;
        public override BigInteger MIN => uint.MinValue;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned int";
        }
    }

    public sealed class TLong : TInteger {

        private static readonly TLong instance = new TLong();

        public static TLong Instance => instance;
        public override int Rank => 4;
        public override int Bits => 32;
        public override BigInteger MAX => TInt.Instance.MAX;
        public override BigInteger MIN => TInt.Instance.MIN;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "long";
        }
    }

    public sealed class TULong : TInteger {

        private static readonly TULong instance = new TULong();

        public static TULong Instance => instance;
        public override int Rank => 4;
        public override int Bits => 32;
        public override BigInteger MAX => TUInt.Instance.MAX;
        public override BigInteger MIN => TUInt.Instance.MIN;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned long";
        }
    }

    public sealed class TLLong : TInteger {

        private static readonly TLLong instance = new TLLong();
        public static TLLong Instance => instance;
        public override int Rank => 5;
        public override int Bits => 64;
        public override BigInteger MAX => long.MaxValue;
        public override BigInteger MIN => long.MinValue;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "long long";
        }
    }

    public sealed class TULLong : TInteger {

        private static readonly TULLong instance = new TULLong();
        public static TULLong Instance => instance;
        public override int Rank => 5;
        public override int Bits => 64;
        public override BigInteger MAX => ulong.MaxValue;
        public override BigInteger MIN => ulong.MinValue;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "unsigned long long";
        }
    }
}
