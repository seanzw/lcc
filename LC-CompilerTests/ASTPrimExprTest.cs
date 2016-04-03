using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.TypeSystem;
using lcc.Token;
using lcc.AST;
using lcc.Parser;
using Parserc;

namespace LC_CompilerTests {
    [TestClass]
    public partial class ASTTests {

        [TestMethod]
        public void LCCTCConstCharEvaluateLegal() {

            var tests = new Dictionary<string, LinkedList<ushort>> {
                {
                    "f",
                    new LinkedList<ushort>(new List<ushort> { 'f' })
                },
                {
                    "\\n",
                    new LinkedList<ushort>(new List<ushort> { '\n' })
                },
                {
                    "\\'",
                    new LinkedList<ushort>(new List<ushort> { '\'' })
                },
                {
                    "\\c",
                    new LinkedList<ushort>(new List<ushort> { 'c' })
                },
                {
                    "\\377",
                    new LinkedList<ushort>(new List<ushort> { 255 })
                },
                {
                    "\\xff",
                    new LinkedList<ushort>(new List<ushort> { 255 })
                },
                {
                    "\\7",
                    new LinkedList<ushort>(new List<ushort> { 7 })
                },
                {
                    "\\76",
                    new LinkedList<ushort>(new List<ushort> { 7 * 8 + 6 })
                },
                {
                    "\\0223",
                    new LinkedList<ushort>(new List<ushort> { 0x12, '3' })
                },
                {
                    "abcd",
                    new LinkedList<ushort>(new List<ushort> { 'a', 'b', 'c', 'd' })
                }
            };

            foreach (var test in tests) {
                var values = ASTConstChar.Evaluate(1, test.Key);
                Assert.IsTrue(values.SequenceEqual(test.Value));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ASTErrEscapedSequenceOutOfRange), "Too long escaped sequence")]
        public void LCCTCConstCharIllegal1() {
            string src = "\\xfff";
            var values = ASTConstChar.Evaluate(1, src);
        }

        [TestMethod]
        [ExpectedException(typeof(ASTErrEscapedSequenceOutOfRange), "Too long escaped sequence")]
        public void LCCTCConstCharIllegal2() {
            string src = "\\777";
            var values = ASTConstChar.Evaluate(1, src);
        }

        [TestMethod]
        [ExpectedException(typeof(ASTErrUnknownType), "Multi-character")]
        public void LCCTCConstCharIllegalMultiChar1() {
            string src = "'\\0223'";
            var ast = new ASTConstChar(new T_CONST_CHAR(1, src));
            ast.TypeCheck(new ASTEnv());
        }

        [TestMethod]
        [ExpectedException(typeof(ASTErrUnknownType), "Multi-character")]
        public void LCCTCConstCharIllegalMultiChar2() {
            string src = "L'\\0223'";
            var ast = new ASTConstChar(new T_CONST_CHAR(1, src));
            ast.TypeCheck(new ASTEnv());
        }

        [TestMethod]
        public void LCCTCStringConcat() {
            string src = "\"a\" \"b\"";
            var truth = new List<ushort> { 'a', 'b' };
            var result = Utility.parse(src, Parser.PrimaryExpression().End());
            Assert.AreEqual(1, result.Count());
            Assert.IsFalse(result.First().Remain.More());
            Assert.IsTrue(result.First().Value is ASTString);
            var ast = result.First().Value as ASTString;
            Assert.IsTrue(truth.SequenceEqual(ast.values));
        }

        [TestMethod]
        public void LCCTCConstIntValue() {
            var tests = new Dictionary<string, BigInteger> {
                {
                    "123",
                    123
                },
                {
                    "0xffL",
                    255
                },
                {
                    "0",
                    0
                },
                {
                    "0377",
                    255
                },
                {
                    "0XF",
                    15
                }
            };
            foreach (var test in tests) {
                var result = Utility.parse(test.Key, Parser.PrimaryExpression().End());
                Assert.AreEqual(1, result.Count());
                Assert.IsFalse(result.First().Remain.More());
                Assert.IsTrue(result.First().Value is ASTConstInt);
                var ast = result.First().Value as ASTConstInt;
                Assert.AreEqual(test.Value, ast.value);
            }
        }

        [TestMethod]
        public void LCCTCConstIntType() {
            var tests = new Dictionary<string, T> {
                {
                    "233",
                    TInt.Instance.Const(T.LR.R)
                },
                {
                    "4294967296",
                    TLLong.Instance.Const(T.LR.R)
                },
                {
                    "0xFFFFFFFF",
                    TUInt.Instance.Const(T.LR.R)
                },
                {
                    "23u",
                    TUInt.Instance.Const(T.LR.R)
                },
                {
                    "0ull",
                    TULLong.Instance.Const(T.LR.R)
                }
            };
            foreach (var test in tests) {
                var result = Utility.parse(test.Key, Parser.PrimaryExpression().End());
                Assert.AreEqual(1, result.Count());
                Assert.IsFalse(result.First().Remain.More());
                Assert.IsTrue(result.First().Value is ASTConstInt);
                var ast = result.First().Value as ASTConstInt;
                Assert.AreEqual(test.Value, ast.type);
            }
        }

        [TestMethod]
        public void LCCTCConstFloatValue() {
            var tests = new Dictionary<string, double> {
                {
                    "1.0",
                    1.0
                },
                {
                    "1e2",
                    100.0
                },
                {
                    "1e+2",
                    100.0
                },
                {
                    "1e-2",
                    0.01
                },
                {
                    "1.56e3",
                    1560.0
                },
                {
                    "0xfp2",
                    15 * 4.0
                },
                {
                    "0X4p-2",
                    4 * 0.25
                },
                {
                    "0xf.fp-3",
                    (15.0 + 15.0 / 16.0) * 0.125
                }
            };

            foreach (var test in tests) {
                var result = Utility.parse(test.Key, Parser.PrimaryExpression().End());
                Assert.AreEqual(1, result.Count());
                Assert.IsFalse(result.First().Remain.More());
                Assert.IsTrue(result.First().Value is ASTConstFloat);
                var ast = result.First().Value as ASTConstFloat;
                Assert.AreEqual(test.Value, ast.value, 0.0001);
            }
        }
    }
}
