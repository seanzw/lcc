using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.Type;

namespace LC_CompilerTests {
    [TestClass]
    public class TypeToStringTest {
        [TestMethod]
        public void LCCTypeToChar() {

            var tests = new Dictionary<Type, string> {
                {
                    TypeChar.Variable,
                    "char "
                },
                {
                    TypeSignedChar.Constant,
                    "const signed char "
                },
                {
                    TypeUnsignedChar.Variable,
                    "unsigned char "
                },
                {
                    TypeVoid.Variable,
                    "void "
                },
                {
                    TypeShort.Variable,
                    "short "
                },
                {
                    TypeUnsignedShort.Constant,
                    "const unsigned short "
                },
                {
                    TypeInt.Constant,
                    "const int "
                },
                {
                    TypeUnsignedInt.Variable,
                    "unsigned int "
                },
                {
                    TypeLong.Constant,
                    "const long "
                },
                {
                    TypeUnsignedLong.Variable,
                    "unsigned long "
                },
                {
                    TypeLongLong.Constant,
                    "const long long "
                },
                {
                    TypeUnsignedLongLong.Variable,
                    "unsigned long long "
                },
                {
                    TypeFloat.Variable,
                    "float "
                },
                {
                    TypeDouble.Constant,
                    "const double "
                },
                {
                    TypeBool.Constant,
                    "const _Bool "
                }
            };

            foreach (var test in tests) {
                System.Console.WriteLine(test.Key);
                Assert.AreEqual(test.Key.ToString(), test.Value);
            }
        }
    }
}
