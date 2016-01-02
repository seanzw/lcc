using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Parserc.Parserc;
using lcc.AST;

namespace lcc.Parser {
    static partial class Parser {

        /// <summary>
        /// Match a specific kind of token.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Token.Token> Match<I>()
            where I : Token.Token {
            return Sat<Token.Token>(x => x is I);
        }

        /// <summary>
        /// Match a token and cast it to specific type.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, I> Get<I>()
            where I : Token.Token {
            return Match<I>().Select(x => (I)x);
        }

        /// <summary>
        /// Chain a binary expression with left associative opeartor.
        /// </summary>
        /// <param name="parser"> Basic element parser. </param>
        /// <param name="sep"> Match the operator. </param>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> ChainBinaryExpr(
            this Parserc.Parser<Token.Token, ASTExpr> parser,
            Parserc.Parser<Token.Token, ASTBinaryExpr.Op> sep
            ) {
            return parser.ChainPlus(sep.Select(op => {
                Func<ASTExpr, ASTExpr, ASTExpr> f = (lhs, rhs) => new ASTBinaryExpr(lhs, rhs, op);
                return f;
            }));
        }

    }
}
