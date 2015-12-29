using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegEx;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RegExTests {
    [TestClass]
    public class RegExTests {

        [TestMethod]
        public void reg_plus_match() {

            string src = "a+b";
            Dictionary<string, bool> tests = new Dictionary<string, bool> {
                { "b", false },
                { "a", false },
                { "ab", true },
                { "ac", false },
                { "aaabc", false },
                { "aaaab", true }
            };

            RegEx.RegEx regex = new RegEx.RegEx(src);

            foreach (var test in tests) {
                Assert.AreEqual(regex.Match(test.Key), test.Value);
            }
        }

        [TestMethod]
        public void reg_charset_match() {

            string src = "[a-zA-Z_][a-zA-Z0-9_]*";
            Dictionary<string, bool> tests = new Dictionary<string, bool> {
                { "reg_charset_match",  true },
                { "src",                true },
                { "_Assert",            true },
                { "/**/",               false },
                { "\\n\\t\\r",          false },
                { "0_wrong",            false },
                { "__init__",           true },
                { "a",                  true }
            };

            RegEx.RegEx regex = new RegEx.RegEx(src);

            foreach (var test in tests) {
                Assert.AreEqual(regex.Match(test.Key), test.Value);
            }
        }

        [TestMethod]
        public void reg_wild_match() {

            string src = "(\\\\.|[^$\\\\])*";
            Dictionary<string, bool> tests = new Dictionary<string, bool> {
                { "\\a",        true },
                { "abc",          true },
                { "\\\n",     true },
                { "\\1",        true },
                { "\\,",        true },
                { "\\&",        true },
                { "\\(",        true },
                { "\\ad",      true },
                { "\\acc",         true }
            };

            RegEx.RegEx regex = new RegEx.RegEx(src);

            foreach (var test in tests) {
                Assert.AreEqual(regex.Match(test.Key), test.Value);
            }
        }

        [TestMethod]
        public void reg_exact_match() {

            string src = "reg_str_match";
            Dictionary<string, bool> tests = new Dictionary<string, bool> {
                { "reg_str_match",      true },
                { "reg_",               false },
                { "reg_str_match_",     false },
                { "src",                false },
                { "_Assert",            false },
                { "/**/",               false },
                { "\\n\\t\\r",          false },
                { "0_wrong",            false },
                { "__init__",           false },
                { "a",                  false }
            };

            RegEx.RegEx regex = new RegEx.RegEx(src);

            foreach (var test in tests) {
                Assert.AreEqual(regex.Match(test.Key), test.Value);
            }
        }

        [TestMethod]
        public void reg_regex_match() {

            string src = @"$(\\[^\n\r\t]|[^\n\r\t$\\])*$";
            Dictionary<string, bool> tests = new Dictionary<string, bool> {
                { "$\\\\$",     true },
                { "$\\\n$",     false },
                { "$\n$",       false },
                { "$\\\r\n$",   false }
            };

            RegEx.RegEx regex = new RegEx.RegEx(src);

            foreach (var test in tests) {
                Assert.AreEqual(regex.Match(test.Key), test.Value);
            }
        }

        [TestMethod]
        public void reg_str_match() {

            string src = @"L?""(\\(.|\r\n)|[^\n""\\])*""";
            Dictionary<string, bool> tests = new Dictionary<string, bool> {
                { "\"abc\"",     true },
                { "\"abc",       false },
                { "abc\"",       false },
                { "\"\n\"",      false },
                { "\"\\\n\"",    true },
                { "\"\\\r\n\"",  true },
                { "\"ab\r\n\"",  false }
            };

            RegEx.RegEx regex = new RegEx.RegEx(src);

            foreach (var test in tests) {
                Assert.AreEqual(regex.Match(test.Key), test.Value);
            }
        }

        [TestMethod]
        public void reg_charset_neg_match() {

            string src = "[^\\n]+";
            Dictionary<string, bool> tests = new Dictionary<string, bool> {
                { "reg_str_match",      true },
                { "reg_",               true },
                { "reg_str_match_",     true },
                { "src",                true },
                { "_Assert",            true },
                { "/**/",               true },
                { "\\n\\t\\r",          true },
                { "0_wrong",            true },
                { "__init__",           true },
                { "a",                  true },
                { "abdce\n",            false }
            };

            RegEx.RegEx regex = new RegEx.RegEx(src);

            foreach (var test in tests) {
                Assert.AreEqual(regex.Match(test.Key), test.Value);
            }
        }

        [TestMethod]
        public void reg_replace() {

            string src = @"\r\n?|\n";
            string patch = "\n";
            Dictionary<string, string> tests = new Dictionary<string, string> {
                { "aaa",    "aaa" },
                { "babca",  "babca" },
                { "a\r\nb", "a\nb" }
            };

            RegEx.RegEx regex = new RegEx.RegEx(src);

            foreach (var test in tests) {
                Assert.AreEqual(regex.Replace(test.Key, patch), test.Value);
            }
        }
    }
}
