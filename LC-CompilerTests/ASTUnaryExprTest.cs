using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.TypeSystem;
using lcc.AST;
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
            env.AddSymbol("carr", TChar.Instance.Const().Arr(3), 1);
            env.AddSymbol("pc", TChar.Instance.Const().Ptr(), 2);
            env.AddSymbol("arr", TChar.Instance.None().Arr(4), 1);

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
                Assert.IsTrue(result.First().Value is ASTPreStep);
                var ast = result.First().Value as ASTPreStep;
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

            TStructUnion s = new TStructUnion(
                "s",
                new List<TStructUnion.Field> {
                    new TStructUnion.Field("i", TLLong.Instance.None(), 0),
                    new TStructUnion.Field("ci", TChar.Instance.Const(), 4)
                },
                TStructUnion.Kind.STRUCT);

            TPointer p = new TPointer(s.None());
            TPointer cp = new TPointer(s.Const());

            env.AddSymbol("t", s.None(), 1);
            env.AddSymbol("p", p.None(), 2);
            env.AddSymbol("cp", cp.None(), 3);

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
                Assert.IsTrue(result.First().Value is ASTUnaryOp);
                var ast = result.First().Value as ASTUnaryOp;
                Assert.AreEqual(test.Value, ast.TypeCheck(env));
            }
        }
    }
}
