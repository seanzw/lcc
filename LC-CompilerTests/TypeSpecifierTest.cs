using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.AST;
using lcc.Token;
using lcc.Parser;
using LLexer;
using Parserc;

namespace LC_CompilerTests {
    [TestClass]
    public class TypeSpecifierTest {
        [TestMethod]
        public void LCCGetType() {

            var tests = new Dictionary<string, string> {
                {
                    "void",
                    "void "
                },
                {
                    "char",
                    "char "
                },
                {
                    "char signed ",
                    "signed char "
                },
                {
                    "unsigned char ",
                    "unsigned char "
                },
                {
                    "signed int short",
                    "short "
                },
                {
                    "short unsigned ",
                    "unsigned short "
                },
                {
                    "signed int ",
                    "int "
                },
                {
                    "unsigned ",
                    "unsigned int "
                },
                {
                    "int long",
                    "long "
                },
                {
                    "long unsigned ",
                    "unsigned long "
                },
                {
                    "long long int",
                    "long long "
                },
                {
                    "unsigned long long ",
                    "unsigned long long "
                },
                {
                    "float ",
                    "float "
                },
                {
                    "double ",
                    "double "
                },
                {
                    "_Bool ",
                    "_Bool "
                },
                {
                    "long long long",
                    "TypeError: Unimplemented Type!"
                },
                {
                    "float double ",
                    "TypeError: Unimplemented Type!"
                }
            };

            foreach (var test in tests) {
                var result = Utility.parse(test.Key, Parser.TypeSpecifier().Plus().End());

                Assert.AreEqual(1, result.Count);
                Assert.IsFalse(result[0].remain.More());

                var type = lcc.Type.Utility.GetType(result[0].value, false);
                Assert.AreEqual(type.ToString(), test.Value);
            }
        }
    }
}
