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
        public void LCCTCPreStep() {

            // Environment:
            // const char carr[3];
            // const char *pc;
            // char arr[4];
            ASTEnv env = new ASTEnv();
            env.AddSymbol("carr", new SymbolSignature(TChar.Instance.Const().Arr(3), SymbolKind.OBJECT, null));
            env.AddSymbol("pc", new SymbolSignature(TChar.Instance.Const().Ptr(), SymbolKind.OBJECT, null));
            env.AddSymbol("arr", new SymbolSignature(TChar.Instance.None().Arr(4), SymbolKind.OBJECT, null));

            var tests = new Dictionary<string, T> {
                {
                    "++pc",
                    TChar.Instance.Const().Ptr().R()
                },
                {
                    "--arr[2]",
                    TChar.Instance.None(T.LR.R)
                }
            };

            foreach (var test in tests) {
                var result = Utility.parse(test.Key, Parser.UnaryExpression().End());
                Assert.AreEqual(1, result.Count());
                Assert.IsFalse(result.First().Remain.More());
                Assert.IsTrue(result.First().Value is STPreStep);
                var ast = result.First().Value as STPreStep;
                Assert.AreEqual(test.Value, ast.TypeCheck(env));
            }
        }

        [TestMethod]
        public void LCCTCUnaryExpr() {

            // Environment:
            // struct s {
            //     long long i;
            //     const char ci;
            // } t;
            // struct s *p;
            // const struct s *cp;
            ASTEnv env = new ASTEnv();

            TStructUnion s = new TStruct(
                "s",
                new List<TStructUnion.Field> {
                    new TStructUnion.Field("i", TLLong.Instance.None(), 0),
                    new TStructUnion.Field("ci", TChar.Instance.Const(), 4)
                });

            TPointer p = new TPointer(s.None());
            TPointer cp = new TPointer(s.Const());

            env.AddSymbol("t", new SymbolSignature(s.None(), SymbolKind.OBJECT, null));
            env.AddSymbol("p", new SymbolSignature(p.None(), SymbolKind.OBJECT, null));
            env.AddSymbol("cp", new SymbolSignature(cp.None(), SymbolKind.OBJECT, null));

            var tests = new Dictionary<string, T> {
                {
                    "&t",
                    p.None(T.LR.R)
                },
                {
                    "*p",
                    s.None()
                },
                {
                    "+t.i",
                    TLLong.Instance.None(T.LR.R)
                },
                {
                    "!p->i",
                    TInt.Instance.None(T.LR.R)
                }
            };

            foreach (var test in tests) {
                var result = Utility.parse(test.Key, Parser.UnaryExpression().End());
                Assert.AreEqual(1, result.Count());
                Assert.IsFalse(result.First().Remain.More());
                Assert.IsTrue(result.First().Value is STUnaryOp);
                var ast = result.First().Value as STUnaryOp;
                Assert.AreEqual(test.Value, ast.TypeCheck(env));
            }
        }
    }
}
