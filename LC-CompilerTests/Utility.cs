using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.SyntaxTree;
using lcc.Token;
using lcc.AST;
using lcc.Parser;
using LLexer;
using Parserc;

namespace LC_CompilerTests {
    public class Utility {

        public static string CGen(string src) {
            var results = parse(src, Parser.TranslationUnit().End(), true);
            Assert.AreEqual(1, results.Count());
            Assert.IsFalse(results.First().Remain.More());
            var ast = results.First().Value.ToAST(new lcc.SyntaxTree.Env());
            var gen = new lcc.AST.X86Gen();
            ast.ToX86(gen);
            return gen.ToString();
        }

        public static IEnumerable<IParserResult<Token, R>> parse<R>(
            string src,
            Parser<Token, R> parser,
            bool clear = true
            ) {
            if (clear) {
                lcc.Parser.Env.Clear();
            }
            var tokens = new ReadOnlyCollection<Token>(Lexer.Instance.Scan(src));
            var stream = new TokenStream<Token>(tokens);
            return parser(stream);
        }
    }
}
