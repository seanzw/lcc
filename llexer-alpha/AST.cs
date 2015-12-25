using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace llexer_alpha {

    /**************************************************

    lex
        : rules SPLITER codes
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
        public ASTLex(List<ASTRule> rules, List<string> codes) {
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

    //    public void writeLexer() {

    //        StreamWriter o = new StreamWriter("Lexer.cs");

    //        Action<string> writeHeader = s => {
    //            o.WriteLine("using " + s + ";");
    //        };

    //        writeHeader("System");
    //        writeHeader("System.Collections.Generic");
    //        writeHeader("System.Linq");

    //        /*************************************************************
    //            Write the namespace llexer.
    //        *************************************************************/
    //        o.WriteLine("namespace llexer {");

    //        /************************************************************/

    //        #region Write Utilities Definition.
    //        string utilityDefinition = @"
    //class Utility {

    //    public static readonly int CHARSETSIZE = 1 << 16;
    //    public static readonly int NFATABLESIZE = CHARSETSIZE + 2;
    //    public static readonly int DFATABLESIZE = CHARSETSIZE;
    //    public static readonly int WILD = CHARSETSIZE;
    //    public static readonly int EPSILON = CHARSETSIZE + 1;

    //    public static readonly int EOF = 0x1A;         
    //}";
    //        o.WriteLine(utilityDefinition);
    //        #endregion

    //        #region Write Token Definition.
    //        string tokenDefinition = @"
    //public class Token {
    //    public Token(TOKEN type, string src) {
    //        this.type = type;
    //        this.src = src;
    //    }
    //    public override string ToString() {
    //        return type + "": "" + src;
    //    }
    //    public readonly TOKEN type;
    //    public readonly string src;
    //}";
    //        o.WriteLine(tokenDefinition);
    //        #endregion

    //        #region Write DFA definition.
    //        string DFADefinition = @"
    //public class DFA {

    //    public enum Status {
    //        RUNNING,
    //        FAILED,
    //        SUCCEED
    //    };

    //    public DFA(int[,] table, bool[] final, int[] map) {
    //        this.table = table;
    //        this.final = final;
    //        this.map = map;
    //        reset();
    //    }

    //    /* Reset the DFA to start state. */
    //    public void reset() {
    //        state = 0;
    //    }

    //    /* Get the current status of this DFA. */
    //    public Status status() {
    //        switch (state) {
    //            case SUCCESS_STATE:
    //                return Status.SUCCEED;
    //            case FAILURE_STATE:
    //                return Status.FAILED;
    //            default:
    //                return Status.RUNNING;
    //        }
    //    }

    //    /* Feed an input to this DFA. */
    //    public void scan(int input) {
            
    //        switch (state) {
    //            case SUCCESS_STATE:
    //                state = FAILURE_STATE;
    //                break;
    //            case FAILURE_STATE:
    //                break;
    //            default:
    //                if (map[input] == -1 || table[state, map[input]] == -1) {
    //                    state = final[state] ? SUCCESS_STATE : FAILURE_STATE;
    //                } else {
    //                    state = table[state, map[input]];
    //                }
    //                break;
    //        }
    //    }

    //    private readonly int[,] table;
    //    private readonly bool[] final;
    //    private readonly int[] map;

    //    // Current state.
    //    private int state;

    //    private const int FAILURE_STATE = -1;
    //    private const int SUCCESS_STATE = -2;

    //}";
    //        o.WriteLine(DFADefinition);

    //        #endregion

    //        #region Write Lexer definition
    //        o.WriteLine(tab(1) + "public class Lexer {");

    //        string LexerConstructor = @"
    //    public Lexer(List<DFA> dfas) {
    //        this._dfas = dfas;
    //    }";
    //        o.WriteLine(LexerConstructor);

    //        string LexerScanHeader = @"
    //    /// <summary>
    //    /// Scan the source code and returns a list of tokens.
    //    /// 
    //    /// The following variables are defined for users:
    //    /// llSrc: source string.
    //    /// llPre: llSrc[llPre] is the start of this token.
    //    /// llCur: llSrc[llCur - 1] is the end of this token.
    //    /// llTokens: list of tokens.
    //    /// 
    //    /// </summary>
    //    /// <param name=""llSrc"">Source string to be scanned. </param>
    //    /// <returns> A list of tokens. </returns>
    //    public List<Token> scan(string llSrc) {

    //        llSrc += (Char)Utility.EOF;
    //        int llCur = 0;
    //        int llPre = 0;
    //        List<Token> llTokens = new List<Token>();

    //        Func<DFA.Status, int> _find = s => {
    //            for (int i = 0; i < _dfas.Count(); ++i)
    //                if (_dfas[i].status() == s)
    //                    return i;
    //            return -1;
    //        };

    //        Action<string> llError = (_msg) => {
    //            Console.WriteLine(""Lexe Error: "" + _msg);
    //        };

    //    #region User code.";
    //    o.WriteLine(LexerScanHeader);

    //        #region Write user action.

    //        o.WriteLine(code.code.Substring(1, code.code.Length - 2));
    //        o.WriteLine(tab(3) + "Func<int, bool> _action = (_rule) => {");
    //        o.WriteLine(tab(4) + "switch (_rule) {");

    //        // Helper function.
    //        Action<int, ASTCode> writeAction = (rule, code) => {
    //            o.WriteLine(tab(5) + "case " + rule + ":");
    //            o.WriteLine(code.code);
    //        };
    //        for (int i = 0; i < rules.Count(); ++i) {
    //            writeAction(i, rules[rules.Count() - 1 - i].code);
    //        }
    //        o.WriteLine(tab(5) + "default:");
    //        o.WriteLine(tab(6) + "llError(\"Unkown rule.\");");
    //        o.WriteLine(tab(6) + "return false;");
    //        o.WriteLine(tab(4) + "}");
    //        o.WriteLine(tab(3) + "};");
    //        #endregion


    //        string LexerScanTail = @"
    //        #endregion

    //        _dfas.ForEach(dfa => dfa.reset());
    //        while (llCur < llSrc.Length) {
    //            _dfas.ForEach(dfa => dfa.scan(llSrc[llCur]));
    //            if (_find(DFA.Status.RUNNING) != -1) {
    //                llCur++;
    //                continue;
    //            }
    //            int _rule = _find(DFA.Status.SUCCEED);
    //            if (_rule != -1) {
    //                // Find a token.
    //                if (_action(_rule)) {
    //                    _dfas.ForEach(dfa => dfa.reset());
    //                    if (llCur == llSrc.Length) break;
    //                    _dfas.ForEach(dfa => dfa.scan(llSrc[llCur]));
    //                    llPre = llCur;
    //                    llCur++;
    //                } else {
    //                    return llTokens;
    //                }
    //            } else {
    //                // Cannot match this token.
    //                llError(llSrc.Substring(llPre, llCur - llPre + 1));
    //                return llTokens;
    //            }
    //        }

    //        return llTokens;
    //    }";
    //        o.WriteLine(LexerScanTail);
    //        o.WriteLine(tab(2) + "private readonly List<DFA> _dfas;");
    //        o.WriteLine(tab(1) + "}");
    //        #endregion

    //        #region Write Main function.
    //        o.WriteLine(tab(1) + "public class Main {");
    //        o.WriteLine(tab(2) + "public static List<Token> scan(string src) {");

    //        // Helpers.
    //        string buildMapDefinition = @"
    //        Func<Dictionary<int, int>, int, int[]> buildMap = (dict, wild) => {
    //            int size = 1 << 16;
    //            int[] map = new int[size];
    //            for (int i = 0; i < map.Length; ++i) {
    //                map[i] = dict.ContainsKey(i) ? dict[i] : wild;
    //            }
    //            return map;
    //        };";
    //        o.WriteLine(buildMapDefinition);

    //        // DFAs.
    //        var dfaParameters = new List<Tuple<int[,], bool[], int[], int>>();
    //        for (int i = rules.Count() - 1; i >= 0; --i) {
    //            dfaParameters.Add(rules[i].reg.toNFA().toDFATable().simplify().toDFAParameters());
    //        }

    //        o.WriteLine("\t\t\tList<DFA> dfas = new List<DFA>();");
    //        for (int i = 0; i < dfaParameters.Count(); ++i) {

    //            int[,] trans = dfaParameters[i].Item1;
    //            bool[] finals = dfaParameters[i].Item2;
    //            int[] map = dfaParameters[i].Item3;
    //            int wild = dfaParameters[i].Item4;

    //            o.WriteLine(tab(3) + "#region DFA" + i);
    //            o.WriteLine("\t\t\t{");

    //            // Transition tables.
    //            int NStates = trans.GetLength(0) - 1;
    //            int NGroups = trans.GetLength(1) - 1;
    //            o.WriteLine(tab(4) + "int[,] trans = new int[,] {");
    //            for (int state = 0; state < NStates; ++state) {
    //                o.Write(tab(5) + "{ ");
    //                for (int c = 0; c < NGroups; ++c) {
    //                    o.Write(trans[state, c] + ", ");
    //                }
    //                o.Write(trans[state, NGroups]);
    //                o.WriteLine(" },");
    //            }
    //            o.Write(tab(5) + "{ ");
    //            for (int c = 0; c < NGroups; ++c) {
    //                o.Write(trans[NStates, c] + ", ");
    //            }
    //            o.Write(trans[NStates, NGroups]);
    //            o.WriteLine(" },");
    //            o.WriteLine(tab(4) + "};");

    //            // Final Flags
    //            o.WriteLine(tab(4) + "bool[] finals = new bool[] {");
    //            for (int j = 0; j < finals.Length - 1; ++j) {
    //                o.WriteLine(tab(5) + (finals[j] ? "true" : "false") + ",");
    //            }
    //            o.WriteLine(tab(5) + (finals[finals.Length - 1] ? "true" : "false"));
    //            o.WriteLine(tab(4) + "};");

    //            // Maps.
    //            o.WriteLine(tab(4) + "Dictionary<int, int> dict = new Dictionary<int, int> {");
    //            for (int j = 0; j < map.Length; ++j) {
    //                if (map[j] != wild) {
    //                    if (j < map.Length - 1) {
    //                        o.WriteLine(tab(5) + "{ " + j + ", " + map[j] + " },");
    //                    } else {
    //                        o.WriteLine(tab(5) + "{ " + j + ", " + map[j] + " }");
    //                    }
    //                }
    //            }
    //            o.WriteLine(tab(4) + "};");

    //            // Wild.
    //            o.WriteLine(tab(4) + "int wild = " + wild + ";");

    //            o.WriteLine(tab(4) + "dfas.Add(new DFA(trans, finals, buildMap(dict, wild)));");

    //            o.WriteLine("\t\t\t}");
    //            o.WriteLine(tab(3) + "#endregion");
    //        }

    //        // Finally.

    //        o.WriteLine(tab(3) + "Lexer lexer = new Lexer(dfas);");
    //        o.WriteLine(tab(3) + "return lexer.scan(src);");

    //        o.WriteLine("\t\t}");
    //        o.WriteLine("\t}");
    //        #endregion

    //        // Close namespace.
    //        o.WriteLine("}");
    //        o.Close();
    //    }

        public readonly List<ASTRule> rules;
        public readonly List<string> codes;
    }

    //public class ASTCode : ASTNode {
    //    public ASTCode(string code) {
    //        this.code = code;
    //    }
    //    public override string ToString(int level) {
    //        string str = tab(level);
    //        str += "CODE: ";
    //        str += code;
    //        return str;
    //    }

    //    public readonly string code;
    //}

    public class ASTRule : ASTNode {
        public ASTRule(string regex, List<string> codes) {
            this.regex = regex;
            this.codes = codes;
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
        public readonly string regex;
        public readonly List<string> codes;
    }

    //public class ASTRegStar : ASTRegEx {
    //    public ASTRegStar(ASTRegEx reg) {
    //        this.reg = reg;
    //    }

    //    public override string ToString(int level) {
    //        string str = tab(level);
    //        str += "STAR\n";
    //        str += reg.ToString(level + 1);
    //        return str;
    //    }

    //    public override NFATable toNFA() {
    //        return reg.toNFA().star();
    //    }

    //    private ASTRegEx reg;

    //}

    //public class ASTRegQuestion : ASTRegEx {
    //    public ASTRegQuestion(ASTRegEx reg) {
    //        this.reg = reg;
    //    }

    //    public override string ToString(int level) {
    //        string str = tab(level);
    //        str += "QUESTION\n";
    //        str += reg.ToString(level + 1);
    //        return str;
    //    }

    //    public override NFATable toNFA() {
    //        return reg.toNFA().question();
    //    }

    //    private ASTRegEx reg;
    //}

    //public class ASTRegAnd : ASTRegEx {
    //    public ASTRegAnd(ASTRegEx lhs, ASTRegEx rhs) {
    //        this.lhs = lhs;
    //        this.rhs = rhs;
    //    }

    //    public override string ToString(int level) {
    //        string str = tab(level);
    //        str += "AND\n";
    //        str += lhs.ToString(level + 1);
    //        str += "\n";
    //        str += rhs.ToString(level + 1);
    //        return str;
    //    }

    //    public override NFATable toNFA() {
    //        return lhs.toNFA().mergeAnd(rhs.toNFA());
    //    }

    //    private ASTRegEx lhs, rhs;
    //}

    //public class ASTRegOr : ASTRegEx {

    //    public ASTRegOr(ASTRegEx lhs, ASTRegEx rhs) {
    //        this.lhs = lhs;
    //        this.rhs = rhs;
    //    }

    //    public override string ToString(int level) {
    //        string str = tab(level);
    //        str += "OR\n";
    //        str += lhs.ToString(level + 1);
    //        str += "\n";
    //        str += rhs.ToString(level + 1);
    //        return str;
    //    }

    //    public override NFATable toNFA() {
    //        return lhs.toNFA().mergeOr(rhs.toNFA());
    //    }

    //    private ASTRegEx lhs, rhs;
    //}

    //public class ASTAtom : ASTRegEx {
    //    public ASTAtom(int[] c) {
    //        cs = new int[c.Length];
    //        for (int i = 0; i < cs.Length; ++i) {
    //            cs[i] = c[i];
    //        }
    //    }

    //    public ASTAtom(int c) {
    //        cs = new int[] { c };
    //    }

    //    public override string ToString(int level) {
    //        string str = tab(level);
    //        foreach (int c in cs) {
    //            str += Utility.inputToString(c);
    //        }
    //        return str;
    //    }

    //    public override NFATable toNFA() {

    //        NFATable nfa = new NFATable();
    //        nfa.addState();
    //        nfa.setStateFinal(1);
    //        foreach (int c in cs) {
    //            nfa.addTransition(0, 1, c);
    //        }

    //        return nfa;
    //    }
    //    private int[] cs;
    //}
}
