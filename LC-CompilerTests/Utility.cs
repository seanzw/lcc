using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.AST;
using lcc.Token;
using lcc.Parser;
using LLexer;
using Parserc;

namespace LC_CompilerTests {
    public class Utility {

        public static List<ParserResult<Token, R>> parse<R>(
            string src,
            Parser<Token, R> parser
            ) {
            var tokens = new ReadOnlyCollection<Token>(Lexer.Instance.Scan(src));
            var stream = new TokenStream<Token>(tokens);
            return parser(stream);
        }
    }
}
