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

        public static Parser<I, I> Sat<I>(Func<I, bool> predicate) {
            return 
                Item<I>()
                .Bind(x => predicate(x) ? Result<I, I>(x) : Zero<I, I>());
        }

        public static Parser<I, V> Plus<I, V>(this Parser<I, V> first, Parser<I, V> second) {
            return tokens => {
                return new List<ParserResult<I, V>>(first(tokens).Concat(second(tokens)));
            };
        }


    }
}
