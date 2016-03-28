using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.Type;

namespace LC_CompilerTests {
    [TestClass]
    public class TypeToStringTest {
        [TestMethod]
        public void LCCTypeToChar() {

            var tests = new Dictionary<UnqualifiedType, string> {
                {
                    TypeChar.Instance,
                    "char "
                },
                {
                    TypeSignedChar.Instance,
                    "signed char "
                },
                {
                    TypeUnsignedChar.Instance,
                    "unsigned char "
                },
                {
                    TypeVoid.Instance,
                    "void "
                },
                {
                    TypeShort.Instance,
                    "short "
                },
                {
                    TypeUnsignedShort.Instance,
                    "unsigned short "
                },
                {
                    TypeInt.Instance,
                    "int "
                },
                {
                    TypeUnsignedInt.Instance,
                    "unsigned int "
                },
                {
                    TypeLong.Instance,
                    "long "
                },
                {
                    TypeUnsignedLong.Instance,
                    "unsigned long "
                },
                {
                    TypeLongLong.Instance,
                    "long long "
                },
                {
                    TypeUnsignedLongLong.Instance,
                    "unsigned long long "
                },
                {
                    TypeFloat.Instance,
                    "float "
                },
                {
                    TypeDouble.Instance,
                    "double "
                },
                {
                    TypeBool.Instance,
                    "_Bool "
                }
            };

            foreach (var test in tests) {
                System.Console.WriteLine(test.Key);
                Assert.AreEqual(test.Key.ToString(), test.Value);
            }
        }
    }
}
