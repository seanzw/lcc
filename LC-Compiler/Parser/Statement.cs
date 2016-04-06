using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Parserc.Parserc;
using lcc.Token;
using lcc.AST;
using T = lcc.Token.Token;

namespace lcc.Parser {
    public static partial class Parser {

        /// <summary>
        /// statement
        ///     : labeled-statement
        ///     | case-statement
        ///     | default-statment
        ///     | compound-statement
        ///     | expression-statement
        ///     | if-statement
        ///     | switch-statement
        ///     | while-statement
        ///     | do-statement
        ///     | for-statement
        ///     | jump-statement
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTStmt> Statement() {
            return LabeledStatement().Cast<T, ASTStmt, ASTLabeled>()
                .Or(CaseStatement())
                .Or(DefaultStatement())
                .Or(CompoundStatement())
                .Or(ExpressionStatement())
                .Or(IfStatement())
                .Or(SwitchStatement())
                .Or(WhileStatement())
                .Or(DoStatement())
                .Or(ForStatement())
                .Or(JumpStatement())
                ;
        }

        /// <summary>
        /// labeled-statement
        ///     : identfier : statement
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTLabeled> LabeledStatement() {
            return Identifier()
                .Bind(identifier => Match<T_PUNC_COLON>()
                .Then(Ref(Statement))
                .Select(statement => new ASTLabeled(identifier, statement)));
        }

        /// <summary>
        /// case-statement
        ///     : case constant-expression : statement
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTCase> CaseStatement() {
            return Match<T_KEY_CASE>()
                .Then(ConstantExpression())
                .Bind(expr => Match<T_PUNC_COLON>()
                .Then(Ref(Statement))
                .Select(statement => new ASTCase(expr, statement)));
        }

        /// <summary>
        /// default-statement
        ///     : default : statement
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTDefault> DefaultStatement() {
            return Match<T_KEY_DEFAULT>()
                .Then(Match<T_PUNC_COLON>())
                .Then(Ref(Statement))
                .Select(statement => new ASTDefault(statement));
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
        public static Parserc.Parser<T, ASTCompoundStmt> CompoundStatement() {
            return Ref(Statement).Or(Declaration()).Many().BracelLR().Select(ss => new ASTCompoundStmt(ss));
        }

        /// <summary>
        /// expression-statement
        ///     : expression_opt ;
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTStmt> ExpressionStatement() {
            return Expression().Bind(expr => Match<T_PUNC_SEMICOLON>().Return(expr as ASTStmt))
                .Or(Get<T_PUNC_SEMICOLON>().Select(t => new ASTVoidStmt(t.line)));
        }

        /// <summary>
        /// if-statement
        ///     : if ( expression ) statement
        ///     | if ( expression ) statement else statement
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTIfStmt> IfStatement() {
            return Get<T_KEY_IF>()
                .Bind(t => Expression().ParentLR()
                .Bind(expr => Ref(Statement)
                .Bind(then => Match<T_KEY_ELSE>()
                .Then(Ref(Statement)).ElseNull()
                .Select(other => new ASTIfStmt(t.line, expr, then, other)))));
        }

        /// <summary>
        /// switch-statement
        ///     : switch ( expression ) statement
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTSwitch> SwitchStatement() {
            return Get<T_KEY_SWITCH>()
                .Bind(t => Expression().ParentLR()
                .Bind(expr => Ref(Statement)
                .Select(statement => new ASTSwitch(t.line, expr, statement))));
        }

        /// <summary>
        /// while-statement
        ///     : while ( expression ) statement
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTWhile> WhileStatement() {
            return Match<T_KEY_WHILE>()
                .Then(Expression().ParentLR())
                .Bind(expr => Ref(Statement)
                .Select(statement => new ASTWhile(expr, statement)));
        }

        /// <summary>
        /// do-statement
        ///     : do statement while ( expression ) ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTDo> DoStatement() {
            return Match<T_KEY_DO>()
                .Then(Ref(Statement))
                .Bind(statement => Match<T_KEY_WHILE>()
                .Then(Expression().ParentLR())
                .Bind(expr => Match<T_PUNC_SEMICOLON>().Return(new ASTDo(expr, statement))));
        }

        /// <summary>
        /// for-statement
        ///     : for ( expression_opt ; expression_opt ; expression_opt ) statement
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTForStmt> ForStatement() {
            return Get<T_KEY_FOR>()
                .Bind(t => Match<T_PUNC_PARENTL>()
                .Then(Expression().ElseNull())
                .Bind(init => Match<T_PUNC_SEMICOLON>()
                .Then(Expression().ElseNull())
                .Bind(pred => Match<T_PUNC_SEMICOLON>()
                .Then(Expression().ElseNull())
                .Bind(iter => Match<T_PUNC_PARENTR>()
                .Then(Ref(Statement))
                .Select(statement => new ASTForStmt(t.line, init, pred, iter, statement))))));
        }

        /// <summary>
        /// jump-statement
        ///     : goto identifier ;
        ///     | continue ;
        ///     | break ;
        ///     | return expression_opt
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTStmt> JumpStatement() {
            return Get<T_KEY_GOTO>()
                    .Bind(t => Identifier()
                    .Bind(label => Match<T_PUNC_SEMICOLON>()
                    .Return(new ASTGoto(t.line, label) as ASTStmt)))
                .Else(Get<T_KEY_CONTINUE>()
                    .Bind(t => Match<T_PUNC_SEMICOLON>()
                    .Return(new ASTContinue(t.line) as ASTStmt)))
                .Else(Get<T_KEY_BREAK>()
                    .Bind(t => Match<T_PUNC_SEMICOLON>()
                    .Return(new ASTBreak(t.line) as ASTStmt)))
                .Else(Get<T_KEY_RETURN>()
                    .Bind(t => Expression().ElseNull()
                    .Bind(expr => Match<T_PUNC_SEMICOLON>()
                    .Return(new ASTReturn(t.line, expr) as ASTStmt))));
        }

    }
}
