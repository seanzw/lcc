using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RegExTests {
    [TestClass]
    public class DFATests {
        [TestMethod]
        public void dfa_compressed() {

            string src = @"\$(\\[^\n\r\t]|[^\n\r\t\$\\])*\$";

            RegEx.RegEx regex = new RegEx.RegEx(src);
            RegEx.DFA original = regex.dfa;
            var compressed = original.CompressMap();
            RegEx.DFA reconstructed = new RegEx.DFA(original.table, original.final, compressed.Item1, compressed.Item2);

            for (int i = 0; i < original.map.Length; ++i) {
                Assert.AreEqual(original.map[i], reconstructed.map[i]);
            }

            original.Reset();
            reconstructed.Reset();
            string test = @"$\$(\\[^\n\r\t]|[^\n\r\t\$\\])*\$$";
            foreach (var c in test) {
                original.Scan(c);
                reconstructed.Scan(c);
                Assert.AreEqual(original.status, reconstructed.status);
            }
        }
    }
}
