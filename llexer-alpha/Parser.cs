using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace llexer_alpha {
    public class Parser {

        public Parser() { }

        public ASTLex parse(List<Token> tokens) {
            this.tokens = tokens;
            idx = 0;
            return parseLex();
        }

        private ASTLex parseLex() {

            int idxBk = idx;
            List<ASTRule> rules;
            List<string> codes;

            // lex : rules_list SPLITER code_list.
            if ((rules = parseRules()) != null &&
                next().type == Token.TYPE.SPLITER &&
                (codes = parseCodes()) != null
                ) {
                return new ASTLex(rules, codes);
            }

            idx = idxBk;
            return null;
        }

        private List<ASTRule> parseRules() {

            int idxBk = idx;
            List<ASTRule> rules;
            ASTRule rule;

            // rules : rule rules_tail
            if ((rule = parseRule()) != null &&
                (rules = parseRulesTail()) != null
                ) {
                rules.Add(rule);
                return rules;
            }

            idx = idxBk;
            return null;
        }

        private List<ASTRule> parseRulesTail() {

            int idxBk = idx;
            List<ASTRule> rules;

            // rules_tail : rules
            if ((rules = parseRules()) != null) {
                return rules;
            }

            // rules_tail : epsilon
            idx = idxBk;
            return new List<ASTRule>();
        }

        private ASTRule parseRule() {
            int idxBk = idx;
            List<string> codes;

            // rule : REGEX_LITERAL codes
            Token token = next();
            if (match(token, Token.TYPE.REGEX_LITERAL) &&
                (codes = parseCodes()) != null
                ) {
                return new ASTRule(token.getSrc(), codes);
            }

            idx = idxBk;
            return null;
        }

        private List<string> parseCodes() {

            int idxBk = idx;
            List<string> codes;

            // codes : code codes
            Token token = next();
            if (match(token, Token.TYPE.CODE_LINE) &&
                (codes = parseCodes()) != null
                ) {
                codes.Add(token.getSrc());
                return codes;
            }

            // codes : epsilon
            idx = idxBk;
            return new List<string>();
        }

        private List<string> parseCodesTail() {
            int idxBk = idx;
            List<string> codes;

            // codes_tail : codes
            if ((codes = parseCodes()) != null) {
                return codes;
            }

            // codes_tail : epsilon
            idx = idxBk;
            return new List<string>();
        }

        /// <summary>
        /// Match a token.
        /// </summary>
        /// <param name="type"> Which type of token to be matched? </param>
        /// <returns> True if matched. </returns>
        private bool match(Token token, Token.TYPE type) {
            if (token != null) {
                return token.type == type;
            } else {
                return false;
            }
        }

        private bool more() {
            return idx < tokens.Count();
        }

        private Token next() {
            if (more()) {
                idx++;
                return tokens[idx - 1];
            } else {
                return null;
            }
        }

        private int idx;
        private List<Token> tokens;

        ///* Parse a lex.*/
        //static public bool parse_lex(string src, ref int pos, out ASTLex ret) {

        //    Utility.DEBUG("parse_lex: " + pos);

        //    List<ASTRule> rules;
        //    ASTCode code;
        //    int posBk = pos;
        //    if (parse_rules_list(src, ref pos, out rules) &&
        //        parse_code(src, ref pos, out code)
        //        ) {
        //        ret = new ASTLex(rules, code);
        //        return true;
        //    }

        //    pos = posBk;
        //    ret = null;
        //    return false;
        //}

        ///* Parse a rules_list. */
        //static public bool parse_rules_list(string src, ref int pos, out List<ASTRule> ret) {

        //    Utility.DEBUG("parse_rules_list: " + pos);
        //    ASTRule rule;
        //    List<ASTRule> rules;
        //    int posBk = pos;
        //    if (parse_rule(src, ref pos, out rule) &&
        //        parse_rules_list_prime(src, ref pos, out rules)
        //        ) {
        //        rules.Add(rule);
        //        ret = rules;
        //        return true;
        //    }

        //    pos = posBk;
        //    ret = null;
        //    return false;
        //}

        //static private bool parse_rules_list_prime(string src, ref int pos, out List<ASTRule> ret) {

        //    List<ASTRule> rules;
        //    int posBk = pos;
        //    if (parse_rules_list(src, ref pos, out rules)) {
        //        ret = rules;
        //        return true;
        //    }

        //    /* Match epsilon. */
        //    pos = posBk;
        //    ret = new List<ASTRule>();
        //    return true;
        //}

        ///* Parse a rule. */
        //static private bool parse_rule(string src, ref int pos, out ASTRule ret) {

        //    Utility.DEBUG("parse_rule: " + pos);

        //    ASTRegEx reg;
        //    //ASTToken token;
        //    ASTCode code;
        //    int posBk = pos;
        //    if (parse_reg(src, ref pos, out reg) &&
        //        match(src, ref pos, ' ') &&
        //        parse_code(src, ref pos, out code)
        //        ) {

        //        // Skep this line.
        //        while (src[pos] != '\n') {
        //            pos++;
        //        }
        //        pos++;
        //        ret = new ASTRule(reg, code);
        //        return true;
        //    }

        //    pos = posBk;
        //    ret = null;
        //    return false;
        //}

        //static private bool parse_code(string src, ref int pos, out ASTCode ret) {

        //    int posBk = pos;
        //    if (!match(src, ref pos, '{')) {
        //        pos = posBk;
        //        ret = null;
        //        return false;
        //    }

        //    while (pos < src.Length) {
        //        pos++;
        //        if (src[pos - 1] == '}' && src[pos - 2] == '\n') {
        //            ret = new ASTCode(src.Substring(posBk, pos - posBk));
        //            return true;
        //        }
        //    }

        //    pos = posBk;
        //    ret = null;
        //    return false;
        //}

        //static public bool parse_reg(string src, ref int pos, out ASTRegEx ret) {

        //    Utility.DEBUG("parse_reg: " + pos);

        //    int posBk = pos;
        //    ASTRegEx reg;
        //    if (parse_atom(src, ref pos, out reg) &&
        //        parse_reg_prime(src, ref pos, ref reg)
        //        ) {
        //        ret = reg;
        //        return true;
        //    }

        //    pos = posBk;
        //    if (match(src, ref pos, '[') &&
        //        parse_reg(src, ref pos, out reg) &&
        //        match(src, ref pos, ']') &&
        //        parse_reg_prime(src, ref pos, ref reg)
        //        ) {
        //        ret = reg;
        //        return true;
        //    }

        //    pos = posBk;
        //    ret = null;
        //    return false;
        //}

        ///// <summary>
        ///// Helper parser function to eliminate left recursion.
        ///// </summary>
        ///// <param name="src">Source string. </param>
        ///// <param name="pos">Current position. </param>
        ///// <param name="ret">Inout ASTReg. </param>
        ///// <returns></returns>
        //static public bool parse_reg_prime(string src, ref int pos, ref ASTRegEx ret) {

        //    int posBk = pos;
        //    ASTRegEx next;
        //    if (match(src, ref pos, '*')) {
        //        ret = new ASTRegStar(ret);
        //        return parse_reg_prime(src, ref pos, ref ret);
        //    }

        //    pos = posBk;
        //    if (match(src, ref pos, '+')) {
        //        ret = new ASTRegAnd(ret, new ASTRegStar(ret));
        //        return parse_reg_prime(src, ref pos, ref ret);
        //    }

        //    pos = posBk;
        //    if (match(src, ref pos, '?')) {
        //        ret = new ASTRegQuestion(ret);
        //        return parse_reg_prime(src, ref pos, ref ret);
        //    }

        //    pos = posBk;
        //    if (match(src, ref pos, '-') &&
        //        parse_reg(src, ref pos, out next)
        //        ) {
        //        ret = new ASTRegAnd(ret, next);
        //        return parse_reg_prime(src, ref pos, ref ret);
        //    }

        //    pos = posBk;
        //    if (match(src, ref pos, '|') &&
        //        parse_reg(src, ref pos, out next)
        //        ) {
        //        ret = new ASTRegOr(ret, next);
        //        return parse_reg_prime(src, ref pos, ref ret);
        //    }

        //    // Match epsilon.
        //    pos = posBk;
        //    return true;
        //}

        //static public bool parse_atom(string src, ref int pos, out ASTRegEx ret) {

        //    int posBk = pos;
        //    if (match(src, ref pos, '\\')) {
        //        if (match(src, ref pos, '\\')) {

        //            /* Handle some special char set. */
        //            if (pos + 3 <= src.Length) {
        //                if (src.Substring(pos, 3) == "a-z") {
        //                    ret = new ASTAtom(atoz);
        //                    pos += 3;
        //                    return true;
        //                }

        //                if (src.Substring(pos, 3) == "a-Z") {
        //                    ret = new ASTAtom(atoZ);
        //                    pos += 3;
        //                    return true;
        //                }

        //                if (src.Substring(pos, 3) == "0-9") {
        //                    ret = new ASTAtom(digits);
        //                    pos += 3;
        //                    return true;
        //                }

        //                if (src.Substring(pos, 3) == "WLD") {
        //                    ret = new ASTAtom(CharMap.WILD);
        //                    pos += 3;
        //                    return true;
        //                }
        //            }

        //            /* Handle the escaped char. */
        //            char c;
        //            if (src[pos] == 'n') {
        //                c = '\n';
        //            } else if (src[pos] == 't') {
        //                c = '\t';
        //            } else if (src[pos] == 'r') {
        //                c = '\r';
        //            } else {
        //                c = src[pos];
        //            }

        //            ret = new ASTAtom(c);
        //            pos++;
        //            return true;
        //        }

        //        /* Handle normal char. */
        //        pos = posBk + 1;
        //        if (pos < src.Length) {
        //            ret = new ASTAtom(src[pos]);
        //            pos += 1;
        //            return true;
        //        }
        //    }

        //    pos = posBk;
        //    ret = null;
        //    return false;

        //}

        //private static readonly int[] atoz = {
        //    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
        //    'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        //};

        //private static readonly int[] atoZ = {
        //    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
        //    'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        //    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
        //    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        //};

        //private static readonly int[] digits = {
        //    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        //};

        ///* Helper function to match a char and advance the pos. */
        //static private bool match(string src, ref int pos, char c) {
        //    return pos < src.Length && src[pos++] == c;
        //}

        ///* Helper delegate to eliminate the left recursion. */
        //delegate Func<A, R> Recursive<A, R>(Recursive<A, R> r);
    }
}
