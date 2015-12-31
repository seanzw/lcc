using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using RegEx;
using lcc.Token;
namespace LLexer {
    sealed class Lexer {
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
            _dfas = new List<DFA>(38);
            #region RULE 0
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
            #region RULE 1
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
            #region RULE 2
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
                    96, 97, 98, 99, 103, 104, 113, 114, 65535, 
                };
                int[] value = new int[9] {
                    -1, 2, -1, 0, -1, 1, -1, 3, -1, 
                };
                _dfas.Add(new DFA(table, final, range, value));
            }
            #endregion
            #region RULE 4
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
            #region RULE 5
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
            #region RULE 6
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
            #region RULE 7
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
            #region RULE 8
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
            #region RULE 9
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
            #region RULE 10
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
            #region RULE 11
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
            #region RULE 12
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
            #region RULE 13
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
            #region RULE 14
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
            #region RULE 15
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
            #region RULE 16
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
            #region RULE 17
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
            #region RULE 18
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
            #region RULE 19
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
            #region RULE 20
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
            #region RULE 21
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
            #region RULE 22
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
            #region RULE 23
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
                    100, 101, 102, 104, 105, 110, 111, 114, 115, 121, 122, 65535, 
                };
                int[] value = new int[12] {
                    -1, 3, 5, -1, 1, -1, 4, -1, 0, -1, 2, -1, 
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
            #region RULE 26
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
            #region RULE 27
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
            #region RULE 28
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
            #region RULE 29
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
            #region RULE 30
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
            #region RULE 31
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
            #region RULE 32
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
            #region RULE 33
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
            #region RULE 34
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
            #region RULE 35
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
            #region RULE 36
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
            #region RULE 37
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
        }
        private void _Action(int _rule, ref List<Token> tokens) {
            switch (_rule) {
                case 0:
                    tokens.Add(new T_KEY_AUTO(line));
                    break;
                case 1:
                    tokens.Add(new T_KEY_BREAK(line));
                    break;
                case 2:
                    tokens.Add(new T_KEY_CASE(line));
                    break;
                case 3:
                    tokens.Add(new T_KEY_CHAR(line));
                    break;
                case 4:
                    tokens.Add(new T_KEY_CONST(line));
                    break;
                case 5:
                    tokens.Add(new T_KEY_CONTINUE(line));
                    break;
                case 6:
                    tokens.Add(new T_KEY_DEFAULT(line));
                    break;
                case 7:
                    tokens.Add(new T_KEY_DO(line));
                    break;
                case 8:
                    tokens.Add(new T_KEY_DOUBLE(line));
                    break;
                case 9:
                    tokens.Add(new T_KEY_ELSE(line));
                    break;
                case 10:
                    tokens.Add(new T_KEY_ENUM(line));
                    break;
                case 11:
                    tokens.Add(new T_KEY_EXTERN(line));
                    break;
                case 12:
                    tokens.Add(new T_KEY_FLOAT(line));
                    break;
                case 13:
                    tokens.Add(new T_KEY_FOR(line));
                    break;
                case 14:
                    tokens.Add(new T_KEY_GOTO(line));
                    break;
                case 15:
                    tokens.Add(new T_KEY_IF(line));
                    break;
                case 16:
                    tokens.Add(new T_KEY_INLINE(line));
                    break;
                case 17:
                    tokens.Add(new T_KEY_INT(line));
                    break;
                case 18:
                    tokens.Add(new T_KEY_LONG(line));
                    break;
                case 19:
                    tokens.Add(new T_KEY_REGISTER(line));
                    break;
                case 20:
                    tokens.Add(new T_KEY_RESTRICT(line));
                    break;
                case 21:
                    tokens.Add(new T_KEY_RETURN(line));
                    break;
                case 22:
                    tokens.Add(new T_KEY_SHORT(line));
                    break;
                case 23:
                    tokens.Add(new T_KEY_SIGNED(line));
                    break;
                case 24:
                    tokens.Add(new T_KEY_SIZEOF(line));
                    break;
                case 25:
                    tokens.Add(new T_KEY_STATIC(line));
                    break;
                case 26:
                    tokens.Add(new T_KEY_STRUCT(line));
                    break;
                case 27:
                    tokens.Add(new T_KEY_SWITCH(line));
                    break;
                case 28:
                    tokens.Add(new T_KEY_TYPEDEF(line));
                    break;
                case 29:
                    tokens.Add(new T_KEY_UNION(line));
                    break;
                case 30:
                    tokens.Add(new T_KEY_UNSIGNED(line));
                    break;
                case 31:
                    tokens.Add(new T_KEY_VOID(line));
                    break;
                case 32:
                    tokens.Add(new T_KEY_VOLATILE(line));
                    break;
                case 33:
                    tokens.Add(new T_KEY_WHILE(line));
                    break;
                case 34:
                    tokens.Add(new T_KEY__BOOL(line));
                    break;
                case 35:
                    tokens.Add(new T_KEY__COMPLEX(line));
                    break;
                case 36:
                    tokens.Add(new T_KEY__IMAGINARY(line));
                    break;
                case 37:
                    break;
                default:
                    Error("UNKNOWN RULE: THIS SHOULD NEVER HAPPEN!");
                    break;
            }
        }
    }
}
