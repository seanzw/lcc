using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Token {
    public sealed class T_STRING_LITERAL : Token {

        public enum Prefix {
            L,
            NONE
        }

        public T_STRING_LITERAL(int line, string text) : base(line) {
            if (text[0] == 'L') {
                this.text = text.Substring(2, text.Length - 3);
                this.prefix = Prefix.L;
            } else {
                this.text = text.Substring(1, text.Length - 2);
                this.prefix = Prefix.NONE;
            }
        }

        public override bool Equals(object obj) {
            T_STRING_LITERAL i = obj as T_STRING_LITERAL;
            return i == null ? false : base.Equals(obj)
                && i.prefix == prefix
                && i.text.Equals(text);
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly string text;
        public readonly Prefix prefix;
    }
}
