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
                .Select(exprs => exprs.Count == 1 ? exprs.First() : new CommaExpr(exprs));
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
                    .Select(rexpr => new Assign(lexpr, rexpr, op)))));
        }

        /// <summary>
        /// assignment-operator
        ///     : one of = *= /= %= += -= &lt&lt= &gt&gt= &= ^= |=
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Assign.Op> AssgnmentOperator() {
            return Match<T_PUNC_ASSIGN>().Return(Assign.Op.ASSIGN)
                .Else(Match<T_PUNC_ASSIGN>().Return(Assign.Op.ASSIGN))
                .Else(Match<T_PUNC_MULEQ>().Return(Assign.Op.MULEQ))
                .Else(Match<T_PUNC_DIVEQ>().Return(Assign.Op.DIVEQ))
                .Else(Match<T_PUNC_MODEQ>().Return(Assign.Op.MODEQ))
                .Else(Match<T_PUNC_PLUSEQ>().Return(Assign.Op.PLUSEQ))
                .Else(Match<T_PUNC_MINUSEQ>().Return(Assign.Op.MINUSEQ))
                .Else(Match<T_PUNC_SHIFTLEQ>().Return(Assign.Op.LEFTEQ))
                .Else(Match<T_PUNC_SHIFTREQ>().Return(Assign.Op.RIGHTEQ))
                .Else(Match<T_PUNC_BITANDEQ>().Return(Assign.Op.ANDEQ))
                .Else(Match<T_PUNC_BITXOREQ>().Return(Assign.Op.XOREQ))
                .Else(Match<T_PUNC_BITOREQ>().Return(Assign.Op.OREQ));
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
                    .Select(falseExpr => new CondExpr(predicator, trueExpr, falseExpr)))))
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
            return LogicalANDExpression().ChainBinaryExpr(Match<T_PUNC_LOGOR>().Return(BiExpr.Op.LOGOR));
        }

        /// <summary>
        /// logical-AND-expression
        ///     : OR-expression
        ///     | logical-AND-expression && OR-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> LogicalANDExpression() {
            return ORExpression().ChainBinaryExpr(Match<T_PUNC_LOGAND>().Return(BiExpr.Op.LOGAND));
        }

        /// <summary>
        /// OR-expression
        ///     : XOR-expression
        ///     | OR-expression | XOR-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> ORExpression() {
            return XORExpression().ChainBinaryExpr(Match<T_PUNC_BITOR>().Return(BiExpr.Op.OR));
        }

        /// <summary>
        /// XOR-expression
        ///     : AND-expression
        ///     | XOR-expression ^ AND-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> XORExpression() {
            return ANDExpression().ChainBinaryExpr(Match<T_PUNC_BITXOR>().Return(BiExpr.Op.XOR));
        }

        /// <summary>
        /// AND-expression
        ///     : equality-expression
        ///     | AND-expression & equality-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> ANDExpression() {
            return EqualityExpression().ChainBinaryExpr(Match<T_PUNC_REF>().Return(BiExpr.Op.AND));
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
        public static Parserc.Parser<Token.Token, BiExpr.Op> EqualityOperator() {
            return Match<T_PUNC_EQ>().Return(BiExpr.Op.EQ)
                .Else(Match<T_PUNC_NEQ>().Return(BiExpr.Op.NEQ));
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
        public static Parserc.Parser<Token.Token, BiExpr.Op> RelationalOperator() {
            return Match<T_PUNC_LT>().Return(BiExpr.Op.LT)
                .Else(Match<T_PUNC_GT>().Return(BiExpr.Op.GT))
                .Else(Match<T_PUNC_LE>().Return(BiExpr.Op.LE))
                .Else(Match<T_PUNC_GE>().Return(BiExpr.Op.GE));
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
        public static Parserc.Parser<Token.Token, BiExpr.Op> ShiftOperator() {
            return Match<T_PUNC_SHIFTL>().Return(BiExpr.Op.LEFT)
                .Else(Match<T_PUNC_SHIFTR>().Return(BiExpr.Op.RIGHT));
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
        public static Parserc.Parser<Token.Token, BiExpr.Op> AdditiveOperator() {
            return Match<T_PUNC_PLUS>().Return(BiExpr.Op.PLUS)
                .Else(Match<T_PUNC_MINUS>().Return(BiExpr.Op.MINUS));
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
        public static Parserc.Parser<Token.Token, BiExpr.Op> MultiplicativeOperator() {
            return Match<T_PUNC_STAR>().Return(BiExpr.Op.MULT)
                .Else(Match<T_PUNC_SLASH>().Return(BiExpr.Op.DIV))
                .Else(Match<T_PUNC_MOD>().Return(BiExpr.Op.MOD))
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
                .Select(expr => new Cast(name, expr))));
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
                    .Select(x => new PreStep(x, PreStep.Kind.INC)))
                .Else(Match<T_PUNC_DECRE>()
                    .Then(Ref(UnaryExpression))
                    .Select(x => new PreStep(x, PreStep.Kind.DEC)))
                .Else(UnaryOperator()
                    .Bind(op => Ref(CastExpression)
                    .Select(expr => new UnaryOp(expr, op))))
                .Else(Match<T_KEY_SIZEOF>().Then(Ref(UnaryExpression)).Select(expr => new SizeOf(expr)))
                .Else(Match<T_KEY_SIZEOF>().Then(Ref(TypeName).ParentLR()).Select(name => new SizeOf(name)));
        }

        /// <summary>
        /// unary-operator
        ///     : one of & * + - ~ !
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, UnaryOp.Op> UnaryOperator() {
            return Match<T_PUNC_REF>().Return(UnaryOp.Op.REF)
                .Else(Match<T_PUNC_STAR>().Return(UnaryOp.Op.STAR))
                .Else(Match<T_PUNC_PLUS>().Return(UnaryOp.Op.PLUS))
                .Else(Match<T_PUNC_MINUS>().Return(UnaryOp.Op.MINUS))
                .Else(Match<T_PUNC_BITNOT>().Return(UnaryOp.Op.REVERSE))
                .Else(Match<T_PUNC_LOGNOT>().Return(UnaryOp.Op.NOT))
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
                    .Bind(inits => PostfixExpressionTail(new Compound(name, inits)))));
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
                    .Bind(idx => PostfixExpressionTail(new ArrSub(expr, idx)))
                .Else(Ref(AssignmentExpression).ManySeperatedBy(Match<T_PUNC_COMMA>()).ParentLR()
                    .Bind(args => PostfixExpressionTail(new FuncCall(expr, args))))
                .Else(Match<T_PUNC_DOT>()
                    .Then(Get<T_IDENTIFIER>()
                    .Bind(id => PostfixExpressionTail(new Access(expr, id, Access.Kind.DOT)))))
                .Else(Match<T_PUNC_PTRSEL>()
                    .Then(Get<T_IDENTIFIER>()
                    .Bind(id => PostfixExpressionTail(new Access(expr, id, Access.Kind.PTR)))))
                .Else(Match<T_PUNC_INCRE>()
                    .Bind(_ => PostfixExpressionTail(new PostStep(expr, PostStep.Op.INC))))
                .Else(Match<T_PUNC_DECRE>()
                    .Bind(_ => PostfixExpressionTail(new PostStep(expr, PostStep.Op.DEC))))
                .Else(Result<Token.Token, Expr>(expr));
        }


        /// <summary>
        /// primary-expression
        ///     : identifier
        ///     | constant
        ///     | string-literal.Plus // Allow strings to be concated.
        ///     | ( expression )
        ///     ;
        ///     
        /// Notice that an identifier is a primary expression only when it has
        /// been declared as designating an object for a function.
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Expr> PrimaryExpression() {
            return Identifier().Select(x => x as Expr)
                .Else(Get<T_CONST_CHAR>().Select(x => new ConstChar(x)))
                .Else(Get<T_CONST_INT>().Select(x => new ConstInt(x)))
                .Else(Get<T_CONST_FLOAT>().Select(x => new ConstFloat(x)))
                .Else(Get<T_STRING_LITERAL>().Plus().Select(x => new Str(x)))
                .Else(Ref(Expression).ParentLR());
        }

        /// <summary>
        /// identifier
        ///     : T_IDENTIFIER
        ///     ;
        ///     
        /// Fail if the identifier is a typedef name.
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, Id> Identifier() {
            return Get<T_IDENTIFIER>()
                .Bind(id => Env.IsTypedefName(id.name) ? 
                Zero<Token.Token, Id>() : Result<Token.Token, Id>(new Id(id)));
        }
    }
}
