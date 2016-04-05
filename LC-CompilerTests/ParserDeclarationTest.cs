using System;
using System.Linq;
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
                    new ASTStorageSpecifier(1, ASTStorageSpecifier.Kind.TYPEDEF)
                },
                {
                    "extern",
                    new ASTStorageSpecifier(1, ASTStorageSpecifier.Kind.EXTERN)
                },
                {
                    "static",
                    new ASTStorageSpecifier(1, ASTStorageSpecifier.Kind.STATIC)
                },
                {
                    "auto",
                    new ASTStorageSpecifier(1, ASTStorageSpecifier.Kind.AUTO)
                },
                {
                    "register",
                    new ASTStorageSpecifier(1, ASTStorageSpecifier.Kind.REGISTER)
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
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Kind.VOID)
                },
                {
                    "char",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Kind.CHAR)
                },
                {
                    "short",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Kind.SHORT)
                },
                {
                    "int",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Kind.INT)
                },
                {
                    "long",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Kind.LONG)
                },
                {
                    "float",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Kind.FLOAT)
                },
                {
                    "double",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Kind.DOUBLE)
                },
                {
                    "unsigned",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Kind.UNSIGNED)
                },
                {
                    "signed",
                    new ASTTypeKeySpecifier(1, ASTTypeKeySpecifier.Kind.SIGNED)
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
                    new ASTTypeQualifier(1, ASTTypeQualifier.Kind.CONST)
                },
                {
                    "restrict",
                    new ASTTypeQualifier(1, ASTTypeQualifier.Kind.RESTRICT)
                },
                {
                    "volatile",
                    new ASTTypeQualifier(1, ASTTypeQualifier.Kind.VOLATILE)
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
                    new ASTFunctionSpecifier(1, ASTFunctionSpecifier.Kind.INLINE)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.FunctionSpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDeclarationSpecifiers() {
            var dict = new Dictionary<string, ASTDeclSpecs> {
                {
                    "static const int char inline",
                    new ASTDeclSpecs(
                        new List<ASTDeclSpec> {
                            new ASTStorageSpecifier(1, ASTStorageSpecifier.Kind.STATIC),
                            new ASTTypeQualifier(1, ASTTypeQualifier.Kind.CONST),
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT),
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.CHAR),
                            new ASTFunctionSpecifier(1, ASTFunctionSpecifier.Kind.INLINE)
                        },
                        new List<ASTTypeSpec.Kind> {
                            ASTTypeSpec.Kind.INT,
                            ASTTypeSpec.Kind.CHAR
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
            var dict = new Dictionary<string, IEnumerable<ASTEnumerator>> {
                {
                    "ZERO, ONE = 1, TWO",
                    new List<ASTEnumerator> {
                        new ASTEnumerator(new ASTIdentifier(new T_IDENTIFIER(1, "ZERO")), null),
                        new ASTEnumerator(
                            new ASTIdentifier(new T_IDENTIFIER(1, "ONE")),
                            new ASTConstInt(new T_CONST_INT(1, "1", 10))),
                        new ASTEnumerator(new ASTIdentifier(new T_IDENTIFIER(1, "TWO")), null),
                    }
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
            var dict = new Dictionary<string, ASTDirDeclarator> {
                {
                    "X",
                    new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(1, "X")))
                },
                {
                    "( X )",
                    new ASTParentDeclarator(
                        new ASTDeclarator(
                            new List<ASTPointer>(),
                            new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(1, "X")))))
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.DirectDeclarator().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDeclarator() {
            var dict = new Dictionary<string, ASTDirDeclarator> {
                {
                    "X",
                    new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(1, "X")))
                },
                {
                    "( X )",
                    new ASTParentDeclarator(
                        new ASTDeclarator(
                            new List<ASTPointer>(),
                            new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(1, "X")))))
                },
                {
                    @"
foo(int a, int b, double c, ...)
",
                    new ASTFuncDeclarator(
                        new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(2, "foo"))),
                        new List<ASTParameter> {
                            new ASTParameter(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.INT)
                                    },
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(2, "a"))))),
                            new ASTParameter(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.INT)
                                    },
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(2, "b"))))),
                            new ASTParameter(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.DOUBLE)
                                    },
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.DOUBLE }),
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(2, "c"))))),
                        },
                        true)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.DirectDeclarator().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserStructDeclarator() {
            var dict = new Dictionary<string, ASTStructDeclarator> {
                {
                    "X",
                    new ASTStructDeclarator(
                        new ASTDeclarator(
                            new List<ASTPointer>(),
                            new ASTIdentifierDeclarator(
                                new ASTIdentifier(new T_IDENTIFIER(1, "X")))),
                        null)
                },
                {
                    "( X ) : 4",
                    new ASTStructDeclarator(
                        new ASTDeclarator(
                            new List<ASTPointer>(),
                            new ASTParentDeclarator(
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(1, "X")))))),
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
            var dict = new Dictionary<string, IEnumerable<ASTStructDeclarator>> {
                {
                    "X, ( X ) : 4, : 4",
                    new List<ASTStructDeclarator> {
                        new ASTStructDeclarator(
                            new ASTDeclarator(
                                new List<ASTPointer>(),
                                new ASTIdentifierDeclarator(
                                    new ASTIdentifier(new T_IDENTIFIER(1, "X")))),
                            null),
                        new ASTStructDeclarator(
                            new ASTDeclarator(
                                new List<ASTPointer>(),
                                new ASTIdentifierDeclarator(
                                    new ASTIdentifier(new T_IDENTIFIER(1, "X")))),
                            new ASTConstInt(new T_CONST_INT(1, "4", 10))),
                        new ASTStructDeclarator(
                            null,
                            new ASTConstInt(new T_CONST_INT(1, "4", 10))),
                    }
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.StructDeclaratorList().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserSpecifierQualifierList() {
            var dict = new Dictionary<string, IEnumerable<ASTTypeSpecifierQualifier>> {
                {
                    "const enum what",
                    new List<ASTTypeSpecifierQualifier> {
                        new ASTTypeQualifier(1, ASTTypeQualifier.Kind.CONST),
                        new ASTEnumSpecifier(1, new ASTIdentifier(new T_IDENTIFIER(1, "what")), null)
                    }
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
                        new List<ASTTypeSpecifierQualifier> {
                            new ASTTypeQualifier(1, ASTTypeQualifier.Kind.CONST),
                            new ASTEnumSpecifier(1, new ASTIdentifier(new T_IDENTIFIER(1, "what")), null)
                        },
                        new List<ASTStructDeclarator> {
                            new ASTStructDeclarator(
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTIdentifierDeclarator(
                                        new ASTIdentifier(new T_IDENTIFIER(1, "x")))),
                                    null),
                            new ASTStructDeclarator(
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTIdentifierDeclarator(
                                        new ASTIdentifier(new T_IDENTIFIER(1, "y")))),
                                null),
                            new ASTStructDeclarator(
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTIdentifierDeclarator(
                                        new ASTIdentifier(new T_IDENTIFIER(1, "z")))),
                                null),
                        })
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.StructDeclaration().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserStructUnionSpecifier() {
            var dict = new Dictionary<string, ASTStructUnionSpecifier> {
                {
                    @"
struct ss {
    int n;
    const enum what x, y, z;
}",
                    new ASTStructUnionSpecifier(
                        2,
                        new ASTIdentifier(new T_IDENTIFIER(2, "ss")),
                        new List<ASTStructDeclaration> {
                            new ASTStructDeclaration(
                                new List<ASTTypeSpecifierQualifier> {
                                    new ASTTypeKeySpecifier(3, ASTTypeSpec.Kind.INT)
                                },
                                new List<ASTStructDeclarator> {
                                    new ASTStructDeclarator(
                                        new ASTDeclarator(
                                            new List<ASTPointer>(),
                                            new ASTIdentifierDeclarator(
                                                new ASTIdentifier(new T_IDENTIFIER(3, "n")))),
                                        null)
                                }),
                            new ASTStructDeclaration(
                                new List<ASTTypeSpecifierQualifier> {
                                    new ASTTypeQualifier(4, ASTTypeQualifier.Kind.CONST),
                                    new ASTEnumSpecifier(4, new ASTIdentifier(new T_IDENTIFIER(4, "what")), null)
                                },
                                new List<ASTStructDeclarator> {
                                    new ASTStructDeclarator(
                                        new ASTDeclarator(
                                            new List<ASTPointer>(),
                                            new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(4, "x")))),
                                        null),
                                    new ASTStructDeclarator(
                                        new ASTDeclarator(
                                            new List<ASTPointer>(),
                                            new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(4, "y")))),
                                        null),
                                    new ASTStructDeclarator(
                                        new ASTDeclarator(
                                            new List<ASTPointer>(),
                                            new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(4, "z")))),
                                        null),
                                })
                        },
                        ASTTypeSpec.Kind.STRUCT)
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.StructUnionSpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserParameterDeclaration() {
            var tests = new Dictionary<string, ASTParameter> {
                {
                    "int a",
                    new ASTParameter(
                        new ASTDeclSpecs(
                            new List<ASTDeclSpec> {
                                new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                            },
                            new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                        new ASTDeclarator(
                            new List<ASTPointer>(),
                            new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(1, "a")))))
                }
            };
            foreach (var test in tests) {
                Aux(test.Key, Parser.ParameterDeclaration().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserParameterList() {
            var tests = new Dictionary<string, IEnumerable<ASTParameter>> {
                {
                    "int a, int b, double c",
                    new List<ASTParameter> {
                            new ASTParameter(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                                    },
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(1, "a"))))),
                            new ASTParameter(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                                    },
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(1, "b"))))),
                            new ASTParameter(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.DOUBLE)
                                    },
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.DOUBLE }),
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(1, "c"))))),
                        }
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.ParameterList().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserParameterTypeList() {
            var dict = new Dictionary<string, Tuple<IEnumerable<ASTParameter>, bool>> {
                {
                    @"
int a, int b, double c, ...
",
                    new Tuple<IEnumerable<ASTParameter>, bool>(
                        new List<ASTParameter> {
                            new ASTParameter(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.INT)
                                    },
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(2, "a"))))),
                            new ASTParameter(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.INT)
                                    },
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(2, "b"))))),
                            new ASTParameter(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.DOUBLE)
                                    },
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.DOUBLE }),
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(2, "c"))))),
                        },
                        true)
                },
            };

            foreach (var test in dict) {
                var result = Utility.parse(test.Key, Parser.ParameterTypeList().End());

                // Check the first result.
                Assert.AreEqual(1, result.Count());
                Assert.IsTrue(test.Value.Item1.SequenceEqual(result.First().Value.Item1));
                Assert.AreEqual(test.Value.Item2, result.First().Value.Item2);
                Assert.IsFalse(result.First().Remain.More());
            }
        }

        [TestMethod]
        public void LCCParserArrayDeclarator() {
            var tests = new Dictionary<string, ASTArrDeclarator> {
                {
                    "fa[11]",
                    new ASTArrDeclarator(
                        new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(1, "fa"))),
                        new List<ASTTypeQualifier>(),
                        new ASTConstInt(new T_CONST_INT(1, "11", 10)),
                        false)
                },
                {
                    "y[]",
                    new ASTArrDeclarator(
                        new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(1, "y"))),
                        new List<ASTTypeQualifier>(),
                        null,
                        false)
                },
                {
                    "a[n][6][m]",
                    new ASTArrDeclarator(
                        new ASTArrDeclarator(
                            new ASTArrDeclarator(
                                new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(1, "a"))),
                                new List<ASTTypeQualifier>(),
                                new ASTIdentifier(new T_IDENTIFIER(1, "n")),
                                false),
                            new List<ASTTypeQualifier>(),
                            new ASTConstInt(new T_CONST_INT(1, "6", 10)),
                            false),
                        new List<ASTTypeQualifier>(),
                        new ASTIdentifier(new T_IDENTIFIER(1, "m")),
                        false)
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.DirectDeclarator().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserTypeName() {
            var tests = new Dictionary<string, ASTTypeName> {
                {
                    "int",
                    new ASTTypeName(
                        new List<ASTTypeSpecifierQualifier> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        })
                },
                {
                    "int *",
                    new ASTTypeName(
                        new List<ASTTypeSpecifierQualifier> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        },
                        new ASTAbsDeclarator(
                            new List<ASTPointer> {
                                new ASTPointer(1, new List<ASTTypeQualifier>())
                            },
                            null))
                },
                {
                    "int *[3]",
                    new ASTTypeName(
                        new List<ASTTypeSpecifierQualifier> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        },
                        new ASTAbsDeclarator(
                            new List<ASTPointer> {
                                new ASTPointer(1, new List<ASTTypeQualifier>())
                            },
                            new ASTAbsArrDeclarator(
                                null,
                                new List<ASTTypeQualifier>(),
                                new ASTConstInt(new T_CONST_INT(1, "3", 10)),
                                false)))
                },
                {
                    "int (*)[3]",
                    new ASTTypeName(
                        new List<ASTTypeSpecifierQualifier> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        },
                        new ASTAbsDeclarator(
                            new List<ASTPointer> {},
                            new ASTAbsArrDeclarator(
                                new ASTAbsParentDeclarator(new ASTAbsDeclarator(
                                    new List<ASTPointer> {
                                        new ASTPointer(1, new List<ASTTypeQualifier>())
                                    })),
                                new List<ASTTypeQualifier>(),
                                new ASTConstInt(new T_CONST_INT(1, "3", 10)),
                                false)))
                },
                {
                    "int (*)[*]",
                    new ASTTypeName(
                        new List<ASTTypeSpecifierQualifier> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        },
                        new ASTAbsDeclarator(
                            new List<ASTPointer> {},
                            new ASTAbsArrDeclarator(
                                new ASTAbsParentDeclarator(new ASTAbsDeclarator(
                                    new List<ASTPointer> {
                                        new ASTPointer(1, new List<ASTTypeQualifier>())
                                    })))))
                },
                {
                    "int *[]",
                    new ASTTypeName(
                        new List<ASTTypeSpecifierQualifier> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        },
                        new ASTAbsDeclarator(
                            new List<ASTPointer> {
                                new ASTPointer(1, new List<ASTTypeQualifier>())
                            },
                            new ASTAbsArrDeclarator(
                                null,
                                new List<ASTTypeQualifier>(),
                                null,
                                false)))
                },
                {
                    "int (*)(void)",
                    new ASTTypeName(
                        new List<ASTTypeSpecifierQualifier> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        },
                        new ASTAbsDeclarator(
                            new List<ASTPointer> {},
                            new ASTAbsFuncDeclarator(
                                new ASTAbsParentDeclarator(new ASTAbsDeclarator(
                                    new List<ASTPointer> {
                                        new ASTPointer(1, new List<ASTTypeQualifier>())
                                    })),
                                new List<ASTParameter> {
                                    new ASTParameter(
                                        new ASTDeclSpecs(
                                            new List<ASTDeclSpec> {
                                                new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                                            },
                                            new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }))
                                },
                                false)))
                },
                {
                    "int (*const []) (unsigned int, ...)",
                    new ASTTypeName(
                        new List<ASTTypeSpecifierQualifier> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        },
                        new ASTAbsDeclarator(
                            new List<ASTPointer> {},
                            new ASTAbsFuncDeclarator(
                                new ASTAbsParentDeclarator(new ASTAbsDeclarator(
                                    new List<ASTPointer> {
                                        new ASTPointer(1, new List<ASTTypeQualifier> { new ASTTypeQualifier(1, ASTTypeQualifier.Kind.CONST) })
                                    },
                                    new ASTAbsArrDeclarator(
                                        null,
                                        new List<ASTTypeQualifier>(),
                                        null,
                                        false))),
                                new List<ASTParameter> {
                                    new ASTParameter(
                                        new ASTDeclSpecs(
                                            new List<ASTDeclSpec> {
                                                new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.UNSIGNED),
                                                new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                                            },
                                            new List<ASTTypeSpec.Kind> {
                                                ASTTypeSpec.Kind.UNSIGNED,
                                                ASTTypeSpec.Kind.INT
                                            })
                                        )
                                },
                                true)))
                }

            };
            foreach (var test in tests) {
                Aux(test.Key, Parser.TypeName().End(), test.Value);
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
                        new ASTDeclSpecs(
                            new List<ASTDeclSpec> {
                                new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.INT)
                            },
                            new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                        new List<ASTInitDeclarator> {
                            new ASTInitDeclarator(
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTFuncDeclarator(
                                        new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(2, "foo"))),
                                        new List<ASTParameter> {
                                            new ASTParameter(
                                                new ASTDeclSpecs(
                                                    new List<ASTDeclSpec> {
                                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.INT)
                                                    },
                                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                                new ASTDeclarator(
                                                    new List<ASTPointer>(),
                                                    new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(2, "a"))))),
                                            new ASTParameter(
                                                new ASTDeclSpecs(
                                                    new List<ASTDeclSpec> {
                                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.INT)
                                                    },
                                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                                new ASTDeclarator(
                                                    new List<ASTPointer>(),
                                                    new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(2, "b"))))),
                                            new ASTParameter(
                                                new ASTDeclSpecs(
                                                    new List<ASTDeclSpec> {
                                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.DOUBLE)
                                                    },
                                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.DOUBLE }),
                                                new ASTDeclarator(
                                                    new List<ASTPointer>(),
                                                    new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(2, "c"))))),
                                        },
                                        true)),
                                null)
                        })
                },
                {
                    @"
int foo(a, b, c);
",
                    new ASTDeclaration(
                        new ASTDeclSpecs(
                            new List<ASTDeclSpec> {
                                new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.INT)
                            },
                            new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                        new List<ASTInitDeclarator> {
                            new ASTInitDeclarator(
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTFuncDeclarator(
                                        new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(2, "foo"))),
                                        new List<ASTIdentifier> {
                                            new ASTIdentifier(new T_IDENTIFIER(2, "a")),
                                            new ASTIdentifier(new T_IDENTIFIER(2, "b")),
                                            new ASTIdentifier(new T_IDENTIFIER(2, "c")),
                                        })),
                                null)
                        })
                },
                {
                    @"
int foo();
",
                    new ASTDeclaration(
                        new ASTDeclSpecs(
                            new List<ASTDeclSpec> {
                                new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.INT)
                            },
                            new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                        new List<ASTInitDeclarator> {
                            new ASTInitDeclarator(
                                new ASTDeclarator(
                                    new List<ASTPointer>(),
                                    new ASTFuncDeclarator(
                                        new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(2, "foo"))),
                                        new List<ASTIdentifier>())),
                                null)
                        })
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.Declaration().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserInitializer() {
            var tests = new Dictionary<string, ASTInitializer> {
                {
                    "2 = 3",
                    new ASTInitializer(new ASTAssignExpr(
                        new ASTConstInt(new T_CONST_INT(1, "2", 10)),
                        new ASTConstInt(new T_CONST_INT(1, "3", 10)),
                        ASTAssignExpr.Op.ASSIGN))
                },
                {
                    "{ .what = {1, 2, 3 }, }",
                    new ASTInitializer(new List<ASTInitItem> {
                        new ASTInitItem(new ASTInitializer(new List<ASTInitItem> {
                            new ASTInitItem(new ASTInitializer(new ASTConstInt(new T_CONST_INT(1, "1", 10)))),
                            new ASTInitItem(new ASTInitializer(new ASTConstInt(new T_CONST_INT(1, "2", 10)))),
                            new ASTInitItem(new ASTInitializer(new ASTConstInt(new T_CONST_INT(1, "3", 10))))
                        }),
                        new List<ASTDesignator> {
                            new ASTDesignator(new ASTIdentifier(new T_IDENTIFIER(1, "what")))
                        })
                    })
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.Initializer().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDesignation() {
            var tests = new Dictionary<string, IEnumerable<ASTDesignator>> {
                {
                    ".ref[3].w = ",
                    new List<ASTDesignator> {
                        new ASTDesignator(new ASTIdentifier(new T_IDENTIFIER(1, "ref"))),
                        new ASTDesignator(new ASTConstInt(new T_CONST_INT(1, "3", 10))),
                        new ASTDesignator(new ASTIdentifier(new T_IDENTIFIER(1, "w")))
                    }
                }
            };
            foreach (var test in tests) {
                Aux(test.Key, Parser.Designation().End().Select(x => x as IEnumerable<ASTDesignator>), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserTypedef() {
            Env.PushScope();
            Env.AddTypedefName(1, "a");
            var tests = new Dictionary<string, ASTParameter> {
                {
                    "int a",
                    new ASTParameter(
                        new ASTDeclSpecs(
                            new List<ASTDeclSpec> {
                                new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                            },
                            new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                        new ASTDeclarator(
                            new List<ASTPointer>(),
                            new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(1, "a")))))
                }
            };
            foreach (var test in tests) {
                Aux(test.Key, Parser.ParameterDeclaration().End(), test.Value);
            }
            Env.PopScope();
        }

        [TestMethod]
        [ExpectedException(typeof(TypedefRedefined), "Redefine typedef name.")]
        public void LCCParserTypeRedefined() {
            Env.PushScope();
            Env.AddTypedefName(1, "a");
            var tests = new Dictionary<string, ASTDeclaration> {
                {
                    "typedef int a;",
                    new ASTDeclaration(
                        new ASTDeclSpecs(
                            new List<ASTDeclSpec> {
                                new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                            },
                            new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                        new List<ASTInitDeclarator> {
                            new ASTInitDeclarator(
                                new ASTDeclarator(new List<ASTPointer>(),
                                new ASTIdentifierDeclarator(new ASTIdentifier(new T_IDENTIFIER(1, "a")))))
                        })
                }
            };
            foreach (var test in tests) {
                Aux(test.Key, Parser.Declaration().End(), test.Value);
            }
            Env.PopScope();

        }
    }
}
