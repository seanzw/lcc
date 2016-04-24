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

        private DeclSpecs ProcessSS(IEnumerable<DeclSpec> ss) {
            // At most one storage-class specifier may be given in the declaration specifiers.
            var stors = ss.OfType<STStoreSpec>();
            STStoreSpec.Kind storage = STStoreSpec.Kind.NONE;
            if (stors.Count() > 1) return null;
            else if (stors.Count() == 1) storage = stors.First().kind;

            // At most one function specifier.
            var funcs = ss.OfType<STFuncSpec>();
            STFuncSpec.Kind function = STFuncSpec.Kind.NONE;
            if (funcs.Count() > 1) return null;
            else if (funcs.Count() == 1) function = funcs.First().kind;

            // At least one type specifier shall be given in the declaration specifiers.
            var specs = ss.OfType<TypeSpec>();
            if (specs.Count() == 0) return null;

            foreach (var spec in specs) {
                if (spec.kind == TypeSpec.Kind.TYPEDEF ||
                    spec.kind == TypeSpec.Kind.STRUCT ||
                    spec.kind == TypeSpec.Kind.UNION ||
                    spec.kind == TypeSpec.Kind.ENUM)
                    if (specs.Count() != 1) return null;
                    // User-defined types.
                    else return new DeclSpecs(ss, storage, spec as TypeUserSpec, function);
            }

            // Built-in type.
            var keys = from s in specs select s.kind;
            return new DeclSpecs(ss, storage, keys, function);
        }


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
            Dictionary<string, TypeQual> dict = new Dictionary<string, TypeQual> {
                {
                    "const",
                    new TypeQual(1, TypeQual.Kind.CONST)
                },
                {
                    "restrict",
                    new TypeQual(1, TypeQual.Kind.RESTRICT)
                },
                {
                    "volatile",
                    new TypeQual(1, TypeQual.Kind.VOLATILE)
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
            var dict = new Dictionary<string, DeclSpecs> {
                {
                    "static const int char inline",
                    new DeclSpecs(
                        new List<DeclSpec> {
                            new STStoreSpec(1, STStoreSpec.Kind.STATIC),
                            new TypeQual(1, TypeQual.Kind.CONST),
                            new STTypeKeySpec(1, TypeSpec.Kind.INT),
                            new STTypeKeySpec(1, TypeSpec.Kind.CHAR),
                            new STFuncSpec(1, STFuncSpec.Kind.INLINE)
                        },
                        STStoreSpec.Kind.STATIC,
                        new List<TypeSpec.Kind> {
                            TypeSpec.Kind.INT,
                            TypeSpec.Kind.CHAR
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
                    new STEnum(new Id(new T_IDENTIFIER(1, "ZERO")), null)
                },
                {
                    "ZERO = 1",
                    new STEnum(
                        new Id(new T_IDENTIFIER(1, "ZERO")),
                        new ConstInt(new T_CONST_INT(1, "1", 10))
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
                        new STEnum(new Id(new T_IDENTIFIER(1, "ZERO")), null),
                        new STEnum(
                            new Id(new T_IDENTIFIER(1, "ONE")),
                            new ConstInt(new T_CONST_INT(1, "1", 10))),
                        new STEnum(new Id(new T_IDENTIFIER(1, "TWO")), null),
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
                        new Id(new T_IDENTIFIER(2, "Foo")),
                        new LinkedList<STEnum>(new List<STEnum> {
                            new STEnum(new Id(new T_IDENTIFIER(3, "ZERO")), null),
                            new STEnum(
                                new Id(new T_IDENTIFIER(4, "ONE")),
                                new ConstInt(new T_CONST_INT(4, "1", 10))),
                            new STEnum(new Id(new T_IDENTIFIER(5, "TWO")), null),
                        }))
                },
                {
                    @"
enum what
",
                    new STEnumSpec(
                        2,
                        new Id(new T_IDENTIFIER(2, "what")),
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
                            new STEnum(new Id(new T_IDENTIFIER(3, "ZERO")), null),
                            new STEnum(
                                new Id(new T_IDENTIFIER(4, "ONE")),
                                new ConstInt(new T_CONST_INT(4, "1", 10))),
                            new STEnum(new Id(new T_IDENTIFIER(5, "TWO")), null),
                        }))
                },
                {
                    "enum test",
                    new STEnumSpec(1, new Id(new T_IDENTIFIER(1, "test")), null)
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.EnumSpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDirectDeclarator() {
            var dict = new Dictionary<string, DirDeclarator> {
                {
                    "X",
                    new IdDeclarator(new Id(new T_IDENTIFIER(1, "X")))
                },
                {
                    "( X )",
                    new ParDeclarator(
                        new Declarator(
                            new List<Ptr>(),
                            new IdDeclarator(new Id(new T_IDENTIFIER(1, "X")))))
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.DirectDeclarator().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDeclarator() {
            var dict = new Dictionary<string, DirDeclarator> {
                {
                    "X",
                    new IdDeclarator(new Id(new T_IDENTIFIER(1, "X")))
                },
                {
                    "( X )",
                    new ParDeclarator(
                        new Declarator(
                            new List<Ptr>(),
                            new IdDeclarator(new Id(new T_IDENTIFIER(1, "X")))))
                },
                {
                    @"
foo(int a, int b, double c, ...)
",
                    new FuncDeclarator(
                        new IdDeclarator(new Id(new T_IDENTIFIER(2, "foo"))),
                        new List<STParam> {
                            new STParam(
                                new DeclSpecs(
                                    new List<DeclSpec> {
                                        new STTypeKeySpec(2, TypeSpec.Kind.INT)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<TypeSpec.Kind> { TypeSpec.Kind.INT }),
                                new Declarator(
                                    new List<Ptr>(),
                                    new IdDeclarator(new Id(new T_IDENTIFIER(2, "a"))))),
                            new STParam(
                                new DeclSpecs(
                                    new List<DeclSpec> {
                                        new STTypeKeySpec(2, TypeSpec.Kind.INT)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<TypeSpec.Kind> { TypeSpec.Kind.INT }),
                                new Declarator(
                                    new List<Ptr>(),
                                    new IdDeclarator(new Id(new T_IDENTIFIER(2, "b"))))),
                            new STParam(
                                new DeclSpecs(
                                    new List<DeclSpec> {
                                        new STTypeKeySpec(2, TypeSpec.Kind.DOUBLE)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<TypeSpec.Kind> { TypeSpec.Kind.DOUBLE }),
                                new Declarator(
                                    new List<Ptr>(),
                                    new IdDeclarator(new Id(new T_IDENTIFIER(2, "c"))))),
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
            var dict = new Dictionary<string, StructDeclarator> {
                {
                    "X",
                    new StructDeclarator(
                        new Declarator(
                            new List<Ptr>(),
                            new IdDeclarator(
                                new Id(new T_IDENTIFIER(1, "X")))),
                        null)
                },
                {
                    "( X ) : 4",
                    new StructDeclarator(
                        new Declarator(
                            new List<Ptr>(),
                            new ParDeclarator(
                                new Declarator(
                                    new List<Ptr>(),
                                    new IdDeclarator(new Id(new T_IDENTIFIER(1, "X")))))),
                        new ConstInt(new T_CONST_INT(1, "4", 10)))
                },
                {
                    ": 4",
                    new StructDeclarator(
                        null,
                        new ConstInt(new T_CONST_INT(1, "4", 10)))
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.StructDeclarator().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserStructDeclaratorList() {
            var dict = new Dictionary<string, IEnumerable<StructDeclarator>> {
                {
                    "X, ( X ) : 4, : 4",
                    new List<StructDeclarator> {
                        new StructDeclarator(
                            new Declarator(
                                new List<Ptr>(),
                                new IdDeclarator(
                                    new Id(new T_IDENTIFIER(1, "X")))),
                            null),
                        new StructDeclarator(
                            new Declarator(
                                new List<Ptr>(),
                                new ParDeclarator(new Declarator(
                                    new List<Ptr>(),
                                    new IdDeclarator(new Id(new T_IDENTIFIER(1, "X")))))),
                            new ConstInt(new T_CONST_INT(1, "4", 10))),
                        new StructDeclarator(
                            null,
                            new ConstInt(new T_CONST_INT(1, "4", 10))),
                    }
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.StructDeclaratorList().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserSpecifierQualifierList() {
            var dict = new Dictionary<string, DeclSpecs> {
                {
                    "const enum what",
                    ProcessSS(
                        new List<TypeSpecQual> {
                            new TypeQual(1, TypeQual.Kind.CONST),
                            new STEnumSpec(1, new Id(new T_IDENTIFIER(1, "what")), null)
                        })
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.SpecifierQualifierList().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserStructDeclaration() {
            var dict = new Dictionary<string, StructDeclaration> {
                {
                    "const enum what x, y, z;",
                    new StructDeclaration(
                        ProcessSS(
                            new List<TypeSpecQual> {
                                new TypeQual(1, TypeQual.Kind.CONST),
                                new STEnumSpec(1, new Id(new T_IDENTIFIER(1, "what")), null)
                            }),
                        new List<StructDeclarator> {
                            new StructDeclarator(
                                new Declarator(
                                    new List<Ptr>(),
                                    new IdDeclarator(
                                        new Id(new T_IDENTIFIER(1, "x")))),
                                    null),
                            new StructDeclarator(
                                new Declarator(
                                    new List<Ptr>(),
                                    new IdDeclarator(
                                        new Id(new T_IDENTIFIER(1, "y")))),
                                null),
                            new StructDeclarator(
                                new Declarator(
                                    new List<Ptr>(),
                                    new IdDeclarator(
                                        new Id(new T_IDENTIFIER(1, "z")))),
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
            var dict = new Dictionary<string, StructUnionSpec> {
                {
                    @"
struct ss {
    int n;
    const enum what x, y, z;
}",
                    new StructUnionSpec(
                        2,
                        new Id(new T_IDENTIFIER(2, "ss")),
                        new List<StructDeclaration> {
                            new StructDeclaration(
                                ProcessSS(new List<TypeSpecQual> {
                                    new STTypeKeySpec(3, TypeSpec.Kind.INT)
                                }),
                                new List<StructDeclarator> {
                                    new StructDeclarator(
                                        new Declarator(
                                            new List<Ptr>(),
                                            new IdDeclarator(
                                                new Id(new T_IDENTIFIER(3, "n")))),
                                        null)
                                }),
                            new StructDeclaration(
                                ProcessSS(new List<TypeSpecQual> {
                                    new TypeQual(4, TypeQual.Kind.CONST),
                                    new STEnumSpec(4, new Id(new T_IDENTIFIER(4, "what")), null)
                                }),
                                new List<StructDeclarator> {
                                    new StructDeclarator(
                                        new Declarator(
                                            new List<Ptr>(),
                                            new IdDeclarator(new Id(new T_IDENTIFIER(4, "x")))),
                                        null),
                                    new StructDeclarator(
                                        new Declarator(
                                            new List<Ptr>(),
                                            new IdDeclarator(new Id(new T_IDENTIFIER(4, "y")))),
                                        null),
                                    new StructDeclarator(
                                        new Declarator(
                                            new List<Ptr>(),
                                            new IdDeclarator(new Id(new T_IDENTIFIER(4, "z")))),
                                        null),
                                })
                        },
                        TypeSpec.Kind.STRUCT)
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
                        new DeclSpecs(
                            new List<DeclSpec> {
                                new STTypeKeySpec(1, TypeSpec.Kind.INT)
                            },
                            STStoreSpec.Kind.NONE,
                            new List<TypeSpec.Kind> { TypeSpec.Kind.INT }),
                        new Declarator(
                            new List<Ptr>(),
                            new IdDeclarator(new Id(new T_IDENTIFIER(1, "a")))))
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
                                new DeclSpecs(
                                    new List<DeclSpec> {
                                        new STTypeKeySpec(1, TypeSpec.Kind.INT)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<TypeSpec.Kind> { TypeSpec.Kind.INT }),
                                new Declarator(
                                    new List<Ptr>(),
                                    new IdDeclarator(new Id(new T_IDENTIFIER(1, "a"))))),
                            new STParam(
                                new DeclSpecs(
                                    new List<DeclSpec> {
                                        new STTypeKeySpec(1, TypeSpec.Kind.INT)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<TypeSpec.Kind> { TypeSpec.Kind.INT }),
                                new Declarator(
                                    new List<Ptr>(),
                                    new IdDeclarator(new Id(new T_IDENTIFIER(1, "b"))))),
                            new STParam(
                                new DeclSpecs(
                                    new List<DeclSpec> {
                                        new STTypeKeySpec(1, TypeSpec.Kind.DOUBLE)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<TypeSpec.Kind> { TypeSpec.Kind.DOUBLE }),
                                new Declarator(
                                    new List<Ptr>(),
                                    new IdDeclarator(new Id(new T_IDENTIFIER(1, "c"))))),
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
                                new DeclSpecs(
                                    new List<DeclSpec> {
                                        new STTypeKeySpec(2, TypeSpec.Kind.INT)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<TypeSpec.Kind> { TypeSpec.Kind.INT }),
                                new Declarator(
                                    new List<Ptr>(),
                                    new IdDeclarator(new Id(new T_IDENTIFIER(2, "a"))))),
                            new STParam(
                                new DeclSpecs(
                                    new List<DeclSpec> {
                                        new STTypeKeySpec(2, TypeSpec.Kind.INT)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<TypeSpec.Kind> { TypeSpec.Kind.INT }),
                                new Declarator(
                                    new List<Ptr>(),
                                    new IdDeclarator(new Id(new T_IDENTIFIER(2, "b"))))),
                            new STParam(
                                new DeclSpecs(
                                    new List<DeclSpec> {
                                        new STTypeKeySpec(2, TypeSpec.Kind.DOUBLE)
                                    },
                                    STStoreSpec.Kind.NONE,
                                    new List<TypeSpec.Kind> { TypeSpec.Kind.DOUBLE }),
                                new Declarator(
                                    new List<Ptr>(),
                                    new IdDeclarator(new Id(new T_IDENTIFIER(2, "c"))))),
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
            var tests = new Dictionary<string, ArrDeclarator> {
                {
                    "fa[11]",
                    new ArrDeclarator(
                        new IdDeclarator(new Id(new T_IDENTIFIER(1, "fa"))),
                        new List<TypeQual>(),
                        new ConstInt(new T_CONST_INT(1, "11", 10)),
                        false)
                },
                {
                    "y[]",
                    new ArrDeclarator(
                        new IdDeclarator(new Id(new T_IDENTIFIER(1, "y"))),
                        new List<TypeQual>(),
                        null,
                        false)
                },
                {
                    "a[n][6][m]",
                    new ArrDeclarator(
                        new ArrDeclarator(
                            new ArrDeclarator(
                                new IdDeclarator(new Id(new T_IDENTIFIER(1, "a"))),
                                new List<TypeQual>(),
                                new Id(new T_IDENTIFIER(1, "n")),
                                false),
                            new List<TypeQual>(),
                            new ConstInt(new T_CONST_INT(1, "6", 10)),
                            false),
                        new List<TypeQual>(),
                        new Id(new T_IDENTIFIER(1, "m")),
                        false)
                }
            };

            foreach (var test in tests) {
                Aux(test.Key, Parser.DirectDeclarator().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserTypeName() {
            var tests = new Dictionary<string, TypeName> {
                {
                    "int",
                    new TypeName(
                        ProcessSS(new List<TypeSpecQual> {
                            new STTypeKeySpec(1, TypeSpec.Kind.INT)
                        }))
                },
                {
                    "int *",
                    new TypeName(
                        ProcessSS(new List<TypeSpecQual> {
                            new STTypeKeySpec(1, TypeSpec.Kind.INT)
                        }),
                        new AbsDeclarator(
                            new List<Ptr> {
                                new Ptr(1, new List<TypeQual>())
                            },
                            null))
                },
                {
                    "int *[3]",
                    new TypeName(
                        ProcessSS(new List<TypeSpecQual> {
                            new STTypeKeySpec(1, TypeSpec.Kind.INT)
                        }),
                        new AbsDeclarator(
                            new List<Ptr> {
                                new Ptr(1, new List<TypeQual>())
                            },
                            new STAbsArrDeclarator(
                                new STAbsDirDeclaratorNil(1),
                                new List<TypeQual>(),
                                new ConstInt(new T_CONST_INT(1, "3", 10)),
                                false)))
                },
                {
                    "int (*)[3]",
                    new TypeName(
                        ProcessSS(new List<TypeSpecQual> {
                            new STTypeKeySpec(1, TypeSpec.Kind.INT)
                        }),
                        new AbsDeclarator(
                            new List<Ptr> {},
                            new STAbsArrDeclarator(
                                new STAbsParDeclarator(new AbsDeclarator(
                                    new List<Ptr> {
                                        new Ptr(1, new List<TypeQual>())
                                    },
                                    null)),
                                new List<TypeQual>(),
                                new ConstInt(new T_CONST_INT(1, "3", 10)),
                                false)))
                },
                {
                    "int (*)[*]",
                    new TypeName(
                        ProcessSS(new List<TypeSpecQual> {
                            new STTypeKeySpec(1, TypeSpec.Kind.INT)
                        }),
                        new AbsDeclarator(
                            new List<Ptr> {},
                            new STAbsArrDeclarator(
                                new STAbsParDeclarator(new AbsDeclarator(
                                    new List<Ptr> {
                                        new Ptr(1, new List<TypeQual>())
                                    },
                                    null)))))
                },
                {
                    "int *[]",
                    new TypeName(
                        ProcessSS(new List<TypeSpecQual> {
                            new STTypeKeySpec(1, TypeSpec.Kind.INT)
                        }),
                        new AbsDeclarator(
                            new List<Ptr> {
                                new Ptr(1, new List<TypeQual>())
                            },
                            new STAbsArrDeclarator(
                                new STAbsDirDeclaratorNil(1),
                                new List<TypeQual>(),
                                null,
                                false)))
                },
                {
                    "int (*)(void)",
                    new TypeName(
                        ProcessSS(new List<TypeSpecQual> {
                            new STTypeKeySpec(1, TypeSpec.Kind.INT)
                        }),
                        new AbsDeclarator(
                            new List<Ptr> {},
                            new STAbsFuncDeclarator(
                                new STAbsParDeclarator(new AbsDeclarator(
                                    new List<Ptr> {
                                        new Ptr(1, new List<TypeQual>())
                                    },
                                    null)),
                                new List<STParam> {
                                    new STParam(
                                        new DeclSpecs(
                                            new List<DeclSpec> {
                                                new STTypeKeySpec(1, TypeSpec.Kind.VOID)
                                            },
                                            STStoreSpec.Kind.NONE,
                                            new List<TypeSpec.Kind> { TypeSpec.Kind.VOID }))
                                },
                                false)))
                },
                {
                    "int (*const []) (unsigned int, ...)",
                    new TypeName(
                        ProcessSS(new List<TypeSpecQual> {
                            new STTypeKeySpec(1, TypeSpec.Kind.INT)
                        }),
                        new AbsDeclarator(
                            new List<Ptr> {},
                            new STAbsFuncDeclarator(
                                new STAbsParDeclarator(new AbsDeclarator(
                                    new List<Ptr> {
                                        new Ptr(1, new List<TypeQual> { new TypeQual(1, TypeQual.Kind.CONST) })
                                    },
                                    new STAbsArrDeclarator(
                                        new STAbsDirDeclaratorNil(1),
                                        new List<TypeQual>(),
                                        null,
                                        false))),
                                new List<STParam> {
                                    new STParam(
                                        new DeclSpecs(
                                            new List<DeclSpec> {
                                                new STTypeKeySpec(1, TypeSpec.Kind.UNSIGNED),
                                                new STTypeKeySpec(1, TypeSpec.Kind.INT)
                                            },
                                            STStoreSpec.Kind.NONE,
                                            new List<TypeSpec.Kind> {
                                                TypeSpec.Kind.UNSIGNED,
                                                TypeSpec.Kind.INT
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
                        new DeclSpecs(
                            new List<DeclSpec> {
                                new STTypeKeySpec(2, TypeSpec.Kind.INT)
                            },
                            STStoreSpec.Kind.NONE,
                            new List<TypeSpec.Kind> { TypeSpec.Kind.INT }),
                        new List<STInitDeclarator> {
                            new STInitDeclarator(
                                new Declarator(
                                    new List<Ptr>(),
                                    new FuncDeclarator(
                                        new IdDeclarator(new Id(new T_IDENTIFIER(2, "foo"))),
                                        new List<STParam> {
                                            new STParam(
                                                new DeclSpecs(
                                                    new List<DeclSpec> {
                                                        new STTypeKeySpec(2, TypeSpec.Kind.INT)
                                                    },
                                                    STStoreSpec.Kind.NONE,
                                                    new List<TypeSpec.Kind> { TypeSpec.Kind.INT }),
                                                new Declarator(
                                                    new List<Ptr>(),
                                                    new IdDeclarator(new Id(new T_IDENTIFIER(2, "a"))))),
                                            new STParam(
                                                new DeclSpecs(
                                                    new List<DeclSpec> {
                                                        new STTypeKeySpec(2, TypeSpec.Kind.INT)
                                                    },
                                                    STStoreSpec.Kind.NONE,
                                                    new List<TypeSpec.Kind> { TypeSpec.Kind.INT }),
                                                new Declarator(
                                                    new List<Ptr>(),
                                                    new IdDeclarator(new Id(new T_IDENTIFIER(2, "b"))))),
                                            new STParam(
                                                new DeclSpecs(
                                                    new List<DeclSpec> {
                                                        new STTypeKeySpec(2, TypeSpec.Kind.DOUBLE)
                                                    },
                                                    STStoreSpec.Kind.NONE,
                                                    new List<TypeSpec.Kind> { TypeSpec.Kind.DOUBLE }),
                                                new Declarator(
                                                    new List<Ptr>(),
                                                    new IdDeclarator(new Id(new T_IDENTIFIER(2, "c"))))),
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
                        new DeclSpecs(
                            new List<DeclSpec> {
                                new STTypeKeySpec(2, TypeSpec.Kind.INT)
                            },
                            STStoreSpec.Kind.NONE,
                            new List<TypeSpec.Kind> { TypeSpec.Kind.INT }),
                        new List<STInitDeclarator> {
                            new STInitDeclarator(
                                new Declarator(
                                    new List<Ptr>(),
                                    new FuncDeclarator(
                                        new IdDeclarator(new Id(new T_IDENTIFIER(2, "foo"))),
                                        new List<Id> {
                                            new Id(new T_IDENTIFIER(2, "a")),
                                            new Id(new T_IDENTIFIER(2, "b")),
                                            new Id(new T_IDENTIFIER(2, "c")),
                                        })),
                                null)
                        })
                },
                {
                    @"
int foo();
",
                    new STDeclaration(
                        new DeclSpecs(
                            new List<DeclSpec> {
                                new STTypeKeySpec(2, TypeSpec.Kind.INT)
                            },
                            STStoreSpec.Kind.NONE,
                            new List<TypeSpec.Kind> { TypeSpec.Kind.INT }),
                        new List<STInitDeclarator> {
                            new STInitDeclarator(
                                new Declarator(
                                    new List<Ptr>(),
                                    new FuncDeclarator(
                                        new IdDeclarator(new Id(new T_IDENTIFIER(2, "foo"))),
                                        new List<Id>())),
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
                        new ConstInt(new T_CONST_INT(1, "2", 10)),
                        new ConstInt(new T_CONST_INT(1, "3", 10)),
                        STAssignExpr.Op.ASSIGN))
                },
                {
                    "{ .what = {1, 2, 3 }, }",
                    new STInitializer(new List<STInitItem> {
                        new STInitItem(new STInitializer(new List<STInitItem> {
                            new STInitItem(new STInitializer(new ConstInt(new T_CONST_INT(1, "1", 10)))),
                            new STInitItem(new STInitializer(new ConstInt(new T_CONST_INT(1, "2", 10)))),
                            new STInitItem(new STInitializer(new ConstInt(new T_CONST_INT(1, "3", 10))))
                        }),
                        new List<STDesignator> {
                            new STDesignator(new Id(new T_IDENTIFIER(1, "what")))
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
                        new STDesignator(new Id(new T_IDENTIFIER(1, "ref"))),
                        new STDesignator(new ConstInt(new T_CONST_INT(1, "3", 10))),
                        new STDesignator(new Id(new T_IDENTIFIER(1, "w")))
                    }
                }
            };
            foreach (var test in tests) {
                Aux(test.Key, Parser.Designation().End().Select(x => x as IEnumerable<STDesignator>), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserTypedef() {
            lcc.Parser.Env.PushScope();
            lcc.Parser.Env.AddTypedefName(1, "a");
            var tests = new Dictionary<string, STParam> {
                {
                    "int a",
                    new STParam(
                        new DeclSpecs(
                            new List<DeclSpec> {
                                new STTypeKeySpec(1, TypeSpec.Kind.INT)
                            },
                            STStoreSpec.Kind.NONE,
                            new List<TypeSpec.Kind> { TypeSpec.Kind.INT }),
                        new Declarator(
                            new List<Ptr>(),
                            new IdDeclarator(new Id(new T_IDENTIFIER(1, "a")))))
                },
                {
                    "const a",
                    new STParam(
                        new DeclSpecs(
                            new List<DeclSpec> {
                                new TypeQual(1, TypeQual.Kind.CONST),
                                new STTypedefName(new Id(new T_IDENTIFIER(1, "a")))
                            },
                            STStoreSpec.Kind.NONE,
                            new STTypedefName(new Id(new T_IDENTIFIER(1, "a")))))
                }
            };
            foreach (var test in tests) {
                Aux(test.Key, Parser.ParameterDeclaration().End(), test.Value);
            }
            lcc.Parser.Env.PopScope();
        }

        [TestMethod]
        [ExpectedException(typeof(TypedefRedefined), "Redefine typedef name.")]
        public void LCCParserTypeRedefined() {
            lcc.Parser.Env.PushScope();
            lcc.Parser.Env.AddTypedefName(1, "a");
            var tests = new Dictionary<string, STDeclaration> {
                {
                    "typedef int a;",
                    new STDeclaration(
                        new DeclSpecs(
                            new List<DeclSpec> {
                                new STTypeKeySpec(1, TypeSpec.Kind.INT)
                            },
                            STStoreSpec.Kind.NONE,
                            new List<TypeSpec.Kind> { TypeSpec.Kind.INT }),
                        new List<STInitDeclarator> {
                            new STInitDeclarator(
                                new Declarator(new List<Ptr>(),
                                new IdDeclarator(new Id(new T_IDENTIFIER(1, "a")))))
                        })
                }
            };
            foreach (var test in tests) {
                Aux(test.Key, Parser.Declaration().End(), test.Value);
            }
            lcc.Parser.Env.PopScope();

        }
    }
}
