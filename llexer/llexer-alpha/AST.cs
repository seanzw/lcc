using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RegEx;

namespace llexer_alpha {

    /**************************************************

    lex
        : codes SPLITER rules SPLITER codes
        ;

    rules
        : rule rules_tail
        ;

    rules_tail
        : rules
        | epsilon
        ;

    rule
        : REGEX_LITERAL codes
        ;

    codes
        : CODE_LINE codes
        | epsilon
        ;
    
    ***************************************************/

    public abstract class ASTNode {
        public ASTNode() { }

        /* Convert the reg AST to a string. */
        abstract public string ToString(int level);
        public override string ToString() {
            return ToString(0);
        }
        protected string tab(int n) {
            string ret = "";
            for (int i = 0; i < n; ++i) {
                ret += "    ";
            }
            return ret;
        }
    }

    public class ASTLex : ASTNode {
        public ASTLex(List<string> headers, List<ASTRule> rules, List<string> codes) {
            this.headers = headers;
            this.rules = rules;
            this.codes = codes;
        }

        public override string ToString(int level) {
            string str = tab(level);
            str += "Lex: \n";
            for (int i = rules.Count() - 1; i >= 0; --i) {
                str += rules[i].ToString(level + 1);
                str += '\n';
            }
            return str;
        }

