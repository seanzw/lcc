using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;
using lcc.Token;
using lcc.AST;

namespace lcc.SyntaxTree {

    /// <summary>
    /// The base class for all expression.
    /// </summary>
    public abstract class Expr : Stmt {

        public override IEnumerable<AST.Stmt> ToAST(Env env) {
            return new List<AST.Stmt> { GetASTExpr(env) };
        }

        public virtual AST.Expr GetASTExpr(Env env) {
            throw new NotImplementedException();
        }
    }

    public sealed class STCommaExpr : Expr, IEquatable<STCommaExpr> {

        public STCommaExpr(IEnumerable<Expr> exprs) {
            this.exprs = exprs;
        }

        public override Position Pos => exprs.First().Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STCommaExpr);
        }

        public bool Equals(STCommaExpr x) {
            return x != null && exprs.SequenceEqual(x.exprs);
        }

        public override int GetHashCode() {
            return exprs.Aggregate(0, (acc, expr) => acc ^ expr.GetHashCode());
        }

        public readonly IEnumerable<Expr> exprs;
    }

    public sealed class STAssignExpr : Expr, IEquatable<STAssignExpr> {

        public enum Op {
            ASSIGN,
            MULEQ,
            DIVEQ,
            MODEQ,
            PLUSEQ,
            MINUSEQ,
            SHIFTLEQ,
            SHIFTREQ,
            BITANDEQ,
            BITXOREQ,
            BITOREQ
        }

        public STAssignExpr(Expr lexpr, Expr rexpr, Op op) {
            this.lexpr = lexpr;
            this.rexpr = rexpr;
            this.op = op;
        }

        public override Position Pos => lexpr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STAssignExpr);
        }

        public bool Equals(STAssignExpr expr) {
            return expr != null
                && expr.lexpr.Equals(lexpr)
                && expr.rexpr.Equals(rexpr)
                && expr.op == op;
        }

        public override int GetHashCode() {
            return lexpr.GetHashCode() ^ rexpr.GetHashCode() ^ op.GetHashCode();
        }

        public readonly Expr lexpr;
        public readonly Expr rexpr;
        public readonly Op op;
    }

    public sealed class STCondExpr : Expr, IEquatable<STCondExpr> {

        public STCondExpr(Expr predicator, Expr trueExpr, Expr falseExpr) {
            this.predicator = predicator;
            this.trueExpr = trueExpr;
            this.falseExpr = falseExpr;
        }

        public override Position Pos => predicator.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STCondExpr);
        }

        public bool Equals(STCondExpr x) {
            return x != null && x.predicator.Equals(predicator)
                && x.trueExpr.Equals(trueExpr)
                && x.falseExpr.Equals(falseExpr);
        }

        public override int GetHashCode() {
            return predicator.GetHashCode() ^ trueExpr.GetHashCode() ^ falseExpr.GetHashCode();
        }

        public readonly Expr predicator;
        public readonly Expr trueExpr;
        public readonly Expr falseExpr;
    }

    public sealed class BiExpr : Expr, IEquatable<BiExpr> {

        public enum Op {
            MULT,   // *
            DIV,    // /
            MOD,    // %
            PLUS,   // +
            MINUS,  // -
            LEFT,   // <<
            RIGHT,  // >>
            LT,     // <
            GT,     // >
            LE,     // <=
            GE,     // >=
            EQ,     // ==
            NEQ,    // !=
            AND,    // &
            XOR,    // ^
            OR,     // |
            LOGAND, // &&
            LOGOR   // ||
        }

        public BiExpr(Expr lhs, Expr rhs, Op op) {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }

        public override Position Pos => lhs.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as BiExpr);
        }

        public bool Equals(BiExpr x) {
            return x != null
                && x.lhs.Equals(lhs)
                && x.rhs.Equals(rhs)
                && x.op.Equals(op);
        }

        public override int GetHashCode() {
            return lhs.GetHashCode() ^ rhs.GetHashCode() ^ op.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr lExpr = lhs.GetASTExpr(env);
            AST.Expr rExpr = rhs.GetASTExpr(env);
            switch (op) {
                case Op.MULT:
                case Op.DIV:
                case Op.MOD:
                    /// Multiplicative operators.
                    /// Each of the operands shall have arithmetic type.
                    if (!lExpr.Type.IsArithmetic || !rExpr.Type.IsArithmetic) {
                        throw new Error(Pos, string.Format("operands of operator {0} shall have arithmetic type", op));
                    }
                    /// The operands of the % operator shall have integer type.
                    if (op == Op.MOD) {
                        if (!lExpr.Type.IsInteger || !rExpr.Type.IsInteger) {
                            throw new Error(Pos, string.Format("operands of operator {0} shall have integer type", op));
                        }
                    }
                    /// The usual arithmetic conversions are performed on the operands.
                    T type = lExpr.Type.UsualArithConversion(rExpr.Type);
                    return new AST.BiExpr(type, env.GetASTEnv(), lExpr, rExpr, op);

                default:
                    throw new NotImplementedException();
            }
        }

        public readonly Expr lhs;
        public readonly Expr rhs;
        public readonly Op op;
    }


    /// <summary>
    /// ( type-name ) cast-expression
    /// </summary>
    public sealed class Cast : Expr, IEquatable<Cast> {

        public Cast(TypeName name, Expr expr) {
            this.name = name;
            this.expr = expr;
        }

        public override Position Pos => name.Pos;
        public bool Equals(Cast x) {
            return x != null && x.name.Equals(name) && x.expr.Equals(expr);
        }

        public override bool Equals(object obj) {
            return Equals(obj as Cast);
        }

        public override int GetHashCode() {
            return name.GetHashCode() ^ expr.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr e = expr.GetASTExpr(env);
            if (e == null) {
                return null;
            }

            T type = name.GetT(env);

            /// Unless the type name specifies a void type, the type name shall specify qualified or unqualified
            /// scalar type and the operand shall have scalar type.
            if (!type.IsVoid) {
                if (!type.IsScalar) {
                    throw new Error(Pos, "the type name shall specify scalar type");
                }
                if (!e.Type.IsScalar) {
                    throw new Error(Pos, "the operand shall have scalar type");
                }
            }

            /// The following cast is illegal.
            /// 1. Conversions between pointers and floating types.
            /// 2. Conversions between pointers to functions and pointers to objects (including void*)
            /// 3. Conversions between pointers to functions and integers.
            /// http://en.cppreference.com/w/c/language/cast
            CheckIllegalCast(type.nake, e.Type.nake);
            CheckIllegalCast(e.Type.nake, type.nake);

            /// If the value of the expression is represented with greater precision or range than
            /// required by the type named by the cast, then the cast specifies a conversion even
            /// if the type of the expression is the same as the named type.
            /// NOTE: This is used for constant expression that has float type. In this implementation
            /// all constant float expression are represented in double.
            /// TODO: Support constant expression cast.

            /// A cast that specifies no conversion has no effect on the type or value of an expression.
            /// However, the result of a cast is always rvalue, therefore we still need to new VarExpr.
            /// Notice that since the result is rvalue, the qualifiers will be discarded.
            return new AST.Cast(type.nake, env.GetASTEnv(), e);
        }

        private void CheckIllegalCast(TUnqualified s, TUnqualified t) {
            TPointer p = s as TPointer;
            if (p != null && t.IsFloat) {
                throw new Error(Pos, "cannot cast between pointers and floating types");
            }
            if (p != null && p.element.IsFunc && t.IsInteger) {
                throw new Error(Pos, "cannot cast between pointers to functions and integers");
            }
            TPointer q = t as TPointer;
            if (p != null && p.element.IsFunc && q != null && q.element.IsObject) {
                throw new Error(Pos, "cannot cast between pointers to functions and pointers to objects");
            }
        }

        public readonly TypeName name;
        public readonly Expr expr;
    }

    /// <summary>
    /// ++i
    /// --i
    /// </summary>
    public sealed class PreStep : Expr, IEquatable<PreStep> {

        public enum Kind {
            INC,
            DEC
        }

        public PreStep(Expr expr, Kind kind) {
            this.expr = expr;
            this.kind = kind;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as PreStep);
        }

        public bool Equals(PreStep x) {
            return x != null && x.expr.Equals(expr) && x.kind == kind;
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr e = expr.GetASTExpr(env);
            if (!e.Type.IsReal && !e.Type.IsPointer) {
                throw new Error(expr.Pos, string.Format("{0} on type {1}", kind, e.Type));
            }
            if (!e.IsLValue) {
                throw new Error(Pos, "cannot modify rvalue");
            }
            if (e.Type.qualifiers.isConstant) {
                throw new Error(Pos, "cannot modify constant value");
            }
            return new AST.PreStep(e.Type, env.GetASTEnv(), e, kind);
        }

        public readonly Expr expr;
        public readonly Kind kind;
    }

    /// <summary>
    /// & * + - ~ !
    /// </summary>
    public sealed class UnaryOp : Expr, IEquatable<UnaryOp> {

        public enum Op {
            /// <summary>
            /// &
            /// </summary>
            REF,
            /// <summary>
            /// *
            /// </summary>
            STAR,
            /// <summary>
            /// +
            /// </summary>
            PLUS,
            /// <summary>
            /// -
            /// </summary>
            MINUS,
            /// <summary>
            /// ~
            /// </summary>
            REVERSE,
            /// <summary>
            /// ！
            /// </summary>
            NOT
        }

        public UnaryOp(Expr expr, Op op) {
            this.expr = expr;
            this.op = op;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as UnaryOp);
        }

        public bool Equals(UnaryOp x) {
            return x != null && x.expr.Equals(expr) && x.op == op;
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr e = expr.GetASTExpr(env);
            switch (op) {
                case Op.REF:
                    /// The operand of the unary & operator shall be either a function designator, 
                    /// the result of a [] or unary * operator, or an lvalue that designates an object
                    /// that is not as bit-FieldAccessException and is not declared with the register
                    /// storage-class specifier.
                    if (e is AST.ArrSub) {
                        // The & operator is removed and the [] operator were changed to a + operator.
                        AST.ArrSub x = e as AST.ArrSub;
                        return new AST.BiExpr(x.arr.Type, env.GetASTEnv(), x.arr, x.idx, BiExpr.Op.PLUS);
                    }
                    if (e is AST.UnaryOp && (e as AST.UnaryOp).op == Op.STAR) {
                        // If the operand is the result of a unary * operator, neither that operator nor the &
                        // operator is evaluated and the result is as if both were omitted, except that the constraints
                        // on the opoerators still apply and the result is not an lvalue.
                        // To make sure the result is an rvalue, change the & and * operator to + operator with the second
                        // operand 0.
                        AST.UnaryOp x = e as AST.UnaryOp;
                        return new AST.BiExpr(x.expr.Type, env.GetASTEnv(), x.expr, new AST.ConstIntExpr(TInt.Instance, 0), BiExpr.Op.PLUS);
                    }
                    if (!e.IsLValue) {
                        throw new Error(Pos, "cannot take address of rvalue");
                    }
                    if (e.Type.IsBitField) {
                        throw new Error(Pos, "cannot take address of bit-field");
                    }
                    // TODO: Check register storage-class specifier.
                    return new AST.UnaryOp(e.Type.Ptr(), env.GetASTEnv(), e, Op.REF);
                case Op.STAR:
                    /// The operand of the uanry * operator shall have pointer type.
                    if (!e.Type.IsPointer) {
                        throw new Error(Pos, "cannot deref none pointer type");
                    }
                    return new AST.UnaryOp((e.Type.nake as TPointer).element, env.GetASTEnv(), e, Op.STAR);
                case Op.PLUS:
                case Op.MINUS:
                    /// The operand of the unary + or - operator shall have arithmetic type.
                    /// The result has integer promoted type.
                    if (!e.Type.IsArithmetic) {
                        throw new Error(Pos, "the operand of the unary + or - operator shall have arithmetic type");
                    }
                    return new AST.UnaryOp(e.Type.IntPromote(), env.GetASTEnv(), e, op);
                case Op.REVERSE:
                    /// Of the ~ operator, integer type.
                    /// The result has integer promoted type.
                    if (!e.Type.IsInteger) {
                        throw new Error(Pos, "the operand of the unary ~ operator shall have integer type");
                    }
                    return new AST.UnaryOp(e.Type.IntPromote(), env.GetASTEnv(), e, Op.REVERSE);
                case Op.NOT:
                    /// Of the ! operator, scalar type.
                    /// The result has type int.
                    if (!e.Type.IsScalar) {
                        throw new Error(Pos, "the operand of the unary ! operator shall have scalar type");
                    }
                    return new AST.UnaryOp(TInt.Instance.None(), env.GetASTEnv(), e, Op.NOT);
                default:
                    throw new InvalidOperationException("Unknown unary opeartor");
            }
        }

        public readonly Expr expr;
        public readonly Op op;
    }


    /// <summary>
    /// sizeof unary-expression
    /// sizeof ( type-name )
    /// </summary>
    public sealed class SizeOf : Expr, IEquatable<SizeOf> {

        public SizeOf(Expr expr) {
            this.expr = expr;
        }

        public SizeOf(TypeName name) {
            this.name = name;
        }

        public override Position Pos => expr == null ? name.Pos : expr.Pos;


        public bool Equals(SizeOf x) {
            return x != null && NullableEquals(x.name, name) && NullableEquals(x.expr, expr);
        }

        public override bool Equals(object obj) {
            return Equals(obj as SizeOf);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            T type;
            AST.Expr e;
            if (name != null) {
                type = name.GetT(env);
            } else {
                e = expr.GetASTExpr(env);
                type = e.Type;
            }

            if (type.IsVarArray) {
                throw new Error(Pos, "do not support variable length array");
            }

            /// The sizeof operator shall not be applied to an expression that has function type or an imcomplete type,
            /// to the parenthesized name of such a type, or to an expression that designates a bit-field member.
            if (type.IsFunc) {
                throw new Error(Pos, "cannot apply sizeof to function type");
            }
            if (!type.IsComplete) {
                throw new Error(Pos, "cannot apply sizeof to imcomplete type");
            }
            if (type.IsBitField) {
                throw new Error(Pos, "cannot apply sizeof to bit-field member");
            }

            /// The sizeof operator yields the size (in bytes) of its operand.
            /// The result has type an unsigned integer type size_t, which is defined in "stddef.h".
            /// Here I take unsigned int.
            return new AST.ConstIntExpr(TUInt.Instance, type.Size);

        }

        /// <summary>
        /// Nullable.
        /// </summary>
        public readonly Expr expr;

        /// <summary>
        /// Nullable.
        /// </summary>
        public readonly TypeName name;
    }

    /// <summary>
    /// arr[idx].
    /// </summary>
    public sealed class ArrSub : Expr, IEquatable<ArrSub> {

        public ArrSub(Expr arr, Expr idx) {
            this.arr = arr;
            this.idx = idx;
        }

        public override Position Pos => arr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ArrSub);
        }

        public bool Equals(ArrSub x) {
            return x != null && x.arr.Equals(arr) && x.idx.Equals(idx);
        }

        public override int GetHashCode() {
            return arr.GetHashCode() ^ idx.GetHashCode();
        }

        /// <summary>
        /// arr[idx] where
        ///     - arr -> pointer to object T
        ///     - idx -> integer type
        /// 
        /// Returns AST expr.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr arrExpr = arr.GetASTExpr(env);
            if (!arrExpr.Type.IsPointer && !arrExpr.Type.IsArray) {
                throw new Error(arr.Pos, "subscripting none pointer type");
            }

            AST.Expr idxExpr = idx.GetASTExpr(env);
            if (!idxExpr.Type.IsInteger) {
                throw new Error(idx.Pos, "subscripting none integer type");
            }

            T type = arrExpr.Type.IsPointer ?
                (arrExpr.Type.nake as TPointer).element :
                (arrExpr.Type.nake as TArray).element;
            return new AST.ArrSub(type, env.GetASTEnv(), arrExpr, idxExpr);
        }

        public readonly Expr arr;
        public readonly Expr idx;
    }

    /// <summary>
    /// s.i
    /// p->i
    /// </summary>
    public sealed class Access : Expr, IEquatable<Access> {

        public enum Kind {
            DOT,
            PTR
        }

        public Access(Expr aggregation, T_IDENTIFIER token, Kind type) {
            this.agg = aggregation;
            this.field = token.name;
            this.kind = type;
        }

        public override Position Pos => agg.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as Access);
        }

        public bool Equals(Access x) {
            return x != null && x.agg.Equals(agg)
                && x.field.Equals(field)
                && x.kind == kind;
        }

        public override int GetHashCode() {
            return agg.GetHashCode() ^ field.GetHashCode();
        }

        /// <summary>
        /// 
        /// A postfix expression followed by the DOT operator and an identifier designates a member.
        /// The value of the member is an lvalue if the the first expression is an lvalue.
        /// The result is a so-qualified version of the type of the designated member.
        /// 
        /// A postfix expression followed by the -> operator and an identifier designates a member.
        /// The value of the member is always an lvalue!
        /// The result is a so-qualified version of the type of the designated member.
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr aggExpr = agg.GetASTExpr(env);
            T aggType = aggExpr.Type;

            if (kind == Kind.PTR) {
                if (!aggType.IsPointer)
                    throw new Error(agg.Pos, "member reference base type is not a pointer");
                aggType = (aggType.nake as TPointer).element;
            }

            if (!aggType.IsStruct && !aggType.IsUnion)
                throw new Error(agg.Pos, "member reference base type is not a struct or union");

            TStructUnion s = aggType.nake as TStructUnion;

            T m = s.GetType(field);
            if (m == null) {
                throw new Error(agg.Pos, string.Format("no member named {0} in {1}", field, s.ToString()));
            }

            return new AST.Access(aggType.Unnest(m), env.GetASTEnv(), aggExpr, field, kind);
        }

        public readonly Expr agg;
        public readonly string field;
        public readonly Kind kind;
    }

    /// <summary>
    /// i++
    /// i--
    /// </summary>
    public sealed class PostStep : Expr, IEquatable<PostStep> {

        public enum Kind {
            INC,
            DEC
        }

        public PostStep(Expr expr, Kind kind) {
            this.expr = expr;
            this.kind = kind;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as PostStep);
        }

        public bool Equals(PostStep x) {
            return x != null && x.expr.Equals(expr) && x.kind == kind;
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        /// <summary>
        /// From C99 standard:
        /// "
        ///   The operand of the postfix increment or decrement operator shall have qualified
        /// or unqualified real or pointer type and shall be a modifiable lvalue.
        /// "
        ///  
        /// However test with clang and gcc shows that complex type can also be incremented.
        /// 
        /// Returns the same type but as rvalue.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override AST.Expr GetASTExpr(Env env) {
            AST.Expr e = expr.GetASTExpr(env);
            if (!e.Type.IsPointer && !e.Type.IsReal) {
                throw new Error(expr.Pos, string.Format("{0} on type {1}", kind, e.Type));
            }

            if (!e.IsLValue) {
                throw new Error(expr.Pos, "cannot modify rvalue");
            }

            if (e.Type.qualifiers.isConstant) {
                throw new Error(expr.Pos, "cannot modify constant value");
            }

            return new AST.PostStep(e.Type, env.GetASTEnv(), e, kind);
        }

        public readonly Expr expr;
        public readonly Kind kind;
    }

    /// <summary>
    /// ( type-name ) { initializer-list [,] }
    /// </summary>
    public sealed class STCompound : Expr, IEquatable<STCompound> {

        public STCompound(TypeName name, IEnumerable<STInitItem> inits) {
            this.name = name;
            this.inits = inits;
        }

        public bool Equals(STCompound x) {
            return x != null && x.name.Equals(name) && x.inits.SequenceEqual(inits);
        }

        public override bool Equals(object obj) {
            return Equals(obj as STCompound);
        }

        public override int GetHashCode() {
            return name.GetHashCode();
        }

        public override Position Pos => name.Pos;

        public readonly TypeName name;
        public readonly IEnumerable<STInitItem> inits;
    }

    public sealed class FuncCall : Expr, IEquatable<FuncCall> {

        public FuncCall(Expr expr, IEnumerable<Expr> args) {
            this.expr = expr;
            this.args = args;
        }

        public override Position Pos => expr.Pos;

        public bool Equals(FuncCall x) {
            return x != null && x.expr.Equals(expr) && x.args.SequenceEqual(args);
        }

        public override bool Equals(object obj) {
            return Equals(obj as FuncCall);
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public readonly Expr expr;
        public readonly IEnumerable<Expr> args;
    }

    /// <summary>
    /// Represents an identfier.
    /// </summary>
    public sealed class Id : Expr, IEquatable<Id> {

        public Id(T_IDENTIFIER token) {
            pos = new Position { line = token.line };
            symbol = token.name;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as Id);
        }

        public bool Equals(Id x) {
            return x != null && x.symbol.Equals(symbol) && x.pos.Equals(pos);
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            var entry = env.GetSymbol(symbol);
            if (entry == null) {
                // This is an undeclared identifier.
                throw new ErrUndefinedIdentifier(pos, symbol);
            } else {
                switch (entry.kind) {
                    case SymbolEntry.Kind.OBJECT:
                        return new AST.ObjExpr(entry.type, env.GetASTEnv(), symbol);
                    case SymbolEntry.Kind.FUNCTION:
                        return new AST.FuncDesignator(entry.type, env.GetASTEnv(), symbol);
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public readonly string symbol;

        private readonly Position pos;
    }

    public abstract class Constant : Expr {

        public Constant(Position pos) {
            this.pos = pos;
        }

        public override Position Pos => pos;

        protected static Func<char, bool> IsOctal = (char c) => c >= '0' && c <= '7';
        protected static Func<char, bool> IsDecimal = (char c) => c >= '0' && c <= '9';
        protected static Func<char, bool> IsHexadecimal = (char c) => IsDecimal(c) || char.ToLower(c) >= 'a' && char.ToLower(c) <= 'f';
        protected static Func<char, int> GetHexadecimal = (char c) => {
            if (IsDecimal(c)) return c - '0';
            else return char.ToLower(c) - 'a' + 10;
        };

        protected readonly Position pos;
    }

    /// <summary>
    /// Represent an integer constant with its value and its type (rvalue).
    /// The type of an integer constant is the first of the corresponding list in which its value can be represented.
    /// 
    /// Sufix               Decimal Constant                Octal or Hexadecimal Constant
    /// ------------------------------------------------------------------------------------------
    /// NONE                int                             int
    ///                     long int                        unsigned int
    ///                     long long int                   long int
    ///                                                     unsigned long int
    ///                                                     long long int
    ///                                                     unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// U                   unsigned int                    unsigned int
    ///                     unsigned long int               unsigned long int
    ///                     unsigned long long int          unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// L                   long int                        long int
    ///                     long long int                   unsigned long int
    ///                                                     long long int
    ///                                                     unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// UL                  unsigned long int               unsigned long int
    ///                     unsigned long long int          unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// LL                  long long int                   long long int
    ///                                                     unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// ULL                 unsigned long long int          unsigned long long int
    /// ------------------------------------------------------------------------------------------
    /// 
    /// </summary>
    public sealed class ConstInt : Constant, IEquatable<ConstInt> {

        public ConstInt(T_CONST_INT token) : base(new Position { line = token.line }) {
            value = Evaluate(token);

            // Select the proper type for this constant.
            switch (token.suffix) {
                case T_CONST_INT.Suffix.NONE:
                    if (token.n != 10)
                        // Octal or hexadecimal constant.
                        type = FitInType(pos, value,
                            TInt.Instance,
                            TUInt.Instance,
                            TLong.Instance,
                            TULong.Instance,
                            TLLong.Instance,
                            TULLong.Instance);
                    else
                        // Decimal constant.
                        type = FitInType(pos, value,
                            TInt.Instance,
                            TLong.Instance,
                            TLLong.Instance);
                    break;
                case T_CONST_INT.Suffix.U:
                    type = FitInType(pos, value,
                        TUInt.Instance,
                        TULong.Instance,
                        TULLong.Instance);
                    break;
                case T_CONST_INT.Suffix.L:
                    if (token.n != 10)
                        type = FitInType(pos, value,
                            TLong.Instance,
                            TULong.Instance,
                            TLLong.Instance,
                            TULLong.Instance);
                    else
                        type = FitInType(pos, value,
                            TLong.Instance,
                            TLLong.Instance);
                    break;
                case T_CONST_INT.Suffix.UL:
                    type = FitInType(pos, value,
                        TULong.Instance,
                        TULLong.Instance);
                    break;
                case T_CONST_INT.Suffix.LL:
                    if (token.n != 10)
                        type = FitInType(pos, value,
                            TLLong.Instance,
                            TULLong.Instance);
                    else
                        type = FitInType(pos, value, TLLong.Instance);
                    break;
                case T_CONST_INT.Suffix.ULL:
                    type = FitInType(pos, value, TULLong.Instance);
                    break;
            }
        }

        public override bool Equals(object obj) {
            return Equals(obj as ConstInt);
        }

        public bool Equals(ConstInt x) {
            return x != null && x.pos.Equals(pos)
                && x.value == value
                && x.type.Equals(type);
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            return new AST.ConstIntExpr(type, value);
        }

        /// <summary>
        /// Evaluate the text and get the value.
        /// </summary>
        /// <param name="token"> Token to be evaluated. </param>
        /// <returns> BigInteger. </returns>
        private static BigInteger Evaluate(T_CONST_INT token) {

            BigInteger value = 0;

            foreach (var c in token.text) {
                value = value * token.n + GetHexadecimal(c);
            }

            return value;
        }

        private static TInteger FitInType(Position pos, BigInteger value, params TInteger[] types) {
            foreach (var type in types) {
                if (value >= type.MIN && value <= type.MAX) {
                    return type;
                }
            }
            throw new ErrIntegerLiteralOutOfRange(pos);
        }

        public readonly BigInteger value;
        public readonly TInteger type;
    }

    /// <summary>
    /// A character is either an octal sequence, a hexadecimal sequence or an ascii character.
    /// An octal sequence and a hexadecimal sequence shall be in the range of representable values for the type unsigned char.
    /// </summary>
    public sealed class ConstChar : Constant {

        public ConstChar(T_CONST_CHAR token) : base(new Position { line = token.line }) {

            // Do not support multi-character characer.
            if (token.prefix == T_CONST_CHAR.Prefix.L) {
                throw new EUnknownType(pos, "multi-character");
            }

            values = Evaluate(pos, token.text);

            // Do not support multi-character characer.
            if (values.Count() > 1) {
                throw new EUnknownType(pos, "multi-character");
            }
        }

        public override bool Equals(object obj) {
            return Equals(obj as ConstChar);
        }

        public bool Equals(ConstChar x) {
            return x != null && x.pos.Equals(pos)
                && x.values.SequenceEqual(values);
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        public override AST.Expr GetASTExpr(Env env) {
            return new AST.ConstIntExpr(TChar.Instance, values.First());
        }

        /// <summary>
        /// Evaluate the text to a list of integer character constant.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<ushort> Evaluate(Position pos, string text) {

            BigInteger value = 0;
            LinkedList<ushort> values = new LinkedList<ushort>();

            // state 0: initial state.
            // state 1: \.
            // state 2: \x.
            // state 3: \o.
            // state 4: \oo.

            const int INITIAL = 0;
            const int ESCAPED = 1;
            const int HEXADEC = 2;
            const int OCTAL1 = 3;
            const int OCTAL2 = 4;

            int state = INITIAL;

            // Check the value is within the range of unsigned char
            // and push it into the list.
            Action<BigInteger> PushValue = v => {
                if (v < TUChar.Instance.MIN || v > TUChar.Instance.MAX) {
                    throw new ErrEscapedSequenceOutOfRange(pos, text);
                }
                values.AddLast((ushort)v);  // Store the current result.
            };

            for (int i = 0; i < text.Length; ++i) {

                switch (state) {
                    case INITIAL:
                        // Initial state.
                        if (text[i] == '\\') {
                            state = ESCAPED;
                        } else {
                            values.AddLast(text[i]);
                        }
                        break;
                    case ESCAPED:
                        // Seen one backslash.
                        if (text[i] == 'x') {
                            state = HEXADEC;
                        } else if (IsOctal(text[i])) {
                            value = value * 8 + (text[i] - '0');
                            state = OCTAL1;
                        } else {
                            state = INITIAL;
                            switch (text[i]) {
                                case 'a':
                                    values.AddLast(7);
                                    break;
                                case 'b':
                                    values.AddLast(8);
                                    break;
                                case 'f':
                                    values.AddLast(12);
                                    break;
                                case 'n':
                                    values.AddLast(10);
                                    break;
                                case 'r':
                                    values.AddLast(13);
                                    break;
                                case 't':
                                    values.AddLast(9);
                                    break;
                                case 'v':
                                    values.AddLast(11);
                                    break;
                                default:
                                    values.AddLast(text[i]);
                                    break;
                            }
                        }
                        break;
                    case HEXADEC:
                        if (IsHexadecimal(text[i])) {
                            value = value * 16 + GetHexadecimal(text[i]);
                        } else {
                            // Check the value is within the range of unsinged char.
                            PushValue(value);
                            value = 0;              // Reset the value to zero;
                            i--;                    // Backward one char.
                            state = INITIAL;        // Reset the state.
                        }
                        break;
                    case OCTAL1:
                        if (IsOctal(text[i])) {
                            value = value * 8 + (text[i] - '0');
                            state = OCTAL2;
                        } else {
                            // Check the value is within the range of unsinged char.
                            PushValue(value);
                            value = 0;
                            i--;
                            state = INITIAL;
                        }
                        break;
                    case OCTAL2:
                        if (IsOctal(text[i])) {
                            value = value * 8 + (text[i] - '0');
                        } else {
                            i--;
                        }
                        // Check the value is within the range of unsinged char.
                        PushValue(value);
                        state = INITIAL;
                        value = 0;
                        break;
                }
            }

            // Must in escaped sequence.
            if (state != INITIAL) {
                PushValue(value);
            }

            return values;
        }

        /// <summary>
        /// Store the values from Evaluate().
        /// </summary>
        public readonly IEnumerable<ushort> values;
    }

    /// <summary>
    /// A float constant is composed with
    /// 
    /// significant-part [eEpP] exponent-part
    /// 
    /// whole-part . fraction-part [eEpP] exponent-part.
    /// 
    /// Significant-part is processed as rational number with base 10 or 16.
    /// Exponent-part is processes as a decimal integer, to which 10 or 2 will be raised.
    /// 
    /// </summary>
    public sealed class ConstFloat : Constant, IEquatable<ConstFloat> {
        public ConstFloat(T_CONST_FLOAT token) : base(new Position { line = token.line }) {
            value = Evaluate(token);
            switch (token.suffix) {
                case T_CONST_FLOAT.Suffix.NONE:
                    t = TDouble.Instance;
                    break;
                case T_CONST_FLOAT.Suffix.F:
                    t = TSingle.Instance;
                    break;
                case T_CONST_FLOAT.Suffix.L:
                    t = TLDouble.Instance;
                    break;
            }
        }

        public override bool Equals(object obj) {
            return Equals(obj as ConstFloat);
        }

        public bool Equals(ConstFloat x) {
            return x != null && x.pos.Equals(pos)
                && Math.Abs(x.value - value) < 0.0001f
                && x.t.Equals(t);
        }

        public override AST.Expr GetASTExpr(Env env) {
            return new AST.ConstFloatExpr(t, value);
        }

        private static double Evaluate(T_CONST_FLOAT token) {

            const int WHOLE = 0;
            const int FRACT = 1;
            const int PEXPO = 2;
            const int NEXPO = 3;

            int state = WHOLE;

            double value = 0;
            int fractCount = -1;
            int exponPart = 0;

            Func<char, bool> IsExpon = c => char.ToLower(c) == 'p' || char.ToLower(c) == 'e';

            foreach (var c in token.text) {
                switch (state) {
                    case WHOLE:
                        if (c == '.') state = FRACT;
                        else if (IsExpon(c)) state = PEXPO;
                        else value = value * token.n + GetHexadecimal(c);
                        break;
                    case FRACT:
                        if (IsExpon(c)) state = PEXPO;
                        else value += GetHexadecimal(c) * Math.Pow(token.n, fractCount--);
                        break;
                    case PEXPO:
                        if (c == '-') state = NEXPO;
                        else if (c == '+') state = PEXPO;
                        else exponPart = exponPart * 10 + c - '0';
                        break;
                    case NEXPO:
                        exponPart = exponPart * 10 - (c - '0');
                        break;
                }
            }

            value *= Math.Pow(token.n == 10 ? 10 : 2, exponPart);

            return value;
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        public readonly double value;
        public readonly TArithmetic t;
    }

    /// <summary>
    /// A string is variable sequence of char.
    /// Notice: string is array of char, lvalue. which means
    ///     "what"[0] = 'c';
    /// is totally legal.
    /// 
    /// Each char is the same as in constant character integer defined in ASTConstChar.
    /// </summary>
    public sealed class Str : Expr, IEquatable<Str> {
        public Str(LinkedList<T_STRING_LITERAL> tokens) {
            pos = new Position { line = tokens.First().line };
            values = Evaluate(tokens);
            type = TChar.Instance.None().Arr(values.Count());
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as Str);
        }

        public bool Equals(Str s) {
            return s != null && s.pos.Equals(pos) && s.values.SequenceEqual(values);
        }

        public override int GetHashCode() {
            return values.First();
        }

        public static IEnumerable<ushort> Evaluate(LinkedList<T_STRING_LITERAL> tokens) {

            IEnumerable<ushort> values = new LinkedList<ushort>();

            foreach (var t in tokens) {
                Position pos = new Position { line = t.line };
                if (t.prefix == T_STRING_LITERAL.Prefix.L) {
                    throw new EUnknownType(pos, "multi-character");
                }
                values = values.Concat(ConstChar.Evaluate(pos, t.text));
            }
            return values;
        }

        public readonly Position pos;
        public readonly IEnumerable<ushort> values;
        public readonly T type;
    }
}
