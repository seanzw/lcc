using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Parserc.Parserc;
using static Parserc.CharParserc;

namespace Parserc.Examples {

    /// <summary>
    /// A simple parser calculates simple arithmetic expression.
    /// Supported operator:
    /// + - ^
    /// </summary>
    static public class Arithmetic {

        public static int Eval(string expr) {
            CharStream tokens = new CharStream(expr);
            var results = Expr().End()(tokens);
            if (results.Count() == 0) {
                throw new ArgumentException("Syntax Error: failed parsing!");
            } else if (results.Count() > 1) {
                throw new ArgumentException("Syntax Error: ambiguous result!");
            } else {
                return results.First().Value;
            }
        }

        public static Parser<char, int> Expr() {
            return Term().ChainPlus(Add());
        }

        public static Parser<char, int> Term() {
            return Factor().ChainPlus(Exp());
        }

        public static Parser<char, int> Factor() {
            return Integer()
                .Or(Ref(Expr).Bracket(Character('('), Character(')')));
        }

        public static Parser<char, Func<int, int, int>> Add() {
            return Character('+')
                .Bind(_ => Result<char, Func<int, int, int>>((x, y) => x + y))
                .Or(Character('-')
                .Bind(_ => Result<char, Func<int, int, int>>((x, y) => x - y)));
        }

        public static Parser<char, Func<int, int, int>> Exp() {
            return Character('^')
                .Bind(_ => Result<char, Func<int, int, int>>((x, y) => {
                    int ret = 1;
                    while (y != 0) {
                        if ((y & 1) == 1) {
                            ret *= x;
                        }
                        x *= x;
                        y >>= 1;
                    }
                    return ret;
                }));
        }
    }
}
