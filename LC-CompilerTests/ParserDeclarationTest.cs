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
        public void LCCParserStorageClassSpecifier() {
            Dictionary<string, ASTStorageSpecifier> dict = new Dictionary<string, ASTStorageSpecifier> {
                {
                    "typedef",
                    new ASTStorageSpecifier(1, ASTStorageSpecifier.Type.TYPEDEF)
                },
                {
                    "extern",
                    new ASTStorageSpecifier(1, ASTStorageSpecifier.Type.EXTERN)
                },
                {
                    "static",
                    new ASTStorageSpecifier(1, ASTStorageSpecifier.Type.STATIC)
                },
                {
                    "auto",
                    new ASTStorageSpecifier(1, ASTStorageSpecifier.Type.AUTO)
                },
                {
                    "register",
                    new ASTStorageSpecifier(1, ASTStorageSpecifier.Type.REGISTER)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.StorageClassSpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserTypeKeySpecifier() {
            Dictionary<string, ASTTypeKeySpecifier> dict = new Dictionary<string, ASTTypeKeySpecifier> {
                {
                    "void",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Type.VOID)
                },
                {
                    "char",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Type.CHAR)
                },
                {
                    "short",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Type.SHORT)
                },
                {
                    "int",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Type.INT)
                },
                {
                    "long",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Type.LONG)
                },
                {
                    "float",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Type.FLOAT)
                },
                {
                    "double",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Type.DOUBLE)
                },
                {
                    "unsigned",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Type.UNSIGNED)
                },
                {
                    "signed",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Type.SIGNED)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.TypeKeySpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserTypeQualifier() {
            Dictionary<string, ASTTypeQualifier> dict = new Dictionary<string, ASTTypeQualifier> {
                {
                    "const",
                    new ASTTypeQualifier(1, ASTTypeQualifier.Type.CONST)
                },
                {
                    "restrict",
                    new ASTTypeQualifier(1, ASTTypeQualifier.Type.RESTRICT)
                },
                {
                    "volatile",
                    new ASTTypeQualifier(1, ASTTypeQualifier.Type.VOLATILE)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.TypeQualifier().End(), test.Value);
            }
        }


        [TestMethod]
        public void LCCParserFunctionSpecifier() {
            Dictionary<string, ASTFunctionSpecifier> dict = new Dictionary<string, ASTFunctionSpecifier> {
                {
                    "inline",
                    new ASTFunctionSpecifier(1, ASTFunctionSpecifier.Type.INLINE)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.FunctionSpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDeclarationSpecifiers() {
            Dictionary<string, LinkedList<ASTDeclarationSpecifier>> dict = new Dictionary<string, LinkedList<ASTDeclarationSpecifier>> {
                {
                    "static const int char inline",
                    new LinkedList<ASTDeclarationSpecifier>(new List<ASTDeclarationSpecifier> {
                        new ASTStorageSpecifier(1, ASTStorageSpecifier.Type.STATIC),
                        new ASTTypeQualifier(1, ASTTypeQualifier.Type.CONST),
                        new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Type.INT),
                        new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Type.CHAR),
                        new ASTFunctionSpecifier(1, ASTFunctionSpecifier.Type.INLINE)
                    })
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.DeclarationSpecifiers().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserEnumerator() {
            Dictionary<string, ASTEnumerator> dict = new Dictionary<string, ASTEnumerator> {
                {
                    "ZERO",
                    new ASTEnumerator(new ASTIdentifier(new T_IDENTIFIER(1, "ZERO")), null)
                },
                {
                    "ZERO = 1",
                    new ASTEnumerator(
                        new ASTIdentifier(new T_IDENTIFIER(1, "ZERO")),
                        new ASTConstInt(new T_CONST_INT(1, "1", 10))
                    )
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.Enumerator().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserEnumeratorList() {
            Dictionary<string, LinkedList<ASTEnumerator>> dict = new Dictionary<string, LinkedList<ASTEnumerator>> {
                {
                    "ZERO, ONE = 1, TWO",
                    new LinkedList<ASTEnumerator>(new List<ASTEnumerator> {
                        new ASTEnumerator(new ASTIdentifier(new T_IDENTIFIER(1, "ZERO")), null),
                        new ASTEnumerator(
                            new ASTIdentifier(new T_IDENTIFIER(1, "ONE")),
                            new ASTConstInt(new T_CONST_INT(1, "1", 10))),
                        new ASTEnumerator(new ASTIdentifier(new T_IDENTIFIER(1, "TWO")), null),
                    })
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.EnumeratorList().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserEnumSpecifier() {
            Dictionary<string, ASTEnumSpecifier> dict = new Dictionary<string, ASTEnumSpecifier> {
                {
                    @"
enum Foo {
    ZERO,
    ONE = 1,
    TWO
}",
                    new ASTEnumSpecifier(
                        2,
                        new ASTIdentifier(new T_IDENTIFIER(2, "Foo")),
                        new LinkedList<ASTEnumerator>(new List<ASTEnumerator> {
                            new ASTEnumerator(new ASTIdentifier(new T_IDENTIFIER(3, "ZERO")), null),
                            new ASTEnumerator(
                                new ASTIdentifier(new T_IDENTIFIER(4, "ONE")),
                                new ASTConstInt(new T_CONST_INT(4, "1", 10))),
                            new ASTEnumerator(new ASTIdentifier(new T_IDENTIFIER(5, "TWO")), null),
                        }))
                },
                {
                    @"
enum what
",
                    new ASTEnumSpecifier(
                        2,
                        new ASTIdentifier(new T_IDENTIFIER(2, "what")),
                        null)
                },
                {
                    @"
enum {
    ZERO,
    ONE = 1,
    TWO,
}",
                    new ASTEnumSpecifier(
                        2,
                        null,
                        new LinkedList<ASTEnumerator>(new List<ASTEnumerator> {
                            new ASTEnumerator(new ASTIdentifier(new T_IDENTIFIER(3, "ZERO")), null),
                            new ASTEnumerator(
                                new ASTIdentifier(new T_IDENTIFIER(4, "ONE")),
                                new ASTConstInt(new T_CONST_INT(4, "1", 10))),
                            new ASTEnumerator(new ASTIdentifier(new T_IDENTIFIER(5, "TWO")), null),
                        }))
                },
                {
                    "enum test",
                    new ASTEnumSpecifier(1, new ASTIdentifier(new T_IDENTIFIER(1, "test")), null)
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.EnumSpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDirectDeclarator() {
            Dictionary<string, ASTDeclarator> dict = new Dictionary<string, ASTDeclarator> {
                {
                    "X",
                    new ASTDeclaratorIdentifier(new ASTIdentifier(new T_IDENTIFIER(1, "X")))
                },
                {
                    "( X )",
                    new ASTDeclaratorIdentifier(new ASTIdentifier(new T_IDENTIFIER(1, "X")))
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.DirectDeclarator().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDeclarator() {
            Dictionary<string, ASTDeclarator> dict = new Dictionary<string, ASTDeclarator> {
                {
                    "X",
                    new ASTDeclaratorIdentifier(new ASTIdentifier(new T_IDENTIFIER(1, "X")))
                },
                {
                    "( X )",
                    new ASTDeclaratorIdentifier(new ASTIdentifier(new T_IDENTIFIER(1, "X")))
                },
                {
                    @"
foo(int a, int b, double c, ...)
",
                    new ASTFunctionParameter(
                        new ASTDeclaratorIdentifier(new ASTIdentifier(new T_IDENTIFIER(2, "foo"))),
                        new ASTParameterType(
                            new LinkedList<ASTParameter>(new List<ASTParameter> {
                                new ASTParameterDeclarator(
                                    new LinkedList<ASTDeclarationSpecifier>(new List<ASTDeclarationSpecifier> {
                                        new ASTTypeKeySpecifier(2, ASTTypeKeySpecifier.Type.INT)}),
                                    new ASTDeclaratorIdentifier(new ASTIdentifier(new T_IDENTIFIER(2, "a")))),
                                new ASTParameterDeclarator(
                                    new LinkedList<ASTDeclarationSpecifier>(new List<ASTDeclarationSpecifier> {
                                        new ASTTypeKeySpecifier(2, ASTTypeKeySpecifier.Type.INT)}),
                                    new ASTDeclaratorIdentifier(new ASTIdentifier(new T_IDENTIFIER(2, "b")))),
                                new ASTParameterDeclarator(
                                    new LinkedList<ASTDeclarationSpecifier>(new List<ASTDeclarationSpecifier> {
                                        new ASTTypeKeySpecifier(2, ASTTypeKeySpecifier.Type.DOUBLE)}),
                                    new ASTDeclaratorIdentifier(new ASTIdentifier(new T_IDENTIFIER(2, "c")))),
                            }),
                            true))
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.Declarator().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserStructDeclarator() {
            Dictionary<string, ASTStructDeclarator> dict = new Dictionary<string, ASTStructDeclarator> {
                {
                    "X",
                    new ASTStructDeclarator(
                        new ASTDeclaratorIdentifier(
                            new ASTIdentifier(new T_IDENTIFIER(1, "X"))),
                        null)
                },
                {
                    "( X ) : 4",
                    new ASTStructDeclarator(
                        new ASTDeclaratorIdentifier(
                            new ASTIdentifier(new T_IDENTIFIER(1, "X"))),
                        new ASTConstInt(new T_CONST_INT(1, "4", 10)))
                },
                {
                    ": 4",
                    new ASTStructDeclarator(
                        null,
                        new ASTConstInt(new T_CONST_INT(1, "4", 10)))
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.StructDeclarator().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserStructDeclaratorList() {
            var dict = new Dictionary<string, LinkedList<ASTStructDeclarator>> {
                {
                    "X, ( X ) : 4, : 4",
                    new LinkedList<ASTStructDeclarator>(new List<ASTStructDeclarator> {
                        new ASTStructDeclarator(
                            new ASTDeclaratorIdentifier(
                                new ASTIdentifier(new T_IDENTIFIER(1, "X"))),
                            null),
                        new ASTStructDeclarator(
                            new ASTDeclaratorIdentifier(
                                new ASTIdentifier(new T_IDENTIFIER(1, "X"))),
                            new ASTConstInt(new T_CONST_INT(1, "4", 10))),
                        new ASTStructDeclarator(
                            null,
                            new ASTConstInt(new T_CONST_INT(1, "4", 10))),
                    })
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.StructDeclaratorList().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserSpecifierQualifierList() {
            var dict = new Dictionary<string, LinkedList<ASTTypeSpecifierQualifier>> {
                {
                    "const enum what",
                    new LinkedList<ASTTypeSpecifierQualifier>(new List<ASTTypeSpecifierQualifier> {
                        new ASTTypeQualifier(1, ASTTypeQualifier.Type.CONST),
                        new ASTEnumSpecifier(1, new ASTIdentifier(new T_IDENTIFIER(1, "what")), null)
                    })
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.SpecifierQualifierList().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserStructDeclaration() {
            var dict = new Dictionary<string, ASTStructDeclaration> {
                {
                    "const enum what x, y, z;",
                    new ASTStructDeclaration(
                        new LinkedList<ASTTypeSpecifierQualifier>(new List<ASTTypeSpecifierQualifier> {
                            new ASTTypeQualifier(1, ASTTypeQualifier.Type.CONST),
                            new ASTEnumSpecifier(1, new ASTIdentifier(new T_IDENTIFIER(1, "what")), null)
                        }),
                        new LinkedList<ASTStructDeclarator>(new List<ASTStructDeclarator> {
                            new ASTStructDeclarator(
                                new ASTDeclaratorIdentifier(
                                    new ASTIdentifier(new T_IDENTIFIER(1, "x"))),
                                null),
                            new ASTStructDeclarator(
                                new ASTDeclaratorIdentifier(
                                    new ASTIdentifier(new T_IDENTIFIER(1, "y"))),
                                null),
                            new ASTStructDeclarator(
                                new ASTDeclaratorIdentifier(
                                    new ASTIdentifier(new T_IDENTIFIER(1, "z"))),
                                null),
                        }))
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.StructDeclaration().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserStructUnionSpecifier() {
            var dict = new Dictionary<string, ASTStructSpecifier> {
                {
                    @"
struct ss {
    int n;
    const enum what x, y, z;
}",
                    new ASTStructSpecifier(
                        2,
                        new ASTIdentifier(new T_IDENTIFIER(2, "ss")),
                        new LinkedList<ASTStructDeclaration>(new List<ASTStructDeclaration> {
                            new ASTStructDeclaration(
                                new LinkedList<ASTTypeSpecifierQualifier>(new List<ASTTypeSpecifierQualifier> {
                                    new ASTTypeKeySpecifier(3, ASTTypeKeySpecifier.Type.INT)
                                }),
                                new LinkedList<ASTStructDeclarator>(new List<ASTStructDeclarator> {
                                    new ASTStructDeclarator(
                                        new ASTDeclaratorIdentifier(
                                            new ASTIdentifier(new T_IDENTIFIER(3, "n"))),
                                        null)
                                })),
                            new ASTStructDeclaration(
                                new LinkedList<ASTTypeSpecifierQualifier>(new List<ASTTypeSpecifierQualifier> {
                                    new ASTTypeQualifier(4, ASTTypeQualifier.Type.CONST),
                                    new ASTEnumSpecifier(4, new ASTIdentifier(new T_IDENTIFIER(4, "what")), null)
                                }),
                                new LinkedList<ASTStructDeclarator>(new List<ASTStructDeclarator> {
                                    new ASTStructDeclarator(
                                        new ASTDeclaratorIdentifier(
                                            new ASTIdentifier(new T_IDENTIFIER(4, "x"))),
                                        null),
                                    new ASTStructDeclarator(
                                        new ASTDeclaratorIdentifier(
                                            new ASTIdentifier(new T_IDENTIFIER(4, "y"))),
                                        null),
                                    new ASTStructDeclarator(
                                        new ASTDeclaratorIdentifier(
                                            new ASTIdentifier(new T_IDENTIFIER(4, "z"))),
                                        null),
                                }))
                        }))
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.StructUnionSpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserParameterTypeList() {
            var dict = new Dictionary<string, ASTParameterType> {
                {
                    @"
int a, int b, double c, ...
",
                    new ASTParameterType(
                        new LinkedList<ASTParameter>(new List<ASTParameter> {
                            new ASTParameterDeclarator(
                                new LinkedList<ASTDeclarationSpecifier>(new List<ASTDeclarationSpecifier> {
                                    new ASTTypeKeySpecifier(2, ASTTypeKeySpecifier.Type.INT)}),
                                new ASTDeclaratorIdentifier(new ASTIdentifier(new T_IDENTIFIER(2, "a")))),
                            new ASTParameterDeclarator(
                                new LinkedList<ASTDeclarationSpecifier>(new List<ASTDeclarationSpecifier> {
                                    new ASTTypeKeySpecifier(2, ASTTypeKeySpecifier.Type.INT)}),
                                new ASTDeclaratorIdentifier(new ASTIdentifier(new T_IDENTIFIER(2, "b")))),
                            new ASTParameterDeclarator(
                                new LinkedList<ASTDeclarationSpecifier>(new List<ASTDeclarationSpecifier> {
                                    new ASTTypeKeySpecifier(2, ASTTypeKeySpecifier.Type.DOUBLE)}),
                                new ASTDeclaratorIdentifier(new ASTIdentifier(new T_IDENTIFIER(2, "c")))),
                        }),
                        true)
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.ParameterTypeList().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDeclaration() {
            var dict = new Dictionary<string, ASTDeclaration> {
                {
                    @"
int foo(int a, int b, double c, ...);
",
                    new ASTDeclaration(
                        new LinkedList<ASTDeclarationSpecifier>(new List<ASTDeclarationSpecifier> {
                            new ASTTypeKeySpecifier(2, ASTTypeKeySpecifier.Type.INT)
                        }),
                        new LinkedList<ASTDeclarator>(new List<ASTDeclarator> {
                            new ASTFunctionParameter(
                                new ASTDeclaratorIdentifier(new ASTIdentifier(new T_IDENTIFIER(2, "foo"))),
                                new ASTParameterType(
                                    new LinkedList<ASTParameter>(new List<ASTParameter> {
                                        new ASTParameterDeclarator(
                                            new LinkedList<ASTDeclarationSpecifier>(new List<ASTDeclarationSpecifier> {
                                                new ASTTypeKeySpecifier(2, ASTTypeKeySpecifier.Type.INT)}),
                                            new ASTDeclaratorIdentifier(new ASTIdentifier(new T_IDENTIFIER(2, "a")))),
                                        new ASTParameterDeclarator(
                                            new LinkedList<ASTDeclarationSpecifier>(new List<ASTDeclarationSpecifier> {
                                                new ASTTypeKeySpecifier(2, ASTTypeKeySpecifier.Type.INT)}),
                                            new ASTDeclaratorIdentifier(new ASTIdentifier(new T_IDENTIFIER(2, "b")))),
                                        new ASTParameterDeclarator(
                                            new LinkedList<ASTDeclarationSpecifier>(new List<ASTDeclarationSpecifier> {
                                                new ASTTypeKeySpecifier(2, ASTTypeKeySpecifier.Type.DOUBLE)}),
                                            new ASTDeclaratorIdentifier(new ASTIdentifier(new T_IDENTIFIER(2, "c")))),
                                    }),
                                    true))
                        }))
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.Declaration().End(), test.Value);
            }
        }
    }
}
