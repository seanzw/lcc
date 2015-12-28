using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using RegEx;

using LLexerAlpha;

namespace LLexer {

    /// <summary>
    /// Lexer for llexer-alpha.
    /// This is a singleton class.
    /// </summary>
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

        #region API for user.
        /// <summary>
        /// Is there any more character in the src?
        /// </summary>
        /// <returns> True if the src string is not empty. </returns>
        private bool More() {
            return _idx < _src.Length;
        }

        /// <summary>
        /// Peek the next char.
        /// </summary>
        /// <returns> Char. </returns>
        private char Peek() {
            if (More()) {
                return _src[_idx];
            } else {
                return NONE;
            }
        }

        /// <summary>
        /// Eat the next Char.
        /// </summary>
        /// <returns> Char. </returns>
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

        /// <summary>
        /// Current matched src.
        /// </summary>
        private string text {
            get {
                return _text.ToString();
            }
        }
        #endregion

        /// <summary>
        /// Initialize the Lexer code.
        /// </summary>
        private Lexer() {

            _text = new StringBuilder();

            List<string> regSrcs = new List<string> {
                @"\$(\\[^\n\r\t]|[^\n\r\t\$\\])*\$",
                @"[^\$ \n\r\t%][^\r\n]*",
                @"[ \n\r\t]+",
                @"%%",
            };

            _dfas = new List<DFA>(regSrcs.Count());
            foreach (string src in regSrcs) {
                RegEx.RegEx regex = new RegEx.RegEx(src);
                _dfas.Add(regex.dfa);
            }

        }

        private void _Action(int _rule, ref List<Token> lltokens) {
            switch (_rule) {
                case 0:
                    lltokens.Add(new T_REGEX(text.Substring(1, text.Length - 2)));
                    break;
                case 1:
                    lltokens.Add(new T_CODE(text));
                    break;
                case 2:
                    // Ignore space.
                    break;
                case 3:
                    lltokens.Add(new T_SPLITER());
                    break;
                default:
                    Error("UNKNOWN RULE: THIS SHOULD NEVER HAPPEN!");
                    break;
            }
        }
    }
}
