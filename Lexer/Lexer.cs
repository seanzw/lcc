using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using RegEx;
using Lexer;
namespace LLexer {
    public sealed class Lexer {
        private int _idx;
        private string _src;
        private StringBuilder _text;
        private const char NONE = (char)0;
        private readonly List<DFA> _dfas;
        private static readonly Lexer instance = new Lexer();

        public static Lexer Instance {
            get {
                return instance;
            }
        }

        public List<Token> Scan(string src) {

            _idx = 0;
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
                    Error("Unmatched token " + text);
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
                    Error("Unmatched token " + text);
                }
            }

            return tokens;
        }

        private bool More() {
            return _idx < _src.Length;
        }

        private char Peek() {
            if (More()) {
                return _src[_idx];
            } else {
                return NONE;
            }
        }

        private char Next() {
            if (More()) {
                _text.Append(_src[_idx]);
                return _src[_idx++];
            } else {
                return NONE;
            }
        }

        private void Error(string msg) {
            throw new ArgumentException("Lexer Error: " + msg);
        }

        private string text {
            get {
                return _text.ToString();
            }
        }
        private Lexer() {
            _text = new StringBuilder();
            _dfas = new List<DFA>(4);
            #region RULE 0
            {
                bool[] final = new bool[4] {
                    false, true, false, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, -1, },
                    { -1, -1, -1, },
                    { 1, 3, 2, },
                    { 2, 2, 2, },
                };
                int[] range = new int[10] {
                    0, 8, 10, 12, 13, 35, 36, 91, 92, 65535, 
                };
                int[] value = new int[10] {
                    -1, 2, -1, 2, -1, 2, 0, 2, 1, 2, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 1
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, -1, },
                    { 1, 1, },
                };
                int[] range = new int[11] {
                    0, 8, 9, 10, 12, 13, 31, 32, 35, 37, 65535, 
                };
                int[] value = new int[11] {
                    -1, 0, 1, -1, 0, -1, 0, 1, 0, 1, 0, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 2
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { 1, },
                };
                int[] range = new int[7] {
                    8, 10, 12, 13, 31, 32, 65535, 
                };
                int[] value = new int[7] {
                    -1, 0, -1, 0, -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 3
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, },
                    { -1, },
                    { 1, },
                };
                int[] range = new int[3] {
                    36, 37, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
        }
        private void _Action(int _rule, ref List<Token> tokens) {
            switch (_rule) {
                case 0:
                    tokens.Add(new T_REGEX(text.Substring(1, text.Length - 2)));
                    break;
                case 1:
                    tokens.Add(new T_CODE(text));
                    break;
                case 2:
                    break;
                case 3:
                    tokens.Add(new T_SPLITER());
                    break;
                default:
                    Error("UNKNOWN RULE: THIS SHOULD NEVER HAPPEN!");
                    break;
            }
        }
    }
}
