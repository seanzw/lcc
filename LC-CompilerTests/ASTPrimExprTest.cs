using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.Type;
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
            Assert.AreEqual(1, result.Count);
            Assert.IsFalse(result[0].remain.More());
            Assert.IsTrue(result[0].value is ASTString);
            var ast = result[0].value as ASTString;
            Assert.IsTrue(truth.SequenceEqual(ast.Values));
        }
    }
}
