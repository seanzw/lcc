using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegEx
{
    public class RegEx {

        /// <summary>
        /// Constructs a regular expression.
        /// </summary>
        /// <param name="src"> The regular expression. </param>
        public RegEx(string src) {
            Parser parser = new Parser();
            ASTExpression ast = parser.parse(src);
            if (ast == null) {
                throw new ArgumentException("Can't parse this regex. Sorry~");
            }
            NFATable nfaTable = ast.gen();
            DFATable dfaTable = nfaTable.toDFATable();
            dfa = dfaTable.toDFA();
        }

        public bool match(string str) {

            dfa.reset();
            foreach (char c in str) {
                dfa.scan(c);
            }
            // Feed the 0 char.
            dfa.scan(Const.NONE);
            return dfa.status() == DFA.Status.SUCCEED;

        }

        public string replace(string str, string patch) {

            dfa.reset();
            string ret = "";

            int pre = 0;
            for (int i = 0; i < str.Length; ++i) {
                dfa.scan(str[i]);
                switch (dfa.status()) {
                    case DFA.Status.SUCCEED:
                        ret += patch;
                        pre = i;
                        dfa.reset();
                        dfa.scan(str[i]);
                        break;
                    case DFA.Status.FAILED:
                        ret += str.Substring(pre, i - pre);
                        pre = i;
                        dfa.reset();
                        dfa.scan(str[i]);
                        break;
                    default:
                        break;
                }
            }

            if (dfa.status() == DFA.Status.RUN) {
                dfa.scan(Const.NONE);
                if (dfa.status() == DFA.Status.SUCCEED) {
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
