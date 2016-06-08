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
    public partial class SyntaxTreeTest {

        [TestMethod]
        public void LCCTCArrSub() {
            string source = @"
{
    int a[10], b;
    int x[3][5];
    a;
    b;
    a[3];
    x[3][2];
    x[b];
    b++;
    b--;
}
";
            var env = new Env();
            var result = Utility.parse(source, lcc.Parser.Parser.CompoundStatement().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            lcc.AST.Stmt stmt = result.First().Value.ToAST(env);
            lcc.AST.CompoundStmt s = stmt as lcc.AST.CompoundStmt;
            Assert.IsNotNull(s);
            //Assert.AreEqual(4, stmt.stmts.Count());
        }

        [TestMethod]
        public void LCCTCAccess() {
            string source = @"
{
    struct s {
        int i;
        const int ci;
    };
    struct s s;
    const struct s cs;
    s.i;
    s.ci;
    cs.i;
    cs.ci;
}
";
            List<T> types = new List<T> {
                TInt.Instance.None(),
                TInt.Instance.Const(),
                TInt.Instance.Const(),
                TInt.Instance.Const()
            };
            var env = new Env();
            var result = Utility.parse(source, lcc.Parser.Parser.CompoundStatement().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            lcc.AST.Stmt stmt = result.First().Value.ToAST(env);
            lcc.AST.CompoundStmt conmpund = stmt as lcc.AST.CompoundStmt;
            Assert.IsNotNull(conmpund);
            IEnumerable<lcc.AST.Expr> exprs = conmpund.stmts.OfType<lcc.AST.Expr>();
            Assert.AreEqual(types.Count(), exprs.Count());
            var tests = exprs.Zip(types, (s, t) => new Tuple<lcc.AST.Expr, T>(s as lcc.AST.Expr, t));
            foreach (var test in tests) {
                Assert.AreEqual(test.Item2, test.Item1.Type);
            }
        }

        [TestMethod]
        public void LCCTCUnary() {
            string source = @"
{
    int a, *b;
    --a;
    ++b;
    &a;
    char c;
    c;
    +c;
    -c;
    ~c;
    !c;
    !b;
}
";
            List<T> types = new List<T> {
                TInt.Instance.None(),
                TInt.Instance.None().Ptr(),
                TInt.Instance.None().Ptr(),
                TChar.Instance.None(),
                TInt.Instance.None(),
                TInt.Instance.None(),
                TInt.Instance.None(),
                TInt.Instance.None(),
                TInt.Instance.None()
            };
            var env = new Env();
            var result = Utility.parse(source, lcc.Parser.Parser.CompoundStatement().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            lcc.AST.Stmt stmt = result.First().Value.ToAST(env);
            lcc.AST.CompoundStmt conmpund = stmt as lcc.AST.CompoundStmt;
            Assert.IsNotNull(conmpund);
            IEnumerable<lcc.AST.Expr> exprs = conmpund.stmts.OfType<lcc.AST.Expr>();
            Assert.AreEqual(types.Count(), exprs.Count());
            var tests = exprs.Zip(types, (s, t) => new Tuple<lcc.AST.Expr, T>(s as lcc.AST.Expr, t));
            foreach (var test in tests) {
                Assert.AreEqual(test.Item2, test.Item1.Type);
            }
        }

        [TestMethod]
        public void LCCTCSizeof() {
            string source = @"
{
    int a[32];
    sizeof a;
    sizeof(int);
    sizeof(double);
    sizeof(struct { char c; int a; });
    sizeof(a[0]);
}
";
            List<T> types = new List<T> {
                TUInt.Instance.None(),
                TUInt.Instance.None(),
                TUInt.Instance.None(),
                TUInt.Instance.None(),
                TUInt.Instance.None()
            };
            List<int> values = new List<int> {
                128,
                4,
                8,
                8,
                4
            };
            var env = new Env();
            var result = Utility.parse(source, lcc.Parser.Parser.CompoundStatement().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            lcc.AST.Stmt stmt = result.First().Value.ToAST(env);
            lcc.AST.CompoundStmt conmpund = stmt as lcc.AST.CompoundStmt;
            Assert.IsNotNull(conmpund);
            IEnumerable<lcc.AST.Expr> exprs = conmpund.stmts.OfType<lcc.AST.Expr>();
            Assert.AreEqual(types.Count(), exprs.Count());
            {
                var tests = exprs.Zip(types, (s, t) => new Tuple<lcc.AST.Expr, T>(s as lcc.AST.Expr, t));
                foreach (var test in tests) {
                    Assert.AreEqual(test.Item2, test.Item1.Type);
                }
            }
            {
                var tests = exprs.Zip(values, (s, t) => new Tuple<lcc.AST.ConstIntExpr, int>(s as lcc.AST.ConstIntExpr, t));
                foreach (var test in tests) {
                    Assert.AreEqual(test.Item2, test.Item1.value);
                }
            }
        }

        [TestMethod]
        public void LCCTCPreStep() {
            string source = @"
{
    int a;
    --a;
    float b;
    ++b;
    int* c;
    ++c;
}
";
            List<T> types = new List<T> {
                TInt.Instance.None(),
                TSingle.Instance.None(),
                TInt.Instance.None().Ptr()
            };
            var env = new Env();
            var result = Utility.parse(source, lcc.Parser.Parser.CompoundStatement().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            lcc.AST.Stmt stmt = result.First().Value.ToAST(env);
            lcc.AST.CompoundStmt conmpund = stmt as lcc.AST.CompoundStmt;
            Assert.IsNotNull(conmpund);
            IEnumerable<lcc.AST.Expr> exprs = conmpund.stmts.OfType<lcc.AST.Expr>();
            Assert.AreEqual(types.Count(), exprs.Count());
            var tests = exprs.Zip(types, (s, t) => new Tuple<lcc.AST.Expr, T>(s as lcc.AST.Expr, t));
            foreach (var test in tests) {
                Assert.AreEqual(test.Item2, test.Item1.Type);
            }
        }

        [TestMethod]
        public void LCCTCBiExpr() {
            string source = @"
{
    char c;
    unsigned char uc;
    signed char sc;
    short s;
    unsigned short us;
    int i;
    unsigned int ui;
    long l;
    unsigned long ul;
    long long ll;
    unsigned long long ull;
    float f;
    double d;
    long double ld;
    int* ip1;
    int* ip2;
    
    c * uc;
    s * f;
    ll * i;

    d / f;
    
    i % ui;

    f + ll;
    ip1 + l;
    i + ip2;

    ld - f;
    ip1 - ip2;
}
";
            List<T> types = new List<T> {
                TInt.Instance.None(),
                TSingle.Instance.None(),
                TLLong.Instance.None(),

                TDouble.Instance.None(),

                TUInt.Instance.None(),

                TSingle.Instance.None(),
                TInt.Instance.None().Ptr(),
                TInt.Instance.None().Ptr(),

                TLDouble.Instance.None(),
                TInt.Instance.None(),
            };
            var env = new Env();
            var result = Utility.parse(source, lcc.Parser.Parser.CompoundStatement().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            lcc.AST.Stmt stmt = result.First().Value.ToAST(env);
            lcc.AST.CompoundStmt conmpund = stmt as lcc.AST.CompoundStmt;
            Assert.IsNotNull(conmpund);
            IEnumerable<lcc.AST.Expr> exprs = conmpund.stmts.OfType<lcc.AST.Expr>();
            Assert.AreEqual(types.Count(), exprs.Count());
            var tests = exprs.Zip(types, (s, t) => new Tuple<lcc.AST.Expr, T>(s as lcc.AST.Expr, t));
            foreach (var test in tests) {
                Assert.AreEqual(test.Item2, test.Item1.Type);
            }
        }

        [TestMethod]
        public void LCCTCFuncCall() {
            string source = @"
{
    int (*foo)(char);
    int (*bar)(double, int, ...);
    int x;
    int y;
    y = foo(x);
    y = bar(y, x, y, x);
}
";
            List<T> types = new List<T> {
                TInt.Instance.None(),
                TInt.Instance.None()
            };
            var env = new Env();
            var result = Utility.parse(source, lcc.Parser.Parser.CompoundStatement().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            lcc.AST.Stmt stmt = result.First().Value.ToAST(env);
            lcc.AST.CompoundStmt conmpund = stmt as lcc.AST.CompoundStmt;
            Assert.IsNotNull(conmpund);
            IEnumerable<lcc.AST.Expr> exprs = conmpund.stmts.OfType<lcc.AST.Expr>();
            Assert.AreEqual(types.Count(), exprs.Count());
            var tests = exprs.Zip(types, (s, t) => new Tuple<lcc.AST.Expr, T>(s as lcc.AST.Expr, t));
            foreach (var test in tests) {
                Assert.AreEqual(test.Item2, test.Item1.Type);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ETypeError), "imcomplete argument")]
        public void LCCTCFuncCallERR1() {
            string source = @"
{
    int (*foo)(char);
    int (*bar)(double, int, ...);
    struct s s;
    int x;
    int y;
    y = bar(s, y, x, y, x);
}
";
            List<T> types = new List<T> {
                TInt.Instance.None(),
            };
            var env = new Env();
            var result = Utility.parse(source, lcc.Parser.Parser.CompoundStatement().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            lcc.AST.Stmt stmt = result.First().Value.ToAST(env);
            lcc.AST.CompoundStmt conmpund = stmt as lcc.AST.CompoundStmt;
            Assert.IsNotNull(conmpund);
            IEnumerable<lcc.AST.Expr> exprs = conmpund.stmts.OfType<lcc.AST.Expr>();
            Assert.AreEqual(types.Count(), exprs.Count());
            var tests = exprs.Zip(types, (s, t) => new Tuple<lcc.AST.Expr, T>(s as lcc.AST.Expr, t));
            foreach (var test in tests) {
                Assert.AreEqual(test.Item2, test.Item1.Type);
            }
        }

        [TestMethod]
        public void LCCTCUsualArithmeticConversion() {
            Dictionary<Tuple<TArithmetic, TArithmetic>, TArithmetic> tests = new Dictionary<Tuple<TArithmetic, TArithmetic>, TArithmetic> {
                {
                    new Tuple<TArithmetic, TArithmetic>(TLDouble.Instance, TLDouble.Instance),
                    TLDouble.Instance
                },
                {
                    new Tuple<TArithmetic, TArithmetic>(TLDouble.Instance, TInt.Instance),
                    TLDouble.Instance
                },
                {
                    new Tuple<TArithmetic, TArithmetic>(TLDouble.Instance, TChar.Instance),
                    TLDouble.Instance
                },
                {
                    new Tuple<TArithmetic, TArithmetic>(TLDouble.Instance, TUShort.Instance),
                    TLDouble.Instance
                },
                {
                    new Tuple<TArithmetic, TArithmetic>(TLDouble.Instance, TLLong.Instance),
                    TLDouble.Instance
                },
                {
                    new Tuple<TArithmetic, TArithmetic>(TSingle.Instance, TLDouble.Instance),
                    TLDouble.Instance
                },
                {
                    new Tuple<TArithmetic, TArithmetic>(TLDouble.Instance, TCLDouble.Instance),
                    TCLDouble.Instance
                },
                {
                    new Tuple<TArithmetic, TArithmetic>(TLDouble.Instance, TCDouble.Instance),
                    TCLDouble.Instance
                },
                {
                    new Tuple<TArithmetic, TArithmetic>(TChar.Instance, TShort.Instance),
                    TInt.Instance
                },
                {
                    new Tuple<TArithmetic, TArithmetic>(TChar.Instance, TLong.Instance),
                    TLong.Instance
                },
                {
                    new Tuple<TArithmetic, TArithmetic>(TChar.Instance, TUInt.Instance),
                    TUInt.Instance
                },
                {
                    new Tuple<TArithmetic, TArithmetic>(TChar.Instance, TULLong.Instance),
                    TULLong.Instance
                },
                {
                    new Tuple<TArithmetic, TArithmetic>(TLLong.Instance, TULong.Instance),
                    TLLong.Instance
                },

            };
            foreach (var test in tests) {
                Assert.AreEqual(test.Value, test.Key.Item1.UsualArithConversion(test.Key.Item2));
            }
        }

    }
}
