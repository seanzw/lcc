using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Parserc.Parserc;
using lcc.Token;
using lcc.SyntaxTree;

namespace lcc.Parser {
    public static partial class Parser {

        /// <summary>
        /// constant-expression
        ///     : conditional-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> ConstantExpression() {
            return ConditionalExpression();
        }

        /// <summary>
        /// expression
        ///     : assignment-expression
        ///     | expression , assignment-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> Expression() {
            return AssignmentExpression().PlusSeperatedBy(Match<T_PUNC_COMMA>())
                .Select(exprs => exprs.Count == 1 ? exprs.First() : new STCommaExpr(exprs));
        }

        /// <summary>
        /// assignment-expression
        ///     : conditional-expression
        ///     | unary-expression assignment-operator assignment-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> AssignmentExpression() {
            return ConditionalExpression()
                .Or(UnaryExpression()
                    .Bind(lexpr => AssgnmentOperator()
                    .Bind(op => AssignmentExpression()
                    .Select(rexpr => new STAssignExpr(lexpr, rexpr, op)))));
        }

        /// <summary>
        /// assignment-operator
        ///     : one of = *= /= %= += -= &lt&lt= &gt&gt= &= ^= |=
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, STAssignExpr.Op> AssgnmentOperator() {
            return Match<T_PUNC_ASSIGN>().Return(STAssignExpr.Op.ASSIGN)
                .Else(Match<T_PUNC_ASSIGN>().Return(STAssignExpr.Op.ASSIGN))
                .Else(Match<T_PUNC_MULEQ>().Return(STAssignExpr.Op.MULEQ))
                .Else(Match<T_PUNC_DIVEQ>().Return(STAssignExpr.Op.DIVEQ))
                .Else(Match<T_PUNC_MODEQ>().Return(STAssignExpr.Op.MODEQ))
                .Else(Match<T_PUNC_PLUSEQ>().Return(STAssignExpr.Op.PLUSEQ))
                .Else(Match<T_PUNC_MINUSEQ>().Return(STAssignExpr.Op.MINUSEQ))
                .Else(Match<T_PUNC_SHIFTLEQ>().Return(STAssignExpr.Op.SHIFTLEQ))
                .Else(Match<T_PUNC_SHIFTREQ>().Return(STAssignExpr.Op.SHIFTREQ))
                .Else(Match<T_PUNC_BITANDEQ>().Return(STAssignExpr.Op.BITANDEQ))
                .Else(Match<T_PUNC_BITXOREQ>().Return(STAssignExpr.Op.BITXOREQ))
                .Else(Match<T_PUNC_BITOREQ>().Return(STAssignExpr.Op.BITOREQ));
        }

        /// <summary>
        /// conditional-expression
        ///     : logical-OR-expression
        ///     | logical-OR-expression ? expression : conditional-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> ConditionalExpression() {
            return LogicalORExpression()
                .Bind(predicator => Match<T_PUNC_QUESTION>()
                    .Then(Ref(Expression)
                    .Bind(trueExpr => Match<T_PUNC_COLON>()
                    .Then(Ref(ConditionalExpression)
                    .Select(falseExpr => new STCondExpr(predicator, trueExpr, falseExpr)))))
                    .Else(Result<Token.Token, Expr>(predicator))
                );
        }

        /// <summary>
        /// logical-OR-expression
        ///     : logical-AND-expression
        ///     | logical-OR-expression || logical-AND-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> LogicalORExpression() {
            return LogicalANDExpression().ChainBinaryExpr(Match<T_PUNC_LOGOR>().Return(STBiExpr.Op.LOGOR));
        }

        /// <summary>
        /// logical-AND-expression
        ///     : OR-expression
        ///     | logical-AND-expression && OR-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> LogicalANDExpression() {
            return ORExpression().ChainBinaryExpr(Match<T_PUNC_LOGAND>().Return(STBiExpr.Op.LOGAND));
        }

        /// <summary>
        /// OR-expression
        ///     : XOR-expression
        ///     | OR-expression | XOR-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> ORExpression() {
            return XORExpression().ChainBinaryExpr(Match<T_PUNC_BITOR>().Return(STBiExpr.Op.OR));
        }

        /// <summary>
        /// XOR-expression
        ///     : AND-expression
        ///     | XOR-expression ^ AND-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> XORExpression() {
            return ANDExpression().ChainBinaryExpr(Match<T_PUNC_BITXOR>().Return(STBiExpr.Op.XOR));
        }

        /// <summary>
        /// AND-expression
        ///     : equality-expression
        ///     | AND-expression & equality-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> ANDExpression() {
            return EqualityExpression().ChainBinaryExpr(Match<T_PUNC_REF>().Return(STBiExpr.Op.AND));
        }

        /// <summary>
        /// equality-expression
        ///     : relational-expression
        ///     | equality-expression == relational-expression
        ///     | equality-expression != relational-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> EqualityExpression() {
            return RelationalExpression().ChainBinaryExpr(EqualityOperator());
        }

        /// <summary>
        /// equality-operator
        ///     : one of == !=
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, STBiExpr.Op> EqualityOperator() {
            return Match<T_PUNC_EQ>().Return(STBiExpr.Op.EQ)
                .Else(Match<T_PUNC_NEQ>().Return(STBiExpr.Op.NEQ));
        }


        /// <summary>
        /// relational-expression
        ///     : shift-expression
        ///     | relational-expression relational-operator shift-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> RelationalExpression() {
            return ShiftExpression().ChainBinaryExpr(RelationalOperator());
        }

        /// <summary>
        /// relational-operator
        ///     : one of &lt &gt &lt= &gt=
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, STBiExpr.Op> RelationalOperator() {
            return Match<T_PUNC_LT>().Return(STBiExpr.Op.LT)
                .Else(Match<T_PUNC_GT>().Return(STBiExpr.Op.GT))
                .Else(Match<T_PUNC_LE>().Return(STBiExpr.Op.LE))
                .Else(Match<T_PUNC_GE>().Return(STBiExpr.Op.GE));
        }

        /// <summary>
        /// shift-expression
        ///     : additive-expression
        ///     | shift-expression &lt&lt additive-expression
        ///     | shift-expression &gt&gt additive-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> ShiftExpression() {
            return AdditiveExpressiion().ChainBinaryExpr(ShiftOperator());
        }

        /// <summary>
        /// shift-operator
        ///     : one of &lt&lt &gt&gt
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, STBiExpr.Op> ShiftOperator() {
            return Match<T_PUNC_SHIFTL>().Return(STBiExpr.Op.LEFT)
                .Else(Match<T_PUNC_SHIFTR>().Return(STBiExpr.Op.RIGHT));
        }

        /// <summary>
        /// additive-expression
        ///     : multiplicative-expression
        ///     | additive-expression + multiplicative-expression
        ///     | additive-expression - multiplicative-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> AdditiveExpressiion() {
            return MultiplicativeExpression().ChainBinaryExpr(AdditiveOperator());
        }

        /// <summary>
        /// additive-operator
        ///     : one of + -
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, STBiExpr.Op> AdditiveOperator() {
            return Match<T_PUNC_PLUS>().Return(STBiExpr.Op.PLUS)
                .Else(Match<T_PUNC_MINUS>().Return(STBiExpr.Op.MINUS));
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
        public static Parserc.Parser<Token.Token, Expr> MultiplicativeExpression() {
            return CastExpression().ChainBinaryExpr(MultiplicativeOperator());
        }

        /// <summary>
        /// mult-operator
        ///     : one of * / %
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, STBiExpr.Op> MultiplicativeOperator() {
            return Match<T_PUNC_STAR>().Return(STBiExpr.Op.MULT)
                .Else(Match<T_PUNC_SLASH>().Return(STBiExpr.Op.DIV))
                .Else(Match<T_PUNC_MOD>().Return(STBiExpr.Op.MOD))
                ;
        }

        /// <summary>
        /// cast-expression
        ///     : unary-expression
        ///     | ( type-name ) cast-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> CastExpression() {
            return UnaryExpression()
                .Else(Ref(TypeName).ParentLR().Bind(name => Ref(CastExpression)
                .Select(expr => new STCast(name, expr))));
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
        public static Parserc.Parser<Token.Token, Expr> UnaryExpression() {
            return PostfixExpression()
                .Else(Match<T_PUNC_INCRE>()
                    .Then(Ref(UnaryExpression))
                    .Select(x => new STPreStep(x, STPreStep.Kind.INC)))
                .Else(Match<T_PUNC_DECRE>()
                    .Then(Ref(UnaryExpression))
                    .Select(x => new STPreStep(x, STPreStep.Kind.DEC)))
                .Else(UnaryOperator()
                    .Bind(op => Ref(CastExpression)
                    .Select(expr => new STUnaryOp(expr, op))))
                .Else(Match<T_KEY_SIZEOF>().Then(Ref(UnaryExpression)).Select(expr => new STSizeOf(expr)))
                .Else(Match<T_KEY_SIZEOF>().Then(Ref(TypeName).ParentLR()).Select(name => new STSizeOf(name)));
        }

        /// <summary>
        /// unary-operator
        ///     : one of & * + - ~ !
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, STUnaryOp.Op> UnaryOperator() {
            return Match<T_PUNC_REF>().Return(STUnaryOp.Op.REF)
                .Else(Match<T_PUNC_STAR>().Return(STUnaryOp.Op.STAR))
                .Else(Match<T_PUNC_PLUS>().Return(STUnaryOp.Op.PLUS))
                .Else(Match<T_PUNC_MINUS>().Return(STUnaryOp.Op.MINUS))
                .Else(Match<T_PUNC_BITNOT>().Return(STUnaryOp.Op.REVERSE))
                .Else(Match<T_PUNC_LOGNOT>().Return(STUnaryOp.Op.NOT))
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
        public static Parserc.Parser<Token.Token, Expr> PostfixExpression() {
            return PrimaryExpression().Bind(x => PostfixExpressionTail(x))
                .Else(Ref(TypeName).ParentLR()
                    .Bind(name => Ref(InitItem)
                        .PlusSeperatedBy(Match<T_PUNC_COMMA>()).Option(Match<T_PUNC_COMMA>()).BracelLR()
                    .Bind(inits => PostfixExpressionTail(new STCompound(name, inits)))));
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
        public static Parserc.Parser<Token.Token, Expr> PostfixExpressionTail(Expr expr) {
            return Ref(Expression)
                    .Bracket(Match<T_PUNC_SUBSCRIPTL>(), Match<T_PUNC_SUBSCRIPTR>())
                    .Bind(idx => PostfixExpressionTail(new STArrSub(expr, idx)))
                .Else(Ref(AssignmentExpression).ManySeperatedBy(Match<T_PUNC_COMMA>()).ParentLR()
                    .Bind(args => PostfixExpressionTail(new STFuncCall(expr, args))))
                .Else(Match<T_PUNC_DOT>()
                    .Then(Get<T_IDENTIFIER>()
                    .Bind(id => PostfixExpressionTail(new STAccess(expr, id, STAccess.Kind.DOT)))))
                .Else(Match<T_PUNC_PTRSEL>()
                    .Then(Get<T_IDENTIFIER>()
                    .Bind(id => PostfixExpressionTail(new STAccess(expr, id, STAccess.Kind.PTR)))))
                .Else(Match<T_PUNC_INCRE>()
                    .Bind(_ => PostfixExpressionTail(new STPostStep(expr, STPostStep.Kind.INC))))
                .Else(Match<T_PUNC_DECRE>()
                    .Bind(_ => PostfixExpressionTail(new STPostStep(expr, STPostStep.Kind.DEC))))
                .Else(Result<Token.Token, Expr>(expr));
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
        public static Parserc.Parser<Token.Token, Expr> PrimaryExpression() {
            return Get<T_IDENTIFIER>().Select(x => new Id(x) as Expr)
                .Else(Get<T_CONST_CHAR>().Select(x => new ConstChar(x)))
                .Else(Get<T_CONST_INT>().Select(x => new ConstInt(x)))
                .Else(Get<T_CONST_FLOAT>().Select(x => new ConstFloat(x)))
                .Else(Get<T_STRING_LITERAL>().Plus().Select(x => new Str(x)))
                .Else(Ref(Expression).ParentLR());
        }
    }
}
