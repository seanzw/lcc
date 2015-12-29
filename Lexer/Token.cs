using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOKEN = System.String;

namespace Lexer {
    abstract public class Token {

        public enum TYPE {
            REGEX_LITERAL,
            CODE_LINE,
            LBRACKET,
            RBRACKET,
            SPLITER
        }

        protected Token(TYPE type) {
            this.type = type;
        }

        public override string ToString() {
            return type.ToString();
        }

        public readonly TYPE type;
    }

    public sealed class T_SPLITER : Token {
        public T_SPLITER() : base(TYPE.SPLITER) { }
    }

    abstract public class TokenSymbol : Token {
        protected TokenSymbol(string src, TYPE type) : base(type) {
            this.src = src;
        }

        public override string ToString() {
            return type + ": " + src;
        }

        public readonly string src;
    }

    public sealed class T_REGEX : TokenSymbol {
        public T_REGEX(string src) : base(src, TYPE.REGEX_LITERAL) { }
    }

    public sealed class T_CODE : TokenSymbol {
        public T_CODE(string src) : base(src, TYPE.CODE_LINE) { }
    }


}
