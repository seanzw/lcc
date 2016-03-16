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
                    new TypeChar(false),
                    "char "
                },
                {
                    new TypeSignedChar(true),
                    "const signed char "
                },
                {
                    new TypeUnsignedChar(false),
                    "unsigned char "
                },
                {
                    new TypeVoid(false),
                    "void "
                },
                {
                    new TypeShort(false),
                    "short "
                },
                {
                    new TypeUnsignedShort(true),
                    "const unsigned short "
                },
                {
                    new TypeInt(true),
                    "const int "
                },
                {
                    new TypeUnsignedInt(false),
                    "unsigned int "
                },
                {
                    new TypeLong(true),
                    "const long "
                },
                {
                    new TypeUnsignedLong(false),
                    "unsigned long "
                },
                {
                    new TypeLongLong(true),
                    "const long long "
                },
                {
                    new TypeUnsignedLongLong(false),
                    "unsigned long long "
                },
            };

            foreach (var test in tests) {
                System.Console.WriteLine(test.Key);
                Assert.AreEqual(test.Key.ToString(), test.Value);
            }
        }
    }
}
