using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.Token;
using lcc.TypeSystem;

namespace lcc.AST {

    /// <summary>
    /// Represents an identfier.
    /// </summary>
    public sealed class ASTId : ASTExpr, IEquatable<ASTId> {

        public ASTId(T_IDENTIFIER token) {
            pos = new Position { line = token.line };
            name = token.name;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTId);
        }

        public bool Equals(ASTId x) {
            return x != null && x.name.Equals(name) && x.pos.Equals(pos);
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        /// <summary>
        /// Check that the identfifier is defined as variable.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override T TypeCheck(ASTEnv env) {
            T type = env.TSymbol(name);
            if (type == null) throw new ASTErrUndefinedIdentifier(pos, name);
            else return type;
        }

        public readonly string name;

        private readonly Position pos;
    }

    public abstract class ASTConstant : ASTExpr {

        public ASTConstant(Position pos) {
            this.pos = pos;
        }

        public override Position Pos => pos;

        protected static Func<char, bool> IsOctal = (char c) => c >= '0' && c <= '7';
        protected static Func<char, bool> IsDecimal = (char c) => c >= '0' && c <= '9';
        protected static Func<char, bool> IsHexadecimal = (char c) => IsDecimal(c) || char.ToLower(c) >= 'a' && char.ToLower(c) <= 'f';
        protected static Func<char, int> GetHexadecimal = (char c) => {
            if (IsDecimal(c)) return c - '0';
            else return char.ToLower(c) - 'a' + 10;
        };

        protected readonly Position pos;
    }

    /// <summary>
    /// Represent an integer constant with its value and its type (rvalue).
    /// The type of an integer constant is the first of the corresponding list in which its value can be represented.
    /// 
    /// Sufix               Decimal Constant                Octal or Hexadecimal Constant
    /// ------------------------------------------------------------------------------------------
    /// NONE                int                             int
    ///                     long int                        unsigned int
    ///                     long long int                   long int
    ///                                                     unsigned long int
    ///                                                     long long int
    ///                                                     unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// U                   unsigned int                    unsigned int
    ///                     unsigned long int               unsigned long int
    ///                     unsigned long long int          unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// L                   long int                        long int
    ///                     long long int                   unsigned long int
    ///                                                     long long int
    ///                                                     unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// UL                  unsigned long int               unsigned long int
    ///                     unsigned long long int          unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// LL                  long long int                   long long int
    ///                                                     unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// ULL                 unsigned long long int          unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// 
    /// </summary>
    public sealed class ASTConstInt : ASTConstant, IEquatable<ASTConstInt> {

        public ASTConstInt(T_CONST_INT token) : base(new Position { line = token.line }) {
            value = Evaluate(token);

            // Select the proper type for this constant.
            switch (token.suffix) {
                case T_CONST_INT.Suffix.NONE:
                    if (token.n != 10)
                        // Octal or hexadecimal constant.
                        type = FitInType(pos, value,
                            TInt.Instance,
                            TUInt.Instance,
                            TLong.Instance,
                            TULong.Instance,
                            TLLong.Instance,
                            TULLong.Instance);
                    else
                        // Decimal constant.
                        type = FitInType(pos, value,
                            TInt.Instance,
                            TLong.Instance,
                            TLLong.Instance);
                    break;
                case T_CONST_INT.Suffix.U:
                    type = FitInType(pos, value,
                        TUInt.Instance,
                        TULong.Instance,
                        TULLong.Instance);
                    break;
                case T_CONST_INT.Suffix.L:
                    if (token.n != 10)
                        type = FitInType(pos, value,
                            TLong.Instance,
                            TULong.Instance,
                            TLLong.Instance,
                            TULLong.Instance);
                    else
                        type = FitInType(pos, value,
                            TLong.Instance,
                            TLLong.Instance);
                    break;
                case T_CONST_INT.Suffix.UL:
                    type = FitInType(pos, value,
                        TULong.Instance,
                        TULLong.Instance);
                    break;
                case T_CONST_INT.Suffix.LL:
                    if (token.n != 10)
                        type = FitInType(pos, value,
                            TLLong.Instance,
                            TULLong.Instance);
                    else
                        type = FitInType(pos, value, TLLong.Instance);
                    break;
                case T_CONST_INT.Suffix.ULL:
                    type = FitInType(pos, value, TULLong.Instance);
                    break;
            }
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTConstInt);
        }
        
        public bool Equals(ASTConstInt x) {
            return x != null && x.pos.Equals(pos)
                && x.value == value
                && x.type.Equals(type);
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        public override T TypeCheck(ASTEnv env) {
            return type;
        }

        /// <summary>
        /// Evaluate the text and get the value.
        /// </summary>
        /// <param name="token"> Token to be evaluated. </param>
        /// <returns> BigInteger. </returns>
        private static BigInteger Evaluate(T_CONST_INT token) {

            BigInteger value = 0;
            
            foreach (var c in token.text) {
                value = value * token.n + GetHexadecimal(c);
            }

            return value;
        }

        private static T FitInType(Position pos, BigInteger value, params TInteger[] types) { 
            foreach (var type in types) {
                if (value >= type.MIN && value <= type.MAX) {
                    return type.Const(T.LR.R);
                }
            }
            throw new ASTErrIntegerLiteralOutOfRange(pos);
        }

        public readonly BigInteger value;
        public readonly T type;
    }

    /// <summary>
    /// A character is either an octal sequence, a hexadecimal sequence or an ascii character.
    /// An octal sequence and a hexadecimal sequence shall be in the range of representable values for the type unsigned char.
    /// </summary>
    public sealed class ASTConstChar : ASTConstant {

        public ASTConstChar(T_CONST_CHAR token) : base(new Position { line = token.line }) {

            // Do not support multi-character characer.
            if (token.prefix == T_CONST_CHAR.Prefix.L) {
                throw new ASTErrUnknownType(pos, "multi-character");
            }

            values = Evaluate(pos, token.text);

            // Do not support multi-character characer.
            if (values.Count() > 1) {
                throw new ASTErrUnknownType(pos, "multi-character");
            }

            type = TUChar.Instance.Const(T.LR.R);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTConstChar);
        }

        public bool Equals(ASTConstChar x) {
            return x != null && x.pos.Equals(pos)
                && x.values.SequenceEqual(values)
                && x.type.Equals(type);
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        /// <summary>
        /// Type check.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override T TypeCheck(ASTEnv env) {
            return type;
        }

        /// <summary>
        /// Evaluate the text to a list of integer character constant.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<ushort> Evaluate(Position pos, string text) {

            BigInteger value = 0;
            LinkedList<ushort> values = new LinkedList<ushort>();

            // state 0: initial state.
            // state 1: \.
            // state 2: \x.
            // state 3: \o.
            // state 4: \oo.

            const int INITIAL = 0;
            const int ESCAPED = 1;
            const int HEXADEC = 2;
            const int OCTAL1 = 3;
            const int OCTAL2 = 4;

            int state = INITIAL;

            // Check the value is within the range of unsigned char
            // and push it into the list.
            Action<BigInteger> PushValue = v => {
                if (v < TUChar.Instance.MIN || v > TUChar.Instance.MAX) {
                    throw new ASTErrEscapedSequenceOutOfRange(pos, text);
                }
                values.AddLast((ushort)v);  // Store the current result.
            };

            for (int i = 0; i < text.Length; ++i) {

                switch (state) {
                    case INITIAL:
                        // Initial state.
                        if (text[i] == '\\') {
                            state = ESCAPED;
                        } else {
                            values.AddLast(text[i]);
                        }
                        break;
                    case ESCAPED:
                        // Seen one backslash.
                        if (text[i] == 'x') {
                            state = HEXADEC;
                        } else if (IsOctal(text[i])) {
                            value = value * 8 + (text[i] - '0');
                            state = OCTAL1;
                        } else {
                            state = INITIAL;
                            switch (text[i]) {
                                case 'a':
                                    values.AddLast(7);
                                    break;
                                case 'b':
                                    values.AddLast(8);
                                    break;
                                case 'f':
                                    values.AddLast(12);
                                    break;
                                case 'n':
                                    values.AddLast(10);
                                    break;
                                case 'r':
                                    values.AddLast(13);
                                    break;
                                case 't':
                                    values.AddLast(9);
                                    break;
                                case 'v':
                                    values.AddLast(11);
                                    break;
                                default:
                                    values.AddLast(text[i]);
                                    break;
                            }
                        }
                        break;
                    case HEXADEC:
                        if (IsHexadecimal(text[i])) {
                            value = value * 16 + GetHexadecimal(text[i]);
                        } else {
                            // Check the value is within the range of unsinged char.
                            PushValue(value);
                            value = 0;              // Reset the value to zero;
                            i--;                    // Backward one char.
                            state = INITIAL;        // Reset the state.
                        }
                        break;
                    case OCTAL1:
                        if (IsOctal(text[i])) {
                            value = value * 8 + (text[i] - '0');
                            state = OCTAL2;
                        } else {
                            // Check the value is within the range of unsinged char.
                            PushValue(value);
                            value = 0;
                            i--;
                            state = INITIAL;
                        }
                        break;
                    case OCTAL2:
                        if (IsOctal(text[i])) {
                            value = value * 8 + (text[i] - '0');
                        } else {
                            i--;
                        }
                        // Check the value is within the range of unsinged char.
                        PushValue(value);
                        state = INITIAL;
                        value = 0;
                        break;
                }
            }

            // Must in escaped sequence.
            if (state != INITIAL) {
                PushValue(value);
            }

            return values;
        }

        /// <summary>
        /// Store the values from Evaluate().
        /// </summary>
        public readonly IEnumerable<ushort> values;

        /// <summary>
        /// Type of this constant.
        /// </summary>
        public readonly T type;
    }

    /// <summary>
    /// A float constant is composed with
    /// 
    /// significant-part [eEpP] exponent-part
    /// 
    /// whole-part . fraction-part [eEpP] exponent-part.
    /// 
    /// Significant-part is processed as rational number with base 10 or 16.
    /// Exponent-part is processes as a decimal integer, to which 10 or 2 will be raised.
    /// 
    /// </summary>
    public sealed class ASTConstFloat : ASTConstant, IEquatable<ASTConstFloat> {
        public ASTConstFloat(T_CONST_FLOAT token) : base(new Position { line = token.line }) {
            value = Evaluate(token);
            switch (token.suffix) {
                case T_CONST_FLOAT.Suffix.NONE:
                    type = TDouble.Instance.Const(T.LR.R);
                    break;
                case T_CONST_FLOAT.Suffix.F:
                    type = TFloat.Instance.Const(T.LR.R);
                    break;
                case T_CONST_FLOAT.Suffix.L:
                    type = TLDouble.Instance.Const(T.LR.R);
                    break;
            }
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTConstFloat);
        }

        public bool Equals(ASTConstFloat x) {
            return x != null && x.pos.Equals(pos)
                && Math.Abs(x.value - value) < 0.0001f
                && x.type.Equals(type);
        }

        private static double Evaluate(T_CONST_FLOAT token) {

            const int WHOLE = 0;
            const int FRACT = 1;
            const int PEXPO = 2;
            const int NEXPO = 3;

            int state = WHOLE;

            double value = 0;
            int fractCount = -1;
            int exponPart = 0;

            Func<char, bool> IsExpon = c => char.ToLower(c) == 'p' || char.ToLower(c) == 'e';

            foreach (var c in token.text) {
                switch (state) {
                    case WHOLE:
                        if (c == '.') state = FRACT;
                        else if (IsExpon(c)) state = PEXPO;
                        else value = value * token.n + GetHexadecimal(c);
                        break;
                    case FRACT:
                        if (IsExpon(c)) state = PEXPO;
                        else value += GetHexadecimal(c) * Math.Pow(token.n, fractCount--);
                        break;
                    case PEXPO:
                        if (c == '-') state = NEXPO;
                        else if (c == '+') state = PEXPO;
                        else exponPart = exponPart * 10 + c - '0';
                        break;
                    case NEXPO:
                        exponPart = exponPart * 10 - (c - '0');
                        break;
                }
            }

            value *= Math.Pow(token.n == 10 ? 10 : 2, exponPart);

            return value;
        }

        public override T TypeCheck(ASTEnv env) {
            return type;
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        public readonly double value;
        public readonly T type;
    }

    /// <summary>
    /// A string is variable sequence of char.
    /// Notice: string is array of char, lvalue. which means
    ///     "what"[0] = 'c';
    /// is totally legal.
    /// 
    /// Each char is the same as in constant character integer defined in ASTConstChar.
    /// </summary>
    public sealed class ASTString : ASTExpr, IEquatable<ASTString> {
        public ASTString(LinkedList<T_STRING_LITERAL> tokens) {
            pos = new Position { line = tokens.First().line };
            values = Evaluate(tokens);
            var arrType = new TArray(TChar.Instance.None(T.LR.L), values.Count());
            type = arrType.None(T.LR.L);
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTString);
        }

        public bool Equals(ASTString s) {
            return s != null && s.pos.Equals(pos) && s.values.SequenceEqual(values);
        }

        public override int GetHashCode() {
            return values.First();
        }

        public override T TypeCheck(ASTEnv env) {
            return type;
        }

        public static IEnumerable<ushort> Evaluate(LinkedList<T_STRING_LITERAL> tokens) {

            IEnumerable<ushort> values = new LinkedList<ushort>();

            foreach (var t in tokens) {
                Position pos = new Position { line = t.line };
                if (t.prefix == T_STRING_LITERAL.Prefix.L) {
                    throw new ASTErrUnknownType(pos, "multi-character");
                }
                values = values.Concat(ASTConstChar.Evaluate(pos, t.text));
            }
            return values;
        }

        public readonly Position pos;
        public readonly IEnumerable<ushort> values;
        public readonly T type;
    }
}
