using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Token {
    abstract class T_CONST : Token {
        protected T_CONST(int line) : base(line) { }
    }

    sealed class T_CONST_INT : T_CONST {
        public enum Suffix {
            U,
            L,
            LL,
            UL,
            ULL,
            NONE
        }

        public T_CONST_INT(int line, string text, int n) : base(line) {

            text = text.ToUpper();
            this.n = n;

            int tail = text.Length - 1;
            for (tail = text.Length - 1; tail >=0; --tail) {
                if (text[tail] != 'U' && text[tail] != 'L') {
                    tail++;
                    break;
                }
            }
            this.text = text.Substring(0, tail);
            if (tail == text.Length) {
                suffix = Suffix.NONE;
            } else {
                string suffix_text = text.Substring(tail);
                switch (suffix_text) {
                    case "L":
                        suffix = Suffix.L;
                        break;
                    case "LL":
                        suffix = Suffix.LL;
                        break;
                    case "U":
                        suffix = Suffix.U;
                        break;
                    case "ULL":
                    case "LLU":
                        suffix = Suffix.ULL;
                        break;
                    case "UL":
                    case "LU":
                        suffix = Suffix.UL;
                        break;
                    default:
                        throw new ArgumentException("T_CONST_INTEGER: Unrecognized suffix: " + suffix_text);
                }
            }
        }

        public readonly Suffix suffix;
        public readonly int n;
        public readonly string text;
    }

    sealed class T_CONST_FLOAT : T_CONST {

        public enum Suffix {
            F,
            L,
            NONE
        }

        public T_CONST_FLOAT(int line, string text, int n) : base(line) {
            this.n = n;
            switch (text[text.Length - 1]) {
                case 'f':
                case 'F':
                    suffix = Suffix.F;
                    this.text = text.Substring(0, text.Length - 1);
                    break;
                case 'l':
                case 'L':
                    suffix = Suffix.L;
                    this.text = text.Substring(0, text.Length - 1);
                    break;
                default:
                    suffix = Suffix.NONE;
                    this.text = text;
                    break;
            }
            
        }

        public readonly Suffix suffix;
        public readonly int n;
        public readonly string text;
    }

    sealed class T_CONST_CHAR : T_CONST {
        public enum Prefix {
            L,
            NONE
        }
        public T_CONST_CHAR(int line, string text) : base(line) {
            if (text[0] == 'L') {
                this.prefix = Prefix.L;
                this.text = text.Substring(2, text.Length - 3);
            } else {
                this.prefix = Prefix.NONE;
                this.text = text.Substring(1, text.Length - 2);
            }
        }
        public readonly string text;
        public readonly Prefix prefix;
    }
}
