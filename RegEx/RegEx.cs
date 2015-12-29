using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegEx {
    public class RegEx {

        /// <summary>
        /// Constructs a regular expression.
        /// </summary>
        /// <param name="src"> The regular expression. </param>
        public RegEx(string src) {
            ASTExpr ast = Parser.Instance.Parse(src);
            if (ast == null) {
                throw new ArgumentException("Can't parse this regex. Sorry~");
            }
            NFATable nfaTable = ast.ToNFATable();
            DFATable dfaTable = nfaTable.ToDFATable();
            dfa = dfaTable.ToDFA();
        }

        public bool Match(string str) {

            dfa.Reset();
            foreach (char c in str) {
                dfa.Scan(c);
            }
            // Feed the 0 char.
            dfa.Scan(Const.NONE);
            return dfa.status == DFA.Status.SUCCEED;

        }

        public string Replace(string str, string patch) {

            dfa.Reset();
            string ret = "";

            int pre = 0;
            for (int i = 0; i < str.Length; ++i) {
                dfa.Scan(str[i]);
                switch (dfa.status) {
                    case DFA.Status.SUCCEED:
                        ret += patch;
                        pre = i;
                        dfa.Reset();
                        dfa.Scan(str[i]);
                        break;
                    case DFA.Status.FAILED:
                        ret += str.Substring(pre, i - pre);
                        pre = i;
                        dfa.Reset();
                        dfa.Scan(str[i]);
                        break;
                    default:
                        break;
                }
            }

            if (dfa.status == DFA.Status.RUN) {
                dfa.Scan(Const.NONE);
                if (dfa.status == DFA.Status.SUCCEED) {
                    ret += patch;
                    return ret;
                }
            }

            ret += str.Substring(pre);
            return ret;
        }


        public readonly DFA dfa;
    }
}
