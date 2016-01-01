using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lcc.Test;

namespace lcc {
    class Program {
        static void Main(string[] args) {

            TestLexer.TestKeyword();
            TestLexer.TestIdentifier();
            TestLexer.TestConstantInt();
            TestLexer.TestConstantFloat();
            TestLexer.TestConstantChar();
            TestLexer.TestStringLiteral();
            TestLexer.TestPunctuator();
            TestLexer.TestComment();
            TestLexer.TestHelloWorld();
            TestLexer.TestIllegal();
        }
    }
}
