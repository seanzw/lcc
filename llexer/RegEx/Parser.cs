using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegEx {
    public class Parser {

        public Parser() {}

        public ASTExpression parse(string src) {
            idx = 0;
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

            int idxBk = idx;
            ASTExpression expr;
            ASTTerm term;

            // expression : term expression_tail
            if ((term = parseTerm()) != null &&
                (expr = parseExpressionTail()) != null
                ) {
                expr.terms.AddFirst(term);
                return expr;
            }

            idx = idxBk;
            return null;
        }

        private ASTExpression parseExpressionTail() {
            int idxBk = idx;
            ASTExpression expr;

            // expression_tail : | expression
            if (next() == '|' &&
                (expr = parseExpression()) != null
                ) {
                return expr;
            }

            // expression_tail : epsilon
            idx = idxBk;
            return new ASTExpression();
        }

        private ASTTerm parseTerm() {

            int idxBk = idx;
            ASTFactor factor;
            ASTTerm term;

            // term : factor term_tail
            if ((factor = parseFactor()) != null &&
                (term = parseTermTail()) != null
                ) {
                term.factors.AddFirst(factor);
                return term;
            }

            idx = idxBk;
            return null;

        }

        private ASTTerm parseTermTail() {

            int idxBk = idx;
            ASTTerm term;

            // term_tail : term
            if ((term = parseTerm()) != null) {
                return term;
            }

            // term_tail : epsilon
            idx = idxBk;
            return new ASTTerm();
        }

        private ASTFactor parseFactor() {

            int idxBk = idx;

            ASTRegEx atom;
            if ((atom = parseAtom()) != null) {
                ASTFactor.MetaChar meta = parseMeta();
                return new ASTFactor(atom, meta);
            }

            idx = idxBk;
            return null;
        }

        private ASTFactor.MetaChar parseMeta() {
            int idxBk = idx;
            switch (next()) {
                case '*':
                    return ASTFactor.MetaChar.STAR;
                case '+':
                    return ASTFactor.MetaChar.PLUS;
                case '?':
                    return ASTFactor.MetaChar.QUES;
                case NONE:
                default:
                    idx = idxBk;
                    return ASTFactor.MetaChar.NULL;
            }
        }

        private ASTRegEx parseAtom() {

            int idxBk = idx;
            ASTRegEx expr;
            ASTCharSet charset;

            // atom : character
            if ((charset = parseCharacter()) != null) {
                return charset;
            }

            // atom : ( expression )
            idx = idxBk;
            if (next() == '(' &&
                (expr = parseExpression()) != null &&
                next() == ')'
                ) {
                return expr;
            }

            // atom : [ charset ]
            idx = idxBk;
            if (next() == '[' &&
                (charset = parseCharSet()) != null &&
                next() == ']'
                ) {
                return charset;
            }

            // atom : [ ^ charset ]
            idx = idxBk;
            if (next() == '[' &&
                next() == '^' &&
                (charset = parseCharSet()) != null &&
                next() == ']'
                ) {
                return new ASTCharSetNeg(charset.set);
            }

            idx = idxBk;
            return null;
        }

        private ASTCharSet parseCharSet() {

            int idxBk = idx;
            ASTCharSet set1, set2;
            // charset : charrange charset_tail
            if ((set1 = parseCharRange()) != null &&
                (set2 = parseCharSetTail()) != null
                ) {
                set1.set.UnionWith(set2.set);
                return set1;
            }

            idx = idxBk;
            return null;
        }

        private ASTCharSet parseCharSetTail() {

            int idxBk = idx;
            ASTCharSet set;

            // charset_tail : charset
            if ((set = parseCharSet()) != null) {
                return set;
            }

            // charset_tail : epsilon
            idx = idxBk;
            return new ASTCharSet(new HashSet<char>());
        }

        private ASTCharSet parseCharRange() {

            int idxBk = idx;
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
            idx = idxBk;
            if ((set1 = parseCharacter()) != null) {
                return set1;
            }

            idx = idxBk;
            return null;
        }

        private ASTCharSet parseCharacter() {

            int idxBk = idx;
            char c;
            
            // character : \ anychar
            if (next() == '\\') {
                c = next();
                switch (c) {
                    case NONE:
                        idx = idxBk;
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
            idx = idxBk;
            c = next();
            switch (c) {
                case NONE:
                    idx = idxBk;
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
                    idx = idxBk;
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
                idx++;
                return src[idx - 1];
            } else {
                return NONE;
            }
        }

        private bool more() {
            return idx < src.Length;
        }

        #endregion
        private int idx;
        private string src;

        private const char NONE = (char)0;
    }
}
