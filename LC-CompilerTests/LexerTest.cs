using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.Token;
using LLexer;

namespace LC_CompilerTests {
    [TestClass]
    public class LexerTest {

        private static void Aux(string src, List<Token> truth) {
            var tokens = Lexer.Instance.Scan(src);
            Assert.AreEqual(tokens.Count, truth.Count);
            for (int i = 0; i < tokens.Count; ++i) {
                Assert.AreEqual(tokens[i], truth[i]);
            }
        }

        [TestMethod]
        public void LCCLexerKeyword() {
            string src = @"
auto auto break char const 
double enum do inline int long goto 
restrict short volatile register signed unsigned
union struct void typedef switch _Complex _Imaginary
            ";
            List<Token> truth = new List<Token> {
                new T_KEY_AUTO(2),
                new T_KEY_AUTO(2),
                new T_KEY_BREAK(2),
                new T_KEY_CHAR(2),
                new T_KEY_CONST(2),
                new T_KEY_DOUBLE(3),
                new T_KEY_ENUM(3),
                new T_KEY_DO(3),
                new T_KEY_INLINE(3),
                new T_KEY_INT(3),
                new T_KEY_LONG(3),
                new T_KEY_GOTO(3),
                new T_KEY_RESTRICT(4),
                new T_KEY_SHORT(4),
                new T_KEY_VOLATILE(4),
                new T_KEY_REGISTER(4),
                new T_KEY_SIGNED(4),
                new T_KEY_UNSIGNED(4),
                new T_KEY_UNION(5),
                new T_KEY_STRUCT(5),
                new T_KEY_VOID(5),
                new T_KEY_TYPEDEF(5),
                new T_KEY_SWITCH(5),
                new T_KEY__COMPLEX(5),
                new T_KEY__IMAGINARY(5)
            };
            Aux(src, truth);
        }

        [TestMethod]
        public void LCCLexerIdentifier() {
            string src = @"
int a char b double _a int a0 what c_0_CA_0
            ";
            List<Token> truth = new List<Token> {
                new T_KEY_INT(2),
                new T_IDENTIFIER(2, "a"),
                new T_KEY_CHAR(2),
                new T_IDENTIFIER(2, "b"),
                new T_KEY_DOUBLE(2),
                new T_IDENTIFIER(2, "_a"),
                new T_KEY_INT(2),
                new T_IDENTIFIER(2, "a0"),
                new T_IDENTIFIER(2, "what"),
                new T_IDENTIFIER(2, "c_0_CA_0")
            };
            Aux(src, truth);
        }

        [TestMethod]
        public void LCCLexerConstantInt() {
            string src = @"
0123 012u 043l 076ll 054Ull 045Lu
0x123u 0x234l 0XFELL
123u
            ";
            List<Token> truth = new List<Token> {
                new T_CONST_INT(2, "0123", 8),
                new T_CONST_INT(2, "012u", 8),
                new T_CONST_INT(2, "043l", 8),
                new T_CONST_INT(2, "076ll", 8),
                new T_CONST_INT(2, "054Ull", 8),
                new T_CONST_INT(2, "045Lu", 8),
                new T_CONST_INT(3, "123u", 16),
                new T_CONST_INT(3, "234l", 16),
                new T_CONST_INT(3, "FELL", 16),
                new T_CONST_INT(4, "123u", 10)
            };
            Aux(src, truth);
        }

        [TestMethod]
        public void LCCLexerConstantFloat() {
            string src = @"
1.23e+4f .34f 1.23 .45 2e4l
0x1f.34p12f 0X23p28l
            ";
            List<Token> truth = new List<Token> {
                new T_CONST_FLOAT(2, "1.23e+4f", 10),
                new T_CONST_FLOAT(2, ".34f", 10),
                new T_CONST_FLOAT(2, "1.23", 10),
                new T_CONST_FLOAT(2, ".45", 10),
                new T_CONST_FLOAT(2, "2e4l", 10),
                new T_CONST_FLOAT(3, "1f.34p12f", 16),
                new T_CONST_FLOAT(3, "23p28l", 16)
            };
            Aux(src, truth);
        }

        [TestMethod]
        public void LCCLexerConstantChar() {
            string src = @"
'\'' 'a' '\""' '""' '\76' '\xABC' '\n' L'\n'
            ";
            List<Token> truth = new List<Token> {
                new T_CONST_CHAR(2, @"'\''"),
                new T_CONST_CHAR(2, @"'a'"),
                new T_CONST_CHAR(2, @"'\""'"),
                new T_CONST_CHAR(2, @"'""'"),
                new T_CONST_CHAR(2, @"'\76'"),
                new T_CONST_CHAR(2, @"'\xABC'"),
                new T_CONST_CHAR(2, @"'\n'"),
                new T_CONST_CHAR(2, @"L'\n'")
            };
            Aux(src, truth);
        }

        [TestMethod]
        public void LCCLexerStringLiteral() {
            string src = @"
""abc""
""\n""
""""
L""\xABCD""
""\\""
""\
""
""\\n""
            ";
            List<Token> truth = new List<Token> {
                new T_STRING_LITERAL(2, @"""abc"""),
                new T_STRING_LITERAL(3, @"""\n"""),
                new T_STRING_LITERAL(4, @""""""),
                new T_STRING_LITERAL(5, @"L""\xABCD"""),
                new T_STRING_LITERAL(6, @"""\\"""),
                new T_STRING_LITERAL(7, "\"\\\r\n\""),
                new T_STRING_LITERAL(9, @"""\\n""")
            };
            Aux(src, truth);
        }

