using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Parserc.Primitive;

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
        public static Parser<char, char> Character(char c) {
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
            return Lower().Or(Upper());
        }

        public static Parser<char, char> AlphaNum() {
            return Letter().Or(Digit());
        }

        /// <summary>
        /// Match a word, which can be empty.
        /// </summary>
        /// <returns></returns>
        public static Parser<char, string> Word() {
            return Letter().Many().Bind(xs => {
                return Primitive.Result<char, string>(new string(xs.ToArray()));
            });
        }

        public static Parser<char, uint> Natural() {
            return Digit().Plus().Bind(xs => {
                uint x = 0;
                foreach (char c in xs) {
                    x = x * 10 + (uint)char.GetNumericValue(c);
                }
                return Result<char, uint>(x);
            });
        }

        /// <summary>
        /// Parse an integer.
        /// </summary>
        /// <returns></returns>
        public static Parser<char, int> Integer() {
            var Sign = Character('-')
                .Bind(_ => Result<char, Func<uint, int>>(x => -(int)x))
                .Or(Result<char, Func<uint, int>>(x => (int)x));
            return Sign
                .Bind(f => Natural()
                .Bind(u => Result<char, int>(f(u))));
        }

        /// <summary>
        /// Parser an integer list as {int,int,int}.
        /// </summary>
        /// <returns></returns>
        public static Parser<char, LinkedList<int>> IntegerList() {
            return Integer()
                .ManySeperatedBy(Character(','))
                .Bracket(Character('{'), Character('}'));
        }
    }
}
