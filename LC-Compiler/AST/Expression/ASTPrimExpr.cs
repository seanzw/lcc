using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.Token;

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
            value = Evaluate(token);
        }

        public override bool Equals(object obj) {
            ASTConstChar cc = obj as ASTConstChar;
            return cc == null ? false : base.Equals(cc)
                && cc.value == value
                && cc.prefix == prefix;
        }

        public bool Equals(ASTConstChar cc) {
            return base.Equals(cc)
                && cc.value == value
                && cc.prefix == prefix;
        }

        public override int GetHashCode() {
            return line;
        }

        /// <summary>
        /// TODO: Evaluate the text and get the char.
        /// </summary>
        /// <param name="token"> Token to be evaluated. </param>
        /// <returns> Char. </returns>
        private char Evaluate(T_CONST_CHAR token) {
            return 'a';
        }

        public readonly char value;
        public readonly T_CONST_CHAR.Prefix prefix;
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
        public ASTString(T_STRING_LITERAL token) {
            line = token.line;
            text = token.text;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTString s = obj as ASTString;
            return s == null ? false : base.Equals(s)
                && s.line == line
                && s.text.Equals(text);
        }

        public bool Equals(ASTString s) {
            return base.Equals(s)
                && s.line == line
                && s.text.Equals(text);
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly int line;
        public readonly string text;
    }
}
