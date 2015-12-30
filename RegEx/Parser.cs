using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Parserc.Parserc;
using static Parserc.PChar.CharParser;

namespace RegEx {

    /// <summary>
    /// Parser written in parser combinator.
    /// </summary>
    static class Parser {

        /***********************************************************
    
        expression
            : term '|' expression
            | term
            ;

        term
            : factor term
            | factor
            ;

        factor
            : atom meta
            ;

        meta
            : '*'
            | '+'
            | '?'
            | epsilon
            ;

        atom
            : singlechar
            | '(' expression ')'
            | '[' charset ']'
            | '[' '^' charset ']'
            ;

        charset
            : charrange charset
            | charrange
            ;

        charrange
            : singlechar '-' singlechar
            | singlechar
            ;

    *************************************************************/

        public static ASTExpr Parse(string src) {
            Parserc.PChar.CharStream tokens = new Parserc.PChar.CharStream(src);
            var results = Expression().End()(tokens);
            if (results.Count == 0) {
                throw new ArgumentException("Syntax Error: failed parsing!");
            } else if (results.Count > 1) {
                throw new ArgumentException("Syntax Error: ambiguous result!");
            } else {
                return results.First().value;
            }
        }

        static Parserc.Parser<char, ASTExpr> Expression() {
            return Term()
                .PlusSeperatedBy(Character('|'))
                .Bind(terms => Result<char, ASTExpr>(new ASTExpr(terms)));
        }

        static Parserc.Parser<char, ASTTerm> Term() {
            return Factor()
                .Plus()
                .Bind(factors => Result<char, ASTTerm>(new ASTTerm(factors)));
        }

        static Parserc.Parser<char, ASTFactor> Factor() {
            return Atom()
                .Bind(atom => Meta()
                .Bind(meta => Result<char, ASTFactor>(new ASTFactor(atom, meta))));
        }

        static Parserc.Parser<char, ASTFactor.MetaChar> Meta() {
            return Character('*').Bind(x => Result<char, ASTFactor.MetaChar>(ASTFactor.MetaChar.STAR))
                .Else(Character('+').Bind(x => Result<char, ASTFactor.MetaChar>(ASTFactor.MetaChar.PLUS)))
                .Else(Character('?').Bind(x => Result<char, ASTFactor.MetaChar>(ASTFactor.MetaChar.QUES)))
                .Else(Result<char, ASTFactor.MetaChar>(ASTFactor.MetaChar.NULL));
        }

        static Parserc.Parser<char, ASTRegEx> Atom() {
            return SingleChar().Trans<char, ASTCharSet, ASTRegEx>()
                .Or(Ref(Expression)
                    .Bracket(Character('('), Character(')'))
                    .Trans<char, ASTExpr, ASTRegEx>())
                .Or(CharSet()
                    .Bracket(Character('['), Character(']'))
                    .Trans<char, ASTCharSet, ASTRegEx>())
                .Or(Character('^')
                    .Bind(_ => CharSet()
                    .Bind(charset => Result<char, ASTCharSetNeg>(new ASTCharSetNeg(charset.set))))
                    .Bracket(Character('['), Character(']'))
                    .Trans<char, ASTCharSetNeg, ASTRegEx>());
        }

        static Parserc.Parser<char, ASTCharSet> CharSet() {
            return CharRange().Plus().Bind(sets => {
                HashSet<char> union = new HashSet<char>();
                foreach (var set in sets) {
                    union.UnionWith(set);
                }
                return Result<char, ASTCharSet>(new ASTCharSet(union));
            });
        }

        static Parserc.Parser<char, HashSet<char>> CharRange() {
            return SingleChar()
                .Bind(beg => 
                    Character('-')
                    .Then(SingleChar()
                    .Bind(end => {
                        HashSet<char> set = new HashSet<char>();
                        for (char i = beg.set.First(); i <= end.set.First(); ++i) {
                            set.Add(i);
                        }
                        return Result<char, HashSet<char>>(set);
                    }))
                    .Else(Result<char, HashSet<char>>(beg.set))
                );
        }

        static Parserc.Parser<char, ASTCharSet> SingleChar() {
            return Character('\\')
                .Then(Character('r')
                    .Then(Result<char, ASTCharSet>(new ASTCharSet(new HashSet<char> { '\r' })))
                    .Else(Character('n')
                    .Then(Result<char, ASTCharSet>(new ASTCharSet(new HashSet<char> { '\n' }))))
                    .Else(Character('t')
                    .Then(Result<char, ASTCharSet>(new ASTCharSet(new HashSet<char> { '\t' }))))
                    .Else(Item<char>()
                    .Bind(c => Result<char, ASTCharSet>(new ASTCharSet(new HashSet<char> { c })))))
                .Else(Item<char>()
                    .Bind(c => {
                        switch (c) {
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
                                return Zero<char, ASTCharSet>();
                            case '.':
                                return Result<char, ASTCharSet>(new ASTCharSetWild());
                            default:
                                return Result<char, ASTCharSet>(new ASTCharSet(new HashSet<char> { c }));
                        }
                    }));
        }
    }
}
