using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.Token;
using lcc.Type;

namespace lcc.AST {

    public sealed class ASTIdentifier : ASTExpr {

        public ASTIdentifier(T_IDENTIFIER token) {
            line = token.line;
            name = token.name;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTIdentifier id = obj as ASTIdentifier;
            if (id == null) {
                return false;
            } else {
                return base.Equals(obj)
                    && id.name.Equals(name)
                    && id.line == line;
            }
        }

        public bool Equals(ASTIdentifier id) {
            return base.Equals(id)
                && id.name.Equals(name)
                && id.line == line;
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly string name;
        public readonly int line;
    }

    public abstract class ASTConstant : ASTExpr {

        public ASTConstant(int line) {
            this.line = line;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTConstant c = obj as ASTConstant;
            return c == null ? false : base.Equals(c)
                && c.line == line;
        }

        public bool Equals(ASTConstant c) {
            return base.Equals(c)
                && c.line == line;
        }

        public override int GetHashCode() {
            return line;
        }

        protected static Func<char, bool> IsOctal = (char c) => c >= '0' && c <= '7';
        protected static Func<char, bool> IsDecimal = (char c) => c >= '0' && c <= '9';
        protected static Func<char, bool> IsHexadecimal = (char c) => IsDecimal(c) || char.ToLower(c) >= 'a' && char.ToLower(c) <= 'f';
        protected static Func<char, int> GetHexadecimal = (char c) => {
            if (IsDecimal(c)) return c - '0';
            else return char.ToLower(c) - 'a' + 10;
        };

        public readonly int line;
    }

    /// <summary>
    /// Represent an integer constant with its value and its type.
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
    public sealed class ASTConstInt : ASTConstant {

        public ASTConstInt(T_CONST_INT token) : base(token.line) {
            value = Evaluate(token);

            // Select the proper type for this constant.
            switch (token.suffix) {
                case T_CONST_INT.Suffix.NONE:
                    if (token.n != 10)
                        // Octal or hexadecimal constant.
                        type = FitInType(token.line, value,
                            TypeInt.Instance,
                            TypeUnsignedInt.Instance,
                            TypeLong.Instance,
                            TypeUnsignedLong.Instance,
                            TypeLongLong.Instance,
                            TypeUnsignedLongLong.Instance);
                    else
                        // Decimal constant.
                        type = FitInType(token.line, value,
                            TypeInt.Instance,
                            TypeLong.Instance,
                            TypeLongLong.Instance);
                    break;
                case T_CONST_INT.Suffix.U:
                    type = FitInType(token.line, value,
                        TypeUnsignedInt.Instance,
                        TypeUnsignedLong.Instance,
                        TypeUnsignedLongLong.Instance);
                    break;
                case T_CONST_INT.Suffix.L:
                    if (token.n != 10)
                        type = FitInType(token.line, value,
                            TypeLong.Instance,
                            TypeUnsignedLong.Instance,
                            TypeLongLong.Instance,
                            TypeUnsignedLongLong.Instance);
                    else
                        type = FitInType(token.line, value,
                            TypeLong.Instance,
                            TypeLongLong.Instance);
                    break;
                case T_CONST_INT.Suffix.UL:
                    type = FitInType(token.line, value,
                        TypeUnsignedLong.Instance,
                        TypeUnsignedLongLong.Instance);
                    break;
                case T_CONST_INT.Suffix.LL:
                    if (token.n != 10)
                        type = FitInType(token.line, value,
                            TypeLongLong.Instance,
                            TypeUnsignedLongLong.Instance);
                    else
                        type = FitInType(token.line, value, TypeLongLong.Instance);
                    break;
                case T_CONST_INT.Suffix.ULL:
                    type = FitInType(token.line, value, TypeUnsignedLongLong.Instance);
                    break;
            }
        }

        public override bool Equals(object obj) {
            ASTConstInt ci = obj as ASTConstInt;
            return ci == null ? false : base.Equals(ci)
                && ci.value == value
                && ci.type.Equals(type);
        }
        
        public bool Equals(ASTConstInt ci) {
            return base.Equals(ci)
                && ci.value == value
                && ci.type.Equals(type);
        }

        public override int GetHashCode() {
            return line;
        }

