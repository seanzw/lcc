using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using llexer_alpha;

namespace llexer_alpha_test {
    [TestClass]
    public class RegTest {
        [TestMethod]
        public void testParse() {
            StreamReader f = new StreamReader("../../reg.test");
            while (f.Peek() >= 0) {
                string src = f.ReadLine();
                if (src[0] == '#') {
                    continue;
                }
                Console.WriteLine(src);
                ASTRegEx reg;
                int pos = 0;
                
                Parser.parse_reg(src, ref pos, out reg);
                Console.WriteLine(reg.ToString(0));
                NFATable nfa = reg.toNFA();
                Console.WriteLine(nfa);
                //Console.WriteLine(nfa.toDFATable());
                Console.WriteLine(nfa.toDFATable().simplify());
            }
            f.Close();
        }

        [TestMethod]
        public void testDFA() {
            StreamReader f = new StreamReader("../../reg.test");

            DFA dfa;
            DFATable table;
            {
                string src = "[\\a]*";
                int pos = 0;
                ASTRegEx reg;
                Parser.parse_reg(src, ref pos, out reg);
                table = reg.toNFA().toDFATable().simplify();
                dfa = table.toDFA();
            }

            while (f.Peek() >= 0) {
                string src = f.ReadLine();
                if (src[0] != '#') {
                    Console.WriteLine(src);
                    ASTRegEx reg;
                    int pos = 0;
                    Parser.parse_reg(src, ref pos, out reg);
                    table = reg.toNFA().toDFATable().simplify();
                    dfa = table.toDFA();
                } else {
                    dfa.reset();
                    DFA.Status truth = src[2] == '1' ? DFA.Status.SUCCEED : DFA.Status.FAILED;
                    for (int i = 4; i < src.Length; ++i) {
                        dfa.scan(src[i]);
                    }
                    // Feed EOF to dfa.
                    dfa.scan(Utility.EOF);
                    Assert.AreEqual(dfa.status(), truth);
                    if (dfa.status() == truth) {
                        Console.Write("SUCCEED! Src: ");
                        Console.Write(src.Substring(4));
                        Console.Write(' ');
                        Console.WriteLine(dfa.status());
                    } else {
                        // Do the test again and print out the information.
                        Console.WriteLine("Failed!");
                        Console.WriteLine(table);
                        Console.WriteLine(src.Substring(4));
                        dfa.reset();
                        for (int i = 4; i < src.Length; ++i) {
                            Console.Write("Eating ");
                            Console.Write(src[i]);
                            Console.Write(" ");
                            dfa.scan(src[i]);
                            Console.WriteLine(dfa.status());
                        }
                        dfa.scan(Utility.EOF);
                        Console.WriteLine(dfa.status());
                    }
                }
            }
            f.Close();
        }
    }
}
