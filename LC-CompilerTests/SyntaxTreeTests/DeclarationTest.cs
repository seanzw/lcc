using System.Numerics;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.TypeSystem;
using lcc.Token;
using lcc.SyntaxTree;
using Parserc;

namespace LC_CompilerTests {
    public partial class SyntaxTreeTests {
        [TestMethod]
        public void LCCTCDeclaratorLegal() {

            var tests = new Dictionary<string, Tuple<string, T, IEnumerable<Tuple<string, T>>>> {
                {
                    "a[]",
                    new Tuple<string, T, IEnumerable<Tuple<string, T>>>("a", TInt.Instance.None().IArr(), null)
                },
                {
                    "a[10]",
                    new Tuple<string, T, IEnumerable<Tuple<string, T>>>("a", TInt.Instance.None().Arr(10), null)
                },
                {
                    "*a[10]",
                    new Tuple<string, T, IEnumerable<Tuple<string, T>>>("a", TInt.Instance.None().Ptr().Arr(10), null)
                },
                {
                    "* const *a[]",
                    new Tuple<string, T, IEnumerable<Tuple<string, T>>>("a", TInt.Instance.None().Ptr(TQualifiers.C).Ptr().IArr(), null)
                },
                {
                    "a[2][4]",
                    new Tuple<string, T, IEnumerable<Tuple<string, T>>>("a", TInt.Instance.None().Arr(4).Arr(2), null)
                },
                {
                    "f(int a)",
                    new Tuple<string, T, IEnumerable<Tuple<string, T>>>("f", TInt.Instance.None().Func(new List<T> {
                        TInt.Instance.None()
                    }, false),
                    new List<Tuple<string, T>> {
                        new Tuple<string, T>("a", TInt.Instance.None())
                    })
                },
                {
                    "f(int a, int b, ...)",
                    new Tuple<string, T, IEnumerable<Tuple<string, T>>>("f", TInt.Instance.None().Func(new List<T> {
                        TInt.Instance.None(),
                        TInt.Instance.None(),
                    }, true),
                    new List<Tuple<string, T>> {
                        new Tuple<string, T>("a", TInt.Instance.None()),
                        new Tuple<string, T>("b", TInt.Instance.None())
                    })
                },
                {
                    "f()",
                    new Tuple<string, T, IEnumerable<Tuple<string, T>>>("f", TInt.Instance.None().Func(new List<T>(), false), Enumerable.Empty<Tuple<string, T>>())
                },
                {
                    "f(int a[10], ...)",
                    new Tuple<string, T, IEnumerable<Tuple<string, T>>>("f", TInt.Instance.None().Func(new List<T> {
                        TInt.Instance.None().Ptr()
                    }, true),
                    new List<Tuple<string, T>> {
                        new Tuple<string, T>("a", TInt.Instance.None().Ptr())
                    })
                },
                {
                    "f(void)",
                    new Tuple<string, T, IEnumerable<Tuple<string, T>>>("f", TInt.Instance.None().Func(new List<T>(), false), Enumerable.Empty<Tuple<string, T>>())
                },
                {
                    "f(int [], ...)",
                    new Tuple<string, T, IEnumerable<Tuple<string, T>>>("f", TInt.Instance.None().Func(new List<T> {
                        TInt.Instance.None().Ptr()
                    }, true),
                    new List<Tuple<string, T>> {
                        new Tuple<string, T>(null, TInt.Instance.None().Ptr())
                    })
                },
                {
                    "(*pfi)()",
                    new Tuple<string, T, IEnumerable<Tuple<string, T>>>("pfi", TInt.Instance.None().Func(new List<T>(), false).Ptr(), Enumerable.Empty<Tuple<string, T>>())
                },
                {
                    "*fip()",
                    new Tuple<string, T, IEnumerable<Tuple<string, T>>>("fip", TInt.Instance.None().Ptr().Func(new List<T>(), false), Enumerable.Empty<Tuple<string, T>>())
                },
                {
                    "(*apfi[3])(int *x, int *y)",
                    new Tuple<string, T, IEnumerable<Tuple<string, T>>>("apfi", TInt.Instance.None().Func(new List<T> {
                        TInt.Instance.None().Ptr(),
                        TInt.Instance.None().Ptr()
                    }, false).Ptr().Arr(3),
                    null)
                },
                {
                    "(*fpfi(int (*)(long), int))(int, ...)",
                    new Tuple<string, T, IEnumerable<Tuple<string, T>>>("fpfi", TInt.Instance.None().Func(new List<T> {
                        TInt.Instance.None()
                    }, true).Ptr().Func(new List<T> {
                        TInt.Instance.None().Func(new List<T> {
                            TLong.Instance.None()
                        }, false).Ptr(),
                        TInt.Instance.None()
                    }, false),
                    new List<Tuple<string, T>> {
                        new Tuple<string, T>(null, TInt.Instance.None().Func(new List<T> {TLong.Instance.None()}, false).Ptr()),
                        new Tuple<string, T>(null, TInt.Instance.None())
                    })
                }
            };
            
            foreach (var test in tests) {
                var env = new Env();
                var result = Utility.Parse(test.Key, lcc.Parser.Parser.Declarator());
                Assert.AreEqual(1, result.Count());
                Assert.IsFalse(result.First().Remain.More());
                var root = result.First().Value as Declarator;
                Assert.IsFalse(root == null);
                var type = root.Declare(env, TInt.Instance.None());
                Assert.AreEqual(test.Value.Item1, type.Item1);
                Assert.AreEqual(test.Value.Item2, type.Item2);
                if (test.Value.Item3 != null)
                    Assert.IsTrue(test.Value.Item3.SequenceEqual(type.Item3));
                else
                    Assert.IsNull(type.Item3);
            }
        }

