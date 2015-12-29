using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parserc.PChar {

    /// <summary>
    /// Some useful parsers for processing char.
    /// </summary>
    public static class CharParser {

        /// <summary>
        /// Match a single char.
        /// </summary>
        /// <param name="c"> Char to be matched. </param>
        /// <returns></returns>
        public static Parser<char, char> Char(char c) {
            return Combinator.Sat<char>(x => x == c);
        }

        public static Parser<char, char> Digit() {
            return Combinator.Sat<char>(x => x >= '0' && x <= '9');
        }

        public static Parser<char, char> Lower() {
            return Combinator.Sat<char>(x => x >= 'a' && x <= 'z');
        }

        public static Parser<char, char> Upper() {
            return Combinator.Sat<char>(x => x >= 'A' && x <= 'Z');
        }

        public static Parser<char, char> Letter() {
            return Lower().Plus(Upper());
        }

        public static Parser<char, char> AlphaNum() {
            return Letter().Plus(Digit());
        }

        /// <summary>
        /// Match a word, which can be empty.
        /// This actually is lazy evaluation.
        /// </summary>
        /// <returns></returns>
        public static Parser<char, string> Word() {
            Parser<char, string> neWord = Letter()
                .Bind(x => { return Word()
                .Bind(xs => { return Primitive.Result<char, string>(x + xs); });
                });
            return neWord.Plus(Primitive.Result<char, string>(""));
        }
    }
}
