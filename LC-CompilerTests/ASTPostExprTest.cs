using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.TypeSystem;
using lcc.SyntaxTree;
using lcc.Parser;
using Parserc;

namespace LC_CompilerTests {

    public partial class ASTTests {
        [TestMethod]
        public void LCCTCArrSubTest() {

            // Environment:
            // const char carr[3];
            // char arr[4];
            ASTEnv env = new ASTEnv();
            env.AddSymbol("carr", new SymbolSignature(TChar.Instance.Const().Arr(3), SymbolKind.OBJECT, null));
            env.AddSymbol("arr", new SymbolSignature(TChar.Instance.None().Arr(4), SymbolKind.OBJECT, null));

            var tests = new Dictionary<string, T> {
                {
                    "carr[5]",
                    TChar.Instance.Const()
                },
                {
                    "arr[2]",
                    TChar.Instance.None()
                }
            };

            foreach (var test in tests) {
                var result = Utility.parse(test.Key, Parser.PostfixExpression().End());
                Assert.AreEqual(1, result.Count());
                Assert.IsFalse(result.First().Remain.More());
                Assert.IsTrue(result.First().Value is STArrSub);
                var ast = result.First().Value as STArrSub;
                Assert.AreEqual(test.Value, ast.TypeCheck(env));
            }
        }

        [TestMethod]
        public void LCCTCMemRef() {

            // Environment:
            // struct s {
            //     int i;
            //     const int ci;
            // } t;
            // struct s *p;
            // const struct s *cp;
            ASTEnv env = new ASTEnv();

            TStructUnion s = new TStruct(
                "s",
                new List<TStructUnion.Field> {
                    new TStructUnion.Field("i", TInt.Instance.None(), 0),
                    new TStructUnion.Field("ci", TInt.Instance.Const(), 4)
                });

            TPointer p = new TPointer(s.None());
            TPointer cp = new TPointer(s.Const());

            env.AddSymbol("t", new SymbolSignature(s.None(), SymbolKind.OBJECT, null));
            env.AddSymbol("p", new SymbolSignature(p.None(), SymbolKind.OBJECT, null));
            env.AddSymbol("cp", new SymbolSignature(cp.None(), SymbolKind.OBJECT, null));

            var tests = new Dictionary<string, T> {
                {
                    "t.i",
                    TInt.Instance.None()
                },
                {
                    "p->i",
                    TInt.Instance.None()
                },
                {
                    "p->ci",
                    TInt.Instance.Const()
                },
                {
                    "cp->i",
                    TInt.Instance.Const()
                }
            };

            foreach (var test in tests) {
                var result = Utility.parse(test.Key, Parser.PostfixExpression().End());
                Assert.AreEqual(1, result.Count());
                Assert.IsFalse(result.First().Remain.More());
                Assert.IsTrue(result.First().Value is STAccess);
                var ast = result.First().Value as STAccess;
                Assert.AreEqual(test.Value, ast.TypeCheck(env));
            }
        }
    }
}
