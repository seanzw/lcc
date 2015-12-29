using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Parserc.Primitive;

namespace Parserc {

    public static class Combinator {

        /// <summary>
        /// Bind two parsers together.
        /// The result array is flated.
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


    }
}
