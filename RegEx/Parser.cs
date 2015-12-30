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
            : wild
            | singlechar
            | '(' expression ')'
            | '[' charset ']'
            | '[' '^' charset ']'
            ;

        wild
            : '.'
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
            return Term().PlusSeperatedBy(Character('|')).Select(terms => new ASTExpr(terms));
        }

        static Parserc.Parser<char, ASTTerm> Term() {
            return Factor().Plus().Select(factors => new ASTTerm(factors));
        }

        static Parserc.Parser<char, ASTFactor> Factor() {
            return Atom()
                .Bind(atom => Meta()
                .Select(meta => new ASTFactor(atom, meta)));
        }

        static Parserc.Parser<char, ASTFactor.MetaChar> Meta() {
            return Character('*').Return(ASTFactor.MetaChar.STAR)
                .Else(Character('+').Return(ASTFactor.MetaChar.PLUS))
                .Else(Character('?').Return(ASTFactor.MetaChar.QUES))
                .Else(Result<char, ASTFactor.MetaChar>(ASTFactor.MetaChar.NULL));
        }

        static Parserc.Parser<char, ASTRegEx> Atom() {
            return Wild().Select(x => x as ASTRegEx)
                .Or(SingleChar()
                    .Select(x => new ASTCharSet(new HashSet<char> { x }) as ASTRegEx))
                .Or(Ref(Expression)
                    .Bracket(Character('('), Character(')'))
                    .Select(x => x as ASTRegEx))
                .Or(CharSet()
                    .Bracket(Character('['), Character(']'))
                    .Select(x => x as ASTRegEx))
                .Or(Character('^')
                    .Then(CharSet()
                    .Select(charset => new ASTCharSetNeg(charset.set)))
                    .Bracket(Character('['), Character(']'))
                    .Select(x => x as ASTRegEx));
        }

        static Parserc.Parser<char, ASTCharSet> CharSet() {
            return CharRange().Plus().Select(sets => {
                HashSet<char> union = new HashSet<char>();
                foreach (var set in sets) {
                    union.UnionWith(set);
                }
                return new ASTCharSet(union);
            });
        }

        static Parserc.Parser<char, HashSet<char>> CharRange() {
            return SingleChar()
                .Bind(beg => 
                    Character('-')
                    .Then(SingleChar()
                    .Select(end => {
                        HashSet<char> set = new HashSet<char>();
                        for (char i = beg; i <= end; ++i) {
                            set.Add(i);
                        }
                        return set;
                    }))
                    .Else(Result<char, HashSet<char>>(new HashSet<char> { beg }))
                );
        }

        static Parserc.Parser<char, ASTCharSetWild> Wild() {
            return Character('.').Return(new ASTCharSetWild());
        }

        static Parserc.Parser<char, char> SingleChar() {
            return Character('\\')
                .Then(Character('r').Return('\r')
                    .Else(Character('n').Return('\n'))
                    .Else(Character('t').Return('\t'))
                    .Else(Item<char>()
                    .Select(c => c)))
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
                            case '.':
                                return Zero<char, char>();
                            default:
                                return Result<char, char>(c);
                        }
                    }));
        }
    }
}
