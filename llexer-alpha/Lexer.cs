using System;
using System.Collections.Generic;
using System.Linq;
using RegEx;
using System.Text;
using System.Threading.Tasks;

using Token = llexer_alpha.Token;
using T_REGEX_LITERAL = llexer_alpha.TokenREGEX;
using T_CODE_LINE = llexer_alpha.TokenCODE;
using T_SPLITER = llexer_alpha.TokenSpliter;

namespace llexer {

    /// <summary>
    /// Lexer for llexer-alpha.
    /// This is a singleton class.
    /// </summary>
    public sealed class Lexer {

        private static readonly Lexer instance = new Lexer();

        /// <summary>
        /// Initialize the Lexer code.
        /// </summary>
        private Lexer() {

            List<string> regSrcs = new List<string> {
                @"$(\\.|[^\$\n\\])*$",
                @"[^$ \n\r\t%][^\n]*",
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
                    lltokens.Add(new T_REGEX_LITERAL(lltext));
                    break;
                case 1:
                    lltokens.Add(new T_CODE_LINE(lltext));
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

        public List<Token> scan(string src) {

            // Replacing the line breaker.
            RegEx.RegEx regex = new RegEx.RegEx(@"\r\n?|\n");
            _src = regex.replace(src, "\n");

            List<Token> lltokens = new List<Token>();

            Func<DFA.Status, int> _find = s => {
                for (int i = 0; i < _dfas.Count(); ++i)
                    if (_dfas[i].status() == s)
                        return i;
                return -1;
            };

            Action _reset = () => {
                _dfas.ForEach(dfa => dfa.reset());
                lltext = "";
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

        public static Lexer Instance {
            get {
                return instance;
            }
        }

        #region API for user.
        private bool more() {
            return _src.Length > 0;
        }

        private char peek() {
            if (more()) {
                return _src[0];
            } else {
                return NONE;
            }
        }

        private char next() {
            if (more()) {
                char c = _src[0];
                _src = _src.Substring(1);
                lltext += c;
                return c;
            } else {
                return NONE;
            }
        }

        private void llerror(string msg) {
            throw new ArgumentException("Lexer Error: " + msg);
        }

        private string lltext;
        #endregion

        private string _src;
        private const char NONE = (char)0;

        private readonly List<RegEx.DFA> _dfas;
    }
}
