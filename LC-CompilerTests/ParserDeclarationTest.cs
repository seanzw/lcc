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
            Dictionary<string, ASTStoreSpec> dict = new Dictionary<string, ASTStoreSpec> {
                {
                    "typedef",
                    new ASTStoreSpec(1, ASTStoreSpec.Kind.TYPEDEF)
                },
                {
                    "extern",
                    new ASTStoreSpec(1, ASTStoreSpec.Kind.EXTERN)
                },
                {
                    "static",
                    new ASTStoreSpec(1, ASTStoreSpec.Kind.STATIC)
                },
                {
                    "auto",
                    new ASTStoreSpec(1, ASTStoreSpec.Kind.AUTO)
                },
                {
                    "register",
                    new ASTStoreSpec(1, ASTStoreSpec.Kind.REGISTER)
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
            Dictionary<string, ASTTypeQual> dict = new Dictionary<string, ASTTypeQual> {
                {
                    "const",
                    new ASTTypeQual(1, ASTTypeQual.Kind.CONST)
                },
                {
                    "restrict",
                    new ASTTypeQual(1, ASTTypeQual.Kind.RESTRICT)
                },
                {
                    "volatile",
                    new ASTTypeQual(1, ASTTypeQual.Kind.VOLATILE)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.TypeQualifier().End(), test.Value);
            }
        }


        [TestMethod]
        public void LCCParserFunctionSpecifier() {
            Dictionary<string, ASTFuncSpec> dict = new Dictionary<string, ASTFuncSpec> {
                {
                    "inline",
                    new ASTFuncSpec(1, ASTFuncSpec.Kind.INLINE)
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
                            new ASTStoreSpec(1, ASTStoreSpec.Kind.STATIC),
                            new ASTTypeQual(1, ASTTypeQual.Kind.CONST),
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT),
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.CHAR),
                            new ASTFuncSpec(1, ASTFuncSpec.Kind.INLINE)
                        },
                        ASTStoreSpec.Kind.STATIC,
                        new List<ASTTypeSpec.Kind> {
                            ASTTypeSpec.Kind.INT,
                            ASTTypeSpec.Kind.CHAR
                        },
                        ASTFuncSpec.Kind.INLINE)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.DeclarationSpecifiers().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserEnumerator() {
            Dictionary<string, ASTEnum> dict = new Dictionary<string, ASTEnum> {
                {
                    "ZERO",
                    new ASTEnum(new ASTId(new T_IDENTIFIER(1, "ZERO")), null)
                },
                {
                    "ZERO = 1",
                    new ASTEnum(
                        new ASTId(new T_IDENTIFIER(1, "ZERO")),
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
            var dict = new Dictionary<string, IEnumerable<ASTEnum>> {
                {
                    "ZERO, ONE = 1, TWO",
                    new List<ASTEnum> {
                        new ASTEnum(new ASTId(new T_IDENTIFIER(1, "ZERO")), null),
                        new ASTEnum(
                            new ASTId(new T_IDENTIFIER(1, "ONE")),
                            new ASTConstInt(new T_CONST_INT(1, "1", 10))),
                        new ASTEnum(new ASTId(new T_IDENTIFIER(1, "TWO")), null),
                    }
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.EnumeratorList().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserEnumSpecifier() {
            Dictionary<string, ASTEnumSpec> dict = new Dictionary<string, ASTEnumSpec> {
                {
                    @"
enum Foo {
    ZERO,
    ONE = 1,
    TWO
}",
                    new ASTEnumSpec(
                        2,
                        new ASTId(new T_IDENTIFIER(2, "Foo")),
                        new LinkedList<ASTEnum>(new List<ASTEnum> {
                            new ASTEnum(new ASTId(new T_IDENTIFIER(3, "ZERO")), null),
                            new ASTEnum(
                                new ASTId(new T_IDENTIFIER(4, "ONE")),
                                new ASTConstInt(new T_CONST_INT(4, "1", 10))),
                            new ASTEnum(new ASTId(new T_IDENTIFIER(5, "TWO")), null),
                        }))
                },
                {
                    @"
enum what
",
                    new ASTEnumSpec(
                        2,
                        new ASTId(new T_IDENTIFIER(2, "what")),
                        null)
                },
                {
                    @"
enum {
    ZERO,
    ONE = 1,
    TWO,
}",
                    new ASTEnumSpec(
                        2,
                        null,
                        new LinkedList<ASTEnum>(new List<ASTEnum> {
                            new ASTEnum(new ASTId(new T_IDENTIFIER(3, "ZERO")), null),
                            new ASTEnum(
                                new ASTId(new T_IDENTIFIER(4, "ONE")),
                                new ASTConstInt(new T_CONST_INT(4, "1", 10))),
                            new ASTEnum(new ASTId(new T_IDENTIFIER(5, "TWO")), null),
                        }))
                },
                {
                    "enum test",
                    new ASTEnumSpec(1, new ASTId(new T_IDENTIFIER(1, "test")), null)
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.EnumSpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDirectDeclarator() {
            var dict = new Dictionary<string, ASTDirDecl> {
                {
                    "X",
                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(1, "X")))
                },
                {
                    "( X )",
                    new ASTParDecl(
                        new ASTDecl(
                            new List<ASTPtr>(),
                            new ASTIdDecl(new ASTId(new T_IDENTIFIER(1, "X")))))
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.DirectDeclarator().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDeclarator() {
            var dict = new Dictionary<string, ASTDirDecl> {
                {
                    "X",
                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(1, "X")))
                },
                {
                    "( X )",
                    new ASTParDecl(
                        new ASTDecl(
                            new List<ASTPtr>(),
                            new ASTIdDecl(new ASTId(new T_IDENTIFIER(1, "X")))))
                },
                {
                    @"
foo(int a, int b, double c, ...)
",
                    new ASTFuncDecl(
                        new ASTIdDecl(new ASTId(new T_IDENTIFIER(2, "foo"))),
                        new List<ASTParam> {
                            new ASTParam(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.INT)
                                    },
                                    ASTStoreSpec.Kind.NONE,
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(2, "a"))))),
                            new ASTParam(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.INT)
                                    },
                                    ASTStoreSpec.Kind.NONE,
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(2, "b"))))),
                            new ASTParam(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.DOUBLE)
                                    },
                                    ASTStoreSpec.Kind.NONE,
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.DOUBLE }),
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(2, "c"))))),
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
            var dict = new Dictionary<string, ASTStructDecl> {
                {
                    "X",
                    new ASTStructDecl(
                        new ASTDecl(
                            new List<ASTPtr>(),
                            new ASTIdDecl(
                                new ASTId(new T_IDENTIFIER(1, "X")))),
                        null)
                },
                {
                    "( X ) : 4",
                    new ASTStructDecl(
                        new ASTDecl(
                            new List<ASTPtr>(),
                            new ASTParDecl(
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(1, "X")))))),
                        new ASTConstInt(new T_CONST_INT(1, "4", 10)))
                },
                {
                    ": 4",
                    new ASTStructDecl(
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
            var dict = new Dictionary<string, IEnumerable<ASTStructDecl>> {
                {
                    "X, ( X ) : 4, : 4",
                    new List<ASTStructDecl> {
                        new ASTStructDecl(
                            new ASTDecl(
                                new List<ASTPtr>(),
                                new ASTIdDecl(
                                    new ASTId(new T_IDENTIFIER(1, "X")))),
                            null),
                        new ASTStructDecl(
                            new ASTDecl(
                                new List<ASTPtr>(),
                                new ASTParDecl(new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(1, "X")))))),
                            new ASTConstInt(new T_CONST_INT(1, "4", 10))),
                        new ASTStructDecl(
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
            var dict = new Dictionary<string, IEnumerable<ASTTypeSpecQual>> {
                {
                    "const enum what",
                    new List<ASTTypeSpecQual> {
                        new ASTTypeQual(1, ASTTypeQual.Kind.CONST),
                        new ASTEnumSpec(1, new ASTId(new T_IDENTIFIER(1, "what")), null)
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
                        new List<ASTTypeSpecQual> {
                            new ASTTypeQual(1, ASTTypeQual.Kind.CONST),
                            new ASTEnumSpec(1, new ASTId(new T_IDENTIFIER(1, "what")), null)
                        },
                        new List<ASTStructDecl> {
                            new ASTStructDecl(
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTIdDecl(
                                        new ASTId(new T_IDENTIFIER(1, "x")))),
                                    null),
                            new ASTStructDecl(
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTIdDecl(
                                        new ASTId(new T_IDENTIFIER(1, "y")))),
                                null),
                            new ASTStructDecl(
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTIdDecl(
                                        new ASTId(new T_IDENTIFIER(1, "z")))),
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
            var dict = new Dictionary<string, ASTStructUnionSpec> {
                {
                    @"
struct ss {
    int n;
    const enum what x, y, z;
}",
                    new ASTStructUnionSpec(
                        2,
                        new ASTId(new T_IDENTIFIER(2, "ss")),
                        new List<ASTStructDeclaration> {
                            new ASTStructDeclaration(
                                new List<ASTTypeSpecQual> {
                                    new ASTTypeKeySpecifier(3, ASTTypeSpec.Kind.INT)
                                },
                                new List<ASTStructDecl> {
                                    new ASTStructDecl(
                                        new ASTDecl(
                                            new List<ASTPtr>(),
                                            new ASTIdDecl(
                                                new ASTId(new T_IDENTIFIER(3, "n")))),
                                        null)
                                }),
                            new ASTStructDeclaration(
                                new List<ASTTypeSpecQual> {
                                    new ASTTypeQual(4, ASTTypeQual.Kind.CONST),
                                    new ASTEnumSpec(4, new ASTId(new T_IDENTIFIER(4, "what")), null)
                                },
                                new List<ASTStructDecl> {
                                    new ASTStructDecl(
                                        new ASTDecl(
                                            new List<ASTPtr>(),
                                            new ASTIdDecl(new ASTId(new T_IDENTIFIER(4, "x")))),
                                        null),
                                    new ASTStructDecl(
                                        new ASTDecl(
                                            new List<ASTPtr>(),
                                            new ASTIdDecl(new ASTId(new T_IDENTIFIER(4, "y")))),
                                        null),
                                    new ASTStructDecl(
                                        new ASTDecl(
                                            new List<ASTPtr>(),
                                            new ASTIdDecl(new ASTId(new T_IDENTIFIER(4, "z")))),
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
            var tests = new Dictionary<string, ASTParam> {
                {
                    "int a",
                    new ASTParam(
                        new ASTDeclSpecs(
                            new List<ASTDeclSpec> {
                                new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                            },
                            ASTStoreSpec.Kind.NONE,
                            new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                        new ASTDecl(
                            new List<ASTPtr>(),
                            new ASTIdDecl(new ASTId(new T_IDENTIFIER(1, "a")))))
                }
            };
            foreach (var test in tests) {
                Aux(test.Key, Parser.ParameterDeclaration().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserParameterList() {
            var tests = new Dictionary<string, IEnumerable<ASTParam>> {
                {
                    "int a, int b, double c",
                    new List<ASTParam> {
                            new ASTParam(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                                    },
                                    ASTStoreSpec.Kind.NONE,
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(1, "a"))))),
                            new ASTParam(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                                    },
                                    ASTStoreSpec.Kind.NONE,
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(1, "b"))))),
                            new ASTParam(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.DOUBLE)
                                    },
                                    ASTStoreSpec.Kind.NONE,
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.DOUBLE }),
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(1, "c"))))),
                        }
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.ParameterList().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserParameterTypeList() {
            var dict = new Dictionary<string, Tuple<IEnumerable<ASTParam>, bool>> {
                {
                    @"
int a, int b, double c, ...
",
                    new Tuple<IEnumerable<ASTParam>, bool>(
                        new List<ASTParam> {
                            new ASTParam(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.INT)
                                    },
                                    ASTStoreSpec.Kind.NONE,
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(2, "a"))))),
                            new ASTParam(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.INT)
                                    },
                                    ASTStoreSpec.Kind.NONE,
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(2, "b"))))),
                            new ASTParam(
                                new ASTDeclSpecs(
                                    new List<ASTDeclSpec> {
                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.DOUBLE)
                                    },
                                    ASTStoreSpec.Kind.NONE,
                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.DOUBLE }),
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(2, "c"))))),
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
            var tests = new Dictionary<string, ASTArrDecl> {
                {
                    "fa[11]",
                    new ASTArrDecl(
                        new ASTIdDecl(new ASTId(new T_IDENTIFIER(1, "fa"))),
                        new List<ASTTypeQual>(),
                        new ASTConstInt(new T_CONST_INT(1, "11", 10)),
                        false)
                },
                {
                    "y[]",
                    new ASTArrDecl(
                        new ASTIdDecl(new ASTId(new T_IDENTIFIER(1, "y"))),
                        new List<ASTTypeQual>(),
                        null,
                        false)
                },
                {
                    "a[n][6][m]",
                    new ASTArrDecl(
                        new ASTArrDecl(
                            new ASTArrDecl(
                                new ASTIdDecl(new ASTId(new T_IDENTIFIER(1, "a"))),
                                new List<ASTTypeQual>(),
                                new ASTId(new T_IDENTIFIER(1, "n")),
                                false),
                            new List<ASTTypeQual>(),
                            new ASTConstInt(new T_CONST_INT(1, "6", 10)),
                            false),
                        new List<ASTTypeQual>(),
                        new ASTId(new T_IDENTIFIER(1, "m")),
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
                        new List<ASTTypeSpecQual> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        })
                },
                {
                    "int *",
                    new ASTTypeName(
                        new List<ASTTypeSpecQual> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        },
                        new ASTAbsDecl(
                            new List<ASTPtr> {
                                new ASTPtr(1, new List<ASTTypeQual>())
                            },
                            null))
                },
                {
                    "int *[3]",
                    new ASTTypeName(
                        new List<ASTTypeSpecQual> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        },
                        new ASTAbsDecl(
                            new List<ASTPtr> {
                                new ASTPtr(1, new List<ASTTypeQual>())
                            },
                            new ASTAbsArrDecl(
                                new ASTAbsDirDeclNil(1),
                                new List<ASTTypeQual>(),
                                new ASTConstInt(new T_CONST_INT(1, "3", 10)),
                                false)))
                },
                {
                    "int (*)[3]",
                    new ASTTypeName(
                        new List<ASTTypeSpecQual> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        },
                        new ASTAbsDecl(
                            new List<ASTPtr> {},
                            new ASTAbsArrDecl(
                                new ASTAbsParDecl(new ASTAbsDecl(
                                    new List<ASTPtr> {
                                        new ASTPtr(1, new List<ASTTypeQual>())
                                    },
                                    null)),
                                new List<ASTTypeQual>(),
                                new ASTConstInt(new T_CONST_INT(1, "3", 10)),
                                false)))
                },
                {
                    "int (*)[*]",
                    new ASTTypeName(
                        new List<ASTTypeSpecQual> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        },
                        new ASTAbsDecl(
                            new List<ASTPtr> {},
                            new ASTAbsArrDecl(
                                new ASTAbsParDecl(new ASTAbsDecl(
                                    new List<ASTPtr> {
                                        new ASTPtr(1, new List<ASTTypeQual>())
                                    },
                                    null)))))
                },
                {
                    "int *[]",
                    new ASTTypeName(
                        new List<ASTTypeSpecQual> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        },
                        new ASTAbsDecl(
                            new List<ASTPtr> {
                                new ASTPtr(1, new List<ASTTypeQual>())
                            },
                            new ASTAbsArrDecl(
                                new ASTAbsDirDeclNil(1),
                                new List<ASTTypeQual>(),
                                null,
                                false)))
                },
                {
                    "int (*)(void)",
                    new ASTTypeName(
                        new List<ASTTypeSpecQual> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        },
                        new ASTAbsDecl(
                            new List<ASTPtr> {},
                            new ASTAbsFuncDecl(
                                new ASTAbsParDecl(new ASTAbsDecl(
                                    new List<ASTPtr> {
                                        new ASTPtr(1, new List<ASTTypeQual>())
                                    },
                                    null)),
                                new List<ASTParam> {
                                    new ASTParam(
                                        new ASTDeclSpecs(
                                            new List<ASTDeclSpec> {
                                                new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.VOID)
                                            },
                                            ASTStoreSpec.Kind.NONE,
                                            new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.VOID }))
                                },
                                false)))
                },
                {
                    "int (*const []) (unsigned int, ...)",
                    new ASTTypeName(
                        new List<ASTTypeSpecQual> {
                            new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                        },
                        new ASTAbsDecl(
                            new List<ASTPtr> {},
                            new ASTAbsFuncDecl(
                                new ASTAbsParDecl(new ASTAbsDecl(
                                    new List<ASTPtr> {
                                        new ASTPtr(1, new List<ASTTypeQual> { new ASTTypeQual(1, ASTTypeQual.Kind.CONST) })
                                    },
                                    new ASTAbsArrDecl(
                                        new ASTAbsDirDeclNil(1),
                                        new List<ASTTypeQual>(),
                                        null,
                                        false))),
                                new List<ASTParam> {
                                    new ASTParam(
                                        new ASTDeclSpecs(
                                            new List<ASTDeclSpec> {
                                                new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.UNSIGNED),
                                                new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                                            },
                                            ASTStoreSpec.Kind.NONE,
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
                            ASTStoreSpec.Kind.NONE,
                            new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                        new List<ASTInitDecl> {
                            new ASTInitDecl(
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTFuncDecl(
                                        new ASTIdDecl(new ASTId(new T_IDENTIFIER(2, "foo"))),
                                        new List<ASTParam> {
                                            new ASTParam(
                                                new ASTDeclSpecs(
                                                    new List<ASTDeclSpec> {
                                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.INT)
                                                    },
                                                    ASTStoreSpec.Kind.NONE,
                                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                                new ASTDecl(
                                                    new List<ASTPtr>(),
                                                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(2, "a"))))),
                                            new ASTParam(
                                                new ASTDeclSpecs(
                                                    new List<ASTDeclSpec> {
                                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.INT)
                                                    },
                                                    ASTStoreSpec.Kind.NONE,
                                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                                                new ASTDecl(
                                                    new List<ASTPtr>(),
                                                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(2, "b"))))),
                                            new ASTParam(
                                                new ASTDeclSpecs(
                                                    new List<ASTDeclSpec> {
                                                        new ASTTypeKeySpecifier(2, ASTTypeSpec.Kind.DOUBLE)
                                                    },
                                                    ASTStoreSpec.Kind.NONE,
                                                    new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.DOUBLE }),
                                                new ASTDecl(
                                                    new List<ASTPtr>(),
                                                    new ASTIdDecl(new ASTId(new T_IDENTIFIER(2, "c"))))),
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
                            ASTStoreSpec.Kind.NONE,
                            new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                        new List<ASTInitDecl> {
                            new ASTInitDecl(
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTFuncDecl(
                                        new ASTIdDecl(new ASTId(new T_IDENTIFIER(2, "foo"))),
                                        new List<ASTId> {
                                            new ASTId(new T_IDENTIFIER(2, "a")),
                                            new ASTId(new T_IDENTIFIER(2, "b")),
                                            new ASTId(new T_IDENTIFIER(2, "c")),
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
                            ASTStoreSpec.Kind.NONE,
                            new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                        new List<ASTInitDecl> {
                            new ASTInitDecl(
                                new ASTDecl(
                                    new List<ASTPtr>(),
                                    new ASTFuncDecl(
                                        new ASTIdDecl(new ASTId(new T_IDENTIFIER(2, "foo"))),
                                        new List<ASTId>())),
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
                            new ASTDesignator(new ASTId(new T_IDENTIFIER(1, "what")))
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
                        new ASTDesignator(new ASTId(new T_IDENTIFIER(1, "ref"))),
                        new ASTDesignator(new ASTConstInt(new T_CONST_INT(1, "3", 10))),
                        new ASTDesignator(new ASTId(new T_IDENTIFIER(1, "w")))
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
            var tests = new Dictionary<string, ASTParam> {
                {
                    "int a",
                    new ASTParam(
                        new ASTDeclSpecs(
                            new List<ASTDeclSpec> {
                                new ASTTypeKeySpecifier(1, ASTTypeSpec.Kind.INT)
                            },
                            ASTStoreSpec.Kind.NONE,
                            new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                        new ASTDecl(
                            new List<ASTPtr>(),
                            new ASTIdDecl(new ASTId(new T_IDENTIFIER(1, "a")))))
                },
                {
                    "const a",
                    new ASTParam(
                        new ASTDeclSpecs(
                            new List<ASTDeclSpec> {
                                new ASTTypeQual(1, ASTTypeQual.Kind.CONST),
                                new ASTTypedefName(new ASTId(new T_IDENTIFIER(1, "a")))
                            },
                            ASTStoreSpec.Kind.NONE,
                            new ASTTypedefName(new ASTId(new T_IDENTIFIER(1, "a")))))
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
                            ASTStoreSpec.Kind.NONE,
                            new List<ASTTypeSpec.Kind> { ASTTypeSpec.Kind.INT }),
                        new List<ASTInitDecl> {
                            new ASTInitDecl(
                                new ASTDecl(new List<ASTPtr>(),
                                new ASTIdDecl(new ASTId(new T_IDENTIFIER(1, "a")))))
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
