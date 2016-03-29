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

        public ASTConstant(int line, string text) {
            this.line = line;
            this.text = text;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTConstant c = obj as ASTConstant;
            return c == null ? false : base.Equals(c)
                && c.line == line
                && c.text.Equals(text);
        }

        public bool Equals(ASTConstant c) {
            return base.Equals(c)
                && c.line == line
                && c.text.Equals(text);
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly int line;
        public readonly string text;
    }

    public sealed class ASTConstInt : ASTConstant {

        public ASTConstInt(T_CONST_INT token) : base(token.line, token.text) {
            this.suffix = token.suffix;
            this.value = Evaluate(token);
        }

        public override bool Equals(object obj) {
            ASTConstInt ci = obj as ASTConstInt;
            return ci == null ? false : base.Equals(ci)
                && ci.value == value
                && ci.suffix == suffix;
        }
        
        public bool Equals(ASTConstInt ci) {
            return base.Equals(ci)
                && ci.value == value
                && ci.suffix == suffix;
        }

        public override int GetHashCode() {
            return line;
        }

        /// <summary>
        /// TODO: Evaluate the text and get the value.
        /// </summary>
        /// <param name="token"> Token to be evaluated. </param>
        /// <returns> Long. </returns>
        private long Evaluate(T_CONST_INT token) {
            return 0;
        }

        public readonly long value;
        public readonly T_CONST_INT.Suffix suffix;
    }

    public sealed class ASTConstChar : ASTConstant {

        public ASTConstChar(T_CONST_CHAR token) : base(token.line, token.text) {
            prefix = token.prefix;
        }

        public override bool Equals(object obj) {
            ASTConstChar cc = obj as ASTConstChar;
            return cc == null ? false : base.Equals(cc)
                && cc.prefix == prefix;
        }

        public bool Equals(ASTConstChar cc) {
            return base.Equals(cc) && cc.prefix == prefix;
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

            // Do not support multi-character characer.
            if (prefix == T_CONST_CHAR.Prefix.L) {
                throw new ASTErrUnknownType(line, "multi-character");
            }

            // Do not support multi-character characer.
            if (Values.Count() > 1) {
                throw new ASTErrUnknownType(line, "multi-character");
            }

            return TypeUnsignedChar.Instance.MakeConst();
        }

        /// <summary>
        /// Evaluate the text to a list of integer character constant.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<ushort> Evaluate(int line, string text) {

            Func<char, bool> IsOctal = (char c) => c >= '0' && c <= '7';
            Func<char, bool> IsDecimal = (char c) => c >= '0' && c <= '9';
            Func<char, bool> IsHexadecimal = (char c) => IsDecimal(c) || char.ToLower(c) >= 'a' && char.ToLower(c) <= 'f';
            Func<char, int> GetHexadecimal = (char c) => {
                if (IsDecimal(c)) return c - '0';
                else return char.ToLower(c) - 'a' + 10;
            };

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
                if (v < TypeUnsignedChar.MIN || v > TypeUnsignedChar.MAX) {
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

        public readonly T_CONST_CHAR.Prefix prefix;

        /// <summary>
        /// Store the values from Evaluate().
        /// </summary>
        private IEnumerable<ushort> values;

        /// <summary>
        /// Cache the result from Evaluate().
        /// </summary>
        public IEnumerable<ushort> Values {
            get {
                if (values == null) values = Evaluate(line, text);
                return values;
            }
        }
    }

    public sealed class ASTConstFloat : ASTConstant {
        public ASTConstFloat(T_CONST_FLOAT token) : base(token.line, token.text) {
            suffix = token.suffix;
            value = Evaluate(token);
        }

        public override bool Equals(object obj) {
            ASTConstFloat cf = obj as ASTConstFloat;
            return cf == null ? false : base.Equals(cf)
                && cf.value == value
                && cf.suffix == suffix;
        }

        public bool Equals(ASTConstFloat cf) {
            return base.Equals(cf)
                && cf.value == value
                && cf.suffix == suffix;
        }

        /// <summary>
        /// TODO: Evaluate the text and get the value.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private float Evaluate(T_CONST_FLOAT token) {
            return 0.0f;
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly float value;
        public readonly T_CONST_FLOAT.Suffix suffix;
    }

    public sealed class ASTString : ASTExpr {
        public ASTString(LinkedList<T_STRING_LITERAL> tokens) {
            this.tokens = tokens;
        }

        public override int GetLine() {
            return tokens.First().line;
        }

        public override bool Equals(object obj) {
            ASTString s = obj as ASTString;
            return s == null ? false : base.Equals(s)
                && s.tokens.SequenceEqual(tokens);
        }

        public bool Equals(ASTString s) {
            return base.Equals(s)
                && s.tokens.SequenceEqual(tokens);
        }

        public override int GetHashCode() {
            return tokens.First().line;
        }

        public Type.Type TypeCheck(ASTEnv env) {
            var type = new TypeArray(TypeChar.Instance.MakeType(), Values.Count());
            return type.MakeType();
        }

        public readonly LinkedList<T_STRING_LITERAL> tokens;

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

        private IEnumerable<ushort> values;
        public IEnumerable<ushort> Values {
            get {
                if (values == null) values = Evaluate(tokens);
                return values;
            }
        }
    }
}
