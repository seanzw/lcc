using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lexer;
using static Parserc.Parserc;

namespace Parser {

    /**************************************************

    lex
        : aliases SPLITER codes SPLITER rules SPLITER codes
        ;

    aliases
        : alias aliaes
        | epsilon
        ;
    
    alias
        : ALIAS REGEX_LITERAL
        ;

    rules
        : rule rules
        | rule
        ;

    rule
        : REGEX_LITERAL codes
        ;

    codes
        : CODE_LINE codes
        | epsilon
        ;
    
    ***************************************************/

    static class Parser {

        public static ASTLex Parse(List<Token> tokens) {
            Parserc.TokenStream<Token> stream = new Parserc.TokenStream<Token>(tokens.AsReadOnly());
            var result = Lex().End()(stream);
            if (result.Count() == 0) {
                throw new ArgumentException("Syntax Error: failed parsing!");
            } else if (result.Count() > 1) {
                throw new ArgumentException("Syntax Error: ambiguous result!");
            } else {
                return result.First().Value;
            }
        }

        static Parserc.Parser<Token, ASTLex> Lex() {
            return Alias().Many()
                .Bind(aliases => Match<T_SPLITER>()
                .Then(Code().Many()
                .Bind(headers => Match<T_SPLITER>()
                .Then(Rule().Plus()
                .Bind(rules => Match<T_SPLITER>()
                .Then(Code().Many()
                .Select(codes => new ASTLex(aliases, headers, rules, codes))))))));
        }

        static Parserc.Parser<Token, ASTAlias> Alias() {
            return Match<T_ALIAS>()
                .Select(t => t.src)
                .Bind(alias => RegExLiteral()
                    .Select(regex_literal => new ASTAlias(alias, regex_literal)));
        }

        static Parserc.Parser<Token, ASTRule> Rule() {
            return RegExLiteral()
                .Bind(regex_literal => Code()
                .Many()
                .Select(codes => new ASTRule(regex_literal, codes)));
        }

        static Parserc.Parser<Token, string> Code() {
            return Match<T_CODE>().Select(t => t.src);
        }

        static Parserc.Parser<Token, string> RegExLiteral() {
            return Match<T_REGEX>().Select(t => t.src);
        }

        static Parserc.Parser<Token, T> Match<T>()
            where T : Token {
            return Sat<Token>(t => (t as T) != null)
                .Select(t => t as T);
        }
    }
}
