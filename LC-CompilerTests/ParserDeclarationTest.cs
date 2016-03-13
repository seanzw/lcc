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
            Dictionary<string, ASTDeclStroageSpec> dict = new Dictionary<string, ASTDeclStroageSpec> {
                {
                    "typedef",
                    new ASTDeclStroageSpec(1, ASTDeclStroageSpec.Type.TYPEDEF)
                },
                {
                    "extern",
                    new ASTDeclStroageSpec(1, ASTDeclStroageSpec.Type.EXTERN)
                },
                {
                    "static",
                    new ASTDeclStroageSpec(1, ASTDeclStroageSpec.Type.STATIC)
                },
                {
                    "auto",
                    new ASTDeclStroageSpec(1, ASTDeclStroageSpec.Type.AUTO)
                },
                {
                    "register",
                    new ASTDeclStroageSpec(1, ASTDeclStroageSpec.Type.REGISTER)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.StorageClassSpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserTypeKeySpecifier() {
            Dictionary<string, ASTDeclTypeKeySpec> dict = new Dictionary<string, ASTDeclTypeKeySpec> {
                {
                    "void",
                    new ASTDeclTypeKeySpec(1, ASTDeclTypeKeySpec.Type.VOID)
                },
                {
                    "char",
                    new ASTDeclTypeKeySpec(1, ASTDeclTypeKeySpec.Type.CHAR)
                },
                {
                    "short",
                    new ASTDeclTypeKeySpec(1, ASTDeclTypeKeySpec.Type.SHORT)
                },
                {
                    "int",
                    new ASTDeclTypeKeySpec(1, ASTDeclTypeKeySpec.Type.INT)
                },
                {
                    "long",
                    new ASTDeclTypeKeySpec(1, ASTDeclTypeKeySpec.Type.LONG)
                },
                {
                    "float",
                    new ASTDeclTypeKeySpec(1, ASTDeclTypeKeySpec.Type.FLOAT)
                },
                {
                    "double",
                    new ASTDeclTypeKeySpec(1, ASTDeclTypeKeySpec.Type.DOUBLE)
                },
                {
                    "unsigned",
                    new ASTDeclTypeKeySpec(1, ASTDeclTypeKeySpec.Type.UNSIGNED)
                },
                {
                    "signed",
                    new ASTDeclTypeKeySpec(1, ASTDeclTypeKeySpec.Type.SIGNED)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.TypeKeySpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserTypeQualifier() {
            Dictionary<string, ASTDeclTypeQual> dict = new Dictionary<string, ASTDeclTypeQual> {
                {
                    "const",
                    new ASTDeclTypeQual(1, ASTDeclTypeQual.Type.CONST)
                },
                {
                    "restrict",
                    new ASTDeclTypeQual(1, ASTDeclTypeQual.Type.RESTRICT)
                },
                {
                    "volatile",
                    new ASTDeclTypeQual(1, ASTDeclTypeQual.Type.VOLATILE)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.TypeQualifier().End(), test.Value);
            }
        }


        [TestMethod]
        public void LCCParserFunctionSpecifier() {
            Dictionary<string, ASTDeclFuncSpec> dict = new Dictionary<string, ASTDeclFuncSpec> {
                {
                    "inline",
                    new ASTDeclFuncSpec(1, ASTDeclFuncSpec.Type.INLINE)
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.FunctionSpecifier().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserDeclarationSpecifiers() {
            Dictionary<string, LinkedList<ASTDeclSpec>> dict = new Dictionary<string, LinkedList<ASTDeclSpec>> {
                {
                    "static const int char inline",
                    new LinkedList<ASTDeclSpec>(new List<ASTDeclSpec> {
                        new ASTDeclStroageSpec(1, ASTDeclStroageSpec.Type.STATIC),
                        new ASTDeclTypeQual(1, ASTDeclTypeQual.Type.CONST),
                        new ASTDeclTypeKeySpec(1, ASTDeclTypeKeySpec.Type.INT),
                        new ASTDeclTypeKeySpec(1, ASTDeclTypeKeySpec.Type.CHAR),
                        new ASTDeclFuncSpec(1, ASTDeclFuncSpec.Type.INLINE)
                    })
                }
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.DeclarationSpecifiers().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserEnumerator() {
            Dictionary<string, ASTDeclEnumerator> dict = new Dictionary<string, ASTDeclEnumerator> {
                {
                    "ZERO",
                    new ASTDeclEnumerator(new ASTIdentifier(new T_IDENTIFIER(1, "ZERO")))
                },
                {
                    "ZERO = 1",
                    new ASTDeclEnumerator(
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
            Dictionary<string, LinkedList<ASTDeclEnumerator>> dict = new Dictionary<string, LinkedList<ASTDeclEnumerator>> {
                {
                    "ZERO, ONE = 1, TWO",
                    new LinkedList<ASTDeclEnumerator>(new List<ASTDeclEnumerator> {
                        new ASTDeclEnumerator(new ASTIdentifier(new T_IDENTIFIER(1, "ZERO"))),
                        new ASTDeclEnumerator(
                            new ASTIdentifier(new T_IDENTIFIER(1, "ONE")),
                            new ASTConstInt(new T_CONST_INT(1, "1", 10))),
                        new ASTDeclEnumerator(new ASTIdentifier(new T_IDENTIFIER(1, "TWO"))),
                    })
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.EnumeratorList().End(), test.Value);
            }
        }

        [TestMethod]
        public void LCCParserEnumSpecifier() {
            Dictionary<string, ASTDeclEnumSpec> dict = new Dictionary<string, ASTDeclEnumSpec> {
                {
                    @"
enum Foo {
    ZERO,
    ONE = 1,
    TWO
}",
                    new ASTDeclEnumSpec(
                        new ASTIdentifier(new T_IDENTIFIER(2, "Foo")),
                        new LinkedList<ASTDeclEnumerator>(new List<ASTDeclEnumerator> {
                            new ASTDeclEnumerator(new ASTIdentifier(new T_IDENTIFIER(3, "ZERO"))),
                            new ASTDeclEnumerator(
                                new ASTIdentifier(new T_IDENTIFIER(4, "ONE")),
                                new ASTConstInt(new T_CONST_INT(4, "1", 10))),
                            new ASTDeclEnumerator(new ASTIdentifier(new T_IDENTIFIER(5, "TWO"))),
                        }))
                },
                {
                    @"
enum {
    ZERO,
    ONE = 1,
    TWO
}",
                    new ASTDeclEnumSpec(
                        new LinkedList<ASTDeclEnumerator>(new List<ASTDeclEnumerator> {
                            new ASTDeclEnumerator(new ASTIdentifier(new T_IDENTIFIER(3, "ZERO"))),
                            new ASTDeclEnumerator(
                                new ASTIdentifier(new T_IDENTIFIER(4, "ONE")),
                                new ASTConstInt(new T_CONST_INT(4, "1", 10))),
                            new ASTDeclEnumerator(new ASTIdentifier(new T_IDENTIFIER(5, "TWO"))),
                        }))
                },
                {
                    "enum test",
                    new ASTDeclEnumSpec(new ASTIdentifier(new T_IDENTIFIER(1, "test")))
                },
            };

            foreach (var test in dict) {
                Aux(test.Key, Parser.EnumSpecifier().End(), test.Value);
            }
        }
    }
}
