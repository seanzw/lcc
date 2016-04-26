using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.SyntaxTree;
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
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(truth, result.First().Value);
            Assert.IsFalse(result.First().Remain.More());
        }

        private static void Aux<R>(
            string src,
            Parser<Token, IEnumerable<R>> parser,
            IEnumerable<R> truth
            ) {
            var result = Utility.parse(src, parser);

            // Check the first result.
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(truth.SequenceEqual(result.First().Value));
            Assert.IsFalse(result.First().Remain.More());
        }

        [TestMethod]
        public void LCCParserPrimaryExpression() {
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "tmp",
                    new Id(new T_IDENTIFIER(1, "tmp"))
                },
                {
                    "12356u",
                    new ConstInt(new T_CONST_INT(1, "12356u", 10))
                },
                {
                    "1.264f",
                    new ConstFloat(new T_CONST_FLOAT(1, "1.264f", 10))
                },
                {
                    "'C'",
                    new ConstChar(new T_CONST_CHAR(1, "'C'"))
                },
                {
                    "\"what is this?\"",
                    new Str(new LinkedList<T_STRING_LITERAL>(new List<T_STRING_LITERAL> {
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
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "abc[123]",
                    new STArrSub(
                        new Id(new T_IDENTIFIER(1, "abc")),
                        new ConstInt(new T_CONST_INT(1, "123", 10))
                    )
                },
                {
                    "abc.x",
                    new STAccess(
                        new Id(new T_IDENTIFIER(1, "abc")),
                        new T_IDENTIFIER(1, "x"),
                        STAccess.Kind.DOT
                    )
                },
                {
                    "arr[2].value",
                    new STAccess(
                        new STArrSub(
                            new Id(new T_IDENTIFIER(1, "arr")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))
                        ),
                        new T_IDENTIFIER(1, "value"),
                        STAccess.Kind.DOT
                    )
                },
                {
                    "abc->x",
                    new STAccess(
                        new Id(new T_IDENTIFIER(1, "abc")),
                        new T_IDENTIFIER(1, "x"),
                        STAccess.Kind.PTR
                    )
                },
                {
                    "x++",
                    new STPostStep(
                        new Id(new T_IDENTIFIER(1, "x")),
                        STPostStep.Kind.INC
                    )
                },
                {
                    "x--",
                    new STPostStep(
                        new Id(new T_IDENTIFIER(1, "x")),
                        STPostStep.Kind.DEC
                    )
                },
                {
                    "printf(x)",
                    new STFuncCall(
                        new Id(new T_IDENTIFIER(1, "printf")),
                        new List<Expr> {
                            new Id(new T_IDENTIFIER(1, "x"))
                        }
                    )
                },
                {
                    "printf()",
                    new STFuncCall(
                        new Id(new T_IDENTIFIER(1, "printf")),
                        new List<Expr> {
                        }
                    )
                },
                {
                    "(int) { 1 }",
                    new STCompound(
                        new TypeName(
                            ProcessSS(new List<TypeSpecQual> { new STTypeKeySpec(1, TypeSpec.Kind.INT) })
                        ),
                        new List<STInitItem> {
                            new STInitItem(new Initializer(new ConstInt(new T_CONST_INT(1, "1", 10))))
                        })
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.PostfixExpression(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserUnaryExpression() {

            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "++x--",
                    new STPreStep(
                        new STPostStep(
                            new Id(new T_IDENTIFIER(1, "x")),
                            STPostStep.Kind.DEC),
                        STPreStep.Kind.INC
                    )
                },
                {
                    "--x[2]",
                    new STPreStep(
                        new STArrSub(
                            new Id(new T_IDENTIFIER(1, "x")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))),
                        STPreStep.Kind.DEC
                    )
                },
                {
                    "&x[2]",
                    new STUnaryOp(
                        new STArrSub(
                            new Id(new T_IDENTIFIER(1, "x")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))),
                        STUnaryOp.Op.REF
                    )
                },
                {
                    "*x[2]",
                    new STUnaryOp(
                        new STArrSub(
                            new Id(new T_IDENTIFIER(1, "x")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))),
                        STUnaryOp.Op.STAR
                    )
                },
                {
                    "+x[2]",
                    new STUnaryOp(
                        new STArrSub(
                            new Id(new T_IDENTIFIER(1, "x")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))),
                        STUnaryOp.Op.PLUS
                    )
                },
                {
                    "-x[2]",
                    new STUnaryOp(
                        new STArrSub(
                            new Id(new T_IDENTIFIER(1, "x")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))),
                        STUnaryOp.Op.MINUS
                    )
                },
                {
                    "~x[2]",
                    new STUnaryOp(
                        new STArrSub(
                            new Id(new T_IDENTIFIER(1, "x")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))),
                        STUnaryOp.Op.REVERSE
                    )
                },
                {
                    "!x[2]",
                    new STUnaryOp(
                        new STArrSub(
                            new Id(new T_IDENTIFIER(1, "x")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))),
                        STUnaryOp.Op.NOT
                    )
                },
                {
                    "sizeof a",
                    new STSizeOf(new Id(new T_IDENTIFIER(1, "a")))
                },
                {
                    "sizeof (int)",
                    new STSizeOf(
                        new TypeName(
                            ProcessSS(new List<TypeSpecQual> { new STTypeKeySpec(1, TypeSpec.Kind.INT) })
                        ))
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.UnaryExpression(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserCastExpression() {
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "++x--",
                    new STPreStep(
                        new STPostStep(
                            new Id(new T_IDENTIFIER(1, "x")),
                            STPostStep.Kind.DEC),
                        STPreStep.Kind.INC
                    )
                },
                {
                    "(int)what",
                    new STCast(
                        new TypeName(
                            ProcessSS(new List<TypeSpecQual> { new STTypeKeySpec(1, TypeSpec.Kind.INT) })
                        ),
                        new Id(new T_IDENTIFIER(1, "what")))
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.CastExpression(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserMultiplicativeExpression() {
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "a * b / c % d",
                    new STBiExpr(
                        new STBiExpr(
                            new STBiExpr(
                                new Id(new T_IDENTIFIER(1, "a")),
                                new Id(new T_IDENTIFIER(1, "b")),
                                STBiExpr.Op.MULT),
                            new Id(new T_IDENTIFIER(1, "c")),
                            STBiExpr.Op.DIV),
                        new Id(new T_IDENTIFIER(1, "d")),
                        STBiExpr.Op.MOD)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.MultiplicativeExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserAdditiveOperator() {
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "a + b * 3",
                    new STBiExpr(
                        new Id(new T_IDENTIFIER(1, "a")),
                        new STBiExpr(
                            new Id(new T_IDENTIFIER(1, "b")),
                            new ConstInt(new T_CONST_INT(1, "3", 10)),
                            STBiExpr.Op.MULT),
                        STBiExpr.Op.PLUS)
                },
                {
                    "1 + 2 \n * c \n - 5",
                    new STBiExpr(
                        new STBiExpr(
                            new ConstInt(new T_CONST_INT(1, "1", 10)),
                            new STBiExpr(
                                new ConstInt(new T_CONST_INT(1, "2", 10)),
                                new Id(new T_IDENTIFIER(2, "c")),
                                STBiExpr.Op.MULT),
                            STBiExpr.Op.PLUS),
                        new ConstInt(new T_CONST_INT(3, "5", 10)),
                        STBiExpr.Op.MINUS)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.AdditiveExpressiion().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserShiftExpression() {
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "a << 1",
                    new STBiExpr(
                        new Id(new T_IDENTIFIER(1, "a")),
                        new ConstInt(new T_CONST_INT(1, "1", 10)),
                        STBiExpr.Op.LEFT)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.ShiftExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserRelationalExpression() {
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "a << 1 >= 23",
                    new STBiExpr(
                        new STBiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "1", 10)),
                            STBiExpr.Op.LEFT),
                        new ConstInt(new T_CONST_INT(1, "23", 10)),
                        STBiExpr.Op.GE)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.RelationalExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserEqualityExpression() {
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "a << 1 != 23",
                    new STBiExpr(
                        new STBiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "1", 10)),
                            STBiExpr.Op.LEFT),
                        new ConstInt(new T_CONST_INT(1, "23", 10)),
                        STBiExpr.Op.NEQ)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.EqualityExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserANDExpression() {
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "a >> 1 & 0x1",
                    new STBiExpr(
                        new STBiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "1", 10)),
                            STBiExpr.Op.RIGHT),
                        new ConstInt(new T_CONST_INT(1, "1", 16)),
                        STBiExpr.Op.AND)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.ANDExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserXORExpression() {
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "a >> 1 ^ 0x1",
                    new STBiExpr(
                        new STBiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "1", 10)),
                            STBiExpr.Op.RIGHT),
                        new ConstInt(new T_CONST_INT(1, "1", 16)),
                        STBiExpr.Op.XOR)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.XORExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserORExpression() {
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "a >> 1 | 0x1",
                    new STBiExpr(
                        new STBiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "1", 10)),
                            STBiExpr.Op.RIGHT),
                        new ConstInt(new T_CONST_INT(1, "1", 16)),
                        STBiExpr.Op.OR)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.ORExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserLogicalANDExpression() {
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "a >> 1 == 0x1 && b == 0",
                    new STBiExpr(
                        new STBiExpr(
                            new STBiExpr(
                                new Id(new T_IDENTIFIER(1, "a")),
                                new ConstInt(new T_CONST_INT(1, "1", 10)),
                                STBiExpr.Op.RIGHT),
                            new ConstInt(new T_CONST_INT(1, "1", 16)),
                            STBiExpr.Op.EQ),
                        new STBiExpr(
                            new Id(new T_IDENTIFIER(1, "b")),
                            new ConstInt(new T_CONST_INT(1, "0", 8)),
                            STBiExpr.Op.EQ),
                        STBiExpr.Op.LOGAND)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.LogicalANDExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserLogicalORExpression() {
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "a >> 1 == 0x1 || b == 0",
                    new STBiExpr(
                        new STBiExpr(
                            new STBiExpr(
                                new Id(new T_IDENTIFIER(1, "a")),
                                new ConstInt(new T_CONST_INT(1, "1", 10)),
                                STBiExpr.Op.RIGHT),
                            new ConstInt(new T_CONST_INT(1, "1", 16)),
                            STBiExpr.Op.EQ),
                        new STBiExpr(
                            new Id(new T_IDENTIFIER(1, "b")),
                            new ConstInt(new T_CONST_INT(1, "0", 8)),
                            STBiExpr.Op.EQ),
                        STBiExpr.Op.LOGOR)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.LogicalORExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserConditionalExpression() {
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "a == 0 ? c : d",
                    new STCondExpr(
                        new STBiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "0", 10)),
                            STBiExpr.Op.EQ),
                        new Id(new T_IDENTIFIER(1, "c")),
                        new Id(new T_IDENTIFIER(1, "d")))
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.ConditionalExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserAssignmentExpression() {
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "a == 0 ? c : d",
                    new STCondExpr(
                        new STBiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "0", 10)),
                            STBiExpr.Op.EQ),
                        new Id(new T_IDENTIFIER(1, "c")),
                        new Id(new T_IDENTIFIER(1, "d")))
                },
                {
                    "a += 6",
                    new STAssignExpr(
                        new Id(new T_IDENTIFIER(1, "a")),
                        new ConstInt(new T_CONST_INT(1, "6", 10)),
                        STAssignExpr.Op.PLUSEQ)
                },
                {
                    "val = a = c",
                    new STAssignExpr(
                        new Id(new T_IDENTIFIER(1, "val")),
                        new STAssignExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new Id(new T_IDENTIFIER(1, "c")),
                            STAssignExpr.Op.ASSIGN),
                        STAssignExpr.Op.ASSIGN)
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.AssignmentExpression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserCommaExpressionEquals() {
            var e11 = new Id(new T_IDENTIFIER(1, "a"));
            var e21 = new Id(new T_IDENTIFIER(1, "a"));
            var e12 = new STBiExpr(
                    new Id(new T_IDENTIFIER(2, "b")),
                    new ConstInt(new T_CONST_INT(2, "4", 10)),
                    STBiExpr.Op.PLUS);
            var e22 = new STBiExpr(
                    new Id(new T_IDENTIFIER(2, "b")),
                    new ConstInt(new T_CONST_INT(2, "4", 10)),
                    STBiExpr.Op.PLUS);
            var l1 = new LinkedList<Expr>();
            var l2 = new LinkedList<Expr>();
            var l3 = new LinkedList<Expr>();

            l1.AddFirst(e11);
            l1.AddFirst(e12);

            l2.AddFirst(e22);
            l2.AddFirst(e21);

            l3.AddFirst(e21);
            l3.AddFirst(e22);

            var comma1 = new STCommaExpr(l1);
            var comma2 = new STCommaExpr(l2);
            var comma3 = new STCommaExpr(l3);

            Assert.IsFalse(comma1.Equals(comma2));
            Assert.IsTrue(comma1.Equals(comma3));
        }

        [TestMethod]
        public void LCCParserExpression() {

            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "a = 1, b = 2, c = 3",
                    new STCommaExpr(new LinkedList<Expr>(new List<Expr> {
                        new STAssignExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "1", 10)),
                            STAssignExpr.Op.ASSIGN),
                        new STAssignExpr(
                            new Id(new T_IDENTIFIER(1, "b")),
                            new ConstInt(new T_CONST_INT(1, "2", 10)),
                            STAssignExpr.Op.ASSIGN),
                        new STAssignExpr(
                            new Id(new T_IDENTIFIER(1, "c")),
                            new ConstInt(new T_CONST_INT(1, "3", 10)),
                            STAssignExpr.Op.ASSIGN),
                    }))
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.Expression().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserConstantExpression() {
            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "a == 0 ? c : d",
                    new STCondExpr(
                        new STBiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "0", 10)),
                            STBiExpr.Op.EQ),
                        new Id(new T_IDENTIFIER(1, "c")),
                        new Id(new T_IDENTIFIER(1, "d")))
                }
            };
            foreach (var test in dict) {
                Aux(test.Key, Parser.ConstantExpression().End(), test.Value);
            }
        }
    }
}
