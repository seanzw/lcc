using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using RegEx;

using llexer_alpha;

namespace llexer {

    /// <summary>
    /// Lexer for llexer-alpha.
    /// This is a singleton class.
    /// </summary>
    public sealed class Lexer {

        private int _idx;
        private string _src;
        private StringBuilder _lltext;
        private const char NONE = (char)0;
        private readonly List<DFA> _dfas;
        private static readonly Lexer instance = new Lexer();

        public static Lexer Instance {
            get {
                return instance;
            }
        }

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
        }

        #region API for user.
        /// <summary>
        /// Is there any more character in the src?
        /// </summary>
        /// <returns> True if the src string is not empty. </returns>
        private bool more() {
            return _idx < _src.Length;
        }

        /// <summary>
        /// Peek the next char.
        /// </summary>
        /// <returns> Char. </returns>
        private char peek() {
            if (more()) {
                return _src[_idx];
            } else {
                return NONE;
            }
        }

        /// <summary>
        /// Eat the next Char.
        /// </summary>
        /// <returns> Char. </returns>
        private char next() {
            if (more()) {
                _lltext.Append(_src[_idx]);
                return _src[_idx++];
            } else {
                return NONE;
            }
        }

        private void llerror(string msg) {
            throw new ArgumentException("Lexer Error: " + msg);
        }

        /// <summary>
        /// Current matched src.
        /// </summary>
        private string lltext {
            get {
                return _lltext.ToString();
            }
        }
        #endregion

        /// <summary>
        /// Initialize the Lexer code.
        /// </summary>
        private Lexer() {

            _lltext = new StringBuilder();

            List<string> regSrcs = new List<string> {
                @"\$(\\[^\n\r\t]|[^\n\r\t\$\\])*\$",
                @"[^\$ \n\r\t%][^\r\n]*",
                @"[ \n\r\t]+",
                @"%%",
            };

            _dfas = new List<DFA>(regSrcs.Count());
            foreach (string src in regSrcs) {
                Parser parser = new Parser();
                ASTExpression expr = parser.parse(src);
                _dfas.Add(expr.gen().toDFATable().toDFA());
            }

        }

        private void _action(int _rule, ref List<Token> lltokens) {
            switch (_rule) {
                case 0:
                    lltokens.Add(new T_REGEX(lltext.Substring(1, lltext.Length - 2)));
                    break;
                case 1:
                    lltokens.Add(new T_CODE(lltext));
                    break;
                case 2:
                    // Ignore space.
                    break;
                case 3:
                    lltokens.Add(new T_SPLITER());
                    break;
                default:
                    llerror("UNKNOWN RULE: THIS SHOULD NEVER HAPPEN!");
                    break;
            }
        }
    }
}
