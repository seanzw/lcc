using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegEx {
    public class Parser {

        public Parser() {}

        public ASTExpression parse(string src) {
            this.src = src;
            ASTExpression expr = parseExpression();
            if (more()) {
                return null;
            } else {
                return expr;
            }
        }

        #region
        /*************************************************************************************************

            Notice that for all the parsing functions, when
            1. Successfully matched, returns the ASTNode and modifies the src.
            2. No match is found, returns null and leaves src unchanged.

        **************************************************************************************************/

        private ASTExpression parseExpression() {

            string srcBk = src;
            ASTExpression expr;
            ASTTerm term;

            // expression : term expression_tail
            if ((term = parseTerm()) != null &&
                (expr = parseExpressionTail()) != null
                ) {
                expr.terms.AddFirst(term);
                return expr;
            }

            src = srcBk;
            return null;
        }

        private ASTExpression parseExpressionTail() {
            string srcBk = src;
            ASTExpression expr;

            // expression_tail : | expression
            if (next() == '|' &&
                (expr = parseExpression()) != null
                ) {
                return expr;
            }

            // expression_tail : epsilon
            src = srcBk;
            return new ASTExpression();
        }

        private ASTTerm parseTerm() {

            string srcBk = src;
            ASTFactor factor;
            ASTTerm term;

            // term : factor term_tail
            if ((factor = parseFactor()) != null &&
                (term = parseTermTail()) != null
                ) {
                term.factors.AddFirst(factor);
                return term;
            }

            src = srcBk;
            return null;

        }

        private ASTTerm parseTermTail() {

            string srcBk = src;
            ASTTerm term;

            // term_tail : term
            if ((term = parseTerm()) != null) {
                return term;
            }

            // term_tail : epsilon
            src = srcBk;
            return new ASTTerm();
        }

        private ASTFactor parseFactor() {

            string srcBk = src;

            ASTRegEx atom;
            if ((atom = parseAtom()) != null) {
                ASTFactor.MetaChar meta = parseMeta();
                return new ASTFactor(atom, meta);
            }

            src = srcBk;
            return null;
        }

        private ASTFactor.MetaChar parseMeta() {
            string srcBk = src;
            switch (next()) {
                case '*':
                    return ASTFactor.MetaChar.STAR;
                case '+':
                    return ASTFactor.MetaChar.PLUS;
                case '?':
                    return ASTFactor.MetaChar.QUES;
                case NONE:
                default:
                    src = srcBk;
                    return ASTFactor.MetaChar.NULL;
            }
        }

        private ASTRegEx parseAtom() {

            string srcBk = src;
            ASTRegEx expr;
            ASTCharSet charset;

            // atom : character
            if ((charset = parseCharacter()) != null) {
                return charset;
            }

            // atom : ( expression )
            src = srcBk;
            if (next() == '(' &&
                (expr = parseExpression()) != null &&
                next() == ')'
                ) {
                return expr;
            }

            // atom : [ charset ]
            src = srcBk;
            if (next() == '[' &&
                (charset = parseCharSet()) != null &&
                next() == ']'
                ) {
                return charset;
            }

            // atom : [ ^ charset ]
            src = srcBk;
            if (next() == '[' &&
                next() == '^' &&
                (charset = parseCharSet()) != null &&
                next() == ']'
                ) {
                return new ASTCharSetNeg(charset.set);
            }

            src = srcBk;
            return null;
        }

        private ASTCharSet parseCharSet() {

            string srcBk = src;
            ASTCharSet set1, set2;
            // charset : charrange charset_tail
            if ((set1 = parseCharRange()) != null &&
                (set2 = parseCharSetTail()) != null
                ) {
                set1.set.UnionWith(set2.set);
                return set1;
            }

            src = srcBk;
            return null;
        }

        private ASTCharSet parseCharSetTail() {

            string srcBk = src;
            ASTCharSet set;

            // charset_tail : charset
            if ((set = parseCharSet()) != null) {
                return set;
            }

            // charset_tail : epsilon
            src = srcBk;
            return new ASTCharSet(new HashSet<char>());
        }

        private ASTCharSet parseCharRange() {

            string srcBk = src;
            ASTCharSet set1, set2;

            // charrange : character - character
            if ((set1 = parseCharacter()) != null &&
                next() == '-' &&
                (set2 = parseCharacter()) != null
                ) {
                if (set1.set.Count != 1 || set2.set.Count != 1) {
                    throw new InvalidOperationException("parseCharacter should return only 1 char.");
                }
                char beg = set1.set.First();
                char end = set2.set.First();
                HashSet<char> set = new HashSet<char>();
                for (char i = beg; i <= end; ++i) {
                    set.Add(i);
                }
                return new ASTCharSet(set);
            }

            // charrange : character
            src = srcBk;
            if ((set1 = parseCharacter()) != null) {
                return set1;
            }

            src = srcBk;
            return null;
        }

        private ASTCharSet parseCharacter() {

            string srcBk = src;
            char c;
            
            // character : \ anychar
            if (next() == '\\') {
                c = next();
                switch (c) {
                    case NONE:
                        src = srcBk;
                        return null;
                    case 'r':
                        return new ASTCharSet(new HashSet<char> { '\r' });
                    case 'n':
                        return new ASTCharSet(new HashSet<char> { '\n' });
                    case 't':
                        return new ASTCharSet(new HashSet<char> { '\t' });
                    default:
                        return new ASTCharSet(new HashSet<char> { c });
                }
            }

            // character : anycharexceptmetachar
            src = srcBk;
            c = next();
            switch (c) {
                case NONE:
                    src = srcBk;
                    return null;
                case '*':
                case '|':
                case '?':
                case '+':
                case '(':
                case ')':
                case '[':
                case ']':
                case '-':
                case '^':
                    src = srcBk;
                    return null;
                case '.':
                    return new ASTCharSetWild();
                default:
                    return new ASTCharSet(new HashSet<char> { c });
            }
        }

        #endregion

        #region Helper function.

        private char next() {
            if (more()) {
                char c = src[0];
                src = src.Substring(1);
                return c;
            } else {
                return NONE;
            }
        }

        private bool more() {
            return src.Length > 0;
        }

        #endregion

        private string src;

        private const char NONE = (char)0;
    }
}
