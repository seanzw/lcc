using System.Numerics;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.TypeSystem;
using lcc.Token;
using lcc.SyntaxTree;

namespace LC_CompilerTests {
    public partial class SyntaxTreeTest {
        [TestMethod]
        public void LCCTCDeclaratorLegal() {

            var tests = new Dictionary<string, Tuple<string, T>> {
                {
                    "a[]",
                    new Tuple<string, T>("a", TInt.Instance.None().IArr())
                },
                {
                    "a[10]",
                    new Tuple<string, T>("a", TInt.Instance.None().Arr(10))
                },
                {
                    "*a[10]",
                    new Tuple<string, T>("a", TInt.Instance.None().Ptr().Arr(10))
                },
                {
                    "* const *a[]",
                    new Tuple<string, T>("a", TInt.Instance.None().Ptr(TQualifiers.C).Ptr().IArr())
                },
                {
                    "a[2][4]",
                    new Tuple<string, T>("a", TInt.Instance.None().Arr(4).Arr(2))
                }
            };
            
            foreach (var test in tests) {
                var env = new Env();
                var result = Utility.parse(test.Key, lcc.Parser.Parser.Declarator());
                Assert.AreEqual(1, result.Count());
                Assert.IsFalse(result.First().Remain.More());
                var root = result.First().Value as Declarator;
                Assert.IsFalse(root == null);
                var type = root.Declare(env, TInt.Instance.None());
                Assert.AreEqual(test.Value, type);
            }
        }
    }
}
