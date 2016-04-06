using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Parserc.Parserc;
using lcc.AST;
using lcc.Token;

// Simplify...
using T = lcc.Token.Token;

namespace lcc.Parser {
    public static partial class Parser {

        /// <summary>
        /// Match a specific kind of token.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <returns></returns>
        public static Parserc.Parser<T, T> Match<I>()
            where I : T {
            return Sat<T>(x => x is I);
        }

        /// <summary>
        /// Match a token and cast it to specific type.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <returns></returns>
        public static Parserc.Parser<T, I> Get<I>()
            where I : T {
            return Match<I>().Select(x => (I)x);
        }

        /// <summary>
        /// Chain a binary expression with left associative opeartor.
        /// </summary>
        /// <param name="parser"> Basic element parser. </param>
        /// <param name="sep"> Match the operator. </param>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTExpr> ChainBinaryExpr(
            this Parserc.Parser<T, ASTExpr> parser,
            Parserc.Parser<T, ASTBinaryExpr.Op> sep
            ) {
            return parser.ChainPlus(sep.Select(op => {
                Func<ASTExpr, ASTExpr, ASTExpr> f = (lhs, rhs) => new ASTBinaryExpr(lhs, rhs, op);
                return f;
            }));
        }

        /// <summary>
        /// Match ( parser ).
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parserc.Parser<T, V> ParentLR<V>(this Parserc.Parser<T, V> parser) {
            return parser.Bracket(Match<T_PUNC_PARENTL>(), Match<T_PUNC_PARENTR>());
        }

        /// <summary>
        /// Match { parser }
        /// Notice this will handle all the scope.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parserc.Parser<T, V> BracelLR<V>(this Parserc.Parser<T, V> parser) {
            return Match<T_PUNC_BRACEL>().Bind(_ => {
                Env.PushScope();
                return parser;
            }).Bind(result => Match<T_PUNC_BRACER>().Bind(_ => {
                Env.PopScope();
                return Result<T, V>(result);
            }));
        }

        /// <summary>
        /// Match [ parser ].
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parserc.Parser<T, V> SubLR<V>(this Parserc.Parser<T, V> parser) {
            return parser.Bracket(Match<T_PUNC_SUBSCRIPTL>(), Match<T_PUNC_SUBSCRIPTR>());
        }

        /// <summary>
        /// identifier
        ///     : T_IDENTIFIER
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTId> Identifier() {
            return Get<T_IDENTIFIER>().Select(t => new ASTId(t));
        }
    }

    public class TypedefRedefined : Exception {
        public TypedefRedefined(int line, string name) {
            this.line = line;
            this.name = name;
        }

        public override string ToString() {
            return string.Format("ParserError: line {0} typedef {1} is redefined", line, name);
        }

        private readonly int line;
        private readonly string name;
    }
}
