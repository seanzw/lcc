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

        public ASTLex Parse(List<Token> tokens) {
            this.tokens = tokens;
            idx = 0;
            return ParseLex();
        }

        private ASTLex ParseLex() {

            int idxBk = idx;
            LinkedList<string> headers;
            LinkedList<ASTRule> rules;
            LinkedList<string> codes;

            // lex : codes SPLITER rules SPLITER codes.
            if ((headers = ParseCodes())    != null &&
                (Next() as T_SPLITER)       != null &&
                (rules = ParseRules())      != null &&
                (Next() as T_SPLITER)       != null &&
                (codes = ParseCodes())      != null
                ) {
                return new ASTLex(headers, rules, codes);
            }

            idx = idxBk;
            return null;
        }

        private LinkedList<ASTRule> ParseRules() {

            int idxBk = idx;
            LinkedList<ASTRule> rules;
            ASTRule rule;

            // rules : rule rules_tail
            if ((rule = ParseRule())        != null &&
                (rules = ParseRulesTail())  != null
                ) {
                rules.AddFirst(rule);
                return rules;
            }

            idx = idxBk;
            return null;
        }

        private LinkedList<ASTRule> ParseRulesTail() {

            int idxBk = idx;
            LinkedList<ASTRule> rules;

            // rules_tail : rules
            if ((rules = ParseRules()) != null) {
                return rules;
            }

            // rules_tail : epsilon
            idx = idxBk;
            return new LinkedList<ASTRule>();
        }

        private ASTRule ParseRule() {
            int idxBk = idx;
            LinkedList<string> codes;

            // rule : REGEX_LITERAL codes
            T_REGEX token;
            if ((token = Next() as T_REGEX) != null &&
                (codes = ParseCodes())      != null
                ) {
                return new ASTRule(token.getSrc(), codes);
            }

            idx = idxBk;
            return null;
        }

        private LinkedList<string> ParseCodes() {

            int idxBk = idx;
            LinkedList<string> codes;

            // codes : code codes
            T_CODE token;
            if ((token = Next() as T_CODE)  != null &&
                (codes = ParseCodes())      != null
                ) {
                codes.AddFirst(token.getSrc());
                return codes;
            }

            // codes : epsilon
            idx = idxBk;
            return new LinkedList<string>();
        }

        private bool More() {
            return idx < tokens.Count();
        }

        private Token Next() {
            if (More()) {
                return tokens[idx++];
            } else {
                return null;
            }
        }

        private int idx;
        private List<Token> tokens;
        
    }
}
