using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Parserc.Parserc;
using lcc.Token;
using lcc.AST;

namespace lcc.Parser {
    public static partial class Parser {

        /// <summary>
        /// statement
        ///     : labeled-statement
        ///     | case-statement
        ///     | default-statment
        ///     | compound-statement
        ///     | expression-statement
        ///     | while-statement
        ///     | do-statement
        ///     | for-statement
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTStatement> Statement() {
            return LabeledStatement().Cast<Token.Token, ASTStatement, ASTLabeledStatement>()
                .Or(CaseStatement().Cast<Token.Token, ASTStatement, ASTCaseStatement>())
                .Or(DefaultStatement().Cast<Token.Token, ASTStatement, ASTDefaultStatement>())
                .Or(CompoundStatement().Cast<Token.Token, ASTStatement, ASTCompoundStatement>())
                ;
        }

        /// <summary>
        /// labeled-statement
        ///     : identfier : statement
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTLabeledStatement> LabeledStatement() {
            return Identifier()
                .Bind(identifier => Match<T_PUNC_COLON>()
                .Then(Ref(Statement))
                .Select(statement => new ASTLabeledStatement(identifier, statement)));
        }

        /// <summary>
        /// case-statement
        ///     : case constant-expression : statement
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTCaseStatement> CaseStatement() {
            return Match<T_KEY_CASE>()
                .Then(ConstantExpression())
                .Bind(expr => Match<T_PUNC_COLON>()
                .Then(Ref(Statement))
                .Select(statement => new ASTCaseStatement(expr, statement)));
        }

        /// <summary>
        /// default-statement
        ///     : default : statement
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTDefaultStatement> DefaultStatement() {
            return Match<T_KEY_DEFAULT>()
                .Then(Match<T_PUNC_COLON>())
                .Then(Ref(Statement))
                .Select(statement => new ASTDefaultStatement(statement));
        }

        /// <summary>
        /// compound-statement
        ///     : { block-item-list_opt }
        ///     ;
        ///     
        /// block-item-list
        ///     : block-item
        ///     | block-item-list block-item
        ///     ;
        ///     
        /// block-item
        ///     : declaration
        ///     | statement
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTCompoundStatement> CompoundStatement() {
            return Declaration()
                .Cast<Token.Token, ASTStatement, ASTDeclaration>()
                .Or(Ref(Statement))
                .Many()
                .BracelLR()
                .Select(statements => new ASTCompoundStatement(statements));
        }

        /// <summary>
        /// expression-statement
        ///     : expression_opt ;
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTStatement> ExpressionStatement() {
            return Expression().Bind(expr => Match<T_PUNC_SEMICOLON>().Return(expr as ASTStatement))
                .Or(Get<T_PUNC_SEMICOLON>().Select(t => new ASTVoidStatement(t.line) as ASTStatement));
        }

        /// <summary>
        /// while-statement
        ///     : while ( expression ) statement
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTWhileStatement> WhileStatement() {
            return Match<T_KEY_WHILE>()
                .Then(Expression().ParentLR())
                .Bind(expr => Ref(Statement)
                .Select(statement => new ASTWhileStatement(expr, statement)));
        }

        /// <summary>
        /// do-statement
        ///     : do statement while ( expression ) ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTDoStatement> DoStatement() {
            return Match<T_KEY_DO>()
                .Then(Ref(Statement))
                .Bind(statement => Match<T_KEY_WHILE>()
                .Then(Expression().ParentLR())
                .Bind(expr => Match<T_PUNC_SEMICOLON>().Return(new ASTDoStatement(expr, statement))));
        }

        /// <summary>
        /// for-statement
        ///     : for ( expression_opt ; expression_opt ; expression_opt ) statement
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTForStatement> ForStatement() {
            return Get<T_KEY_FOR>()
                .Bind(t => Match<T_PUNC_PARENTL>()
                .Then(Expression().ElseNull())
                .Bind(init => Match<T_PUNC_SEMICOLON>()
                .Then(Expression().ElseNull())
                .Bind(pred => Match<T_PUNC_SEMICOLON>()
                .Then(Expression().ElseNull())
                .Bind(iter => Match<T_PUNC_PARENTR>()
                .Then(Ref(Statement))
                .Select(statement => new ASTForStatement(t.line, init, pred, iter, statement))))));
        }
    }
}