        public Type.Type TypeCheck(ASTEnv env) {
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

        private static Type.Type FitInType(int line, BigInteger value, params IntegerType[] types) { 
            foreach (var type in types) {
                if (value >= type.MIN && value <= type.MAX) {
                    return type.MakeConst();
                }
            }
            throw new ASTErrIntegerLiteralOutOfRange(line);
        }

        public readonly BigInteger value;
        public readonly Type.Type type;
    }

    /// <summary>
    /// A character is either an octal sequence, a hexadecimal sequence or an ascii character.
    /// An octal sequence and a hexadecimal sequence shall be in the range of representable values for the type unsigned char.
    /// </summary>
    public sealed class ASTConstChar : ASTConstant {

        public ASTConstChar(T_CONST_CHAR token) : base(token.line) {

            // Do not support multi-character characer.
            if (token.prefix == T_CONST_CHAR.Prefix.L) {
                throw new ASTErrUnknownType(line, "multi-character");
            }

            values = Evaluate(line, token.text);

            // Do not support multi-character characer.
            if (values.Count() > 1) {
                throw new ASTErrUnknownType(line, "multi-character");
            }

            type = TypeUnsignedChar.Instance.MakeConst();
        }

        public override bool Equals(object obj) {
            ASTConstChar cc = obj as ASTConstChar;
            return cc == null ? false : base.Equals(cc)
                && cc.values.SequenceEqual(values)
                && cc.type.Equals(type);
        }

        public bool Equals(ASTConstChar cc) {
            return base.Equals(cc)
                && cc.values.SequenceEqual(values)
                && cc.type.Equals(type);
        }

        public override int GetHashCode() {
            return line;
        }

        /// <summary>
        /// Type check.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public Type.Type TypeCheck(ASTEnv env) {
            return type;
        }

        /// <summary>
        /// Evaluate the text to a list of integer character constant.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<ushort> Evaluate(int line, string text) {

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
                if (v < TypeUnsignedChar.Instance.MIN || v > TypeUnsignedChar.Instance.MAX) {
                    throw new ASTErrEscapedSequenceOutOfRange(line, text);
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
        public readonly Type.Type type;
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
    public sealed class ASTConstFloat : ASTConstant {
        public ASTConstFloat(T_CONST_FLOAT token) : base(token.line) {
            value = Evaluate(token);
            switch (token.suffix) {
                case T_CONST_FLOAT.Suffix.NONE:
                    type = TypeDouble.Instance.MakeConst();
                    break;
                case T_CONST_FLOAT.Suffix.F:
                    type = TypeFloat.Instance.MakeConst();
                    break;
                case T_CONST_FLOAT.Suffix.L:
                    type = TypeLongDouble.Instance.MakeConst();
                    break;
            }
        }

        public override bool Equals(object obj) {
            ASTConstFloat cf = obj as ASTConstFloat;
            return cf == null ? false : base.Equals(cf)
                && cf.value == value
                && cf.type.Equals(type);
        }

        public bool Equals(ASTConstFloat cf) {
            return base.Equals(cf)
                && cf.value == value
                && cf.type.Equals(type);
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

        public override int GetHashCode() {
            return line;
        }

        public readonly double value;
        public readonly Type.Type type;
    }

    public sealed class ASTString : ASTExpr {
        public ASTString(LinkedList<T_STRING_LITERAL> tokens) {
            this.line = tokens.First().line;
            values = Evaluate(tokens);
            var arrType = new TypeArray(TypeChar.Instance.MakeType(), values.Count());
            this.type = arrType.MakeType();
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTString s = obj as ASTString;
            return s == null ? false : base.Equals(s)
                && s.values.SequenceEqual(values);
        }

        public bool Equals(ASTString s) {
            return base.Equals(s)
                && s.values.SequenceEqual(values);
        }

        public override int GetHashCode() {
            return values.First();
        }

        public Type.Type TypeCheck(ASTEnv env) {
            return type;
        }

        public static IEnumerable<ushort> Evaluate(LinkedList<T_STRING_LITERAL> tokens) {

            IEnumerable<ushort> values = new LinkedList<ushort>();

            foreach (var t in tokens) {
                if (t.prefix == T_STRING_LITERAL.Prefix.L) {
                    throw new ASTErrUnknownType(t.line, "multi-character");
                }
                values = values.Concat(ASTConstChar.Evaluate(t.line, t.text));
            }
            return values;
        }

        public readonly int line;
        public readonly IEnumerable<ushort> values;
        public readonly Type.Type type;
    }
}
