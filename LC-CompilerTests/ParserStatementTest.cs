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
            var tests = new Dictionary<string, ASTLabeledStatement> {
                {
                    "FOO: x = x + 2;",
                    new ASTLabeledStatement(
                        new ASTIdentifier(new T_IDENTIFIER(1, "FOO")),
                        new ASTAssignExpr(
                            new ASTIdentifier(new T_IDENTIFIER(1, "x")),
                            new ASTBinaryExpr(
                                new ASTIdentifier(new T_IDENTIFIER(1, "x")),
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
            var tests = new Dictionary<string, ASTCaseStatement> {
                {
                    "case 1 : a;",
                    new ASTCaseStatement(
                        new ASTConstInt(new T_CONST_INT(1, "1", 10)),
                        new ASTIdentifier(new T_IDENTIFIER(1, "a")))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.CaseStatement().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDefaultStatement() {
            var tests = new Dictionary<string, ASTDefaultStatement> {
                {
                    "default: a;",
                    new ASTDefaultStatement(new ASTIdentifier(new T_IDENTIFIER(1, "a")))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.DefaultStatement().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserCompoundStatement() {
            var tests = new Dictionary<string, ASTCompoundStatement> {
                {
                    @"
{
    a;
    int x;
}",
                    new ASTCompoundStatement(
                        new List<ASTStatement> {
                            new ASTIdentifier(new T_IDENTIFIER(3, "a")),
                            new ASTDeclaration(
                                new List<ASTDeclarationSpecifier> {
                                    new ASTTypeKeySpecifier(4, ASTTypeSpecifier.Type.INT)
                                },
                                new List<ASTInitDeclarator> {
                                    new ASTInitDeclarator(
                                        new ASTDeclarator(
                                            new List<ASTPointer>(),
                                            new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(4, "x")))))
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
            var tests = new Dictionary<string, ASTStatement> {
                {
                    ";",
                    new ASTVoidStatement(1)
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.ExpressionStatement(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserIfStatement() {
            var tests = new Dictionary<string, ASTIfStatement> {
                {
                    @"
if (i < length)
    x++;",
                    new ASTIfStatement(
                        2,
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(2, "i")),
                            new ASTIdentifier(new T_IDENTIFIER(2, "length")),
                            ASTBinaryExpr.Op.LT),
                        new ASTPostStep(
                            new ASTIdentifier(new T_IDENTIFIER(3, "x")),
                            ASTPostStep.Kind.INC),
                        null)
                },
                {
                    @"
if (i < length)
    x++;
else
    x--;",
                    new ASTIfStatement(
                        2,
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(2, "i")),
                            new ASTIdentifier(new T_IDENTIFIER(2, "length")),
                            ASTBinaryExpr.Op.LT),
                        new ASTPostStep(
                            new ASTIdentifier(new T_IDENTIFIER(3, "x")),
                            ASTPostStep.Kind.INC),
                        new ASTPostStep(
                            new ASTIdentifier(new T_IDENTIFIER(5, "x")),
                            ASTPostStep.Kind.DEC))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.IfStatement(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserSwitchStatement() {
            var tests = new Dictionary<string, ASTSwitchStatement> {
                {
                    @"
switch (x) {
case 0: a;
default: c;
}",
                    new ASTSwitchStatement(
                        2,
                        new ASTIdentifier(new T_IDENTIFIER(2, "x")),
                        new ASTCompoundStatement(new LinkedList<ASTStatement>(new List<ASTStatement> {
                            new ASTCaseStatement(
                                new ASTConstInt(new T_CONST_INT(3, "0", 8)),
                                new ASTIdentifier(new T_IDENTIFIER(3, "a"))),
                            new ASTDefaultStatement(
                                new ASTIdentifier(new T_IDENTIFIER(4, "c"))) })))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.SwitchStatement().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserWhileStatement() {
            var tests = new Dictionary<string, ASTWhileStatement> {
                {
                    @"
while (x < length) {
    x++;
}",
                    new ASTWhileStatement(
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(2, "x")),
                            new ASTIdentifier(new T_IDENTIFIER(2, "length")),
                            ASTBinaryExpr.Op.LT),
                        new ASTPostStep(
                            new ASTIdentifier(new T_IDENTIFIER(3, "x")),
                            ASTPostStep.Kind.INC))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.WhileStatement(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDoStatement() {
            var tests = new Dictionary<string, ASTDoStatement> {
                {
                    @"
do {
    x++;
} while (x < length);",
                    new ASTDoStatement(
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(4, "x")),
                            new ASTIdentifier(new T_IDENTIFIER(4, "length")),
                            ASTBinaryExpr.Op.LT),
                        new ASTPostStep(
                            new ASTIdentifier(new T_IDENTIFIER(3, "x")),
                            ASTPostStep.Kind.INC))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.DoStatement(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserForStatement() {
            var tests = new Dictionary<string, ASTForStatement> {
                {
                    @"
for (i = 0; i < length; ++i) {
    x += i;
}",
                    new ASTForStatement(
                        2,
                        new ASTAssignExpr(
                            new ASTIdentifier(new T_IDENTIFIER(2, "i")),
                            new ASTConstInt(new T_CONST_INT(2, "0", 8)),
                            ASTAssignExpr.Op.ASSIGN),
                        new ASTBinaryExpr(
                            new ASTIdentifier(new T_IDENTIFIER(2, "i")),
                            new ASTIdentifier(new T_IDENTIFIER(2, "length")),
                            ASTBinaryExpr.Op.LT),
                        new ASTPreStep(
                            new ASTIdentifier(new T_IDENTIFIER(2, "i")),
                            ASTPreStep.Kind.INC),
                        new ASTAssignExpr(
                            new ASTIdentifier(new T_IDENTIFIER(3, "x")),
                            new ASTIdentifier(new T_IDENTIFIER(3, "i")),
                            ASTAssignExpr.Op.PLUSEQ))
                },
                {
                    @"
for (;1;) ;",
                    new ASTForStatement(
                        2,
                        null,
                        new ASTConstInt(new T_CONST_INT(2, "1", 10)),
                        null,
                        new ASTVoidStatement(2))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.ForStatement(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserJumpStatement() {
            var tests = new Dictionary<string, ASTStatement> {
                {
                    "continue;",
                    new ASTContinueStatement(1)
                },
                {
                    "break;",
                    new ASTBreakStatement(1)
                },
                {
                    "goto foo;",
                    new ASTGotoStatement(1, new ASTIdentifier(new T_IDENTIFIER(1, "foo")))
                },
                {
                    "return 0;",
                    new ASTReturnStatement(1, new ASTConstInt(new T_CONST_INT(1, "0", 8)))
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.JumpStatement(), test.Value);
            }
        }
    }
}
