using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parserc {

    public delegate List<ParserResult<I, V>> Parser<I, V>(ITokenStream<I> tokens);

    public static class Parserc {

        /// <summary>
        /// Returns a parser that match everything.
        /// </summary>
        /// <param name="value"> Returned value for this parser. </param>
        /// <returns> A result parser. </returns>
        public static Parser<I, V> Result<I, V>(V value) {
            return tokens => {
                var tokensNew = tokens.Copy();
                return new List<ParserResult<I, V>> {
                    new ParserResult<I, V>(value, tokensNew)
                };
            };
        }

        /// <summary>
        /// Returns a parser that always fails.
        /// </summary>
        /// <returns></returns>
        public static Parser<I, V> Zero<I, V>() {
            return tokens => {
                return new List<ParserResult<I, V>>();
            };
        }

        /// <summary>
        /// Consumes and returns a token.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <returns></returns>
        public static Parser<I, I> Item<I>() {
            return tokens => {
                var tokensNew = tokens.Copy();
                if (tokens.More()) {
                    return new List<ParserResult<I, I>> {
                        new ParserResult<I, I>(tokensNew.Next(), tokensNew)
                    };
                } else {
                    return new List<ParserResult<I, I>>();
                }
            };
        }

        /// <summary>
        /// Bind two parsers together.
        /// The result list is flatened.
        /// </summary>
        /// <typeparam name="I"> Input type. </typeparam>
        /// <typeparam name="V"> Return value type. </typeparam>
        /// <param name="first"> The first parser. </param>
        /// <param name="second"> Takes a value and return a parser. </param>
        /// <returns> A new parser. </returns>
        public static Parser<I, V2> Bind<I, V1, V2>(this Parser<I, V1> first, Func<V1, Parser<I, V2>> second) {
            return tokens => {
                var ret = new List<ParserResult<I, V2>>();
                foreach (var r in first(tokens)) {
                    foreach (var s in second(r.value)(r.remain)) {
                        ret.Add(s);
                    }
                }
                return ret;
            };
        }

        /// <summary>
        /// Match one token that satisifys predicate.
        /// </summary>
        /// <typeparam name="I"> Token type. </typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Parser<I, I> Sat<I>(Func<I, bool> predicate) {
            return 
                Item<I>()
                .Bind(x => predicate(x) ? Result<I, I>(x) : Zero<I, I>());
        }

        /// <summary>
        /// Applys parser1 and parser2.
        /// Returns the combined result.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Parser<I, V> Or<I, V>(this Parser<I, V> first, Parser<I, V> second) {
            return tokens => {
                return new List<ParserResult<I, V>>(first(tokens).Concat(second(tokens)));
            };
        }

        /// <summary>
        /// Apply the parser one or more times.
        /// Result is stored in a linked list.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<I, LinkedList<V>> Plus<I, V>(this Parser<I, V> parser) {
            return parser
                .Bind(x => parser
                .Many()
                .Bind(xs => {
                    xs.AddFirst(x);
                    return Result<I, LinkedList<V>>(xs);
                }));
        }

        /// <summary>
        /// Apply the parser zero or more times.
        /// Result is stored in a LinkedList.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<I, LinkedList<V>> Many<I, V>(this Parser<I, V> parser) {
            return parser
                .Plus()
                .Or(Result<I, LinkedList<V>>(new LinkedList<V>()));
        }

        /// <summary>
        /// Apply the parser in the following pattern.
        /// 
        /// [x:xs | x = parser, xs = many [y | _ = sep, y = parser]]
        /// 
        /// parser sep parser ... sep parser
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="parser"></param>
        /// <param name="sep"></param>
        /// <returns></returns>
        public static Parser<I, LinkedList<V>> PlusSeperatedBy<I, V, S>(this Parser<I, V> parser, Parser<I, S> sep) {
            return parser
                .Bind(x => sep
                .Bind(_ => parser)
                .Many()
                .Bind(xs => {
                    xs.AddFirst(x);
                    return Result<I, LinkedList<V>>(xs);
                }));
        }

        /// <summary>
        /// Apply parser in following pattern:
        /// parser sep parser ... sep parser OR
        /// epsilon
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="parser"></param>
        /// <param name="sep"></param>
        /// <returns></returns>
        public static Parser<I, LinkedList<V>> ManySeperatedBy<I, V, S>(this Parser<I, V> parser, Parser<I, S> sep) {
            return parser
                .PlusSeperatedBy(sep)
                .Or(Result<I, LinkedList<V>>(new LinkedList<V>()));
        }

        /// <summary>
        /// Apply the parser in following pattern:
        /// bra parser ket
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="S1"></typeparam>
        /// <typeparam name="S2"></typeparam>
        /// <param name="parser"></param>
        /// <param name="bra"></param>
        /// <param name="ket"></param>
        /// <returns></returns>
        public static Parser<I, V> Bracket<I, V, S1, S2>(this Parser<I, V> parser, Parser<I, S1> bra, Parser<I, S2> ket) {
            return bra
                .Bind(s => parser
                .Bind(x => ket
                .Bind(t => Result<I, V>(x))));
        }

        public static Parser<I, V> ChainPlus<I, V>(this Parser<I, V> parser, Parser<I, Func<V, V, V>> op) {
            Func<V, Parser<I, V>> rest = Recursion.Y<V, Parser<I, V>>(
                rec => x => op
                .Bind(f => parser
                .Bind(y => rec(f(x, y))))
                .Or(Result<I, V>(x))
                );

            return parser.Bind(rest);
        }

        public static Parser<I, V> Ref<I, V>(Func<Parser<I, V>> reference) {
            Parser<I, V> p = null;
            return i => {
                if (p == null) {
                    p = reference();
                }
                return p(i);
            };
        }
    }
}
