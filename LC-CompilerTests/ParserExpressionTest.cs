using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.AST;
using lcc.Token;
using lcc.Parser;
using LLexer;

namespace LC_CompilerTests {
    [TestClass]
    public class ParserExpressionTest {

        private static void Aux<R>(
            string src,
            Parserc.Parser<Token, R> parser,
            R truth
            ) {
            var tokens = new ReadOnlyCollection<Token>(Lexer.Instance.Scan(src));
            var stream = new Parserc.TokenStream<Token>(tokens);
            var result = parser(stream);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(truth, result[0].value);
            Assert.IsFalse(result[0].remain.More());
        }

        [TestMethod]
        public void LCCParserPrimaryExpression() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "tmp",
                    new ASTIdentifier(new T_IDENTIFIER(1, "tmp"))
                },
                {
                    "12356u",
                    new ASTConstInt(new T_CONST_INT(1, "12356u", 10))
                },
                {
                    "1.264f",
                    new ASTConstFloat(new T_CONST_FLOAT(1, "1.264f", 10))
                },
                {
                    "'C'",
                    new ASTConstChar(new T_CONST_CHAR(1, "'C'"))
                },
                {
                    "\"what is this?\"",
                    new ASTString(new T_STRING_LITERAL(1, "\"what is this?\""))
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.PrimaryExpression(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserPostfixExpression() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "abc[123]",
                    new ASTArrSub(
                        new ASTIdentifier(new T_IDENTIFIER(1, "abc")),
                        new ASTConstInt(new T_CONST_INT(1, "123", 10))
                    )
                },
                {
                    "abc.x",
                    new ASTAccess(
                        new ASTIdentifier(new T_IDENTIFIER(1, "abc")),
                        new T_IDENTIFIER(1, "x"),
                        ASTAccess.Type.DOT
                    )
                },
                {
                    "abc->x",
                    new ASTAccess(
                        new ASTIdentifier(new T_IDENTIFIER(1, "abc")),
                        new T_IDENTIFIER(1, "x"),
                        ASTAccess.Type.PTR
                    )
                },
                {
                    "x++",
                    new ASTPostStep(
                        new ASTIdentifier(new T_IDENTIFIER(1, "x")),
                        ASTPostStep.Type.INC
                    )
                },
                {
                    "x--",
                    new ASTPostStep(
                        new ASTIdentifier(new T_IDENTIFIER(1, "x")),
                        ASTPostStep.Type.DEC
                    )
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.PostfixExpression(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserUnaryExpression() {

        }
    }
}
