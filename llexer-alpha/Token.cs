using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOKEN = System.String;

namespace llexer_alpha {
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

        virtual public string getSrc() {
            throw new NotImplementedException();
        }

        public readonly TYPE type;
    }

    public sealed class TokenSpliter : Token {
        public TokenSpliter() : base(TYPE.SPLITER) { }
    }

    abstract public class TokenSymbol : Token {
        protected TokenSymbol(string src, TYPE type) : base(type) {
            this.src = src;
        }

        public override string ToString() {
            return type + ": " + src;
        }

        public override string getSrc() {
            return src;
        }

        protected readonly string src;
    }

    public sealed class TokenREGEX : TokenSymbol {
        public TokenREGEX(string src) : base(src, TYPE.REGEX_LITERAL) { }
    }

    public sealed class TokenCODE : TokenSymbol {
        public TokenCODE(string src) : base(src, TYPE.CODE_LINE) { }
    }


}
