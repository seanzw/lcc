using System;
using System.Collections.Generic;
using RegEx;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RegExTests {
    [TestClass]
    public class ParserTests {
        [TestMethod]
        public void reg_basic_parsing() {
            List<String> srcs = new List<String> {
                "a",
                "a|b",
                "a*",
                "ab*c",
                "abc*|ca*",
                "$(\\\\a|[^$\\\\])*$",
            };
            Parser parser = new Parser();
            foreach (var src in srcs) {
                Console.WriteLine(parser.parse(src));
            }
        }

        [TestMethod]
        public void reg_bracket_parsing() {
            List<String> srcs = new List<String> {
                "(a|b)c",
                "(ab)(cd)(h|g)"
            };
            Parser parser = new Parser();
            foreach (var src in srcs) {
                Console.WriteLine(parser.parse(src));
            }
        }

        [TestMethod]
        public void reg_charset_parsing() {
            List<String> srcs = new List<String> {
                "[^\"a-z]",
                "[a-zA-Z0-9_]",
                "[a-z]",
                "[b-a]",
            };
            Parser parser = new Parser();
            foreach (var src in srcs) {
                Console.WriteLine(parser.parse(src));
            }
        }

        [TestMethod]
        public void reg_meta_parsing() {
            List<String> srcs = new List<String> {
                "a?",
                "a*",
                "a+"
            };
            Parser parser = new Parser();
            foreach (var src in srcs) {
                Console.WriteLine(parser.parse(src));
            }
        }

        [TestMethod]
        public void reg_gen() {
            List<String> srcs = new List<String> {
                "[a-zA-Z_][a-zA-Z0-9_]*",
                "a?",
                "a*",
                "a+",
                "$(\\\\a|[^$\\\\])*$"
            };
            Parser parser = new Parser();
            foreach (var src in srcs) {
                ASTExpression ast = parser.parse(src);
                NFATable nfa = ast.gen();
                Console.WriteLine(nfa);
            }
        }

        [TestMethod]
        public void reg_gen_dfa() {
            List<String> srcs = new List<String> {
                "a|b",
                "[a-zA-Z_][a-zA-Z0-9_]*",
                "a?",
                "a*",
                "a+",
                "$(\\\\a|[^$\\\\])*$"
            };
            Parser parser = new Parser();
            foreach (var src in srcs) {
                Console.WriteLine(src);
                ASTExpression ast = parser.parse(src);
                NFATable nfa = ast.gen();
                DFATable dfa = nfa.toDFATable();
                Console.WriteLine(dfa);
            }
        }
    }
}