        public string writeLexer() {

            StringBuilder src = new StringBuilder();

            #region Helpers.
            Action<int, List<string>> writeLines = (level, ss) => {
                foreach (var s in ss) {
                    src.AppendLine(tab(level) + s);
                }
            };

            Action<int, IEnumerable<int>, bool> writeIntList = (level, ss, bracket) => {
                src.Append(tab(level));
                if (bracket) src.Append("{ ");
                foreach (int s in ss) {
                    src.Append(s + ", ");
                }
                if (bracket) src.AppendLine("},");
                else src.AppendLine();
            };

            Action<int, IEnumerable<bool>, bool> writeBoolList = (level, ss, bracket) => {
                src.Append(tab(level));
                if (bracket) src.Append("{");
                foreach (bool s in ss) {
                    src.Append((s ? "true" : "false") + ", ");
                }
                if (bracket) src.AppendLine("},");
                else src.AppendLine();
            };

            Action<int, string, int[]> writeIntArrayInitializer = (level, name, ss) => {
                src.AppendLine(tab(level) + "int[] " + name + " = new int[" + ss.Count() + "] {");
                writeIntList(level + 1, ss, false);
                src.AppendLine(tab(level) + "};");
            };

            Action<int, string, bool[]> writeBoolArrayInitializer = (level, name, ss) => {
                src.AppendLine(tab(level) + "bool[] " + name + " = new bool[" + ss.Count() + "] {");
                writeBoolList(level + 1, ss, false);
                src.AppendLine(tab(level) + "};");
            };

            Action<int, string, int[,]> writeIntMatInitializer = (level, name, ss) => {
                src.AppendLine(tab(level) + "int[,] " + name + " = new int[,] {");
                for (int i = 0; i < ss.GetLength(0); ++i) {
                    src.Append(tab(level + 1));
                    src.Append("{ ");
                    for (int j = 0; j < ss.GetLength(1); ++j) { 
                        src.Append(ss[i, j] + ", ");
                    }
                    src.AppendLine("},");
                }
                src.AppendLine(tab(level) + "};");
            };
            #endregion

            #region Header.
            List<string> headers = new List<string> {
                "using System;",
                "using System.Linq;",
                "using System.Text;",
                "using System.Collections.Generic;",
                "using RegEx;"
            };
            writeLines(0, headers);
            writeLines(0, this.headers);
            #endregion

            #region LexerBody.
            src.AppendLine("namespace llexer {");
            src.AppendLine(tab(1) + "public sealed class Lexer {");

            #region Attributes.
            List<string> attributes = new List<string> {
                "private int _idx;",
                "private string _src;",
                "private StringBuilder _lltext;",
                "private const char NONE = (char)0;",
                "private readonly List<DFA> _dfas;",
                "private static readonly Lexer instance = new Lexer();"
            };
            writeLines(2, attributes);
            #endregion

            #region Instance.
            string instance = @"
        public static Lexer Instance {
            get {
                return instance;
            }
        }";
            src.AppendLine(instance);
            #endregion

            #region Scan.
            string scan = @"
        public List<Token> scan(string src) {

            _idx = 0;
            _src = src;

            List<Token> lltokens = new List<Token>();

            Func<DFA.Status, int> _find = s => {
                for (int i = 0; i < _dfas.Count(); ++i)
                    if (_dfas[i].status() == s)
                        return i;
                return -1;
            };

            Action _reset = () => {
                _dfas.ForEach(dfa => dfa.reset());
                _lltext.Clear();
            };

            _reset();
            while (more()) {
                _dfas.ForEach(dfa => dfa.scan(peek()));
                if (_find(DFA.Status.RUN) != -1) {
                    next();
                    continue;
                }
                int _rule = _find(DFA.Status.SUCCEED);
                if (_rule != -1) {
                    // Find a token.
                    _action(_rule, ref lltokens);
                    _reset();
                    _dfas.ForEach(dfa => dfa.scan(peek()));
                    next();
                } else {
                    // Cannot match this token.
                    llerror(lltext);
                }
            }

            // Feed the EOF.
            if (_find(DFA.Status.RUN) != -1) {
                _dfas.ForEach(dfa => dfa.scan(NONE));
                int _rule = _find(DFA.Status.SUCCEED);
                if (_rule != -1) {
                    _action(_rule, ref lltokens);
                } else {
                    // Cannot match this token.
                    llerror(lltext);
                }
            }

            return lltokens;
        }";
            src.AppendLine(scan);




            #endregion

            #region APIs.
            string more = @"
        private bool more() {
            return _idx < _src.Length;
        }";
            src.AppendLine(more);

            string peek = @"
        private char peek() {
            if (more()) {
                return _src[_idx];
            } else {
                return NONE;
            }
        }";
            src.AppendLine(peek);

            string next = @"
        private char next() {
            if (more()) {
                _lltext.Append(_src[_idx]);
                return _src[_idx++];
            } else {
                return NONE;
            }
        }";
            src.AppendLine(next);

            string llerror = @"
        private void llerror(string msg) {
            throw new ArgumentException(""Lexer Error: "" + msg);
        }";
            src.AppendLine(llerror);

            string lltext = @"
        private string lltext {
            get {
                return _lltext.ToString();
            }
        }";
            src.AppendLine(lltext);

            #endregion

            #region Initialize.
            src.AppendLine(tab(2) + "private Lexer() {");
            src.AppendLine(tab(3) + "_lltext = new StringBuilder();");
            src.AppendLine(tab(3) + "_dfas = new List<DFA>(" + rules.Count() + ");");

            for (int i = rules.Count() - 1; i >= 0; --i) {
                src.AppendLine(tab(3) + "#region RULE " + (rules.Count() - i - 1));
                src.AppendLine(tab(3) + "{");

                writeBoolArrayInitializer(4, "final", rules[i].dfa.final);
                writeIntMatInitializer(4, "table", rules[i].dfa.table);

                var compressed = rules[i].dfa.shrinkMap();
                writeIntArrayInitializer(4, "range", compressed.Item1);
                writeIntArrayInitializer(4, "value", compressed.Item2);

                src.AppendLine(tab(4) + "_dfas.Add(new DFA(table, final, range, value));");

                src.AppendLine(tab(3) + "}");
                src.AppendLine(tab(3) + "#endregion");
            }


            src.AppendLine(tab(2) + "}");
            #endregion

            #region Action.
            src.AppendLine(tab(2) + "private void _action(int _rule, ref List<Token> lltokens) {");
            src.AppendLine(tab(3) + "switch (_rule) {");
            for (int i = 0; i < rules.Count(); ++i) {
                src.AppendLine(tab(4) + "case " + i + ":");
                writeLines(5, rules[rules.Count() - i - 1].codes);
                src.AppendLine(tab(5) + "break;");
            }
            src.AppendLine(tab(4) + "default:");
            src.AppendLine(tab(5) + "llerror(\"UNKNOWN RULE: THIS SHOULD NEVER HAPPEN!\");");
            src.AppendLine(tab(5) + "break;");
            src.AppendLine(tab(3) + "}");
            src.AppendLine(tab(2) + "}");
            #endregion

            #region User code.
            writeLines(2, codes);
            #endregion

            src.AppendLine(tab(1) + "}");
            src.AppendLine("}");
            #endregion

            return src.ToString();

        }

        public readonly List<string> headers;
        public readonly List<ASTRule> rules;
        public readonly List<string> codes;
    }

    public class ASTRule : ASTNode {
        public ASTRule(string regex, List<string> codes) {
            this.regex = regex;
            this.codes = codes;

            // Construct the DFA.
            Parser parser = new Parser();
            dfa = parser.parse(regex).gen().toDFATable().toDFA();
        }

        public override string ToString(int level) {
            string str = tab(level);
            str += "Rule: ";
            str += regex;
            str += '\n';
            foreach (var code in codes) {
                str += tab(level + 1) + code + "\n";
            }
            return str;
        }

        public readonly DFA dfa;
        public readonly string regex;
        public readonly List<string> codes;
    }
}
