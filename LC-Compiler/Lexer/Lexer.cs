using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using RegEx;
using lcc.Token;
namespace LLexer {
    public sealed class Lexer {
        private int _idx;
        private int _line;
        private int _lineInc;
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
                char c = _src[_idx++];
                _text.Append(c);
                if (c == '\n')
                    _lineInc++;
                return c;
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

        private int line {
            get {
                return _line;
            }
        }
        private Lexer() {
            _text = new StringBuilder();
            _dfas = new List<DFA>(104);
            #region RULE 0
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    41, 42, 46, 47, 65535, 
                };
                int[] value = new int[5] {
                    -1, 1, -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 1
            {
                bool[] final = new bool[5] {
                    false, false, false, true, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, },
                    { -1, -1, 2, -1, },
                    { -1, -1, -1, 3, },
                    { -1, -1, -1, -1, },
                    { -1, 1, -1, -1, },
                };
                int[] range = new int[8] {
                    96, 97, 110, 111, 115, 116, 117, 65535, 
                };
                int[] value = new int[8] {
                    -1, 0, -1, 3, -1, 2, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 2
            {
                bool[] final = new bool[6] {
                    false, false, false, true, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, },
                    { -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, },
                    { -1, -1, 1, -1, -1, },
                };
                int[] range = new int[10] {
                    96, 97, 98, 100, 101, 106, 107, 113, 114, 65535, 
                };
                int[] value = new int[10] {
                    -1, 3, 0, -1, 2, -1, 4, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 3
            {
                bool[] final = new bool[5] {
                    false, false, false, true, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, },
                    { -1, -1, 2, -1, },
                    { -1, -1, -1, 3, },
                    { -1, -1, -1, -1, },
                    { -1, 1, -1, -1, },
                };
                int[] range = new int[9] {
                    96, 97, 98, 99, 100, 101, 114, 115, 65535, 
                };
                int[] value = new int[9] {
                    -1, 1, -1, 0, -1, 3, -1, 2, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 4
            {
                bool[] final = new bool[5] {
                    false, false, false, true, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, },
                    { -1, -1, 2, -1, },
                    { -1, -1, -1, 3, },
                    { -1, -1, -1, -1, },
                    { -1, 1, -1, -1, },
                };
                int[] range = new int[9] {
                    96, 97, 98, 99, 103, 104, 113, 114, 65535, 
                };
                int[] value = new int[9] {
                    -1, 2, -1, 0, -1, 1, -1, 3, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 5
            {
                bool[] final = new bool[6] {
                    false, false, false, true, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, },
                    { -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, },
                    { -1, -1, 1, -1, -1, },
                };
                int[] range = new int[9] {
                    98, 99, 109, 110, 111, 114, 115, 116, 65535, 
                };
                int[] value = new int[9] {
                    -1, 0, -1, 2, 1, -1, 3, 4, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 6
            {
                bool[] final = new bool[9] {
                    false, false, false, true, false, false, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, -1, -1, },
                    { -1, 7, -1, -1, -1, -1, -1, },
                    { -1, -1, 1, -1, -1, -1, -1, },
                    { -1, -1, -1, 8, -1, -1, -1, },
                    { -1, -1, 6, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, 5, -1, -1, },
                };
                int[] range = new int[13] {
                    98, 99, 100, 101, 104, 105, 109, 110, 111, 115, 116, 117, 65535, 
                };
                int[] value = new int[13] {
                    -1, 0, -1, 6, -1, 4, -1, 2, 1, -1, 3, 5, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 7
            {
                bool[] final = new bool[8] {
                    false, false, false, true, false, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, -1, -1, },
                    { -1, -1, 6, -1, -1, -1, -1, },
                    { -1, -1, -1, 7, -1, -1, -1, },
                    { -1, -1, -1, -1, 1, -1, -1, },
                };
                int[] range = new int[12] {
                    96, 97, 99, 100, 101, 102, 107, 108, 115, 116, 117, 65535, 
                };
                int[] value = new int[12] {
                    -1, 3, -1, 0, 1, 2, -1, 5, -1, 6, 4, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 8
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    99, 100, 110, 111, 65535, 
                };
                int[] value = new int[5] {
                    -1, 0, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 9
            {
                bool[] final = new bool[7] {
                    false, false, false, true, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, -1, },
                    { -1, -1, 6, -1, -1, -1, },
                    { -1, -1, -1, 1, -1, -1, },
                };
                int[] range = new int[12] {
                    97, 98, 99, 100, 101, 107, 108, 110, 111, 116, 117, 65535, 
                };
                int[] value = new int[12] {
                    -1, 3, -1, 0, 5, -1, 4, -1, 1, -1, 2, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 10
            {
                bool[] final = new bool[5] {
                    false, false, false, true, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, },
                    { -1, -1, 2, },
                    { 3, -1, -1, },
                    { -1, -1, -1, },
                    { -1, 1, -1, },
                };
                int[] range = new int[7] {
                    100, 101, 107, 108, 114, 115, 65535, 
                };
                int[] value = new int[7] {
                    -1, 0, -1, 1, -1, 2, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 11
            {
                bool[] final = new bool[5] {
                    false, false, false, true, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, },
                    { -1, -1, 2, -1, },
                    { -1, -1, -1, 3, },
                    { -1, -1, -1, -1, },
                    { -1, 1, -1, -1, },
                };
                int[] range = new int[8] {
                    100, 101, 108, 109, 110, 116, 117, 65535, 
                };
                int[] value = new int[8] {
                    -1, 0, -1, 3, 1, -1, 2, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 12
            {
                bool[] final = new bool[7] {
                    false, false, false, false, false, true, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, },
                    { -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, 5, },
                    { 1, -1, -1, -1, -1, },
                    { -1, 6, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, },
                    { -1, -1, 3, -1, -1, },
                };
                int[] range = new int[11] {
                    100, 101, 109, 110, 113, 114, 115, 116, 119, 120, 65535, 
                };
                int[] value = new int[11] {
                    -1, 0, -1, 4, -1, 3, -1, 2, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 13
            {
                bool[] final = new bool[6] {
                    false, false, false, true, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, },
                    { -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, },
                    { -1, -1, 1, -1, -1, },
                };
                int[] range = new int[11] {
                    96, 97, 101, 102, 107, 108, 110, 111, 115, 116, 65535, 
                };
                int[] value = new int[11] {
                    -1, 3, -1, 0, -1, 1, -1, 2, -1, 4, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 14
            {
                bool[] final = new bool[4] {
                    false, false, false, true, 
                };
                int[,] table = new int[,] {
                    { 1, -1, -1, },
                    { -1, 2, -1, },
                    { -1, -1, 3, },
                    { -1, -1, -1, },
                };
                int[] range = new int[7] {
                    101, 102, 110, 111, 113, 114, 65535, 
                };
                int[] value = new int[7] {
                    -1, 0, -1, 1, -1, 2, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 15
            {
                bool[] final = new bool[5] {
                    false, false, false, true, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, },
                    { -1, -1, 2, },
                    { -1, 3, -1, },
                    { -1, -1, -1, },
                    { -1, 1, -1, },
                };
                int[] range = new int[7] {
                    102, 103, 110, 111, 115, 116, 65535, 
                };
                int[] value = new int[7] {
                    -1, 0, -1, 1, -1, 2, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 16
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    101, 102, 104, 105, 65535, 
                };
                int[] value = new int[5] {
                    -1, 1, -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 17
            {
                bool[] final = new bool[7] {
                    false, false, false, false, false, true, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, },
                    { -1, 2, -1, -1, },
                    { -1, -1, -1, 5, },
                    { 1, -1, -1, -1, },
                    { -1, 6, -1, -1, },
                    { -1, -1, -1, -1, },
                    { -1, -1, 3, -1, },
                };
                int[] range = new int[9] {
                    100, 101, 104, 105, 107, 108, 109, 110, 65535, 
                };
                int[] value = new int[9] {
                    -1, 3, -1, 0, -1, 2, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 18
            {
                bool[] final = new bool[4] {
                    false, false, false, true, 
                };
                int[,] table = new int[,] {
                    { 1, -1, -1, },
                    { -1, 2, -1, },
                    { -1, -1, 3, },
                    { -1, -1, -1, },
                };
                int[] range = new int[7] {
                    104, 105, 109, 110, 115, 116, 65535, 
                };
                int[] value = new int[7] {
                    -1, 0, -1, 1, -1, 2, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 19
            {
                bool[] final = new bool[5] {
                    false, false, false, true, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, },
                    { -1, -1, 2, -1, },
                    { -1, -1, -1, 3, },
                    { -1, -1, -1, -1, },
                    { -1, 1, -1, -1, },
                };
                int[] range = new int[8] {
                    102, 103, 107, 108, 109, 110, 111, 65535, 
                };
                int[] value = new int[8] {
                    -1, 3, -1, 0, -1, 2, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 20
            {
                bool[] final = new bool[9] {
                    false, false, false, true, false, false, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, -1, },
                    { -1, 2, -1, -1, -1, -1, },
                    { 3, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, -1, },
                    { -1, -1, 6, -1, -1, -1, },
                    { -1, -1, -1, 7, -1, -1, },
                    { -1, -1, -1, -1, 8, -1, },
                    { -1, -1, -1, -1, -1, 1, },
                };
                int[] range = new int[11] {
                    100, 101, 102, 103, 104, 105, 113, 114, 115, 116, 65535, 
                };
                int[] value = new int[11] {
                    -1, 1, -1, 2, -1, 3, -1, 0, 4, 5, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 21
            {
                bool[] final = new bool[9] {
                    false, false, false, true, false, false, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, 2, },
                    { -1, -1, -1, 3, -1, -1, },
                    { -1, -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, -1, },
                    { -1, -1, 8, -1, -1, -1, },
                    { -1, -1, -1, -1, 1, -1, },
                    { 6, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, 7, -1, -1, },
                };
                int[] range = new int[11] {
                    98, 99, 100, 101, 104, 105, 113, 114, 115, 116, 65535, 
                };
                int[] value = new int[11] {
                    -1, 5, -1, 1, -1, 4, -1, 0, 2, 3, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 22
            {
                bool[] final = new bool[7] {
                    false, false, false, true, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, },
                    { 2, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, },
                    { -1, -1, 6, -1, -1, },
                    { -1, -1, -1, 1, -1, },
                };
                int[] range = new int[10] {
                    100, 101, 109, 110, 113, 114, 115, 116, 117, 65535, 
                };
                int[] value = new int[10] {
                    -1, 1, -1, 4, -1, 0, -1, 2, 3, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 23
            {
                bool[] final = new bool[6] {
                    false, false, false, true, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, },
                    { -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, },
                    { -1, -1, 1, -1, -1, },
                };
                int[] range = new int[9] {
                    103, 104, 110, 111, 113, 114, 115, 116, 65535, 
                };
                int[] value = new int[9] {
                    -1, 1, -1, 2, -1, 3, 0, 4, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 24
            {
                bool[] final = new bool[7] {
                    false, false, false, true, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, -1, },
                    { -1, -1, 6, -1, -1, -1, },
                    { -1, -1, -1, 1, -1, -1, },
                };
                int[] range = new int[12] {
                    99, 100, 101, 102, 103, 104, 105, 109, 110, 114, 115, 65535, 
                };
                int[] value = new int[12] {
                    -1, 5, 4, -1, 2, -1, 1, -1, 3, -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 25
            {
                bool[] final = new bool[7] {
                    false, false, false, true, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, -1, },
                    { -1, -1, 6, -1, -1, -1, },
                    { -1, -1, -1, 1, -1, -1, },
                };
                int[] range = new int[12] {
                    100, 101, 102, 104, 105, 110, 111, 114, 115, 121, 122, 65535, 
                };
                int[] value = new int[12] {
                    -1, 3, 5, -1, 1, -1, 4, -1, 0, -1, 2, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 26
            {
                bool[] final = new bool[7] {
                    false, false, false, true, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 6, -1, -1, -1, -1, },
                    { -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, },
                    { -1, 1, -1, -1, -1, },
                    { -1, -1, 4, -1, -1, },
                    { -1, 5, -1, -1, -1, },
                };
                int[] range = new int[10] {
                    96, 97, 98, 99, 104, 105, 114, 115, 116, 65535, 
                };
                int[] value = new int[10] {
                    -1, 2, -1, 4, -1, 3, -1, 0, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 27
            {
                bool[] final = new bool[7] {
                    false, false, false, true, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, 2, },
                    { -1, 3, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, },
                    { -1, -1, 6, -1, -1, },
                    { -1, -1, -1, 1, -1, },
                };
                int[] range = new int[8] {
                    98, 99, 113, 114, 115, 116, 117, 65535, 
                };
                int[] value = new int[8] {
                    -1, 4, -1, 2, 0, 1, 3, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 28
            {
                bool[] final = new bool[7] {
                    false, false, false, true, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, -1, },
                    { -1, -1, 6, -1, -1, -1, },
                    { -1, -1, -1, 1, -1, -1, },
                };
                int[] range = new int[11] {
                    98, 99, 103, 104, 105, 114, 115, 116, 118, 119, 65535, 
                };
                int[] value = new int[11] {
                    -1, 4, -1, 5, 2, -1, 0, 3, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 29
            {
                bool[] final = new bool[8] {
                    false, false, false, true, false, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, 2, -1, -1, },
                    { -1, -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, -1, },
                    { -1, -1, 6, -1, -1, -1, },
                    { -1, -1, -1, 7, -1, -1, },
                    { -1, -1, -1, -1, 1, -1, },
                };
                int[] range = new int[11] {
                    99, 100, 101, 102, 111, 112, 115, 116, 120, 121, 65535, 
                };
                int[] value = new int[11] {
                    -1, 4, 3, 5, -1, 2, -1, 0, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 30
            {
                bool[] final = new bool[6] {
                    false, false, false, true, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, },
                    { -1, -1, -1, 2, },
                    { -1, 3, -1, -1, },
                    { -1, -1, -1, -1, },
                    { -1, 5, -1, -1, },
                    { -1, -1, 1, -1, },
                };
                int[] range = new int[8] {
                    104, 105, 109, 110, 111, 116, 117, 65535, 
                };
                int[] value = new int[8] {
                    -1, 2, -1, 1, 3, -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 31
            {
                bool[] final = new bool[9] {
                    false, false, false, true, false, false, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 7, -1, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, -1, -1, },
                    { -1, 1, -1, -1, -1, -1, -1, },
                    { -1, -1, 6, -1, -1, -1, -1, },
                    { -1, -1, -1, 8, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, 4, -1, -1, },
                };
                int[] range = new int[14] {
                    99, 100, 101, 102, 103, 104, 105, 109, 110, 114, 115, 116, 117, 65535, 
                };
                int[] value = new int[14] {
                    -1, 6, 5, -1, 4, -1, 3, -1, 1, -1, 2, -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 32
            {
                bool[] final = new bool[5] {
                    false, false, false, true, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, },
                    { -1, -1, 2, -1, },
                    { -1, -1, -1, 3, },
                    { -1, -1, -1, -1, },
                    { -1, 1, -1, -1, },
                };
                int[] range = new int[9] {
                    99, 100, 104, 105, 110, 111, 117, 118, 65535, 
                };
                int[] value = new int[9] {
                    -1, 3, -1, 2, -1, 1, -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 33
            {
                bool[] final = new bool[9] {
                    false, false, false, true, false, false, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, -1, -1, },
                    { -1, -1, 2, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, -1, -1, },
                    { -1, -1, 6, -1, -1, -1, -1, },
                    { -1, -1, -1, 7, -1, -1, -1, },
                    { -1, -1, -1, -1, 8, -1, -1, },
                    { -1, -1, -1, -1, -1, 1, -1, },
                };
                int[] range = new int[15] {
                    96, 97, 100, 101, 104, 105, 107, 108, 110, 111, 115, 116, 117, 118, 65535, 
                };
                int[] value = new int[15] {
                    -1, 3, -1, 6, -1, 5, -1, 2, -1, 1, -1, 4, -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 34
            {
                bool[] final = new bool[6] {
                    false, false, false, true, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, },
                    { -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, },
                    { -1, -1, 1, -1, -1, },
                };
                int[] range = new int[10] {
                    100, 101, 103, 104, 105, 107, 108, 118, 119, 65535, 
                };
                int[] value = new int[10] {
                    -1, 4, -1, 1, 2, -1, 3, -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 35
            {
                bool[] final = new bool[6] {
                    false, false, false, true, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, },
                    { -1, -1, 2, -1, },
                    { -1, -1, -1, 3, },
                    { -1, -1, -1, -1, },
                    { -1, 5, -1, -1, },
                    { -1, -1, 1, -1, },
                };
                int[] range = new int[9] {
                    65, 66, 94, 95, 107, 108, 110, 111, 65535, 
                };
                int[] value = new int[9] {
                    -1, 1, -1, 0, -1, 3, -1, 2, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 36
            {
                bool[] final = new bool[9] {
                    false, false, false, true, false, false, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, -1, -1, -1, },
                    { -1, -1, 6, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, 7, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, 8, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, 1, -1, -1, },
                };
                int[] range = new int[15] {
                    66, 67, 94, 95, 100, 101, 107, 108, 109, 110, 111, 112, 119, 120, 65535, 
                };
                int[] value = new int[15] {
                    -1, 1, -1, 0, -1, 6, -1, 5, 3, -1, 2, 4, -1, 7, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 37
            {
                bool[] final = new bool[11] {
                    false, false, false, true, false, false, false, false, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, -1, -1, 2, -1, },
                    { -1, -1, -1, -1, -1, -1, -1, -1, 3, },
                    { -1, -1, -1, -1, -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, -1, -1, -1, -1, },
                    { -1, -1, 9, -1, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, 1, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, 8, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, 10, -1, -1, -1, },
                    { -1, -1, -1, 7, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, -1, 6, -1, -1, },
                };
                int[] range = new int[18] {
                    72, 73, 94, 95, 96, 97, 102, 103, 104, 105, 108, 109, 110, 113, 114, 120, 121, 65535, 
                };
                int[] value = new int[18] {
                    -1, 1, -1, 0, -1, 3, -1, 4, -1, 5, -1, 2, 6, -1, 7, -1, 8, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 38
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, -1, },
                    { 1, 1, },
                };
                int[] range = new int[9] {
                    47, 57, 64, 90, 94, 95, 96, 122, 65535, 
                };
                int[] value = new int[9] {
                    -1, 1, -1, 0, -1, 0, -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 39
            {
                bool[] final = new bool[7] {
                    false, true, true, true, true, true, false, 
                };
                int[,] table = new int[,] {
                    { 6, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, -1, },
                    { 2, -1, 2, 5, 3, 4, },
                    { -1, -1, -1, -1, 1, -1, },
                    { -1, -1, -1, -1, -1, 1, },
                    { -1, -1, -1, -1, 3, 4, },
                    { -1, 2, -1, -1, -1, -1, },
                };
                int[] range = new int[20] {
                    47, 48, 57, 64, 70, 75, 76, 84, 85, 87, 88, 96, 102, 107, 108, 116, 117, 119, 120, 65535, 
                };
                int[] value = new int[20] {
                    -1, 0, 2, -1, 2, -1, 5, -1, 3, -1, 1, -1, 2, -1, 4, -1, 3, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 40
            {
                bool[] final = new bool[7] {
                    false, false, false, false, true, false, false, 
                };
                int[,] table = new int[,] {
                    { 1, -1, -1, -1, -1, -1, },
                    { -1, 3, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, 4, },
                    { 3, -1, 3, 5, 6, -1, },
                    { -1, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, 2, -1, 4, },
                    { -1, -1, -1, -1, 2, 4, },
                };
                int[] range = new int[20] {
                    47, 48, 57, 64, 70, 75, 76, 84, 85, 87, 88, 96, 102, 107, 108, 116, 117, 119, 120, 65535, 
                };
                int[] value = new int[20] {
                    -1, 0, 2, -1, 2, -1, 4, -1, 5, -1, 1, -1, 2, -1, 3, -1, 5, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 41
            {
                bool[] final = new bool[6] {
                    false, true, true, true, true, true, 
                };
                int[,] table = new int[,] {
                    { 2, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, },
                    { 2, 2, 5, 3, 4, },
                    { -1, -1, -1, 1, -1, },
                    { -1, -1, -1, -1, 1, },
                    { -1, -1, -1, 3, 4, },
                };
                int[] range = new int[12] {
                    47, 48, 55, 75, 76, 84, 85, 107, 108, 116, 117, 65535, 
                };
                int[] value = new int[12] {
                    -1, 0, 1, -1, 4, -1, 2, -1, 3, -1, 2, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 42
            {
                bool[] final = new bool[6] {
                    false, true, false, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 3, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, 1, },
                    { 3, 3, 4, 5, -1, },
                    { -1, -1, 2, -1, 1, },
                    { -1, -1, -1, 2, 1, },
                };
                int[] range = new int[12] {
                    47, 48, 55, 75, 76, 84, 85, 107, 108, 116, 117, 65535, 
                };
                int[] value = new int[12] {
                    -1, 0, 1, -1, 3, -1, 4, -1, 2, -1, 4, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 43
            {
                bool[] final = new bool[6] {
                    false, true, true, true, true, true, 
                };
                int[,] table = new int[,] {
                    { 2, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, },
                    { 2, 2, 5, 3, 4, },
                    { -1, -1, -1, 1, -1, },
                    { -1, -1, -1, -1, 1, },
                    { -1, -1, -1, 3, 4, },
                };
                int[] range = new int[12] {
                    47, 48, 57, 75, 76, 84, 85, 107, 108, 116, 117, 65535, 
                };
                int[] value = new int[12] {
                    -1, 1, 0, -1, 4, -1, 2, -1, 3, -1, 2, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 44
            {
                bool[] final = new bool[6] {
                    false, true, false, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 3, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, 1, },
                    { 3, 3, 4, 5, -1, },
                    { -1, -1, 2, -1, 1, },
                    { -1, -1, -1, 2, 1, },
                };
                int[] range = new int[12] {
                    47, 48, 57, 75, 76, 84, 85, 107, 108, 116, 117, 65535, 
                };
                int[] value = new int[12] {
                    -1, 1, 0, -1, 3, -1, 4, -1, 2, -1, 4, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 45
            {
                bool[] final = new bool[7] {
                    false, true, true, false, true, false, false, 
                };
                int[,] table = new int[,] {
                    { 0, 6, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, },
                    { 2, -1, -1, -1, 1, },
                    { 2, -1, -1, -1, -1, },
                    { 4, -1, 5, -1, 1, },
                    { 2, -1, -1, 3, -1, },
                    { 4, -1, -1, -1, -1, },
                };
                int[] range = new int[18] {
                    42, 43, 44, 45, 46, 47, 57, 68, 69, 70, 75, 76, 100, 101, 102, 107, 108, 65535, 
                };
                int[] value = new int[18] {
                    -1, 3, -1, 3, 1, -1, 0, -1, 2, 4, -1, 4, -1, 2, 4, -1, 4, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 46
            {
                bool[] final = new bool[7] {
                    false, true, true, false, false, true, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, },
                    { 2, -1, -1, -1, 1, },
                    { 2, -1, -1, -1, -1, },
                    { 4, 5, -1, -1, -1, },
                    { -1, -1, 6, -1, 1, },
                    { 2, -1, -1, 3, -1, },
                };
                int[] range = new int[18] {
                    42, 43, 44, 45, 46, 47, 57, 68, 69, 70, 75, 76, 100, 101, 102, 107, 108, 65535, 
                };
                int[] value = new int[18] {
                    -1, 3, -1, 3, 1, -1, 0, -1, 2, 4, -1, 4, -1, 2, 4, -1, 4, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 47
            {
                bool[] final = new bool[6] {
                    false, true, true, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, -1, -1, -1, },
                    { -1, -1, -1, -1, },
                    { 2, -1, -1, 1, },
                    { 2, -1, -1, -1, },
                    { 4, 5, -1, -1, },
                    { 2, -1, 3, -1, },
                };
                int[] range = new int[17] {
                    42, 43, 44, 45, 47, 57, 68, 69, 70, 75, 76, 100, 101, 102, 107, 108, 65535, 
                };
                int[] value = new int[17] {
                    -1, 2, -1, 2, -1, 0, -1, 1, 3, -1, 3, -1, 1, 3, -1, 3, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 48
            {
                bool[] final = new bool[9] {
                    false, false, true, false, false, true, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 1, -1, -1, -1, -1, -1, -1, -1, -1, },
                    { -1, 6, -1, -1, -1, -1, -1, -1, -1, },
                    { 2, -1, 2, 5, -1, -1, -1, -1, 5, },
                    { 2, -1, 2, -1, -1, -1, -1, -1, -1, },
                    { 7, -1, 7, 7, 7, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, -1, -1, -1, -1, },
                    { 6, -1, 6, 6, 6, 4, -1, -1, -1, },
                    { 7, -1, 7, 7, 7, -1, 8, -1, -1, },
                    { 2, -1, 2, -1, -1, -1, -1, 3, -1, },
                };
                int[] range = new int[27] {
                    42, 43, 44, 45, 46, 47, 48, 57, 64, 69, 70, 75, 76, 79, 80, 87, 88, 96, 101, 102, 107, 108, 111, 112, 119, 120, 65535, 
                };
                int[] value = new int[27] {
                    -1, 7, -1, 7, 5, -1, 0, 2, -1, 4, 3, -1, 8, -1, 6, -1, 1, -1, 4, 3, -1, 8, -1, 6, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 49
            {
                bool[] final = new bool[9] {
                    false, false, true, false, true, false, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 7, -1, -1, -1, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, -1, 8, -1, -1, },
                    { 2, -1, 2, 4, -1, -1, -1, -1, 4, },
                    { 2, -1, 2, -1, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, -1, -1, -1, -1, },
                    { 5, -1, 5, 5, 5, 1, 8, -1, -1, },
                    { 5, -1, 5, 5, 5, -1, -1, -1, -1, },
                    { -1, 6, -1, -1, -1, -1, -1, -1, -1, },
                    { 2, -1, 2, -1, -1, -1, -1, 3, -1, },
                };
                int[] range = new int[27] {
                    42, 43, 44, 45, 46, 47, 48, 57, 64, 69, 70, 75, 76, 79, 80, 87, 88, 96, 101, 102, 107, 108, 111, 112, 119, 120, 65535, 
                };
                int[] value = new int[27] {
                    -1, 7, -1, 7, 5, -1, 0, 2, -1, 4, 3, -1, 8, -1, 6, -1, 1, -1, 4, 3, -1, 8, -1, 6, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 50
            {
                bool[] final = new bool[9] {
                    false, false, false, true, false, false, false, false, false, 
                };
                int[,] table = new int[,] {
                    { 4, 5, -1, -1, -1, -1, -1, -1, },
                    { 2, 2, 8, 6, 2, 2, 2, 2, },
                    { -1, 3, -1, -1, -1, -1, -1, -1, },
                    { -1, -1, -1, -1, -1, -1, -1, -1, },
                    { -1, 5, -1, -1, -1, -1, -1, -1, },
                    { 2, -1, 2, 2, 2, 2, 1, -1, },
                    { -1, 3, 6, -1, 6, -1, -1, -1, },
                    { -1, 3, 2, -1, -1, -1, -1, -1, },
                    { -1, 3, 7, -1, -1, -1, -1, -1, },
                };
                int[] range = new int[21] {
                    0, 9, 10, 12, 13, 38, 39, 47, 55, 57, 64, 70, 75, 76, 91, 92, 96, 102, 119, 120, 65535, 
                };
                int[] value = new int[21] {
                    -1, 5, 7, 5, 7, 5, 1, 5, 2, 4, 5, 4, 5, 0, 5, 6, 5, 4, 5, 3, 5, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 51
            {
                bool[] final = new bool[6] {
                    false, false, false, true, false, false, 
                };
                int[,] table = new int[,] {
                    { 1, 2, -1, -1, -1, -1, -1, -1, -1, },
                    { -1, 2, -1, -1, -1, -1, -1, -1, -1, },
                    { 2, 3, 2, 2, 2, 2, 4, -1, -1, },
                    { -1, -1, -1, -1, -1, -1, -1, -1, -1, },
                    { 2, 2, 2, 2, 2, 2, 2, 5, 2, },
                    { 2, 3, 2, 2, 2, 2, 4, -1, 2, },
                };
                int[] range = new int[21] {
                    0, 9, 10, 12, 13, 33, 34, 47, 55, 57, 64, 70, 75, 76, 91, 92, 96, 102, 119, 120, 65535, 
                };
                int[] value = new int[21] {
                    -1, 5, 8, 5, 7, 5, 1, 5, 2, 4, 5, 4, 5, 0, 5, 6, 5, 4, 5, 3, 5, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 52
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    57, 58, 59, 60, 65535, 
                };
                int[] value = new int[5] {
                    -1, 1, -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 53
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    57, 58, 61, 62, 65535, 
                };
                int[] value = new int[5] {
                    -1, 0, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 54
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    36, 37, 59, 60, 65535, 
                };
                int[] value = new int[5] {
                    -1, 1, -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 55
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    36, 37, 61, 62, 65535, 
                };
                int[] value = new int[5] {
                    -1, 0, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 56
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    90, 91, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 57
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    92, 93, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 58
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    39, 40, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 59
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    40, 41, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 60
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    122, 123, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 61
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    124, 125, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 62
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    45, 46, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 63
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    44, 45, 61, 62, 65535, 
                };
                int[] value = new int[5] {
                    -1, 0, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 64
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
                    42, 43, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 65
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
                    44, 45, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 66
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    37, 38, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 67
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    41, 42, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 68
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    42, 43, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 69
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    44, 45, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 70
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    125, 126, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 71
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    32, 33, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 72
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    46, 47, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 73
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
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
            #region RULE 74
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
                    59, 60, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 75
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
                    61, 62, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 76
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    59, 60, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 77
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    61, 62, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 78
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[4] {
                    59, 60, 61, 65535, 
                };
                int[] value = new int[4] {
                    -1, 0, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 79
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[4] {
                    60, 61, 62, 65535, 
                };
                int[] value = new int[4] {
                    -1, 1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 80
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
                    60, 61, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 81
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    32, 33, 60, 61, 65535, 
                };
                int[] value = new int[5] {
                    -1, 0, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 82
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    93, 94, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 83
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    123, 124, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 84
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
                    37, 38, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 85
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
                    123, 124, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 86
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    62, 63, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 87
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    57, 58, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 88
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    58, 59, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 89
            {
                bool[] final = new bool[4] {
                    false, false, false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { 2, },
                    { 3, },
                    { -1, },
                };
                int[] range = new int[3] {
                    45, 46, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 90
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    60, 61, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 91
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    41, 42, 60, 61, 65535, 
                };
                int[] value = new int[5] {
                    -1, 0, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 92
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    46, 47, 60, 61, 65535, 
                };
                int[] value = new int[5] {
                    -1, 0, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 93
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    36, 37, 60, 61, 65535, 
                };
                int[] value = new int[5] {
                    -1, 0, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 94
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    42, 43, 60, 61, 65535, 
                };
                int[] value = new int[5] {
                    -1, 0, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 95
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    44, 45, 60, 61, 65535, 
                };
                int[] value = new int[5] {
                    -1, 0, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 96
            {
                bool[] final = new bool[4] {
                    false, false, false, true, 
                };
                int[,] table = new int[,] {
                    { 1, -1, },
                    { 2, -1, },
                    { -1, 3, },
                    { -1, -1, },
                };
                int[] range = new int[4] {
                    59, 60, 61, 65535, 
                };
                int[] value = new int[4] {
                    -1, 0, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 97
            {
                bool[] final = new bool[4] {
                    false, false, false, true, 
                };
                int[,] table = new int[,] {
                    { 1, -1, },
                    { 2, -1, },
                    { -1, 3, },
                    { -1, -1, },
                };
                int[] range = new int[4] {
                    60, 61, 62, 65535, 
                };
                int[] value = new int[4] {
                    -1, 1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 98
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    37, 38, 60, 61, 65535, 
                };
                int[] value = new int[5] {
                    -1, 0, -1, 1, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 99
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    60, 61, 93, 94, 65535, 
                };
                int[] value = new int[5] {
                    -1, 1, -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 100
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { -1, -1, },
                    { -1, 1, },
                };
                int[] range = new int[5] {
                    60, 61, 123, 124, 65535, 
                };
                int[] value = new int[5] {
                    -1, 1, -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 101
            {
                bool[] final = new bool[2] {
                    false, true, 
                };
                int[,] table = new int[,] {
                    { 1, },
                    { -1, },
                };
                int[] range = new int[3] {
                    43, 44, 65535, 
                };
                int[] value = new int[3] {
                    -1, 0, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 102
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
            #region RULE 103
            {
                bool[] final = new bool[3] {
                    false, true, false, 
                };
                int[,] table = new int[,] {
                    { 2, -1, },
                    { 1, 1, },
                    { 1, -1, },
                };
                int[] range = new int[6] {
                    0, 9, 10, 46, 47, 65535, 
                };
                int[] value = new int[6] {
                    -1, 1, -1, 1, 0, 1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
        }
        private void _Action(int _rule, ref List<Token> tokens) {
            switch (_rule) {
                case 0:
                    Comment();
                    break;
                case 1:
                    tokens.Add(new T_KEY_AUTO(line));
                    break;
                case 2:
                    tokens.Add(new T_KEY_BREAK(line));
                    break;
                case 3:
                    tokens.Add(new T_KEY_CASE(line));
                    break;
                case 4:
                    tokens.Add(new T_KEY_CHAR(line));
                    break;
                case 5:
                    tokens.Add(new T_KEY_CONST(line));
                    break;
                case 6:
                    tokens.Add(new T_KEY_CONTINUE(line));
                    break;
                case 7:
                    tokens.Add(new T_KEY_DEFAULT(line));
                    break;
                case 8:
                    tokens.Add(new T_KEY_DO(line));
                    break;
                case 9:
                    tokens.Add(new T_KEY_DOUBLE(line));
                    break;
                case 10:
                    tokens.Add(new T_KEY_ELSE(line));
                    break;
                case 11:
                    tokens.Add(new T_KEY_ENUM(line));
                    break;
                case 12:
                    tokens.Add(new T_KEY_EXTERN(line));
                    break;
                case 13:
                    tokens.Add(new T_KEY_FLOAT(line));
                    break;
                case 14:
                    tokens.Add(new T_KEY_FOR(line));
                    break;
                case 15:
                    tokens.Add(new T_KEY_GOTO(line));
                    break;
                case 16:
                    tokens.Add(new T_KEY_IF(line));
                    break;
                case 17:
                    tokens.Add(new T_KEY_INLINE(line));
                    break;
                case 18:
                    tokens.Add(new T_KEY_INT(line));
                    break;
                case 19:
                    tokens.Add(new T_KEY_LONG(line));
                    break;
                case 20:
                    tokens.Add(new T_KEY_REGISTER(line));
                    break;
                case 21:
                    tokens.Add(new T_KEY_RESTRICT(line));
                    break;
                case 22:
                    tokens.Add(new T_KEY_RETURN(line));
                    break;
                case 23:
                    tokens.Add(new T_KEY_SHORT(line));
                    break;
                case 24:
                    tokens.Add(new T_KEY_SIGNED(line));
                    break;
                case 25:
                    tokens.Add(new T_KEY_SIZEOF(line));
                    break;
                case 26:
                    tokens.Add(new T_KEY_STATIC(line));
                    break;
                case 27:
                    tokens.Add(new T_KEY_STRUCT(line));
                    break;
                case 28:
                    tokens.Add(new T_KEY_SWITCH(line));
                    break;
                case 29:
                    tokens.Add(new T_KEY_TYPEDEF(line));
                    break;
                case 30:
                    tokens.Add(new T_KEY_UNION(line));
                    break;
                case 31:
                    tokens.Add(new T_KEY_UNSIGNED(line));
                    break;
                case 32:
                    tokens.Add(new T_KEY_VOID(line));
                    break;
                case 33:
                    tokens.Add(new T_KEY_VOLATILE(line));
                    break;
                case 34:
                    tokens.Add(new T_KEY_WHILE(line));
                    break;
                case 35:
                    tokens.Add(new T_KEY__BOOL(line));
                    break;
                case 36:
                    tokens.Add(new T_KEY__COMPLEX(line));
                    break;
                case 37:
                    tokens.Add(new T_KEY__IMAGINARY(line));
                    break;
                case 38:
                    tokens.Add(new T_IDENTIFIER(line, text));
                    break;
                case 39:
                    tokens.Add(new T_CONST_INT(line, text.Substring(2), 16));
                    break;
                case 40:
                    tokens.Add(new T_CONST_INT(line, text.Substring(2), 16));
                    break;
                case 41:
                    tokens.Add(new T_CONST_INT(line, text, 8));
                    break;
                case 42:
                    tokens.Add(new T_CONST_INT(line, text, 8));
                    break;
                case 43:
                    tokens.Add(new T_CONST_INT(line, text, 10));
                    break;
                case 44:
                    tokens.Add(new T_CONST_INT(line, text, 10));
                    break;
                case 45:
                    tokens.Add(new T_CONST_FLOAT(line, text, 10));
                    break;
                case 46:
                    tokens.Add(new T_CONST_FLOAT(line, text, 10));
                    break;
                case 47:
                    tokens.Add(new T_CONST_FLOAT(line, text, 10));
                    break;
                case 48:
                    tokens.Add(new T_CONST_FLOAT(line, text.Substring(2), 16));
                    break;
                case 49:
                    tokens.Add(new T_CONST_FLOAT(line, text.Substring(2), 16));
                    break;
                case 50:
                    tokens.Add(new T_CONST_CHAR(line, text));
                    break;
                case 51:
                    tokens.Add(new T_STRING_LITERAL(line, text));
                    break;
                case 52:
                    tokens.Add(new T_PUNC_SUBSCRIPTL(line));
                    break;
                case 53:
                    tokens.Add(new T_PUNC_SUBSCRIPTR(line));
                    break;
                case 54:
                    tokens.Add(new T_PUNC_BRACEL(line));
                    break;
                case 55:
                    tokens.Add(new T_PUNC_BRACER(line));
                    break;
                case 56:
                    tokens.Add(new T_PUNC_SUBSCRIPTL(line));
                    break;
                case 57:
                    tokens.Add(new T_PUNC_SUBSCRIPTR(line));
                    break;
                case 58:
                    tokens.Add(new T_PUNC_PARENTL(line));
                    break;
                case 59:
                    tokens.Add(new T_PUNC_PARENTR(line));
                    break;
                case 60:
                    tokens.Add(new T_PUNC_BRACEL(line));
                    break;
                case 61:
                    tokens.Add(new T_PUNC_BRACER(line));
                    break;
                case 62:
                    tokens.Add(new T_PUNC_DOT(line));
                    break;
                case 63:
                    tokens.Add(new T_PUNC_PTRSEL(line));
                    break;
                case 64:
                    tokens.Add(new T_PUNC_INCRE(line));
                    break;
                case 65:
                    tokens.Add(new T_PUNC_DECRE(line));
                    break;
                case 66:
                    tokens.Add(new T_PUNC_REF(line));
                    break;
                case 67:
                    tokens.Add(new T_PUNC_STAR(line));
                    break;
                case 68:
                    tokens.Add(new T_PUNC_PLUS(line));
                    break;
                case 69:
                    tokens.Add(new T_PUNC_MINUS(line));
                    break;
                case 70:
                    tokens.Add(new T_PUNC_BITNOT(line));
                    break;
                case 71:
                    tokens.Add(new T_PUNC_LOGNOT(line));
                    break;
                case 72:
                    tokens.Add(new T_PUNC_SLASH(line));
                    break;
                case 73:
                    tokens.Add(new T_PUNC_MOD(line));
                    break;
                case 74:
                    tokens.Add(new T_PUNC_SHIFTL(line));
                    break;
                case 75:
                    tokens.Add(new T_PUNC_SHIFTR(line));
                    break;
                case 76:
                    tokens.Add(new T_PUNC_LT(line));
                    break;
                case 77:
                    tokens.Add(new T_PUNC_GT(line));
                    break;
                case 78:
                    tokens.Add(new T_PUNC_LE(line));
                    break;
                case 79:
                    tokens.Add(new T_PUNC_GE(line));
                    break;
                case 80:
                    tokens.Add(new T_PUNC_EQ(line));
                    break;
                case 81:
                    tokens.Add(new T_PUNC_NEQ(line));
                    break;
                case 82:
                    tokens.Add(new T_PUNC_BITXOR(line));
                    break;
                case 83:
                    tokens.Add(new T_PUNC_BITOR(line));
                    break;
                case 84:
                    tokens.Add(new T_PUNC_LOGAND(line));
                    break;
                case 85:
                    tokens.Add(new T_PUNC_LOGOR(line));
                    break;
                case 86:
                    tokens.Add(new T_PUNC_QUESTION(line));
                    break;
                case 87:
                    tokens.Add(new T_PUNC_COLON(line));
                    break;
                case 88:
                    tokens.Add(new T_PUNC_SEMICOLON(line));
                    break;
                case 89:
                    tokens.Add(new T_PUNC_ELLIPSIS(line));
                    break;
                case 90:
                    tokens.Add(new T_PUNC_ASSIGN(line));
                    break;
                case 91:
                    tokens.Add(new T_PUNC_MULEQ(line));
                    break;
                case 92:
                    tokens.Add(new T_PUNC_DIVEQ(line));
                    break;
                case 93:
                    tokens.Add(new T_PUNC_MODEQ(line));
                    break;
                case 94:
                    tokens.Add(new T_PUNC_PLUSEQ(line));
                    break;
                case 95:
                    tokens.Add(new T_PUNC_MINUSEQ(line));
                    break;
                case 96:
                    tokens.Add(new T_PUNC_SHIFTLEQ(line));
                    break;
                case 97:
                    tokens.Add(new T_PUNC_SHIFTREQ(line));
                    break;
                case 98:
                    tokens.Add(new T_PUNC_BITANDEQ(line));
                    break;
                case 99:
                    tokens.Add(new T_PUNC_BITXOREQ(line));
                    break;
                case 100:
                    tokens.Add(new T_PUNC_BITOREQ(line));
                    break;
                case 101:
                    tokens.Add(new T_PUNC_COMMA(line));
                    break;
                case 102:
                    break;
                case 103:
                    break;
                default:
                    Error("UNKNOWN RULE: THIS SHOULD NEVER HAPPEN!");
                    break;
            }
        }
        void Comment() {
        bool prevStar = false;
        while (More()) {
        if (prevStar && Peek() == '/') {
        Next();
        return;
        }
        prevStar = Next() == '*';
        }
        Error("Unclosed block comment at line " + line);
        }
    }
}
