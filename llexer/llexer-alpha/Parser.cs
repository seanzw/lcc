using System;
using System.Collections.Generic;
using System.Linq;

using llexer_alpha;

namespace llparser {
    public class Parser {

        private static readonly Parser instance = new Parser();

        public static Parser Instance {
            get {
                return instance;
            }
        }

        private Parser() { }

        public ASTLex parse(List<Token> tokens) {
            this.tokens = tokens;
            idx = 0;
            return parseLex();
        }

        private ASTLex parseLex() {

            int idxBk = idx;
            List<string> headers;
            List<ASTRule> rules;
            List<string> codes;

            // lex : codes SPLITER rules SPLITER codes.
            if ((headers = parseCodes()) != null &&
                match(next(), Token.TYPE.SPLITER) &&
                (rules = parseRules()) != null &&
                next().type == Token.TYPE.SPLITER &&
                (codes = parseCodes()) != null
                ) {
                return new ASTLex(headers, rules, codes);
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
                return tokens[idx++];
            } else {
                return null;
            }
        }

        private int idx;
        private List<Token> tokens;
        
    }
}
