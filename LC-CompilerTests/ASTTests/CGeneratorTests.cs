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

            var tests = new List<String> {
                "void_func",
                "quick_sort",
            };

            //var sources = Directory.GetFiles("../../ASTTests/code", "*.c");
            foreach (var test in tests) {

                var source = string.Format("../../ASTTests/code/{0}.c", test);

                string lcc_s = string.Format("{0}_lcc.s", source.Substring(0, source.Length - 2));
                string clang_s = string.Format("{0}_clang.s", source.Substring(0, source.Length - 2));
                string lcc_exe = string.Format("{0}_lcc.exe", source.Substring(0, source.Length - 2));
                string clang_exe = string.Format("{0}_clang.exe", source.Substring(0, source.Length - 2));

                string main = string.Format("{0}_main.c", source.Substring(0, source.Length - 2));
                string main_s = string.Format("{0}_main.s", source.Substring(0, source.Length - 2));

                // Compile with lcc.
                string src = File.ReadAllText(source);
                File.WriteAllText(lcc_s, Utility.CGen(src));

                // Compile with clang.
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "clang";

                p.StartInfo.Arguments = string.Format("-S {0} -masm=intel -o {1}", source, clang_s);
                p.Start();
                p.WaitForExit();

                // Compile the main with clang.
                p.StartInfo.Arguments = string.Format("-S {0} -masm=intel -o {1}", main, main_s);
                p.Start();
                p.WaitForExit();

                // Link them together.
                p.StartInfo.Arguments = string.Format("{0} {1} -o {2}", lcc_s, main_s, lcc_exe);
                p.Start();
                p.WaitForExit();

                p.StartInfo.Arguments = string.Format("{0} {1} -o {2}", clang_s, main_s, clang_exe);
                p.Start();
                p.WaitForExit();

                // Run both program and compare the stdout.
                p.StartInfo.FileName = clang_exe;
                p.StartInfo.Arguments = "";
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                string clang_out = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                p.StartInfo.FileName = lcc_exe;
                p.StartInfo.Arguments = "";
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                string lcc_out = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                Assert.AreEqual(clang_out, lcc_out);

                p.Close();
            }
        }

    }
}
