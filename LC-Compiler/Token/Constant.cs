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
            this.n = n;

            int tail = 0;
            for (tail = 0; tail < text.Length; ++tail) {
                if ((text[tail] < '0' || text[tail] > '7') &&
                    (text[tail] < 'a' || text[tail] > 'f') &&
                    (text[tail] < 'A' || text[tail] > 'F')) {
                    break;
                }
            }
            this.text = text.Substring(0, tail);
            if (tail == text.Length) {
                suffix = Suffix.NONE;
            } else {
                string suffix_text = text.Substring(tail).ToUpper();
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
}
