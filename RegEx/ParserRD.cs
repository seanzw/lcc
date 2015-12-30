using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegEx {

    /***********************************************************
    
        expression
            : term expression_tail
            ;

        expression_tail
            : '|' expression
            | epsilon
            ;

        term
            : factor term_tail
            ;

        term_tail
            : term
            | epsilon
            ;

        factor
            : atom
            | atom meta
            ;

        atom
            : character
            | '(' expression ')'
            | '[' charset ']'
            | '[' '^' charset ']'
            ;

        charset
            : charrange charset_tail
            ;

        charset_tail
            : charset
            | epsilon
            ;

        charrange
            : character '-' character
            | character
            ;

    *************************************************************/

    /// <summary>
    /// Parser written in recursive descent.
    /// </summary>
    class Parser {

        private static Parser instance = new Parser();

        public static Parser Instance {
            get {
                return instance;
            }
        }

        private Parser() {}

        public ASTExpr Parse(string src) {
            idx = 0;
            this.src = src;
            ASTExpr expr = ParseExpr();
            if (More()) {
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

        private ASTExpr ParseExpr() {

            int idxBk = idx;
            ASTExpr expr;
            ASTTerm term;

            // expression : term expression_tail
            if ((term = ParseTerm()) != null &&
                (expr = ParseExprTail()) != null
                ) {
                expr.terms.AddFirst(term);
                return expr;
            }

            idx = idxBk;
            return null;
        }

        private ASTExpr ParseExprTail() {
            int idxBk = idx;
            ASTExpr expr;

            // expression_tail : | expression
            if (Next() == '|' &&
                (expr = ParseExpr()) != null
                ) {
                return expr;
            }

            // expression_tail : epsilon
            idx = idxBk;
            return new ASTExpr();
        }

        private ASTTerm ParseTerm() {

            int idxBk = idx;
            ASTFactor factor;
            ASTTerm term;

            // term : factor term_tail
            if ((factor = ParseFactor()) != null &&
                (term = ParseTermTail()) != null
                ) {
                term.factors.AddFirst(factor);
                return term;
            }

            idx = idxBk;
            return null;

        }

        private ASTTerm ParseTermTail() {

            int idxBk = idx;
            ASTTerm term;

            // term_tail : term
            if ((term = ParseTerm()) != null) {
                return term;
            }

            // term_tail : epsilon
            idx = idxBk;
            return new ASTTerm();
        }

        private ASTFactor ParseFactor() {

            int idxBk = idx;

            ASTRegEx atom;
            if ((atom = ParseAtom()) != null) {
                ASTFactor.MetaChar meta = ParseMeta();
                return new ASTFactor(atom, meta);
            }

            idx = idxBk;
            return null;
        }

        private ASTFactor.MetaChar ParseMeta() {
            int idxBk = idx;
            switch (Next()) {
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

        private ASTRegEx ParseAtom() {

            int idxBk = idx;
            ASTRegEx expr;
            ASTCharSet charset;

            // atom : character
            if ((charset = ParseCharacter()) != null) {
                return charset;
            }

            // atom : ( expression )
            idx = idxBk;
            if (Next() == '(' &&
                (expr = ParseExpr()) != null &&
                Next() == ')'
                ) {
                return expr;
            }

            // atom : [ charset ]
            idx = idxBk;
            if (Next() == '[' &&
                (charset = ParseCharSet()) != null &&
                Next() == ']'
                ) {
                return charset;
            }

            // atom : [ ^ charset ]
            idx = idxBk;
            if (Next() == '[' &&
                Next() == '^' &&
                (charset = ParseCharSet()) != null &&
                Next() == ']'
                ) {
                return new ASTCharSetNeg(charset.set);
            }

            idx = idxBk;
            return null;
        }

        private ASTCharSet ParseCharSet() {

            int idxBk = idx;
            ASTCharSet set1, set2;
            // charset : charrange charset_tail
            if ((set1 = ParseCharRange()) != null &&
                (set2 = ParseCharSetTail()) != null
                ) {
                set1.set.UnionWith(set2.set);
                return set1;
            }

            idx = idxBk;
            return null;
        }

        private ASTCharSet ParseCharSetTail() {

            int idxBk = idx;
            ASTCharSet set;

            // charset_tail : charset
            if ((set = ParseCharSet()) != null) {
                return set;
            }

            // charset_tail : epsilon
            idx = idxBk;
            return new ASTCharSet(new HashSet<char>());
        }

        private ASTCharSet ParseCharRange() {

            int idxBk = idx;
            ASTCharSet set1, set2;

            // charrange : character - character
            if ((set1 = ParseCharacter()) != null &&
                Next() == '-' &&
                (set2 = ParseCharacter()) != null
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
            if ((set1 = ParseCharacter()) != null) {
                return set1;
            }

            idx = idxBk;
            return null;
        }

        private ASTCharSet ParseCharacter() {

            int idxBk = idx;
            char c;
            
            // character : \ anychar
            if (Next() == '\\') {
                c = Next();
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
            c = Next();
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

        private char Next() {
            if (More()) {
                idx++;
                return src[idx - 1];
            } else {
                return NONE;
            }
        }

        private bool More() {
            return idx < src.Length;
        }

        #endregion
        private int idx;
        private string src;

        private const char NONE = (char)0;
    }
}
