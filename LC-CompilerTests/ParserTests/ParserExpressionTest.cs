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
            R truth,
            bool clear = true
            ) {
            // Clear the parser environment.
            if (clear) {
                lcc.Parser.Env.PopScope();
                lcc.Parser.Env.PushScope();
            }
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
                    new ArrSub(
                        new Id(new T_IDENTIFIER(1, "abc")),
                        new ConstInt(new T_CONST_INT(1, "123", 10))
                    )
                },
                {
                    "abc.x",
                    new Access(
                        new Id(new T_IDENTIFIER(1, "abc")),
                        new T_IDENTIFIER(1, "x"),
                        Access.Kind.DOT
                    )
                },
                {
                    "arr[2].value",
                    new Access(
                        new ArrSub(
                            new Id(new T_IDENTIFIER(1, "arr")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))
                        ),
                        new T_IDENTIFIER(1, "value"),
                        Access.Kind.DOT
                    )
                },
                {
                    "abc->x",
                    new Access(
                        new Id(new T_IDENTIFIER(1, "abc")),
                        new T_IDENTIFIER(1, "x"),
                        Access.Kind.PTR
                    )
                },
                {
                    "x++",
                    new PostStep(
                        new Id(new T_IDENTIFIER(1, "x")),
                        PostStep.Kind.INC
                    )
                },
                {
                    "x--",
                    new PostStep(
                        new Id(new T_IDENTIFIER(1, "x")),
                        PostStep.Kind.DEC
                    )
                },
                {
                    "printf(x)",
                    new FuncCall(
                        new Id(new T_IDENTIFIER(1, "printf")),
                        new List<Expr> {
                            new Id(new T_IDENTIFIER(1, "x"))
                        }
                    )
                },
                {
                    "printf()",
                    new FuncCall(
                        new Id(new T_IDENTIFIER(1, "printf")),
                        new List<Expr> {
                        }
                    )
                },
                {
                    "(int) { 1 }",
                    new STCompound(
                        new TypeName(
                            ProcessSS(new List<TypeSpecQual> { new TypeKeySpec(1, TypeSpec.Kind.INT) })
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
                    new PreStep(
                        new PostStep(
                            new Id(new T_IDENTIFIER(1, "x")),
                            PostStep.Kind.DEC),
                        PreStep.Kind.INC
                    )
                },
                {
                    "--x[2]",
                    new PreStep(
                        new ArrSub(
                            new Id(new T_IDENTIFIER(1, "x")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))),
                        PreStep.Kind.DEC
                    )
                },
                {
                    "&x[2]",
                    new UnaryOp(
                        new ArrSub(
                            new Id(new T_IDENTIFIER(1, "x")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))),
                        UnaryOp.Op.REF
                    )
                },
                {
                    "*x[2]",
                    new UnaryOp(
                        new ArrSub(
                            new Id(new T_IDENTIFIER(1, "x")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))),
                        UnaryOp.Op.STAR
                    )
                },
                {
                    "+x[2]",
                    new UnaryOp(
                        new ArrSub(
                            new Id(new T_IDENTIFIER(1, "x")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))),
                        UnaryOp.Op.PLUS
                    )
                },
                {
                    "-x[2]",
                    new UnaryOp(
                        new ArrSub(
                            new Id(new T_IDENTIFIER(1, "x")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))),
                        UnaryOp.Op.MINUS
                    )
                },
                {
                    "~x[2]",
                    new UnaryOp(
                        new ArrSub(
                            new Id(new T_IDENTIFIER(1, "x")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))),
                        UnaryOp.Op.REVERSE
                    )
                },
                {
                    "!x[2]",
                    new UnaryOp(
                        new ArrSub(
                            new Id(new T_IDENTIFIER(1, "x")),
                            new ConstInt(new T_CONST_INT(1, "2", 10))),
                        UnaryOp.Op.NOT
                    )
                },
                {
                    "sizeof a",
                    new SizeOf(new Id(new T_IDENTIFIER(1, "a")))
                },
                {
                    "sizeof (int)",
                    new SizeOf(
                        new TypeName(
                            ProcessSS(new List<TypeSpecQual> { new TypeKeySpec(1, TypeSpec.Kind.INT) })
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
                    new PreStep(
                        new PostStep(
                            new Id(new T_IDENTIFIER(1, "x")),
                            PostStep.Kind.DEC),
                        PreStep.Kind.INC
                    )
                },
                {
                    "(int)what",
                    new Cast(
                        new TypeName(
                            ProcessSS(new List<TypeSpecQual> { new TypeKeySpec(1, TypeSpec.Kind.INT) })
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
                    new BiExpr(
                        new BiExpr(
                            new BiExpr(
                                new Id(new T_IDENTIFIER(1, "a")),
                                new Id(new T_IDENTIFIER(1, "b")),
                                BiExpr.Op.MULT),
                            new Id(new T_IDENTIFIER(1, "c")),
                            BiExpr.Op.DIV),
                        new Id(new T_IDENTIFIER(1, "d")),
                        BiExpr.Op.MOD)
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
                    new BiExpr(
                        new Id(new T_IDENTIFIER(1, "a")),
                        new BiExpr(
                            new Id(new T_IDENTIFIER(1, "b")),
                            new ConstInt(new T_CONST_INT(1, "3", 10)),
                            BiExpr.Op.MULT),
                        BiExpr.Op.PLUS)
                },
                {
                    "1 + 2 \n * c \n - 5",
                    new BiExpr(
                        new BiExpr(
                            new ConstInt(new T_CONST_INT(1, "1", 10)),
                            new BiExpr(
                                new ConstInt(new T_CONST_INT(1, "2", 10)),
                                new Id(new T_IDENTIFIER(2, "c")),
                                BiExpr.Op.MULT),
                            BiExpr.Op.PLUS),
                        new ConstInt(new T_CONST_INT(3, "5", 10)),
                        BiExpr.Op.MINUS)
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
                    new BiExpr(
                        new Id(new T_IDENTIFIER(1, "a")),
                        new ConstInt(new T_CONST_INT(1, "1", 10)),
                        BiExpr.Op.LEFT)
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
                    new BiExpr(
                        new BiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "1", 10)),
                            BiExpr.Op.LEFT),
                        new ConstInt(new T_CONST_INT(1, "23", 10)),
                        BiExpr.Op.GE)
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
                    new BiExpr(
                        new BiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "1", 10)),
                            BiExpr.Op.LEFT),
                        new ConstInt(new T_CONST_INT(1, "23", 10)),
                        BiExpr.Op.NEQ)
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
                    new BiExpr(
                        new BiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "1", 10)),
                            BiExpr.Op.RIGHT),
                        new ConstInt(new T_CONST_INT(1, "1", 16)),
                        BiExpr.Op.AND)
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
                    new BiExpr(
                        new BiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "1", 10)),
                            BiExpr.Op.RIGHT),
                        new ConstInt(new T_CONST_INT(1, "1", 16)),
                        BiExpr.Op.XOR)
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
                    new BiExpr(
                        new BiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "1", 10)),
                            BiExpr.Op.RIGHT),
                        new ConstInt(new T_CONST_INT(1, "1", 16)),
                        BiExpr.Op.OR)
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
                    new BiExpr(
                        new BiExpr(
                            new BiExpr(
                                new Id(new T_IDENTIFIER(1, "a")),
                                new ConstInt(new T_CONST_INT(1, "1", 10)),
                                BiExpr.Op.RIGHT),
                            new ConstInt(new T_CONST_INT(1, "1", 16)),
                            BiExpr.Op.EQ),
                        new BiExpr(
                            new Id(new T_IDENTIFIER(1, "b")),
                            new ConstInt(new T_CONST_INT(1, "0", 8)),
                            BiExpr.Op.EQ),
                        BiExpr.Op.LOGAND)
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
                    new BiExpr(
                        new BiExpr(
                            new BiExpr(
                                new Id(new T_IDENTIFIER(1, "a")),
                                new ConstInt(new T_CONST_INT(1, "1", 10)),
                                BiExpr.Op.RIGHT),
                            new ConstInt(new T_CONST_INT(1, "1", 16)),
                            BiExpr.Op.EQ),
                        new BiExpr(
                            new Id(new T_IDENTIFIER(1, "b")),
                            new ConstInt(new T_CONST_INT(1, "0", 8)),
                            BiExpr.Op.EQ),
                        BiExpr.Op.LOGOR)
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
                    new CondExpr(
                        new BiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "0", 10)),
                            BiExpr.Op.EQ),
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
                    new CondExpr(
                        new BiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "0", 10)),
                            BiExpr.Op.EQ),
                        new Id(new T_IDENTIFIER(1, "c")),
                        new Id(new T_IDENTIFIER(1, "d")))
                },
                {
                    "a += 6",
                    new Assign(
                        new Id(new T_IDENTIFIER(1, "a")),
                        new ConstInt(new T_CONST_INT(1, "6", 10)),
                        Assign.Op.PLUSEQ)
                },
                {
                    "val = a = c",
                    new Assign(
                        new Id(new T_IDENTIFIER(1, "val")),
                        new Assign(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new Id(new T_IDENTIFIER(1, "c")),
                            Assign.Op.ASSIGN),
                        Assign.Op.ASSIGN)
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
            var e12 = new BiExpr(
                    new Id(new T_IDENTIFIER(2, "b")),
                    new ConstInt(new T_CONST_INT(2, "4", 10)),
                    BiExpr.Op.PLUS);
            var e22 = new BiExpr(
                    new Id(new T_IDENTIFIER(2, "b")),
                    new ConstInt(new T_CONST_INT(2, "4", 10)),
                    BiExpr.Op.PLUS);
            var l1 = new LinkedList<Expr>();
            var l2 = new LinkedList<Expr>();
            var l3 = new LinkedList<Expr>();

            l1.AddFirst(e11);
            l1.AddFirst(e12);

            l2.AddFirst(e22);
            l2.AddFirst(e21);

            l3.AddFirst(e21);
            l3.AddFirst(e22);

            var comma1 = new CommaExpr(l1);
            var comma2 = new CommaExpr(l2);
            var comma3 = new CommaExpr(l3);

            Assert.IsFalse(comma1.Equals(comma2));
            Assert.IsTrue(comma1.Equals(comma3));
        }

        [TestMethod]
        public void LCCParserExpression() {

            Dictionary<string, Expr> dict = new Dictionary<string, Expr> {
                {
                    "a = 1, b = 2, c = 3",
                    new CommaExpr(new LinkedList<Expr>(new List<Expr> {
                        new Assign(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "1", 10)),
                            Assign.Op.ASSIGN),
                        new Assign(
                            new Id(new T_IDENTIFIER(1, "b")),
                            new ConstInt(new T_CONST_INT(1, "2", 10)),
                            Assign.Op.ASSIGN),
                        new Assign(
                            new Id(new T_IDENTIFIER(1, "c")),
                            new ConstInt(new T_CONST_INT(1, "3", 10)),
                            Assign.Op.ASSIGN),
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
                    new CondExpr(
                        new BiExpr(
                            new Id(new T_IDENTIFIER(1, "a")),
                            new ConstInt(new T_CONST_INT(1, "0", 10)),
                            BiExpr.Op.EQ),
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
