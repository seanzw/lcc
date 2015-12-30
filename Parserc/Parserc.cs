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

        public static Parser<I, V> End<I, V>(this Parser<I, V> parser) {
            return parser.Bind(x => End<I, V>(x));
        }

        public static Parser<I, V> End<I, V>(V value) {
            return tokens => {
                if (tokens.More()) {
                    return new List<ParserResult<I, V>>();
                } else {
                    var tokensNew = tokens.Copy();
                    return new List<ParserResult<I, V>> {
                        new ParserResult<I, V>(value, tokensNew)
                    };
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
        /// Simpler version of Bind without taking the result of previous parser.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Parser<I, V> Then<I, S, V>(this Parser<I, S> first, Parser<I, V> second) {
            return first.Bind(_ => second);
        }

        /// <summary>
        /// Project the result to another domain.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="V1"></typeparam>
        /// <typeparam name="V2"></typeparam>
        /// <param name="parser"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static Parser<I, V2> Select<I, V1, V2>(this Parser<I, V1> parser, Func<V1, V2> converter) {
            return parser.Bind(x => Result<I, V2>(converter(x)));
        }

        /// <summary>
        /// Return the result if the previous parser succeed.
        /// Bind <-> Select
        /// Then <-> Return
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="V1"></typeparam>
        /// <typeparam name="V2"></typeparam>
        /// <param name="parser"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Parser<I, V2> Return<I, V1, V2>(this Parser<I, V1> parser, V2 result) {
            return parser.Then(Result<I, V2>(result));
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

        public static Parser<I, V> Else<I, V>(this Parser<I, V> first, Parser<I, V> second) {
            return tokens => {
                var r = first(tokens);
                if (r.Count == 0) {
                    return second(tokens);
                } else {
                    return r;
                }
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

        /// <summary>
        /// Apply a left assocative operator.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="parser"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public static Parser<I, V> ChainPlus<I, V>(this Parser<I, V> parser, Parser<I, Func<V, V, V>> op) {
            Func<V, Parser<I, V>> rest = Recursion.Y<V, Parser<I, V>>(
                rec => x => op
                .Bind(f => parser
                .Bind(y => rec(f(x, y))))
                .Or(Result<I, V>(x))
                );

            return parser.Bind(rest);
        }

        /// <summary>
        /// Allow circular reference by delay the evaluation with lambda expression.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="reference"></param>
        /// <returns></returns>
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
