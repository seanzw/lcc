using System.Numerics;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.TypeSystem;
using lcc.Token;
using lcc.SyntaxTree;
using Parserc;

namespace LC_CompilerTests {

    [TestClass]
    public class CGeneratorTests {

        [TestMethod]
        public void LCCCGeneratorTest() {
            var sources = Directory.GetFiles("../../ASTTests/code", "*.c");
            foreach (var source in sources) {

                string lcc = string.Format("{0}_lcc.s", source.Substring(0, source.Length - 2));
                string clang = string.Format("{0}_clang.s", source.Substring(0, source.Length - 2));

                string src = File.ReadAllText(source);
                File.WriteAllText(lcc, Utility.CGen(src));

                // Compile with clang.
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "clang";
                p.StartInfo.Arguments = string.Format("-S {0} -masm=intel -o {1}", source, clang);
                p.Start();

                p.WaitForExit();
                p.Close();
            }
        }

    }
}