        [TestMethod]
        public void LCCLexerPunctuator() {
            string src = @"
++ -- + - [ ] { ) > < <= >>=
            ";
            List<Token> truth = new List<Token> {
                new T_PUNC_INCRE(2),
                new T_PUNC_DECRE(2),
                new T_PUNC_PLUS(2),
                new T_PUNC_MINUS(2),
                new T_PUNC_SUBSCRIPTL(2),
                new T_PUNC_SUBSCRIPTR(2),
                new T_PUNC_BRACEL(2),
                new T_PUNC_PARENTR(2),
                new T_PUNC_GT(2),
                new T_PUNC_LT(2),
                new T_PUNC_LE(2),
                new T_PUNC_SHIFTREQ(2)
            };
            Aux(src, truth);
        }

        [TestMethod]
        public void LCCLexerComment() {
            string src = @"
//++ -- + - [ ] { ) > < <= >>=
/* what ****************/
/*  

*/
            ";
            List<Token> truth = new List<Token>();
            Aux(src, truth);
        }

        [TestMethod]
        public void LCCLexerHelloWorld() {
            string src = @"

/**************************************************
 Hello World!
***************************************************/
int main(int argc, char* argv[]) {

    // Print hello world!
    printf(""Hello world from %s!\n"", ""Sean"");
    return 0;

}
            ";
            List<Token> truth = new List<Token> {
                new T_KEY_INT(6),
                new T_IDENTIFIER(6, "main"),
                new T_PUNC_PARENTL(6),
                new T_KEY_INT(6),
                new T_IDENTIFIER(6, "argc"),
                new T_PUNC_COMMA(6),
                new T_KEY_CHAR(6),
                new T_PUNC_STAR(6),
                new T_IDENTIFIER(6, "argv"),
                new T_PUNC_SUBSCRIPTL(6),
                new T_PUNC_SUBSCRIPTR(6),
                new T_PUNC_PARENTR(6),
                new T_PUNC_BRACEL(6),
                new T_IDENTIFIER(9, "printf"),
                new T_PUNC_PARENTL(9),
                new T_STRING_LITERAL(9, "\"Hello world from %s!\\n\""),
                new T_PUNC_COMMA(9),
                new T_STRING_LITERAL(9, "\"Sean\""),
                new T_PUNC_PARENTR(9),
                new T_PUNC_SEMICOLON(9),
                new T_KEY_RETURN(10),
                new T_CONST_INT(10, "0", 8),
                new T_PUNC_SEMICOLON(10),
                new T_PUNC_BRACER(12)
            };
            Aux(src, truth);
        }


        [TestMethod]
        [ExpectedException(typeof(LexerException), "Unmatched quote.")]
        public void LCCLexerIllegal1() {
            string src = "'\n'";
            var tokens = Lexer.Instance.Scan(src);
        }

        [TestMethod]
        [ExpectedException(typeof(LexerException), "Unmatched quote.")]
        public void LCCLexerIllegalEscaped() {
            string src = "'\\'";
            var tokens = Lexer.Instance.Scan(src);
        }

        [TestMethod]
        [ExpectedException(typeof(LexerException), "Unmatched quote.")]
        public void LCCLexerIllegal2() {
            string src = "'a";
            var tokens = Lexer.Instance.Scan(src);
        }

        [TestMethod]
        [ExpectedException(typeof(LexerException), "Long char.")]
        public void LCCLexerIllegal3() {
            string src = "'\\1234'";
            var tokens = Lexer.Instance.Scan(src);
        }

        [TestMethod]
        [ExpectedException(typeof(LexerException), "Unmatched double quote.")]
        public void LCCLexerIllegal4() {
            string src = "\"abc";
            var tokens = Lexer.Instance.Scan(src);
        }

        [TestMethod]
        [ExpectedException(typeof(LexerException), "Unmatched double quote.")]
        public void LCCLexerIllegal5() {
            string src = "abc\"";
            var tokens = Lexer.Instance.Scan(src);
        }

        [TestMethod]
        [ExpectedException(typeof(LexerException), "Illegal new line inside string.")]
        public void LCCLexerIllegal6() {
            string src = "\"\n\"";
            var tokens = Lexer.Instance.Scan(src);
        }

        [TestMethod]
        [ExpectedException(typeof(LexerException), "Illegal character.")]
        public void LCCLexerIllegal7() {
            string src = "$";
            var tokens = Lexer.Instance.Scan(src);
        }
    }
}
