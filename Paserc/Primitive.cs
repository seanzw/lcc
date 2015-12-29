using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parserc {

    public delegate List<ParserResult<I, V>> Parser<I, V>(ITokenStream<I> tokens);

    public static class Primitive {

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
        /// Consume an item if the token stream is not empty.
        /// </summary>
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

    }

}
