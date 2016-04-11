using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.SyntaxTree;
using lcc.Token;
using lcc.Parser;
using Parserc;

namespace LC_CompilerTests {

    public partial class ParserTests {

        [TestMethod]
        public void LCCParserStorageClassSpecifier() {
            Dictionary<string, STStoreSpec> dict = new Dictionary<string, STStoreSpec> {
                {
                    "typedef",
                    new STStoreSpec(1, STStoreSpec.Kind.TYPEDEF)
                },
                {
                    "extern",
                    new STStoreSpec(1, STStoreSpec.Kind.EXTERN)
                },
                {
                    "static",
                    new STStoreSpec(1, STStoreSpec.Kind.STATIC)
                },
                {
                    "auto",
                    new STStoreSpec(1, STStoreSpec.Kind.AUTO)
                },
                {
                    "register",
                    new STStoreSpec(1, STStoreSpec.Kind.REGISTER)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.StorageClassSpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserTypeKeySpecifier() {
            Dictionary<string, STTypeKeySpec> dict = new Dictionary<string, STTypeKeySpec> {
                {
                    "void",
                    new STTypeKeySpec(1, STTypeKeySpec.Kind.VOID)
                },
                {
                    "char",
                    new STTypeKeySpec(1, STTypeKeySpec.Kind.CHAR)
                },
                {
                    "short",
                    new STTypeKeySpec(1, STTypeKeySpec.Kind.SHORT)
                },
                {
                    "int",
                    new STTypeKeySpec(1, STTypeKeySpec.Kind.INT)
                },
                {
                    "long",
                    new STTypeKeySpec(1, STTypeKeySpec.Kind.LONG)
                },
                {
                    "float",
                    new STTypeKeySpec(1, STTypeKeySpec.Kind.FLOAT)
                },
                {
                    "double",
                    new STTypeKeySpec(1, STTypeKeySpec.Kind.DOUBLE)
                },
                {
                    "unsigned",
                    new STTypeKeySpec(1, STTypeKeySpec.Kind.UNSIGNED)
                },
                {
                    "signed",
                    new STTypeKeySpec(1, STTypeKeySpec.Kind.SIGNED)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.TypeKeySpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserTypeQualifier() {
            Dictionary<string, STTypeQual> dict = new Dictionary<string, STTypeQual> {
                {
                    "const",
                    new STTypeQual(1, STTypeQual.Kind.CONST)
                },
                {
                    "restrict",
                    new STTypeQual(1, STTypeQual.Kind.RESTRICT)
                },
                {
                    "volatile",
                    new STTypeQual(1, STTypeQual.Kind.VOLATILE)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.TypeQualifier().End(), test.Value);
            }
        }


        [TestMethod]
        public void LCCParserFunctionSpecifier() {
            Dictionary<string, STFuncSpec> dict = new Dictionary<string, STFuncSpec> {
                {
                    "inline",
                    new STFuncSpec(1, STFuncSpec.Kind.INLINE)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.FunctionSpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDeclarationSpecifiers() {
            var dict = new Dictionary<string, STDeclSpecs> {
                {
                    "static const int char inline",
                    new STDeclSpecs(
                        new List<STDeclSpec> {
                            new STStoreSpec(1, STStoreSpec.Kind.STATIC),
                            new STTypeQual(1, STTypeQual.Kind.CONST),
                            new STTypeKeySpec(1, STTypeSpec.Kind.INT),
                            new STTypeKeySpec(1, STTypeSpec.Kind.CHAR),
                            new STFuncSpec(1, STFuncSpec.Kind.INLINE)
                        },
                        STStoreSpec.Kind.STATIC,
                        new List<STTypeSpec.Kind> {
                            STTypeSpec.Kind.INT,
                            STTypeSpec.Kind.CHAR
                        },
                        STFuncSpec.Kind.INLINE)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.DeclarationSpecifiers().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserEnumerator() {
            Dictionary<string, STEnum> dict = new Dictionary<string, STEnum> {
                {
                    "ZERO",
                    new STEnum(new STId(new T_IDENTIFIER(1, "ZERO")), null)
                },
                {
                    "ZERO = 1",
                    new STEnum(
                        new STId(new T_IDENTIFIER(1, "ZERO")),
                        new STConstInt(new T_CONST_INT(1, "1", 10))
                    )
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.Enumerator().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserEnumeratorList() {
            var dict = new Dictionary<string, IEnumerable<STEnum>> {
                {
                    "ZERO, ONE = 1, TWO",
                    new List<STEnum> {
                        new STEnum(new STId(new T_IDENTIFIER(1, "ZERO")), null),
                        new STEnum(
                            new STId(new T_IDENTIFIER(1, "ONE")),
                            new STConstInt(new T_CONST_INT(1, "1", 10))),
                        new STEnum(new STId(new T_IDENTIFIER(1, "TWO")), null),
                    }
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.EnumeratorList().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserEnumSpecifier() {
            Dictionary<string, STEnumSpec> dict = new Dictionary<string, STEnumSpec> {
                {
                    @"
enum Foo {
    ZERO,
    ONE = 1,
    TWO
}",
                    new STEnumSpec(
                        2,
                        new STId(new T_IDENTIFIER(2, "Foo")),
                        new LinkedList<STEnum>(new List<STEnum> {
                            new STEnum(new STId(new T_IDENTIFIER(3, "ZERO")), null),
                            new STEnum(
                                new STId(new T_IDENTIFIER(4, "ONE")),
                                new STConstInt(new T_CONST_INT(4, "1", 10))),
                            new STEnum(new STId(new T_IDENTIFIER(5, "TWO")), null),
                        }))
                },
                {
                    @"
enum what
",
                    new STEnumSpec(
                        2,
                        new STId(new T_IDENTIFIER(2, "what")),
                        null)
                },
                {
                    @"
enum {
    ZERO,
    ONE = 1,
    TWO,
}",
                    new STEnumSpec(
                        2,
                        null,
                        new LinkedList<STEnum>(new List<STEnum> {
                            new STEnum(new STId(new T_IDENTIFIER(3, "ZERO")), null),
                            new STEnum(
                                new STId(new T_IDENTIFIER(4, "ONE")),
                                new STConstInt(new T_CONST_INT(4, "1", 10))),
                            new STEnum(new STId(new T_IDENTIFIER(5, "TWO")), null),
                        }))
                },
                {
                    "enum test",
                    new STEnumSpec(1, new STId(new T_IDENTIFIER(1, "test")), null)
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.EnumSpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDirectDeclarator() {
            var dict = new Dictionary<string, STDirDeclarator> {
                {
                    "X",
                    new STIdDeclarator(new STId(new T_IDENTIFIER(1, "X")))
                },
                {
                    "( X )",
                    new STParDeclarator(
                        new STDeclarator(
                            new List<STPtr>(),
                            new STIdDeclarator(new STId(new T_IDENTIFIER(1, "X")))))
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.DirectDeclarator().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDeclarator() {
            var dict = new Dictionary<string, STDirDeclarator> {
                {
                    "X",
                    new STIdDeclarator(new STId(new T_IDENTIFIER(1, "X")))
                },
                {
                    "( X )",
                    new STParDeclarator(
                        new STDeclarator(
                            new List<STPtr>(),
                            new STIdDeclarator(new STId(new T_IDENTIFIER(1, "X")))))
                },
                {
                    @"
foo(int a, int b, double c, ...)
",
                    new STFuncDeclarator(
                        new STIdDeclarator(new STId(new T_IDENTIFIER(2, "foo"))),
                        new List<STParam> {
                            new STParam(
                                new STDeclSpecs(
                                    new List<STDeclSpec> {
                                        new STTypeKeySpec(2, STTypeSpec.Kind.INT)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<STTypeSpec.Kind> { STTypeSpec.Kind.INT }),
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STIdDeclarator(new STId(new T_IDENTIFIER(2, "a"))))),
                            new STParam(
                                new STDeclSpecs(
                                    new List<STDeclSpec> {
                                        new STTypeKeySpec(2, STTypeSpec.Kind.INT)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<STTypeSpec.Kind> { STTypeSpec.Kind.INT }),
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STIdDeclarator(new STId(new T_IDENTIFIER(2, "b"))))),
                            new STParam(
                                new STDeclSpecs(
                                    new List<STDeclSpec> {
                                        new STTypeKeySpec(2, STTypeSpec.Kind.DOUBLE)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<STTypeSpec.Kind> { STTypeSpec.Kind.DOUBLE }),
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STIdDeclarator(new STId(new T_IDENTIFIER(2, "c"))))),
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
            var dict = new Dictionary<string, STStructDeclarator> {
                {
                    "X",
                    new STStructDeclarator(
                        new STDeclarator(
                            new List<STPtr>(),
                            new STIdDeclarator(
                                new STId(new T_IDENTIFIER(1, "X")))),
                        null)
                },
                {
                    "( X ) : 4",
                    new STStructDeclarator(
                        new STDeclarator(
                            new List<STPtr>(),
                            new STParDeclarator(
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STIdDeclarator(new STId(new T_IDENTIFIER(1, "X")))))),
                        new STConstInt(new T_CONST_INT(1, "4", 10)))
                },
                {
                    ": 4",
                    new STStructDeclarator(
                        null,
                        new STConstInt(new T_CONST_INT(1, "4", 10)))
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.StructDeclarator().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserStructDeclaratorList() {
            var dict = new Dictionary<string, IEnumerable<STStructDeclarator>> {
                {
                    "X, ( X ) : 4, : 4",
                    new List<STStructDeclarator> {
                        new STStructDeclarator(
                            new STDeclarator(
                                new List<STPtr>(),
                                new STIdDeclarator(
                                    new STId(new T_IDENTIFIER(1, "X")))),
                            null),
                        new STStructDeclarator(
                            new STDeclarator(
                                new List<STPtr>(),
                                new STParDeclarator(new STDeclarator(
                                    new List<STPtr>(),
                                    new STIdDeclarator(new STId(new T_IDENTIFIER(1, "X")))))),
                            new STConstInt(new T_CONST_INT(1, "4", 10))),
                        new STStructDeclarator(
                            null,
                            new STConstInt(new T_CONST_INT(1, "4", 10))),
                    }
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.StructDeclaratorList().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserSpecifierQualifierList() {
            var dict = new Dictionary<string, IEnumerable<STTypeSpecQual>> {
                {
                    "const enum what",
                    new List<STTypeSpecQual> {
                        new STTypeQual(1, STTypeQual.Kind.CONST),
                        new STEnumSpec(1, new STId(new T_IDENTIFIER(1, "what")), null)
                    }
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.SpecifierQualifierList().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserStructDeclaration() {
            var dict = new Dictionary<string, STStructDeclaration> {
                {
                    "const enum what x, y, z;",
                    new STStructDeclaration(
                        new List<STTypeSpecQual> {
                            new STTypeQual(1, STTypeQual.Kind.CONST),
                            new STEnumSpec(1, new STId(new T_IDENTIFIER(1, "what")), null)
                        },
                        new List<STStructDeclarator> {
                            new STStructDeclarator(
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STIdDeclarator(
                                        new STId(new T_IDENTIFIER(1, "x")))),
                                    null),
                            new STStructDeclarator(
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STIdDeclarator(
                                        new STId(new T_IDENTIFIER(1, "y")))),
                                null),
                            new STStructDeclarator(
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STIdDeclarator(
                                        new STId(new T_IDENTIFIER(1, "z")))),
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
            var dict = new Dictionary<string, STStructUnionSpec> {
                {
                    @"
struct ss {
    int n;
    const enum what x, y, z;
}",
                    new STStructUnionSpec(
                        2,
                        new STId(new T_IDENTIFIER(2, "ss")),
                        new List<STStructDeclaration> {
                            new STStructDeclaration(
                                new List<STTypeSpecQual> {
                                    new STTypeKeySpec(3, STTypeSpec.Kind.INT)
                                },
                                new List<STStructDeclarator> {
                                    new STStructDeclarator(
                                        new STDeclarator(
                                            new List<STPtr>(),
                                            new STIdDeclarator(
                                                new STId(new T_IDENTIFIER(3, "n")))),
                                        null)
                                }),
                            new STStructDeclaration(
                                new List<STTypeSpecQual> {
                                    new STTypeQual(4, STTypeQual.Kind.CONST),
                                    new STEnumSpec(4, new STId(new T_IDENTIFIER(4, "what")), null)
                                },
                                new List<STStructDeclarator> {
                                    new STStructDeclarator(
                                        new STDeclarator(
                                            new List<STPtr>(),
                                            new STIdDeclarator(new STId(new T_IDENTIFIER(4, "x")))),
                                        null),
                                    new STStructDeclarator(
                                        new STDeclarator(
                                            new List<STPtr>(),
                                            new STIdDeclarator(new STId(new T_IDENTIFIER(4, "y")))),
                                        null),
                                    new STStructDeclarator(
                                        new STDeclarator(
                                            new List<STPtr>(),
                                            new STIdDeclarator(new STId(new T_IDENTIFIER(4, "z")))),
                                        null),
                                })
                        },
                        STTypeSpec.Kind.STRUCT)
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.StructUnionSpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserParameterDeclaration() {
            var tests = new Dictionary<string, STParam> {
                {
                    "int a",
                    new STParam(
                        new STDeclSpecs(
                            new List<STDeclSpec> {
                                new STTypeKeySpec(1, STTypeSpec.Kind.INT)
                            },
                            STStoreSpec.Kind.NONE,
                            new List<STTypeSpec.Kind> { STTypeSpec.Kind.INT }),
                        new STDeclarator(
                            new List<STPtr>(),
                            new STIdDeclarator(new STId(new T_IDENTIFIER(1, "a")))))
                }
            };
            foreach (var test in tests) {
                Aux(test.Key, Parser.ParameterDeclaration().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserParameterList() {
            var tests = new Dictionary<string, IEnumerable<STParam>> {
                {
                    "int a, int b, double c",
                    new List<STParam> {
                            new STParam(
                                new STDeclSpecs(
                                    new List<STDeclSpec> {
                                        new STTypeKeySpec(1, STTypeSpec.Kind.INT)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<STTypeSpec.Kind> { STTypeSpec.Kind.INT }),
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STIdDeclarator(new STId(new T_IDENTIFIER(1, "a"))))),
                            new STParam(
                                new STDeclSpecs(
                                    new List<STDeclSpec> {
                                        new STTypeKeySpec(1, STTypeSpec.Kind.INT)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<STTypeSpec.Kind> { STTypeSpec.Kind.INT }),
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STIdDeclarator(new STId(new T_IDENTIFIER(1, "b"))))),
                            new STParam(
                                new STDeclSpecs(
                                    new List<STDeclSpec> {
                                        new STTypeKeySpec(1, STTypeSpec.Kind.DOUBLE)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<STTypeSpec.Kind> { STTypeSpec.Kind.DOUBLE }),
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STIdDeclarator(new STId(new T_IDENTIFIER(1, "c"))))),
                        }
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.ParameterList().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserParameterTypeList() {
            var dict = new Dictionary<string, Tuple<IEnumerable<STParam>, bool>> {
                {
                    @"
int a, int b, double c, ...
",
                    new Tuple<IEnumerable<STParam>, bool>(
                        new List<STParam> {
                            new STParam(
                                new STDeclSpecs(
                                    new List<STDeclSpec> {
                                        new STTypeKeySpec(2, STTypeSpec.Kind.INT)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<STTypeSpec.Kind> { STTypeSpec.Kind.INT }),
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STIdDeclarator(new STId(new T_IDENTIFIER(2, "a"))))),
                            new STParam(
                                new STDeclSpecs(
                                    new List<STDeclSpec> {
                                        new STTypeKeySpec(2, STTypeSpec.Kind.INT)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<STTypeSpec.Kind> { STTypeSpec.Kind.INT }),
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STIdDeclarator(new STId(new T_IDENTIFIER(2, "b"))))),
                            new STParam(
                                new STDeclSpecs(
                                    new List<STDeclSpec> {
                                        new STTypeKeySpec(2, STTypeSpec.Kind.DOUBLE)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<STTypeSpec.Kind> { STTypeSpec.Kind.DOUBLE }),
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STIdDeclarator(new STId(new T_IDENTIFIER(2, "c"))))),
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
            var tests = new Dictionary<string, STArrDeclarator> {
                {
                    "fa[11]",
                    new STArrDeclarator(
                        new STIdDeclarator(new STId(new T_IDENTIFIER(1, "fa"))),
                        new List<STTypeQual>(),
                        new STConstInt(new T_CONST_INT(1, "11", 10)),
                        false)
                },
                {
                    "y[]",
                    new STArrDeclarator(
                        new STIdDeclarator(new STId(new T_IDENTIFIER(1, "y"))),
                        new List<STTypeQual>(),
                        null,
                        false)
                },
                {
                    "a[n][6][m]",
                    new STArrDeclarator(
                        new STArrDeclarator(
                            new STArrDeclarator(
                                new STIdDeclarator(new STId(new T_IDENTIFIER(1, "a"))),
                                new List<STTypeQual>(),
                                new STId(new T_IDENTIFIER(1, "n")),
                                false),
                            new List<STTypeQual>(),
                            new STConstInt(new T_CONST_INT(1, "6", 10)),
                            false),
                        new List<STTypeQual>(),
                        new STId(new T_IDENTIFIER(1, "m")),
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
                        new List<STTypeSpecQual> {
                            new STTypeKeySpec(1, STTypeSpec.Kind.INT)
                        })
                },
                {
                    "int *",
                    new ASTTypeName(
                        new List<STTypeSpecQual> {
                            new STTypeKeySpec(1, STTypeSpec.Kind.INT)
                        },
                        new STAbsDeclarator(
                            new List<STPtr> {
                                new STPtr(1, new List<STTypeQual>())
                            },
                            null))
                },
                {
                    "int *[3]",
                    new ASTTypeName(
                        new List<STTypeSpecQual> {
                            new STTypeKeySpec(1, STTypeSpec.Kind.INT)
                        },
                        new STAbsDeclarator(
                            new List<STPtr> {
                                new STPtr(1, new List<STTypeQual>())
                            },
                            new STAbsArrDeclarator(
                                new STAbsDirDeclaratorNil(1),
                                new List<STTypeQual>(),
                                new STConstInt(new T_CONST_INT(1, "3", 10)),
                                false)))
                },
                {
                    "int (*)[3]",
                    new ASTTypeName(
                        new List<STTypeSpecQual> {
                            new STTypeKeySpec(1, STTypeSpec.Kind.INT)
                        },
                        new STAbsDeclarator(
                            new List<STPtr> {},
                            new STAbsArrDeclarator(
                                new STAbsParDeclarator(new STAbsDeclarator(
                                    new List<STPtr> {
                                        new STPtr(1, new List<STTypeQual>())
                                    },
                                    null)),
                                new List<STTypeQual>(),
                                new STConstInt(new T_CONST_INT(1, "3", 10)),
                                false)))
                },
                {
                    "int (*)[*]",
                    new ASTTypeName(
                        new List<STTypeSpecQual> {
                            new STTypeKeySpec(1, STTypeSpec.Kind.INT)
                        },
                        new STAbsDeclarator(
                            new List<STPtr> {},
                            new STAbsArrDeclarator(
                                new STAbsParDeclarator(new STAbsDeclarator(
                                    new List<STPtr> {
                                        new STPtr(1, new List<STTypeQual>())
                                    },
                                    null)))))
                },
                {
                    "int *[]",
                    new ASTTypeName(
                        new List<STTypeSpecQual> {
                            new STTypeKeySpec(1, STTypeSpec.Kind.INT)
                        },
                        new STAbsDeclarator(
                            new List<STPtr> {
                                new STPtr(1, new List<STTypeQual>())
                            },
                            new STAbsArrDeclarator(
                                new STAbsDirDeclaratorNil(1),
                                new List<STTypeQual>(),
                                null,
                                false)))
                },
                {
                    "int (*)(void)",
                    new ASTTypeName(
                        new List<STTypeSpecQual> {
                            new STTypeKeySpec(1, STTypeSpec.Kind.INT)
                        },
                        new STAbsDeclarator(
                            new List<STPtr> {},
                            new STAbsFuncDeclarator(
                                new STAbsParDeclarator(new STAbsDeclarator(
                                    new List<STPtr> {
                                        new STPtr(1, new List<STTypeQual>())
                                    },
                                    null)),
                                new List<STParam> {
                                    new STParam(
                                        new STDeclSpecs(
                                            new List<STDeclSpec> {
                                                new STTypeKeySpec(1, STTypeSpec.Kind.VOID)
                                            },
                                            STStoreSpec.Kind.NONE,
                                            new List<STTypeSpec.Kind> { STTypeSpec.Kind.VOID }))
                                },
                                false)))
                },
                {
                    "int (*const []) (unsigned int, ...)",
                    new ASTTypeName(
                        new List<STTypeSpecQual> {
                            new STTypeKeySpec(1, STTypeSpec.Kind.INT)
                        },
                        new STAbsDeclarator(
                            new List<STPtr> {},
                            new STAbsFuncDeclarator(
                                new STAbsParDeclarator(new STAbsDeclarator(
                                    new List<STPtr> {
                                        new STPtr(1, new List<STTypeQual> { new STTypeQual(1, STTypeQual.Kind.CONST) })
                                    },
                                    new STAbsArrDeclarator(
                                        new STAbsDirDeclaratorNil(1),
                                        new List<STTypeQual>(),
                                        null,
                                        false))),
                                new List<STParam> {
                                    new STParam(
                                        new STDeclSpecs(
                                            new List<STDeclSpec> {
                                                new STTypeKeySpec(1, STTypeSpec.Kind.UNSIGNED),
                                                new STTypeKeySpec(1, STTypeSpec.Kind.INT)
                                            },
                                            STStoreSpec.Kind.NONE,
                                            new List<STTypeSpec.Kind> {
                                                STTypeSpec.Kind.UNSIGNED,
                                                STTypeSpec.Kind.INT
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
            var dict = new Dictionary<string, STDeclaration> {
                {
                    @"
int foo(int a, int b, double c, ...);
",
                    new STDeclaration(
                        new STDeclSpecs(
                            new List<STDeclSpec> {
                                new STTypeKeySpec(2, STTypeSpec.Kind.INT)
                            },
                            STStoreSpec.Kind.NONE,
                            new List<STTypeSpec.Kind> { STTypeSpec.Kind.INT }),
                        new List<STInitDeclarator> {
                            new STInitDeclarator(
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STFuncDeclarator(
                                        new STIdDeclarator(new STId(new T_IDENTIFIER(2, "foo"))),
                                        new List<STParam> {
                                            new STParam(
                                                new STDeclSpecs(
                                                    new List<STDeclSpec> {
                                                        new STTypeKeySpec(2, STTypeSpec.Kind.INT)
                                                    },
                                                    STStoreSpec.Kind.NONE,
                                                    new List<STTypeSpec.Kind> { STTypeSpec.Kind.INT }),
                                                new STDeclarator(
                                                    new List<STPtr>(),
                                                    new STIdDeclarator(new STId(new T_IDENTIFIER(2, "a"))))),
                                            new STParam(
                                                new STDeclSpecs(
                                                    new List<STDeclSpec> {
                                                        new STTypeKeySpec(2, STTypeSpec.Kind.INT)
                                                    },
                                                    STStoreSpec.Kind.NONE,
                                                    new List<STTypeSpec.Kind> { STTypeSpec.Kind.INT }),
                                                new STDeclarator(
                                                    new List<STPtr>(),
                                                    new STIdDeclarator(new STId(new T_IDENTIFIER(2, "b"))))),
                                            new STParam(
                                                new STDeclSpecs(
                                                    new List<STDeclSpec> {
                                                        new STTypeKeySpec(2, STTypeSpec.Kind.DOUBLE)
                                                    },
                                                    STStoreSpec.Kind.NONE,
                                                    new List<STTypeSpec.Kind> { STTypeSpec.Kind.DOUBLE }),
                                                new STDeclarator(
                                                    new List<STPtr>(),
                                                    new STIdDeclarator(new STId(new T_IDENTIFIER(2, "c"))))),
                                        },
                                        true)),
                                null)
                        })
                },
                {
                    @"
int foo(a, b, c);
",
                    new STDeclaration(
                        new STDeclSpecs(
                            new List<STDeclSpec> {
                                new STTypeKeySpec(2, STTypeSpec.Kind.INT)
                            },
                            STStoreSpec.Kind.NONE,
                            new List<STTypeSpec.Kind> { STTypeSpec.Kind.INT }),
                        new List<STInitDeclarator> {
                            new STInitDeclarator(
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STFuncDeclarator(
                                        new STIdDeclarator(new STId(new T_IDENTIFIER(2, "foo"))),
                                        new List<STId> {
                                            new STId(new T_IDENTIFIER(2, "a")),
                                            new STId(new T_IDENTIFIER(2, "b")),
                                            new STId(new T_IDENTIFIER(2, "c")),
                                        })),
                                null)
                        })
                },
                {
                    @"
int foo();
",
                    new STDeclaration(
                        new STDeclSpecs(
                            new List<STDeclSpec> {
                                new STTypeKeySpec(2, STTypeSpec.Kind.INT)
                            },
                            STStoreSpec.Kind.NONE,
                            new List<STTypeSpec.Kind> { STTypeSpec.Kind.INT }),
                        new List<STInitDeclarator> {
                            new STInitDeclarator(
                                new STDeclarator(
                                    new List<STPtr>(),
                                    new STFuncDeclarator(
                                        new STIdDeclarator(new STId(new T_IDENTIFIER(2, "foo"))),
                                        new List<STId>())),
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
            var tests = new Dictionary<string, STInitializer> {
                {
                    "2 = 3",
                    new STInitializer(new STAssignExpr(
                        new STConstInt(new T_CONST_INT(1, "2", 10)),
                        new STConstInt(new T_CONST_INT(1, "3", 10)),
                        STAssignExpr.Op.ASSIGN))
                },
                {
                    "{ .what = {1, 2, 3 }, }",
                    new STInitializer(new List<STInitItem> {
                        new STInitItem(new STInitializer(new List<STInitItem> {
                            new STInitItem(new STInitializer(new STConstInt(new T_CONST_INT(1, "1", 10)))),
                            new STInitItem(new STInitializer(new STConstInt(new T_CONST_INT(1, "2", 10)))),
                            new STInitItem(new STInitializer(new STConstInt(new T_CONST_INT(1, "3", 10))))
                        }),
                        new List<STDesignator> {
                            new STDesignator(new STId(new T_IDENTIFIER(1, "what")))
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
            var tests = new Dictionary<string, IEnumerable<STDesignator>> {
                {
                    ".ref[3].w = ",
                    new List<STDesignator> {
                        new STDesignator(new STId(new T_IDENTIFIER(1, "ref"))),
                        new STDesignator(new STConstInt(new T_CONST_INT(1, "3", 10))),
                        new STDesignator(new STId(new T_IDENTIFIER(1, "w")))
                    }
                }
            };
            foreach (var test in tests) {
                Aux(test.Key, Parser.Designation().End().Select(x => x as IEnumerable<STDesignator>), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserTypedef() {
            Env.PushScope();
            Env.AddTypedefName(1, "a");
            var tests = new Dictionary<string, STParam> {
                {
                    "int a",
                    new STParam(
                        new STDeclSpecs(
                            new List<STDeclSpec> {
                                new STTypeKeySpec(1, STTypeSpec.Kind.INT)
                            },
                            STStoreSpec.Kind.NONE,
                            new List<STTypeSpec.Kind> { STTypeSpec.Kind.INT }),
                        new STDeclarator(
                            new List<STPtr>(),
                            new STIdDeclarator(new STId(new T_IDENTIFIER(1, "a")))))
                },
                {
                    "const a",
                    new STParam(
                        new STDeclSpecs(
                            new List<STDeclSpec> {
                                new STTypeQual(1, STTypeQual.Kind.CONST),
                                new STTypedefName(new STId(new T_IDENTIFIER(1, "a")))
                            },
                            STStoreSpec.Kind.NONE,
                            new STTypedefName(new STId(new T_IDENTIFIER(1, "a")))))
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
            var tests = new Dictionary<string, STDeclaration> {
                {
                    "typedef int a;",
                    new STDeclaration(
                        new STDeclSpecs(
                            new List<STDeclSpec> {
                                new STTypeKeySpec(1, STTypeSpec.Kind.INT)
                            },
                            STStoreSpec.Kind.NONE,
                            new List<STTypeSpec.Kind> { STTypeSpec.Kind.INT }),
                        new List<STInitDeclarator> {
                            new STInitDeclarator(
                                new STDeclarator(new List<STPtr>(),
                                new STIdDeclarator(new STId(new T_IDENTIFIER(1, "a")))))
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