        [TestMethod]
        public void LCCTCEnvDump() {
            string source = @"
typedef int x;
typedef double xx;
int a;
extern int b;
static int c;
int foo();
static int bar();
x foo2(x);
";
            var env = new Env();
            var result = Utility.Parse(source, lcc.Parser.Parser.Declaration().Plus().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            foreach (var declaration in result.First().Value) {
                declaration.ToAST(env);
            }
            Console.WriteLine(env.Dump());
        }

        [TestMethod]
        public void LCCTCStructDecl() {
            string source = @"
struct A {
    int a;
    char f;
    int c;
    int x:3, :0;
    int y:30, z:4;
    unsigned int w:3, ww:5;
}";
            var env = new Env();
            var result = Utility.Parse(source, lcc.Parser.Parser.StructUnionSpecifier().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            TStruct t = result.First().Value.GetT(env).nake as TStruct;
            Assert.IsNotNull(t);
            Console.WriteLine(env.Dump());
            Console.WriteLine(t.Dump());
            Console.WriteLine(t.Bits);
        }

        [TestMethod]
        public void LCCTCStructRecursiveDecl() {
            string source = @"
struct Node {
    struct Node* next;
}";
            var env = new Env();
            var result = Utility.Parse(source, lcc.Parser.Parser.StructUnionSpecifier().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            TStruct t = result.First().Value.GetT(env).nake as TStruct;
            Assert.IsNotNull(t);
            Console.WriteLine(env.Dump());
            Console.WriteLine(t.Dump());
            Console.WriteLine(t.Bits);
        }

        [TestMethod]
        public void LCCTCEnumDecl() {
            string source = @"
enum hue {
    chartreuse,
    burgundy,
    claret = 20,
    winedark
}";
            var env = new Env();
            var result = Utility.Parse(source, lcc.Parser.Parser.EnumSpecifier().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            TEnum t = result.First().Value.GetT(env).nake as TEnum;
            Assert.IsNotNull(t);
            Console.WriteLine(env.Dump());
            Console.WriteLine(t.Dump());
            Console.WriteLine(t.Bits);
        }
    }
}
