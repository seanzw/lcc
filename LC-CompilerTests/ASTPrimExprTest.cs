using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.Type;
using lcc.Token;
using lcc.AST;

namespace LC_CompilerTests {
    [TestClass]
    public partial class ASTTests {
        [TestMethod]
        public void LCCTCConstCharLegal() {

            var tests = new Dictionary<string, BigInteger> {
                {
                    "f",
                    'f'
                },
                {
                    "\\n",
                    '\n'
                },
                {
                    "\\'",
                    '\''
                },
                {
                    "\\c",
                    'c'
                },
                {
                    "\\377",
                    255
                },
                {
                    "\\xff",
                    255
                },
                {
                    "\\7",
                    7
                },
                {
                    "\\76",
                    7 * 8 + 6
                }
            };

            foreach (var test in tests) {
                var values = ASTConstChar.Evaluate(1, test.Key);
                Assert.AreEqual(1, values.Count);
                Assert.AreEqual(test.Value, values.First());
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
    }
}
