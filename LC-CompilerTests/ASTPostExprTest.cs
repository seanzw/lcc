using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.TypeSystem;
using lcc.AST;
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
            env.AddSymbol("carr", new TArray(TChar.Instance.MakeConst(T.LR.L), 3).MakeType(T.LR.L), 1);
            env.AddSymbol("arr", new TArray(TChar.Instance.MakeType(T.LR.L), 4).MakeType(T.LR.L), 1);

            var tests = new Dictionary<string, T> {
                {
                    "carr[5]",
                    TChar.Instance.MakeConst(T.LR.L)
                },
                {
                    "arr[2]",
                    TChar.Instance.MakeType(T.LR.L)
                }
            };

            foreach (var test in tests) {
                var result = Utility.parse(test.Key, Parser.PostfixExpression().End());
                Assert.AreEqual(1, result.Count);
                Assert.IsFalse(result[0].remain.More());
                Assert.IsTrue(result[0].value is ASTArrSub);
                var ast = result[0].value as ASTArrSub;
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

            TStructUnion s = new TStructUnion(
                "s",
                new List<TStructUnion.Field> {
                    new TStructUnion.Field("i", TInt.Instance.MakeType(T.LR.L), 0),
                    new TStructUnion.Field("ci", TInt.Instance.MakeConst(T.LR.L), 4)
                },
                TStructUnion.Kind.STRUCT);

            TPointer p = new TPointer(s.MakeType(T.LR.L));
            TPointer cp = new TPointer(s.MakeConst(T.LR.L));

            env.AddSymbol("t", s.MakeType(T.LR.L), 1);
            env.AddSymbol("p", p.MakeType(T.LR.L), 2);
            env.AddSymbol("cp", cp.MakeType(T.LR.L), 3);

            var tests = new Dictionary<string, T> {
                {
                    "t.i",
                    TInt.Instance.MakeType(T.LR.L)
                },
                {
                    "p->i",
                    TInt.Instance.MakeType(T.LR.L)
                },
                {
                    "p->ci",
                    TInt.Instance.MakeConst(T.LR.L)
                },
                {
                    "cp->i",
                    TInt.Instance.MakeConst(T.LR.L)
                }
            };

            foreach (var test in tests) {
                var result = Utility.parse(test.Key, Parser.PostfixExpression().End());
                Assert.AreEqual(1, result.Count);
                Assert.IsFalse(result[0].remain.More());
                Assert.IsTrue(result[0].value is ASTAccess);
                var ast = result[0].value as ASTAccess;
                Assert.AreEqual(test.Value, ast.TypeCheck(env));
            }
        }
    }
}
