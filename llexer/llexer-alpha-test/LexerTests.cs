using System;
using llexer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace llexer_alpha_test {
    [TestClass]
    public class LexerTests {
        [TestMethod]
        public void llexer_alpha_lexer() {

            string src = @"
$(\\.|[^\$\n\\])*$  
    lltokens.AddLast(new T_REGEX_LITERAL(lltext));

%%

            ";

            var tokens = Lexer.Instance.scan(src);
            foreach (var token in tokens) {
                Console.WriteLine(token);
            }
        }

        [TestMethod]
        public void llexer_alpha_parser() {

            string src = @"
$(\\.|[^\$\n\\])*$  
    lltokens.AddLast(new T_REGEX_LITERAL(lltext));

$[ \n\r\t]+$

%%

            ";

            var tokens = Lexer.Instance.scan(src);
            llexer_alpha.ASTLex ast = llparser.Parser.Instance.parse(tokens);
            Console.WriteLine(ast);
        }
    }
}
