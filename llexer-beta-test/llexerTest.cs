using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace llexer_beta_test {
    [TestClass]
    public class LexerTest {
        [TestMethod]
        public void testLexer() {

            StreamReader t = new StreamReader("../../../llexer-beta/llexer-beta.ll");
            string src = t.ReadToEnd();

            var tokens = llexer.Main.scan(src);
            foreach (var s in tokens) {
                Console.WriteLine(s);
            }

            t.Close();
        }
    }
}
