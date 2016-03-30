using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.AST;
using lcc.Token;
using lcc.Parser;
using LLexer;
using Parserc;

namespace LC_CompilerTests {
    [TestClass]
    public partial class ParserTests {

        /// <summary>
        /// Parse the string and check if two AST are equal.
        /// </summary>
        /// <typeparam name="R"> Result type. </typeparam>
        /// <param name="src"> Source string. </param>
        /// <param name="parser"></param>
        /// <param name="truth"></param>
        private static void Aux<R>(
            string src,
            Parser<Token, R> parser,
            R truth
            ) {
            var result = Utility.parse(src, parser);

            // Check the first result.
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(truth, result[0].value);
            Assert.IsFalse(result[0].remain.More());
        }

        /// <summary>
        /// Parse the string and check if the two result are the same.
        /// Used for linked list.
        /// </summary>
        /// <typeparam name="R"> LinkedList element type. </typeparam>
        /// <param name="src"></param>
        /// <param name="parser"></param>
        /// <param name="truth"></param>
        private static void Aux<R>(
            string src,
            Parser<Token, LinkedList<R>> parser,
            LinkedList<R> truth
            ) {
            var tokens = new ReadOnlyCollection<Token>(Lexer.Instance.Scan(src));
            var stream = new TokenStream<Token>(tokens);
            var result = parser(stream);

            // Check the first result.
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(truth.SequenceEqual(result[0].value));
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
                    new ASTString(new LinkedList<T_STRING_LITERAL>(new List<T_STRING_LITERAL> {
                        new T_STRING_LITERAL(1, "\"what is this?\"")
                    }))
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
                        ASTAccess.Kind.DOT
                    )
                },
                {
                    "arr[2].value",
                    new ASTAccess(
                        new ASTArrSub(
                            new ASTIdentifier(new T_IDENTIFIER(1, "arr")),
                            new ASTConstInt(new T_CONST_INT(1, "2", 10))
                        ),
                        new T_IDENTIFIER(1, "value"),
                        ASTAccess.Kind.DOT
                    )
                },
                {
                    "abc->x",
                    new ASTAccess(
                        new ASTIdentifier(new T_IDENTIFIER(1, "abc")),
                        new T_IDENTIFIER(1, "x"),
                        ASTAccess.Kind.PTR
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

            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "++x--",
                    new ASTPreStep(
                        new ASTPostStep(
                            new ASTIdentifier(new T_IDENTIFIER(1, "x")),
                            ASTPostStep.Type.DEC),
                        ASTPreStep.Type.INC
                    )
                },
                {
                    "--x[2]",
                    new ASTPreStep(
                        new ASTArrSub(
                            new ASTIdentifier(new T_IDENTIFIER(1, "x")),
                            new ASTConstInt(new T_CONST_INT(1, "2", 10))),
                        ASTPreStep.Type.DEC
                    )
                },
                {
                    "&x[2]",
                    new ASTUnaryOp(
                        new ASTArrSub(
                            new ASTIdentifier(new T_IDENTIFIER(1, "x")),
                            new ASTConstInt(new T_CONST_INT(1, "2", 10))),
                        ASTUnaryOp.Op.REF
                    )
                },
                {
                    "*x[2]",
                    new ASTUnaryOp(
                        new ASTArrSub(
                            new ASTIdentifier(new T_IDENTIFIER(1, "x")),
                            new ASTConstInt(new T_CONST_INT(1, "2", 10))),
                        ASTUnaryOp.Op.STAR
                    )
                },
                {
                    "+x[2]",
                    new ASTUnaryOp(
                        new ASTArrSub(
                            new ASTIdentifier(new T_IDENTIFIER(1, "x")),
                            new ASTConstInt(new T_CONST_INT(1, "2", 10))),
                        ASTUnaryOp.Op.PLUS
                    )
                },
                {
                    "-x[2]",
                    new ASTUnaryOp(
                        new ASTArrSub(
                            new ASTIdentifier(new T_IDENTIFIER(1, "x")),
                            new ASTConstInt(new T_CONST_INT(1, "2", 10))),
                        ASTUnaryOp.Op.MINUS
                    )
                },
                {
                    "~x[2]",
                    new ASTUnaryOp(
                        new ASTArrSub(
                            new ASTIdentifier(new T_IDENTIFIER(1, "x")),
                            new ASTConstInt(new T_CONST_INT(1, "2", 10))),
                        ASTUnaryOp.Op.REVERSE
                    )
                },
                {
                    "!x[2]",
                    new ASTUnaryOp(
                        new ASTArrSub(
                            new ASTIdentifier(new T_IDENTIFIER(1, "x")),
                            new ASTConstInt(new T_CONST_INT(1, "2", 10))),
                        ASTUnaryOp.Op.NOT
                    )
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.UnaryExpression(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserCastExpression() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "++x--",
                    new ASTPreStep(
                        new ASTPostStep(
                            new ASTIdentifier(new T_IDENTIFIER(1, "x")),
                            ASTPostStep.Type.DEC),
                        ASTPreStep.Type.INC
                    )
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.CastExpression(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserMultiplicativeExpression() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "a * b / c % d",
                    new ASTBinaryExpr(
                        new ASTBinaryExpr(
                            new ASTBinaryExpr(
                                new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                                new ASTIdentifier(new T_IDENTIFIER(1, "b")),
                                ASTBinaryExpr.Op.MULT),
                            new ASTIdentifier(new T_IDENTIFIER(1, "c")),
                            ASTBinaryExpr.Op.DIV),
                        new ASTIdentifier(new T_IDENTIFIER(1, "d")),
                        ASTBinaryExpr.Op.MOD)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.MultiplicativeExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserAdditiveOperator() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "a + b * 3",
                    new ASTBinaryExpr(
                        new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "b")),
                            new ASTConstInt(new T_CONST_INT(1, "3", 10)),
                            ASTBinaryExpr.Op.MULT),
                        ASTBinaryExpr.Op.PLUS)
                },
                {
                    "1 + 2 \n * c \n - 5",
                    new ASTBinaryExpr(
                        new ASTBinaryExpr(
                            new ASTConstInt(new T_CONST_INT(1, "1", 10)),
                            new ASTBinaryExpr(
                                new ASTConstInt(new T_CONST_INT(1, "2", 10)),
                                new ASTIdentifier(new T_IDENTIFIER(2, "c")),
                                ASTBinaryExpr.Op.MULT),
                            ASTBinaryExpr.Op.PLUS),
                        new ASTConstInt(new T_CONST_INT(3, "5", 10)),
                        ASTBinaryExpr.Op.MINUS)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.AdditiveExpressiion().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserShiftExpression() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "a << 1",
                    new ASTBinaryExpr(
                        new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                        new ASTConstInt(new T_CONST_INT(1, "1", 10)),
                        ASTBinaryExpr.Op.LEFT)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.ShiftExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserRelationalExpression() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "a << 1 >= 23",
                    new ASTBinaryExpr(
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                            new ASTConstInt(new T_CONST_INT(1, "1", 10)),
                            ASTBinaryExpr.Op.LEFT),
                        new ASTConstInt(new T_CONST_INT(1, "23", 10)),
                        ASTBinaryExpr.Op.GE)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.RelationalExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserEqualityExpression() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "a << 1 != 23",
                    new ASTBinaryExpr(
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                            new ASTConstInt(new T_CONST_INT(1, "1", 10)),
                            ASTBinaryExpr.Op.LEFT),
                        new ASTConstInt(new T_CONST_INT(1, "23", 10)),
                        ASTBinaryExpr.Op.NEQ)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.EqualityExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserANDExpression() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "a >> 1 & 0x1",
                    new ASTBinaryExpr(
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                            new ASTConstInt(new T_CONST_INT(1, "1", 10)),
                            ASTBinaryExpr.Op.RIGHT),
                        new ASTConstInt(new T_CONST_INT(1, "23", 16)),
                        ASTBinaryExpr.Op.AND)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.ANDExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserXORExpression() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "a >> 1 ^ 0x1",
                    new ASTBinaryExpr(
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                            new ASTConstInt(new T_CONST_INT(1, "1", 10)),
                            ASTBinaryExpr.Op.RIGHT),
                        new ASTConstInt(new T_CONST_INT(1, "23", 16)),
                        ASTBinaryExpr.Op.XOR)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.XORExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserORExpression() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "a >> 1 | 0x1",
                    new ASTBinaryExpr(
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                            new ASTConstInt(new T_CONST_INT(1, "1", 10)),
                            ASTBinaryExpr.Op.RIGHT),
                        new ASTConstInt(new T_CONST_INT(1, "23", 16)),
                        ASTBinaryExpr.Op.OR)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.ORExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserLogicalANDExpression() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "a >> 1 == 0x1 && b == 0",
                    new ASTBinaryExpr(
                        new ASTBinaryExpr(
                            new ASTBinaryExpr(
                                new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                                new ASTConstInt(new T_CONST_INT(1, "1", 10)),
                                ASTBinaryExpr.Op.RIGHT),
                            new ASTConstInt(new T_CONST_INT(1, "23", 16)),
                            ASTBinaryExpr.Op.EQ),
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "b")),
                            new ASTConstInt(new T_CONST_INT(1, "0", 8)),
                            ASTBinaryExpr.Op.EQ),
                        ASTBinaryExpr.Op.LOGAND)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.LogicalANDExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserLogicalORExpression() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "a >> 1 == 0x1 || b == 0",
                    new ASTBinaryExpr(
                        new ASTBinaryExpr(
                            new ASTBinaryExpr(
                                new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                                new ASTConstInt(new T_CONST_INT(1, "1", 10)),
                                ASTBinaryExpr.Op.RIGHT),
                            new ASTConstInt(new T_CONST_INT(1, "23", 16)),
                            ASTBinaryExpr.Op.EQ),
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "b")),
                            new ASTConstInt(new T_CONST_INT(1, "0", 8)),
                            ASTBinaryExpr.Op.EQ),
                        ASTBinaryExpr.Op.LOGOR)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.LogicalORExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserConditionalExpression() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "a == 0 ? c : d",
                    new ASTConditionalExpr(
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                            new ASTConstInt(new T_CONST_INT(1, "0", 10)),
                            ASTBinaryExpr.Op.EQ),
                        new ASTIdentifier(new T_IDENTIFIER(1, "c")),
                        new ASTIdentifier(new T_IDENTIFIER(1, "d")))
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.ConditionalExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserAssignmentExpression() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "a == 0 ? c : d",
                    new ASTConditionalExpr(
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                            new ASTConstInt(new T_CONST_INT(1, "0", 10)),
                            ASTBinaryExpr.Op.EQ),
                        new ASTIdentifier(new T_IDENTIFIER(1, "c")),
                        new ASTIdentifier(new T_IDENTIFIER(1, "d")))
                },
                {
                    "a += 6",
                    new ASTAssignExpr(
                        new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                        new ASTConstInt(new T_CONST_INT(1, "6", 10)),
                        ASTAssignExpr.Op.PLUSEQ)
                },
                {
                    "val = a = c",
                    new ASTAssignExpr(
                        new ASTIdentifier(new T_IDENTIFIER(1, "val")),
                        new ASTAssignExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                            new ASTIdentifier(new T_IDENTIFIER(1, "c")),
                            ASTAssignExpr.Op.ASSIGN),
                        ASTAssignExpr.Op.ASSIGN)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.AssignmentExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserCommaExpressionEquals() {
            var e11 = new ASTIdentifier(new T_IDENTIFIER(1, "a"));
            var e21 = new ASTIdentifier(new T_IDENTIFIER(1, "a"));
            var e12 = new ASTBinaryExpr(
                    new ASTIdentifier(new T_IDENTIFIER(2, "b")),
                    new ASTConstInt(new T_CONST_INT(2, "4", 10)),
                    ASTBinaryExpr.Op.PLUS);
            var e22 = new ASTBinaryExpr(
                    new ASTIdentifier(new T_IDENTIFIER(2, "b")),
                    new ASTConstInt(new T_CONST_INT(2, "4", 10)),
                    ASTBinaryExpr.Op.PLUS);
            var l1 = new LinkedList<ASTExpr>();
            var l2 = new LinkedList<ASTExpr>();
            var l3 = new LinkedList<ASTExpr>();

            l1.AddFirst(e11);
            l1.AddFirst(e12);

            l2.AddFirst(e22);
            l2.AddFirst(e21);

            l3.AddFirst(e21);
            l3.AddFirst(e22);

            var comma1 = new ASTCommaExpr(l1);
            var comma2 = new ASTCommaExpr(l2);
            var comma3 = new ASTCommaExpr(l3);

            Assert.IsFalse(comma1.Equals(comma2));
            Assert.IsTrue(comma1.Equals(comma3));
        }

        [TestMethod]
        public void LCCParserExpression() {

            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "a = 1, b = 2, c = 3",
                    new ASTCommaExpr(new LinkedList<ASTExpr>(new List<ASTExpr> {
                        new ASTAssignExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                            new ASTConstInt(new T_CONST_INT(1, "1", 10)),
                            ASTAssignExpr.Op.ASSIGN),
                        new ASTAssignExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "b")),
                            new ASTConstInt(new T_CONST_INT(1, "2", 10)),
                            ASTAssignExpr.Op.ASSIGN),
                        new ASTAssignExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "c")),
                            new ASTConstInt(new T_CONST_INT(1, "3", 10)),
                            ASTAssignExpr.Op.ASSIGN),
                    }))
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.Expression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserConstantExpression() {
            Dictionary<string, ASTExpr> dict = new Dictionary<string, ASTExpr> {
                {
                    "a == 0 ? c : d",
                    new ASTConditionalExpr(
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "a")),
                            new ASTConstInt(new T_CONST_INT(1, "0", 10)),
                            ASTBinaryExpr.Op.EQ),
                        new ASTIdentifier(new T_IDENTIFIER(1, "c")),
                        new ASTIdentifier(new T_IDENTIFIER(1, "d")))
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.ConstantExpression().End(), test.Value);
            }
        }
    }
}
