using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.AST;
using lcc.Token;
using lcc.Parser;
using Parserc;

namespace LC_CompilerTests {
    public partial class ParserTests {

        [TestMethod]
        public void LCCParserLabeledStatement() {
            var tests = new Dictionary<string, ASTLabeled> {
                {
                    "FOO: x = x + 2;",
                    new ASTLabeled(
                        new ASTId(new T_IDENTIFIER(1, "FOO")),
                        new ASTAssignExpr(
                            new ASTId(new T_IDENTIFIER(1, "x")),
                            new ASTBinaryExpr(
                                new ASTId(new T_IDENTIFIER(1, "x")),
                                new ASTConstInt(new T_CONST_INT(1, "2", 10)),
                                ASTBinaryExpr.Op.PLUS),
                            ASTAssignExpr.Op.ASSIGN))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.LabeledStatement().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserCaseStatement() {
            var tests = new Dictionary<string, ASTCase> {
                {
                    "case 1 : a;",
                    new ASTCase(
                        new ASTConstInt(new T_CONST_INT(1, "1", 10)),
                        new ASTId(new T_IDENTIFIER(1, "a")))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.CaseStatement().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDefaultStatement() {
            var tests = new Dictionary<string, ASTDefault> {
                {
                    "default: a;",
                    new ASTDefault(new ASTId(new T_IDENTIFIER(1, "a")))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.DefaultStatement().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserCompoundStatement() {
            var tests = new Dictionary<string, ASTCompoundStmt> {
                {
                    @"
{
    a;
    int x;
}",
                    new ASTCompoundStmt(
                        new List<ASTStmt> {
                            new ASTId(new T_IDENTIFIER(3, "a")),
                            new ASTDeclaration(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(4, ASTTypeSpec.Kind.INT)
                                    },
                                    new List<ASTTypeSpec.Kind> {
                                        ASTTypeSpec.Kind.INT
                                    }),
                                new List<ASTInitDecl> {
                                    new ASTInitDecl(
                                        new ASTDecl(
                                            new List<ASTPtr>(),
                                            new ASTIdDecl(new ASTId(new T_IDENTIFIER(4, "x")))))
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
            var tests = new Dictionary<string, ASTStmt> {
                {
                    ";",
                    new ASTVoidStmt(1)
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.ExpressionStatement(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserIfStatement() {
            var tests = new Dictionary<string, ASTIfStmt> {
                {
                    @"
if (i < length)
    x++;",
                    new ASTIfStmt(
                        2,
                        new ASTBinaryExpr(
                            new ASTId(new T_IDENTIFIER(2, "i")),
                            new ASTId(new T_IDENTIFIER(2, "length")),
                            ASTBinaryExpr.Op.LT),
                        new ASTPostStep(
                            new ASTId(new T_IDENTIFIER(3, "x")),
                            ASTPostStep.Kind.INC),
                        null)
                },
                {
                    @"
if (i < length)
    x++;
else
    x--;",
                    new ASTIfStmt(
                        2,
                        new ASTBinaryExpr(
                            new ASTId(new T_IDENTIFIER(2, "i")),
                            new ASTId(new T_IDENTIFIER(2, "length")),
                            ASTBinaryExpr.Op.LT),
                        new ASTPostStep(
                            new ASTId(new T_IDENTIFIER(3, "x")),
                            ASTPostStep.Kind.INC),
                        new ASTPostStep(
                            new ASTId(new T_IDENTIFIER(5, "x")),
                            ASTPostStep.Kind.DEC))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.IfStatement(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserSwitchStatement() {
            var tests = new Dictionary<string, ASTSwitch> {
                {
                    @"
switch (x) {
case 0: a;
default: c;
}",
                    new ASTSwitch(
                        2,
                        new ASTId(new T_IDENTIFIER(2, "x")),
                        new ASTCompoundStmt(new LinkedList<ASTStmt>(new List<ASTStmt> {
                            new ASTCase(
                                new ASTConstInt(new T_CONST_INT(3, "0", 8)),
                                new ASTId(new T_IDENTIFIER(3, "a"))),
                            new ASTDefault(
                                new ASTId(new T_IDENTIFIER(4, "c"))) })))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.SwitchStatement().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserWhileStatement() {
            var tests = new Dictionary<string, ASTWhile> {
                {
                    @"
while (x < length) {
    x++;
}",
                    new ASTWhile(
                        new ASTBinaryExpr(
                            new ASTId(new T_IDENTIFIER(2, "x")),
                            new ASTId(new T_IDENTIFIER(2, "length")),
                            ASTBinaryExpr.Op.LT),
                        new ASTCompoundStmt(
                            new List<ASTStmt> {
                                new ASTPostStep(
                                    new ASTId(new T_IDENTIFIER(3, "x")),
                                    ASTPostStep.Kind.INC)
                            }))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.WhileStatement(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDoStatement() {
            var tests = new Dictionary<string, ASTDo> {
                {
                    @"
do {
    x++;
} while (x < length);",
                    new ASTDo(
                        new ASTBinaryExpr(
                            new ASTId(new T_IDENTIFIER(4, "x")),
                            new ASTId(new T_IDENTIFIER(4, "length")),
                            ASTBinaryExpr.Op.LT),
                        new ASTCompoundStmt(
                            new List<ASTStmt> {
                                new ASTPostStep(
                                    new ASTId(new T_IDENTIFIER(3, "x")),
                                    ASTPostStep.Kind.INC)
                            }))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.DoStatement(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserForStatement() {
            var tests = new Dictionary<string, ASTForStmt> {
                {
                    @"
for (i = 0; i < length; ++i) {
    x += i;
}",
                    new ASTForStmt(
                        2,
                        new ASTAssignExpr(
                            new ASTId(new T_IDENTIFIER(2, "i")),
                            new ASTConstInt(new T_CONST_INT(2, "0", 8)),
                            ASTAssignExpr.Op.ASSIGN),
                        new ASTBinaryExpr(
                            new ASTId(new T_IDENTIFIER(2, "i")),
                            new ASTId(new T_IDENTIFIER(2, "length")),
                            ASTBinaryExpr.Op.LT),
                        new ASTPreStep(
                            new ASTId(new T_IDENTIFIER(2, "i")),
                            ASTPreStep.Kind.INC),
                        new ASTCompoundStmt(
                            new List<ASTStmt> {
                                new ASTAssignExpr(
                                    new ASTId(new T_IDENTIFIER(3, "x")),
                                    new ASTId(new T_IDENTIFIER(3, "i")),
                                    ASTAssignExpr.Op.PLUSEQ)
                            }))
                },
                {
                    @"
for (;1;) ;",
                    new ASTForStmt(
                        2,
                        null,
                        new ASTConstInt(new T_CONST_INT(2, "1", 10)),
                        null,
                        new ASTVoidStmt(2))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.ForStatement(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserJumpStatement() {
            var tests = new Dictionary<string, ASTStmt> {
                {
                    "continue;",
                    new ASTContinue(1)
                },
                {
                    "break;",
                    new ASTBreak(1)
                },
                {
                    "goto foo;",
                    new ASTGoto(1, new ASTId(new T_IDENTIFIER(1, "foo")))
                },
                {
                    "return 0;",
                    new ASTReturn(1, new ASTConstInt(new T_CONST_INT(1, "0", 8)))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.JumpStatement(), test.Value);
            }
        }
    }
}
