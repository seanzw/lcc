using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace llexer {
    public class Lexer {
        public Lexer(List<DFA> dfas) {
            this._dfas = dfas;
        }

        /// <summary>
        /// Scan the source code and returns a list of tokens.
        /// 
        /// The following variables are defined for users:
        /// llSrc: source string.
        /// llPre: llSrc[llPre] is the start of this token.
        /// llCur: llSrc[llCur - 1] is the end of this token.
        /// llTokens: list of tokens.
        /// 
        /// </summary>
        /// <param name="llSrc">Source string to be scanned. </param>
        /// <returns> A list of tokens. </returns>
        public List<Token> scan(string llSrc) {

            int llCur = 0;
            int llPre = 0;
            List<Token> llTokens = new List<Token>();

            Func<DFA.Status, int> _find = s => {
                for (int i = 0; i < _dfas.Count(); ++i)
                    if (_dfas[i].status() == s)
                        return i;
                return -1;
            };

            Action<string> llError = (_msg) => {
                Console.WriteLine("Lexer Error: " + _msg);
            };

            #region User code.

            Func<int, bool> _action = (_rule) => {
                switch (_rule) {
                    case 1:
                        return true;
                    default:
                        llError("Unkown rule.");
                        return false;
                }
            };
            #endregion


            _dfas.ForEach(dfa => dfa.reset());
            while (llCur < llSrc.Length) {
                _dfas.ForEach(dfa => dfa.scan(llSrc[llCur]));
                if (_find(DFA.Status.RUNNING) != -1) {
                    llCur++;
                    continue;
                }
                int _rule = _find(DFA.Status.SUCCEED);
                if (_rule != -1) {
                    // Find a token.
                    if (_action(_rule)) {
                        _dfas.ForEach(dfa => dfa.reset());
                        if (llCur == llSrc.Length) break;
                        _dfas.ForEach(dfa => dfa.scan(llSrc[llCur]));
                        llPre = llCur;
                        llCur++;
                    } else {
                        return llTokens;
                    }
                } else {
                    // Cannot match this token.
                    llError(llSrc.Substring(llPre, llCur - llPre + 1));
                    return llTokens;
                }
            }

            // Feed the EOF.
            if (_find(DFA.Status.RUNNING) != -1) {
                _dfas.ForEach(dfa => dfa.scan(Utility.EOF));
                int _rule = _find(DFA.Status.SUCCEED);
                if (_rule != -1) {
                    _action(_rule);
                } else {
                    // Cannot match this token.
                    llError(llSrc.Substring(llPre));
                }
            }

            return llTokens;
        }

        private readonly List<DFA> _dfas;
    }
}
