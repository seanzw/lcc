using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.SyntaxTree;
using lcc.Token;
using lcc.Parser;
using Parserc;

namespace LC_CompilerTests {
    public partial class ParserTests {

        [TestMethod]
        public void LCCParserLabeledStatement() {
            var tests = new Dictionary<string, Labeled> {
                {
                    "FOO: x = x + 2;",
                    new Labeled(
                        new Id(new T_IDENTIFIER(1, "FOO")),
                        new Assign(
                            new Id(new T_IDENTIFIER(1, "x")),
                            new BiExpr(
                                new Id(new T_IDENTIFIER(1, "x")),
                                new ConstInt(new T_CONST_INT(1, "2", 10)),
                                BiExpr.Op.PLUS),
                            Assign.Op.ASSIGN))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.LabeledStatement().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserCaseStatement() {
            var tests = new Dictionary<string, Case> {
                {
                    "case 1 : a;",
                    new Case(
                        new ConstInt(new T_CONST_INT(1, "1", 10)),
                        new Id(new T_IDENTIFIER(1, "a")))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.CaseStatement().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDefaultStatement() {
            var tests = new Dictionary<string, Default> {
                {
                    "default: a;",
                    new Default(new Id(new T_IDENTIFIER(1, "a")))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.DefaultStatement().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserCompoundStatement() {
            var tests = new Dictionary<string, CompoundStmt> {
                {
                    @"
{
    a;
    int x;
}",
                    new CompoundStmt(
                        new List<Stmt> {
                            new Id(new T_IDENTIFIER(3, "a")),
                            new Declaration(
                                new DeclSpecs(
                                    new List<DeclSpec> {
                                        new TypeKeySpec(4, TypeSpec.Kind.INT)
                                    },
                                    StoreSpec.Kind.NONE,
                                    new List<TypeSpec.Kind> {
                                        TypeSpec.Kind.INT
                                    }),
                                new List<InitDeclarator> {
                                    new InitDeclarator(
                                        new Declarator(
                                            new List<Ptr>(),
                                            new IdDeclarator(new Id(new T_IDENTIFIER(4, "x")))))
                                }),
                        })
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.CompoundStatement(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserExpressionStatement() {
            var tests = new Dictionary<string, Stmt> {
                {
                    ";",
                    new VoidStmt(1)
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.ExpressionStatement(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserIfStatement() {
            var tests = new Dictionary<string, If> {
                {
                    @"
if (i < length)
    x++;",
                    new If(
                        2,
                        new BiExpr(
                            new Id(new T_IDENTIFIER(2, "i")),
                            new Id(new T_IDENTIFIER(2, "length")),
                            BiExpr.Op.LT),
                        new PostStep(
                            new Id(new T_IDENTIFIER(3, "x")),
                            PostStep.Op.INC),
                        null)
                },
                {
                    @"
if (i < length)
    x++;
else
    x--;",
                    new If(
                        2,
                        new BiExpr(
                            new Id(new T_IDENTIFIER(2, "i")),
                            new Id(new T_IDENTIFIER(2, "length")),
                            BiExpr.Op.LT),
                        new PostStep(
                            new Id(new T_IDENTIFIER(3, "x")),
                            PostStep.Op.INC),
                        new PostStep(
                            new Id(new T_IDENTIFIER(5, "x")),
                            PostStep.Op.DEC))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.IfStatement(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserSwitchStatement() {
            var tests = new Dictionary<string, Switch> {
                {
                    @"
switch (x) {
case 0: a;
default: c;
}",
                    new Switch(
                        2,
                        new Id(new T_IDENTIFIER(2, "x")),
                        new CompoundStmt(new LinkedList<Stmt>(new List<Stmt> {
                            new Case(
                                new ConstInt(new T_CONST_INT(3, "0", 8)),
                                new Id(new T_IDENTIFIER(3, "a"))),
                            new Default(
                                new Id(new T_IDENTIFIER(4, "c"))) })))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.SwitchStatement().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserWhileStatement() {
            var tests = new Dictionary<string, While> {
                {
                    @"
while (x < length) {
    x++;
}",
                    new While(
                        new BiExpr(
                            new Id(new T_IDENTIFIER(2, "x")),
                            new Id(new T_IDENTIFIER(2, "length")),
                            BiExpr.Op.LT),
                        new CompoundStmt(
                            new List<Stmt> {
                                new PostStep(
                                    new Id(new T_IDENTIFIER(3, "x")),
                                    PostStep.Op.INC)
                            }))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.WhileStatement(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDoStatement() {
            var tests = new Dictionary<string, Do> {
                {
                    @"
do {
    x++;
} while (x < length);",
                    new Do(
                        new BiExpr(
                            new Id(new T_IDENTIFIER(4, "x")),
                            new Id(new T_IDENTIFIER(4, "length")),
                            BiExpr.Op.LT),
                        new CompoundStmt(
                            new List<Stmt> {
                                new PostStep(
                                    new Id(new T_IDENTIFIER(3, "x")),
                                    PostStep.Op.INC)
                            }))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.DoStatement(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserForStatement() {
            var tests = new Dictionary<string, For> {
                {
                    @"
for (i = 0; i < length; ++i) {
    x += i;
}",
                    new For(
                        2,
                        new Assign(
                            new Id(new T_IDENTIFIER(2, "i")),
                            new ConstInt(new T_CONST_INT(2, "0", 8)),
                            Assign.Op.ASSIGN),
                        new BiExpr(
                            new Id(new T_IDENTIFIER(2, "i")),
                            new Id(new T_IDENTIFIER(2, "length")),
                            BiExpr.Op.LT),
                        new PreStep(
                            new Id(new T_IDENTIFIER(2, "i")),
                            PreStep.Op.INC),
                        new CompoundStmt(
                            new List<Stmt> {
                                new Assign(
                                    new Id(new T_IDENTIFIER(3, "x")),
                                    new Id(new T_IDENTIFIER(3, "i")),
                                    Assign.Op.PLUSEQ)
                            }))
                },
                {
                    @"
for (;1;) ;",
                    new For(
                        2,
                        null as Expr,
                        new ConstInt(new T_CONST_INT(2, "1", 10)),
                        null,
                        new VoidStmt(2))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.ForStatementExpr(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserJumpStatement() {
            var tests = new Dictionary<string, Stmt> {
                {
                    "continue;",
                    new Continue(1)
                },
                {
                    "break;",
                    new Break(1)
                },
                {
                    "goto foo;",
                    new GoTo(1, "foo")
                },
                {
                    "return 0;",
                    new Return(1, new ConstInt(new T_CONST_INT(1, "0", 8)))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.JumpStatement(), test.Value);
            }
        }
    }
}
