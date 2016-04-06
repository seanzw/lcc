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
        /// constant-expression
        ///     : conditional-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> ConstantExpression() {
            return ConditionalExpression();
        }

        /// <summary>
        /// expression
        ///     : assignment-expression
        ///     | expression , assignment-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> Expression() {
            return AssignmentExpression().PlusSeperatedBy(Match<T_PUNC_COMMA>())
                .Select(exprs => exprs.Count == 1 ? exprs.First() : new ASTCommaExpr(exprs));
        }

        /// <summary>
        /// assignment-expression
        ///     : conditional-expression
        ///     | unary-expression assignment-operator assignment-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> AssignmentExpression() {
            return ConditionalExpression()
                .Or(UnaryExpression()
                    .Bind(lexpr => AssgnmentOperator()
                    .Bind(op => AssignmentExpression()
                    .Select(rexpr => new ASTAssignExpr(lexpr, rexpr, op)))));
        }

        /// <summary>
        /// assignment-operator
        ///     : one of = *= /= %= += -= &lt&lt= &gt&gt= &= ^= |=
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTAssignExpr.Op> AssgnmentOperator() {
            return Match<T_PUNC_ASSIGN>().Return(ASTAssignExpr.Op.ASSIGN)
                .Else(Match<T_PUNC_ASSIGN>().Return(ASTAssignExpr.Op.ASSIGN))
                .Else(Match<T_PUNC_MULEQ>().Return(ASTAssignExpr.Op.MULEQ))
                .Else(Match<T_PUNC_DIVEQ>().Return(ASTAssignExpr.Op.DIVEQ))
                .Else(Match<T_PUNC_MODEQ>().Return(ASTAssignExpr.Op.MODEQ))
                .Else(Match<T_PUNC_PLUSEQ>().Return(ASTAssignExpr.Op.PLUSEQ))
                .Else(Match<T_PUNC_MINUSEQ>().Return(ASTAssignExpr.Op.MINUSEQ))
                .Else(Match<T_PUNC_SHIFTLEQ>().Return(ASTAssignExpr.Op.SHIFTLEQ))
                .Else(Match<T_PUNC_SHIFTREQ>().Return(ASTAssignExpr.Op.SHIFTREQ))
                .Else(Match<T_PUNC_BITANDEQ>().Return(ASTAssignExpr.Op.BITANDEQ))
                .Else(Match<T_PUNC_BITXOREQ>().Return(ASTAssignExpr.Op.BITXOREQ))
                .Else(Match<T_PUNC_BITOREQ>().Return(ASTAssignExpr.Op.BITOREQ));
        }

        /// <summary>
        /// conditional-expression
        ///     : logical-OR-expression
        ///     | logical-OR-expression ? expression : conditional-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> ConditionalExpression() {
            return LogicalORExpression()
                .Bind(predicator => Match<T_PUNC_QUESTION>()
                    .Then(Ref(Expression)
                    .Bind(trueExpr => Match<T_PUNC_COLON>()
                    .Then(Ref(ConditionalExpression)
                    .Select(falseExpr => new ASTConditionalExpr(predicator, trueExpr, falseExpr)))))
                    .Else(Result<Token.Token, ASTExpr>(predicator))
                );
        }

        /// <summary>
        /// logical-OR-expression
        ///     : logical-AND-expression
        ///     | logical-OR-expression || logical-AND-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> LogicalORExpression() {
            return LogicalANDExpression().ChainBinaryExpr(Match<T_PUNC_LOGOR>().Return(ASTBinaryExpr.Op.LOGOR));
        }

        /// <summary>
        /// logical-AND-expression
        ///     : OR-expression
        ///     | logical-AND-expression && OR-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> LogicalANDExpression() {
            return ORExpression().ChainBinaryExpr(Match<T_PUNC_LOGAND>().Return(ASTBinaryExpr.Op.LOGAND));
        }

        /// <summary>
        /// OR-expression
        ///     : XOR-expression
        ///     | OR-expression | XOR-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> ORExpression() {
            return XORExpression().ChainBinaryExpr(Match<T_PUNC_BITOR>().Return(ASTBinaryExpr.Op.OR));
        }

        /// <summary>
        /// XOR-expression
        ///     : AND-expression
        ///     | XOR-expression ^ AND-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> XORExpression() {
            return ANDExpression().ChainBinaryExpr(Match<T_PUNC_BITXOR>().Return(ASTBinaryExpr.Op.XOR));
        }

        /// <summary>
        /// AND-expression
        ///     : equality-expression
        ///     | AND-expression & equality-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> ANDExpression() {
            return EqualityExpression().ChainBinaryExpr(Match<T_PUNC_REF>().Return(ASTBinaryExpr.Op.AND));
        }

        /// <summary>
        /// equality-expression
        ///     : relational-expression
        ///     | equality-expression == relational-expression
        ///     | equality-expression != relational-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> EqualityExpression() {
            return RelationalExpression().ChainBinaryExpr(EqualityOperator());
        }

        /// <summary>
        /// equality-operator
        ///     : one of == !=
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTBinaryExpr.Op> EqualityOperator() {
            return Match<T_PUNC_EQ>().Return(ASTBinaryExpr.Op.EQ)
                .Else(Match<T_PUNC_NEQ>().Return(ASTBinaryExpr.Op.NEQ));
        }


        /// <summary>
        /// relational-expression
        ///     : shift-expression
        ///     | relational-expression relational-operator shift-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> RelationalExpression() {
            return ShiftExpression().ChainBinaryExpr(RelationalOperator());
        }

        /// <summary>
        /// relational-operator
        ///     : one of &lt &gt &lt= &gt=
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTBinaryExpr.Op> RelationalOperator() {
            return Match<T_PUNC_LT>().Return(ASTBinaryExpr.Op.LT)
                .Else(Match<T_PUNC_GT>().Return(ASTBinaryExpr.Op.GT))
                .Else(Match<T_PUNC_LE>().Return(ASTBinaryExpr.Op.LE))
                .Else(Match<T_PUNC_GE>().Return(ASTBinaryExpr.Op.GE));
        }

        /// <summary>
        /// shift-expression
        ///     : additive-expression
        ///     | shift-expression &lt&lt additive-expression
        ///     | shift-expression &gt&gt additive-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> ShiftExpression() {
            return AdditiveExpressiion().ChainBinaryExpr(ShiftOperator());
        }

        /// <summary>
        /// shift-operator
        ///     : one of &lt&lt &gt&gt
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTBinaryExpr.Op> ShiftOperator() {
            return Match<T_PUNC_SHIFTL>().Return(ASTBinaryExpr.Op.LEFT)
                .Else(Match<T_PUNC_SHIFTR>().Return(ASTBinaryExpr.Op.RIGHT));
        }

        /// <summary>
        /// additive-expression
        ///     : multiplicative-expression
        ///     | additive-expression + multiplicative-expression
        ///     | additive-expression - multiplicative-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> AdditiveExpressiion() {
            return MultiplicativeExpression().ChainBinaryExpr(AdditiveOperator());
        }

        /// <summary>
        /// additive-operator
        ///     : one of + -
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTBinaryExpr.Op> AdditiveOperator() {
            return Match<T_PUNC_PLUS>().Return(ASTBinaryExpr.Op.PLUS)
                .Else(Match<T_PUNC_MINUS>().Return(ASTBinaryExpr.Op.MINUS));
        }

        /// <summary>
        /// multiplicative-expression
        ///     : cast-expression
        ///     | multiplicative-expression * cast-expression
        ///     | multiplicative-expression / cast-expression
        ///     | multiplicative-expression % cast-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> MultiplicativeExpression() {
            return CastExpression().ChainBinaryExpr(MultiplicativeOperator());
        }

        /// <summary>
        /// mult-operator
        ///     : one of * / %
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTBinaryExpr.Op> MultiplicativeOperator() {
            return Match<T_PUNC_STAR>().Return(ASTBinaryExpr.Op.MULT)
                .Else(Match<T_PUNC_SLASH>().Return(ASTBinaryExpr.Op.DIV))
                .Else(Match<T_PUNC_MOD>().Return(ASTBinaryExpr.Op.MOD))
                ;
        }

        /// <summary>
        /// cast-expression
        ///     : unary-expression
        ///     | ( type-name ) cast-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> CastExpression() {
            return UnaryExpression()
                .Else(Ref(TypeName).ParentLR().Bind(name => Ref(CastExpression)
                .Select(expr => new ASTCast(name, expr))));
        }

        /// <summary>
        /// unary-expression
        ///     : postfix-expression
        ///     | ++ unary-expression
        ///     | -- unary-expression
        ///     | unary-operator cast-expression
        ///     | sizeof unary-expression
        ///     | sizeof ( type-name )
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> UnaryExpression() {
            return PostfixExpression()
                .Else(Match<T_PUNC_INCRE>()
                    .Then(Ref(UnaryExpression))
                    .Select(x => new ASTPreStep(x, ASTPreStep.Kind.INC)))
                .Else(Match<T_PUNC_DECRE>()
                    .Then(Ref(UnaryExpression))
                    .Select(x => new ASTPreStep(x, ASTPreStep.Kind.DEC)))
                .Else(UnaryOperator()
                    .Bind(op => Ref(CastExpression)
                    .Select(expr => new ASTUnaryOp(expr, op))))
                .Else(Match<T_KEY_SIZEOF>().Then(Ref(UnaryExpression)).Select(expr => new ASTSizeOf(expr)))
                .Else(Match<T_KEY_SIZEOF>().Then(Ref(TypeName).ParentLR()).Select(name => new ASTSizeOf(name)));
        }

        /// <summary>
        /// unary-operator
        ///     : one of & * + - ~ !
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTUnaryOp.Op> UnaryOperator() {
            return Match<T_PUNC_REF>().Return(ASTUnaryOp.Op.REF)
                .Else(Match<T_PUNC_STAR>().Return(ASTUnaryOp.Op.STAR))
                .Else(Match<T_PUNC_PLUS>().Return(ASTUnaryOp.Op.PLUS))
                .Else(Match<T_PUNC_MINUS>().Return(ASTUnaryOp.Op.MINUS))
                .Else(Match<T_PUNC_BITNOT>().Return(ASTUnaryOp.Op.REVERSE))
                .Else(Match<T_PUNC_LOGNOT>().Return(ASTUnaryOp.Op.NOT))
                ;
        }

        /// <summary>
        /// postfix-expression
        ///     : primary-expression postfix-expression-tail
        ///     | ( type-name ) { initializer-list } postfix-expression-tail
        ///     | ( type-name ) { initializer-list , } postfix-expression-tail
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> PostfixExpression() {
            return PrimaryExpression().Bind(x => PostfixExpressionTail(x))
                .Else(Ref(TypeName).ParentLR()
                    .Bind(name => Ref(InitItem)
                        .PlusSeperatedBy(Match<T_PUNC_COMMA>()).Option(Match<T_PUNC_COMMA>()).BracelLR()
                    .Bind(inits => PostfixExpressionTail(new ASTCompound(name, inits)))));
        }

        /// <summary>
        /// postfix-expression-tail
        ///     : [ expression ] postfix-expression-tail
        ///     | ( argunemt-expression-list_opt ) postfix-expression-tail
        ///     | . identifier postfix-expression-tail
        ///     | -> identifier postfix-expression-tail
        ///     | ++ postfix-expression-tail
        ///     | -- postfix-expression-tail
        ///     | epsilon
        ///     ;
        ///     
        /// argument-expression-list
        ///     : assignment-expression
        ///     | argument-expression , assignment-expression
        ///     ;
        /// NOTE:
        /// The trickest part is to implement ++/-- postfix-expression-tail.
        /// Although we don't need the result from previous Match parser,
        /// we have to use Bind here instead of Then,
        /// because we have to use lambda expression to avoid circular reference of itself.
        /// Ref won't work here because PostfixExpressionTail takes one argument.
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> PostfixExpressionTail(ASTExpr expr) {
            return Ref(Expression)
                    .Bracket(Match<T_PUNC_SUBSCRIPTL>(), Match<T_PUNC_SUBSCRIPTR>())
                    .Bind(idx => PostfixExpressionTail(new ASTArrSub(expr, idx)))
                .Else(Ref(AssignmentExpression).ManySeperatedBy(Match<T_PUNC_COMMA>()).ParentLR()
                    .Bind(args => PostfixExpressionTail(new ASTFuncCall(expr, args))))
                .Else(Match<T_PUNC_DOT>()
                    .Then(Get<T_IDENTIFIER>()
                    .Bind(id => PostfixExpressionTail(new ASTAccess(expr, id, ASTAccess.Kind.DOT)))))
                .Else(Match<T_PUNC_PTRSEL>()
                    .Then(Get<T_IDENTIFIER>()
                    .Bind(id => PostfixExpressionTail(new ASTAccess(expr, id, ASTAccess.Kind.PTR)))))
                .Else(Match<T_PUNC_INCRE>()
                    .Bind(_ => PostfixExpressionTail(new ASTPostStep(expr, ASTPostStep.Kind.INC))))
                .Else(Match<T_PUNC_DECRE>()
                    .Bind(_ => PostfixExpressionTail(new ASTPostStep(expr, ASTPostStep.Kind.DEC))))
                .Else(Result<Token.Token, ASTExpr>(expr));
        }


        /// <summary>
        /// primary-expression
        ///     : identifier
        ///     | constant
        ///     | string-literal.Plus // Allow strings to be concated.
        ///     | ( expression )
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTExpr> PrimaryExpression() {
            return Get<T_IDENTIFIER>().Select(x => new ASTId(x) as ASTExpr)
                .Else(Get<T_CONST_CHAR>().Select(x => new ASTConstChar(x)))
                .Else(Get<T_CONST_INT>().Select(x => new ASTConstInt(x)))
                .Else(Get<T_CONST_FLOAT>().Select(x => new ASTConstFloat(x)))
                .Else(Get<T_STRING_LITERAL>().Plus().Select(x => new ASTString(x)))
                .Else(Ref(Expression).ParentLR());
        }
    }
}
