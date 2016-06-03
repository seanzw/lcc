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
            env.PushScope(ScopeKind.FUNCTION);
            var result = Utility.parse(source, lcc.Parser.Parser.CompoundStatement().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            IEnumerable<lcc.AST.Stmt> stmts = result.First().Value.ToAST(env);
            Assert.AreEqual(1, stmts.Count());
            lcc.AST.CompoundStmt stmt = stmts.First() as lcc.AST.CompoundStmt;
            Assert.IsNotNull(stmt);
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
            env.PushScope(ScopeKind.FUNCTION);
            var result = Utility.parse(source, lcc.Parser.Parser.CompoundStatement().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            IEnumerable<lcc.AST.Stmt> stmts = result.First().Value.ToAST(env);
            Assert.AreEqual(1, stmts.Count());
            lcc.AST.CompoundStmt stmt = stmts.First() as lcc.AST.CompoundStmt;
            Assert.IsNotNull(stmt);
            Assert.AreEqual(types.Count(), stmt.stmts.Count());
            var tests = stmt.stmts.Zip(types, (s, t) => new Tuple<lcc.AST.Expr, T>(s as lcc.AST.Expr, t));
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
            env.PushScope(ScopeKind.FUNCTION);
            var result = Utility.parse(source, lcc.Parser.Parser.CompoundStatement().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            IEnumerable<lcc.AST.Stmt> stmts = result.First().Value.ToAST(env);
            Assert.AreEqual(1, stmts.Count());
            lcc.AST.CompoundStmt stmt = stmts.First() as lcc.AST.CompoundStmt;
            Assert.IsNotNull(stmt);
            Assert.AreEqual(types.Count(), stmt.stmts.Count());
            var tests = stmt.stmts.Zip(types, (s, t) => new Tuple<lcc.AST.Expr, T>(s as lcc.AST.Expr, t));
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
                TUInt.Instance.Const(),
                TUInt.Instance.Const(),
                TUInt.Instance.Const(),
                TUInt.Instance.Const(),
                TUInt.Instance.Const()
            };
            List<int> values = new List<int> {
                128,
                4,
                8,
                8,
                4
            };
            var env = new Env();
            env.PushScope(ScopeKind.FUNCTION);
            var result = Utility.parse(source, lcc.Parser.Parser.CompoundStatement().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            IEnumerable<lcc.AST.Stmt> stmts = result.First().Value.ToAST(env);
            Assert.AreEqual(1, stmts.Count());
            lcc.AST.CompoundStmt stmt = stmts.First() as lcc.AST.CompoundStmt;
            Assert.IsNotNull(stmt);
            Assert.AreEqual(types.Count(), stmt.stmts.Count());
            {
                var tests = stmt.stmts.Zip(types, (s, t) => new Tuple<lcc.AST.Expr, T>(s as lcc.AST.Expr, t));
                foreach (var test in tests) {
                    Assert.AreEqual(test.Item2, test.Item1.Type);
                }
            }
            {
                var tests = stmt.stmts.Zip(values, (s, t) => new Tuple<lcc.AST.ConstIntExpr, int>(s as lcc.AST.ConstIntExpr, t));
                foreach (var test in tests) {
                    Assert.AreEqual(test.Item2, test.Item1.value);
                }
            }
        }

    }
}
