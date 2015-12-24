using System;
using System.Collections.Generic;
using System.Linq;
namespace llexer {

    class Utility {

        public static readonly int CHARSETSIZE = 1 << 16;
        public static readonly int NFATABLESIZE = CHARSETSIZE + 2;
        public static readonly int DFATABLESIZE = CHARSETSIZE;
        public static readonly int WILD = CHARSETSIZE;
        public static readonly int EPSILON = CHARSETSIZE + 1;

        public static readonly int EOF = 0x1A;         
    }

    public class Token {
        public Token(TOKEN type, string src) {
            this.type = type;
            this.src = src;
        }
        public override string ToString() {
            return type + ": " + src;
        }
        public readonly TOKEN type;
        public readonly string src;
    }

    public class DFA {

        public enum Status {
            RUNNING,
            FAILED,
            SUCCEED
        };

        public DFA(int[,] table, bool[] final, int[] map) {
            this.table = table;
            this.final = final;
            this.map = map;
            reset();
        }

        /* Reset the DFA to start state. */
        public void reset() {
            state = 0;
        }

        /* Get the current status of this DFA. */
        public Status status() {
            switch (state) {
                case SUCCESS_STATE:
                    return Status.SUCCEED;
                case FAILURE_STATE:
                    return Status.FAILED;
                default:
                    return Status.RUNNING;
            }
        }

        /* Feed an input to this DFA. */
        public void scan(int input) {
            
            switch (state) {
                case SUCCESS_STATE:
                    state = FAILURE_STATE;
                    break;
                case FAILURE_STATE:
                    break;
                default:
                    if (map[input] == -1 || table[state, map[input]] == -1) {
                        state = final[state] ? SUCCESS_STATE : FAILURE_STATE;
                    } else {
                        state = table[state, map[input]];
                    }
                    break;
            }
        }

        private readonly int[,] table;
        private readonly bool[] final;
        private readonly int[] map;

        // Current state.
        private int state;

        private const int FAILURE_STATE = -1;
        private const int SUCCESS_STATE = -2;

    }
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

