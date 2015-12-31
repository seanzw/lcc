using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RegEx;

namespace Lexer {

    
    public abstract class ASTNode {
        public ASTNode() { }

        /* Convert the reg AST to a string. */
        abstract public string ToString(int level);
        public override string ToString() {
            return ToString(0);
        }
        protected string Tab(int n) {
            string ret = "";
            for (int i = 0; i < n; ++i) {
                ret += "    ";
            }
            return ret;
        }
    }

    public class ASTLex : ASTNode {
        public ASTLex(LinkedList<string> headers, LinkedList<ASTRule> rules, LinkedList<string> codes) {
            this.headers = headers;
            this.rules = rules;
            this.codes = codes;
        }

        public override string ToString(int level) {
            string str = Tab(level);
            str += "Lex: \n";
            foreach (var rule in rules) {
                str += rule.ToString(level + 1);
                str += '\n';
            }
            return str;
        }

        public string WriteLexer() {

            StringBuilder src = new StringBuilder();

            #region Helpers.
            Action<int, IEnumerable<string>> WriteLines = (level, ss) => {
                foreach (var s in ss) {
                    src.AppendLine(Tab(level) + s);
                }
            };

            Action<int, IEnumerable<int>, bool> WriteIntList = (level, ss, bracket) => {
                src.Append(Tab(level));
                if (bracket) src.Append("{ ");
                foreach (int s in ss) {
                    src.Append(s + ", ");
                }
                if (bracket) src.AppendLine("},");
                else src.AppendLine();
            };

            Action<int, IEnumerable<bool>, bool> WriteBoolList = (level, ss, bracket) => {
                src.Append(Tab(level));
                if (bracket) src.Append("{");
                foreach (bool s in ss) {
                    src.Append((s ? "true" : "false") + ", ");
                }
                if (bracket) src.AppendLine("},");
                else src.AppendLine();
            };

            Action<int, string, int[]> WriteIntArrayInitializer = (level, name, ss) => {
                src.AppendLine(Tab(level) + "int[] " + name + " = new int[" + ss.Count() + "] {");
                WriteIntList(level + 1, ss, false);
                src.AppendLine(Tab(level) + "};");
            };

            Action<int, string, bool[]> WriteBoolArrayInitializer = (level, name, ss) => {
                src.AppendLine(Tab(level) + "bool[] " + name + " = new bool[" + ss.Count() + "] {");
                WriteBoolList(level + 1, ss, false);
                src.AppendLine(Tab(level) + "};");
            };

            Action<int, string, int[,]> WriteIntMatInitializer = (level, name, ss) => {
                src.AppendLine(Tab(level) + "int[,] " + name + " = new int[,] {");
                for (int i = 0; i < ss.GetLength(0); ++i) {
                    src.Append(Tab(level + 1));
                    src.Append("{ ");
                    for (int j = 0; j < ss.GetLength(1); ++j) { 
                        src.Append(ss[i, j] + ", ");
                    }
                    src.AppendLine("},");
                }
                src.AppendLine(Tab(level) + "};");
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
            WriteLines(0, headers);
            WriteLines(0, this.headers);
            #endregion

            #region LexerBody.
            src.AppendLine("namespace LLexer {");
            src.AppendLine(Tab(1) + "sealed class Lexer {");

            #region Attributes.
            List<string> attributes = new List<string> {
                "private int _idx;",
                "private int _line;",
                "private int _lineInc;",
                "private string _src;",
                "private StringBuilder _text;",
                "private const char NONE = (char)0;",
                "private readonly List<DFA> _dfas;",
                "private static readonly Lexer instance = new Lexer();"
            };
            WriteLines(2, attributes);
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
        public List<Token> Scan(string src) {

            _idx = 0;
            _line = 1;
            _lineInc = 0;
            _src = src;

            List<Token> tokens = new List<Token>();

            Func<DFA.Status, int> _find = s => {
                for (int i = 0; i < _dfas.Count(); ++i)
                    if (_dfas[i].status == s)
                        return i;
                return -1;
            };

            Action _reset = () => {
                _dfas.ForEach(dfa => dfa.Reset());
                _text.Clear();
                _line += _lineInc;
                _lineInc = 0;
            };

            _reset();
            while (More()) {
                _dfas.ForEach(dfa => dfa.Scan(Peek()));
                if (_find(DFA.Status.RUN) != -1) {
                    Next();
                    continue;
                }
                int _rule = _find(DFA.Status.SUCCEED);
                if (_rule != -1) {
                    // Find a token.
                    _Action(_rule, ref tokens);
                    _reset();
                    _dfas.ForEach(dfa => dfa.Scan(Peek()));
                    Next();
                } else {
                    // Cannot match this token.
                    Error(""Unmatched token "" + text);
                }
            }

            // Feed the EOF.
            if (_find(DFA.Status.RUN) != -1) {
                _dfas.ForEach(dfa => dfa.Scan(NONE));
                int _rule = _find(DFA.Status.SUCCEED);
                if (_rule != -1) {
                    _Action(_rule, ref tokens);
                } else {
                    // Cannot match this token.
                    Error(""Unmatched token "" + text);
                }
            }

            return tokens;
        }";
            src.AppendLine(scan);
            #endregion

            #region APIs.
            string more = @"
        private bool More() {
            return _idx < _src.Length;
        }";
            src.AppendLine(more);

            string peek = @"
        private char Peek() {
            if (More()) {
                return _src[_idx];
            } else {
                return NONE;
            }
        }";
            src.AppendLine(peek);

            string next = @"
        private char Next() {
            if (More()) {
                char c = _src[_idx++];
                _text.Append(c);
                if (c == '\n')
                    _lineInc++;
                return c;
            } else {
                return NONE;
            }
        }";
            src.AppendLine(next);

            string llerror = @"
        private void Error(string msg) {
            throw new ArgumentException(""Lexer Error: "" + msg);
        }";
            src.AppendLine(llerror);

            string lltext = @"
        private string text {
            get {
                return _text.ToString();
            }
        }";
            src.AppendLine(lltext);

            string lline = @"
        private int line {
            get {
                return _line;
            }
        }";
            src.AppendLine(lline);

            #endregion

            #region Initialize.
            src.AppendLine(Tab(2) + "private Lexer() {");
            src.AppendLine(Tab(3) + "_text = new StringBuilder();");
            src.AppendLine(Tab(3) + "_dfas = new List<DFA>(" + rules.Count() + ");");

            {
                int i = 0;
                foreach (var rule in rules) {
                    src.AppendLine(Tab(3) + "#region RULE " + i);
                    src.AppendLine(Tab(3) + "{");

                    WriteBoolArrayInitializer(4, "final", rule.dfa.final);
                    WriteIntMatInitializer(4, "table", rule.dfa.table);

                    var compressed = rule.dfa.CompressMap();
                    WriteIntArrayInitializer(4, "range", compressed.Item1);
                    WriteIntArrayInitializer(4, "value", compressed.Item2);

                    src.AppendLine(Tab(4) + "_dfas.Add(new DFA(table, final, range, value));");

                    src.AppendLine(Tab(3) + "}");
                    src.AppendLine(Tab(3) + "#endregion");
                    i++;
                }
            }


            src.AppendLine(Tab(2) + "}");
            #endregion

            #region Action.
            src.AppendLine(Tab(2) + "private void _Action(int _rule, ref List<Token> tokens) {");
            src.AppendLine(Tab(3) + "switch (_rule) {");

            {
                int i = 0;
                foreach (var rule in rules) {
                    src.AppendLine(Tab(4) + "case " + i + ":");
                    WriteLines(5, rule.codes);
                    src.AppendLine(Tab(5) + "break;");
                    i++;
                }
            }

            src.AppendLine(Tab(4) + "default:");
            src.AppendLine(Tab(5) + "Error(\"UNKNOWN RULE: THIS SHOULD NEVER HAPPEN!\");");
            src.AppendLine(Tab(5) + "break;");
            src.AppendLine(Tab(3) + "}");
            src.AppendLine(Tab(2) + "}");
            #endregion

            #region User code.
            WriteLines(2, codes);
            #endregion

            src.AppendLine(Tab(1) + "}");
            src.AppendLine("}");
            #endregion

            return src.ToString();

        }

        public readonly LinkedList<string> headers;
        public readonly LinkedList<ASTRule> rules;
        public readonly LinkedList<string> codes;
    }

    public class ASTRule : ASTNode {
        public ASTRule(string regex_literal, LinkedList<string> codes) {
            this.regex_literal = regex_literal;
            this.codes = codes;

            // Construct the DFA.
            RegEx.RegEx regex = new RegEx.RegEx(regex_literal);
            dfa = regex.dfa;
        }

        public override string ToString(int level) {
            string str = Tab(level);
            str += "Rule: ";
            str += regex_literal;
            str += '\n';
            foreach (var code in codes) {
                str += Tab(level + 1) + code + "\n";
            }
            return str;
        }

        public readonly DFA dfa;
        public readonly string regex_literal;
        public readonly LinkedList<string> codes;
    }
}
