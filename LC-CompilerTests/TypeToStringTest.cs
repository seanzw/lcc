using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.TypeSystem;

namespace LC_CompilerTests {
    [TestClass]
    public class TypeToStringTest {

        [TestMethod]
        public void LCCTypeToChar() {

            var tests = new Dictionary<TUnqualified, string> {
                {
                    TChar.Instance,
                    "char"
                },
                {
                    TSChar.Instance,
                    "signed char"
                },
                {
                    TUChar.Instance,
                    "unsigned char"
                },
                {
                    TVoid.Instance,
                    "void"
                },
                {
                    TShort.Instance,
                    "short"
                },
                {
                    TUShort.Instance,
                    "unsigned short"
                },
                {
                    TInt.Instance,
                    "int"
                },
                {
                    TUInt.Instance,
                    "unsigned int"
                },
                {
                    TLong.Instance,
                    "long"
                },
                {
                    TULong.Instance,
                    "unsigned long"
                },
                {
                    TLLong.Instance,
                    "long long"
                },
                {
                    TULLong.Instance,
                    "unsigned long long"
                },
                {
                    TFloat.Instance,
                    "float"
                },
                {
                    TDouble.Instance,
                    "double"
                },
                {
                    TBool.Instance,
                    "_Bool"
                },
                {
                    new TArray(TChar.Instance.None(), 3),
                    "(char)[3]"
                }
            };

            foreach (var test in tests) {
                System.Console.WriteLine(test.Key);
                Assert.AreEqual(test.Value, test.Key.ToString());
            }
        }
    }
}