            llSrc += (Char)Utility.EOF;
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
                Console.WriteLine("Lexe Error: " + _msg);
            };

        #region User code.

    Func<int, int> code = (i) => {
        while (i < llSrc.Length) {
            if (llSrc[i] == '}' && llSrc[i - 1] == '\n') {
                return i + 1;
            }
            i++;
        }
        return -1;
    };

    Func<int, int> atom = (i) => {
        if (i < llSrc.Length) {
            if (llSrc[i++] == '\\') {
                if (i + 3 < llSrc.Length) {
                    string sub = llSrc.Substring(i, 3);
                    if (sub == "WLD" ||
                        sub == "a-z" ||
                        sub == "a-Z" ||
                        sub == "0-9"
                        ) {
                        return i + 3;
                    }
                }
                if (i + 1 < llSrc.Length) {
                    return i + 1;
                } else {
                    return -1;
                }
            } else {
                return i;
            }
        } else {
            return -1;
        }
    };

            Func<int, bool> _action = (_rule) => {
                switch (_rule) {
                    case 0:
{
    // Code block.
    llCur = code(llPre);
    if (llCur != -1) {
        llTokens.Add(new Token(TOKEN.CODE, llSrc.Substring(llPre, llCur - llPre)));
        return true;
    } else {
        llError("Unknown token at " + ", maybe CODE?");
        return false;
    }
}
                    case 1:
{
    llTokens.Add(new Token(TOKEN.LBRACKET, llSrc.Substring(llPre, llCur - llPre)));
    return true;
}
                    case 2:
{
    llTokens.Add(new Token(TOKEN.RBRACKET, llSrc.Substring(llPre, llCur - llPre)));
    return true;
}
                    case 3:
{
    llTokens.Add(new Token(TOKEN.STAR, llSrc.Substring(llPre, llCur - llPre)));
    return true;
}
                    case 4:
{
    llTokens.Add(new Token(TOKEN.PLUS, llSrc.Substring(llPre, llCur - llPre)));
    return true;
}
                    case 5:
{
    llTokens.Add(new Token(TOKEN.AND, llSrc.Substring(llPre, llCur - llPre)));
    return true;
}
                    case 6:
{
    llTokens.Add(new Token(TOKEN.OR, llSrc.Substring(llPre, llCur - llPre)));
    return true;
}
                    case 7:
{
    llCur = atom(llCur);
    if (llCur != -1) {
        llTokens.Add(new Token(TOKEN.ATOM, llSrc.Substring(llPre, llCur - llPre)));
        return true;
    } else {
        llError("Unknown token, maybe ATOM?");
        return false;
    }
}
                    case 8:
{
    // Ignore space.
    return true;
}
                    case 9:
{
    // Ignore line comment.
    return true;
}
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

            return llTokens;
        }
        private readonly List<DFA> _dfas;
    }
    public class Main {
        public static List<Token> scan(string src) {

            Func<Dictionary<int, int>, int, int[]> buildMap = (dict, wild) => {
                int size = 1 << 16;
                int[] map = new int[size];
                for (int i = 0; i < map.Length; ++i) {
                    map[i] = dict.ContainsKey(i) ? dict[i] : wild;
                }
                return map;
            };
			List<DFA> dfas = new List<DFA>();
            #region DFA0
			{
                int[,] trans = new int[,] {
                    { 1 },
                    { -1 },
                };
                bool[] finals = new bool[] {
                    false,
                    true
                };
                Dictionary<int, int> dict = new Dictionary<int, int> {
                    { 123, 0 },
                };
                int wild = -1;
                dfas.Add(new DFA(trans, finals, buildMap(dict, wild)));
			}
            #endregion
            #region DFA1
			{
                int[,] trans = new int[,] {
                    { 1 },
                    { -1 },
                };
                bool[] finals = new bool[] {
                    false,
                    true
                };
                Dictionary<int, int> dict = new Dictionary<int, int> {
                    { 91, 0 },
                };
                int wild = -1;
                dfas.Add(new DFA(trans, finals, buildMap(dict, wild)));
			}
            #endregion
            #region DFA2
			{
                int[,] trans = new int[,] {
                    { 1 },
                    { -1 },
                };
                bool[] finals = new bool[] {
                    false,
                    true
                };
                Dictionary<int, int> dict = new Dictionary<int, int> {
                    { 93, 0 },
                };
                int wild = -1;
                dfas.Add(new DFA(trans, finals, buildMap(dict, wild)));
			}
            #endregion
            #region DFA3
			{
                int[,] trans = new int[,] {
                    { 1 },
                    { -1 },
                };
                bool[] finals = new bool[] {
                    false,
                    true
                };
                Dictionary<int, int> dict = new Dictionary<int, int> {
                    { 42, 0 },
                };
                int wild = -1;
                dfas.Add(new DFA(trans, finals, buildMap(dict, wild)));
			}
            #endregion
            #region DFA4
			{
                int[,] trans = new int[,] {
                    { 1 },
                    { -1 },
                };
                bool[] finals = new bool[] {
                    false,
                    true
                };
                Dictionary<int, int> dict = new Dictionary<int, int> {
                    { 43, 0 },
                };
                int wild = -1;
                dfas.Add(new DFA(trans, finals, buildMap(dict, wild)));
			}
            #endregion
            #region DFA5
			{
                int[,] trans = new int[,] {
                    { 1 },
                    { -1 },
                };
                bool[] finals = new bool[] {
                    false,
                    true
                };
                Dictionary<int, int> dict = new Dictionary<int, int> {
                    { 45, 0 },
                };
                int wild = -1;
                dfas.Add(new DFA(trans, finals, buildMap(dict, wild)));
			}
            #endregion
            #region DFA6
			{
                int[,] trans = new int[,] {
                    { 1 },
                    { -1 },
                };
                bool[] finals = new bool[] {
                    false,
                    true
                };
                Dictionary<int, int> dict = new Dictionary<int, int> {
                    { 124, 0 },
                };
                int wild = -1;
                dfas.Add(new DFA(trans, finals, buildMap(dict, wild)));
			}
            #endregion
            #region DFA7
			{
                int[,] trans = new int[,] {
                    { 1 },
                    { -1 },
                };
                bool[] finals = new bool[] {
                    false,
                    true
                };
                Dictionary<int, int> dict = new Dictionary<int, int> {
                    { 92, 0 },
                };
                int wild = -1;
                dfas.Add(new DFA(trans, finals, buildMap(dict, wild)));
			}
            #endregion
            #region DFA8
			{
                int[,] trans = new int[,] {
                    { 1, 2, 3, 4 },
                    { 5, 6, 7, 8 },
                    { 5, 6, 7, 8 },
                    { 5, 6, 7, 8 },
                    { 5, 6, 7, 8 },
                    { 5, 6, 7, 8 },
                    { 5, 6, 7, 8 },
                    { 5, 6, 7, 8 },
                    { 5, 6, 7, 8 },
                };
                bool[] finals = new bool[] {
                    false,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true
                };
                Dictionary<int, int> dict = new Dictionary<int, int> {
                    { 9, 3 },
                    { 10, 1 },
                    { 13, 2 },
                    { 32, 0 },
                };
                int wild = -1;
                dfas.Add(new DFA(trans, finals, buildMap(dict, wild)));
			}
            #endregion
            #region DFA9
			{
                int[,] trans = new int[,] {
                    { 1, -1 },
                    { 2, -1 },
                    { -1, 3 },
                    { -1, 3 },
                };
                bool[] finals = new bool[] {
                    false,
                    false,
                    true,
                    true
                };
                Dictionary<int, int> dict = new Dictionary<int, int> {
                    { 10, -1 },
                    { 26, -1 },
                    { 47, 0 },
                };
                int wild = 1;
                dfas.Add(new DFA(trans, finals, buildMap(dict, wild)));
			}
            #endregion
            Lexer lexer = new Lexer(dfas);
            return lexer.scan(src);
		}
	}
}
