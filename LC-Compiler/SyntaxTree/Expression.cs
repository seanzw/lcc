using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;
using lcc.Token;

namespace lcc.SyntaxTree {

    /// <summary>
    /// The base class for all expression.
    /// </summary>
    public abstract class Expr : Stmt {

        public virtual T TypeCheck(Env env) {
            throw new NotImplementedException();
        }

        public virtual AST.Expr ToAST(Env env) {
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

    public sealed class STBiExpr : Expr, IEquatable<STBiExpr> {

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

        public STBiExpr(Expr lhs, Expr rhs, Op op) {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }

        public override Position Pos => lhs.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STBiExpr);
        }

        public bool Equals(STBiExpr x) {
            return x != null
                && x.lhs.Equals(lhs)
                && x.rhs.Equals(rhs)
                && x.op.Equals(op);
        }

        public override int GetHashCode() {
            return lhs.GetHashCode() ^ rhs.GetHashCode() ^ op.GetHashCode();
        }

        public readonly Expr lhs;
        public readonly Expr rhs;
        public readonly Op op;
    }


    /// <summary>
    /// ( type-name ) expr
    /// </summary>
    public sealed class STCast : Expr, IEquatable<STCast> {

        public STCast(TypeName name, Expr expr) {
            this.name = name;
            this.expr = expr;
        }

        public override Position Pos => name.Pos;
        public bool Equals(STCast x) {
            return x != null && x.name.Equals(name) && x.expr.Equals(expr);
        }

        public override bool Equals(object obj) {
            return Equals(obj as STCast);
        }

        public override int GetHashCode() {
            return name.GetHashCode() ^ expr.GetHashCode();
        }

        public readonly TypeName name;
        public readonly Expr expr;
    }

    /// <summary>
    /// ++i
    /// --i
    /// </summary>
    public sealed class STPreStep : Expr, IEquatable<STPreStep> {

        public enum Kind {
            INC,
            DEC
        }

        public STPreStep(Expr expr, Kind kind) {
            this.expr = expr;
            this.kind = kind;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STPreStep);
        }

        public bool Equals(STPreStep x) {
            return x != null && x.expr.Equals(expr) && x.kind == kind;
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public override T TypeCheck(Env env) {
            T t = expr.TypeCheck(env);
            if (!t.IsArithmetic && !t.IsPointer) {
                throw new Error(expr.Pos, string.Format("{0} on type {1}", kind, t));
            }
            if (!t.IsModifiable) {
                throw new Error(expr.Pos, string.Format("cannot assign to type {0}", t));
            }
            return t.R();
        }

        public readonly Expr expr;
        public readonly Kind kind;
    }

    /// <summary>
    /// & * + - ~ !
    /// </summary>
    public sealed class STUnaryOp : Expr, IEquatable<STUnaryOp> {

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

        public STUnaryOp(Expr expr, Op op) {
            this.expr = expr;
            this.op = op;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STUnaryOp);
        }

        public bool Equals(STUnaryOp x) {
            return x != null && x.expr.Equals(expr) && x.op == op;
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public override T TypeCheck(Env env) {
            T t = expr.TypeCheck(env);
            switch (op) {
                case Op.REF:
                    if (t.IsRValue)
                        throw new Error(expr.Pos, string.Format("cannot take address of rvalue of {0}", t));
                    return t.Ptr().R();
                case Op.STAR:
                    if (!t.IsPointer)
                        throw new Error(expr.Pos, string.Format("indirection requires pointer type ({0} provided)", t));
                    return (t.nake as TPointer).element;
                case Op.PLUS:
                case Op.MINUS:
                    if (!t.IsArithmetic)
                        throw new Error(expr.Pos, string.Format("invalid argument type '{0}' to unary expression", t));
                    return t.IntPromote().R();
                case Op.REVERSE:
                    if (!t.IsInteger)
                        throw new Error(expr.Pos, string.Format("invalid argument type '{0}' to unary expression", t));
                    return t.IntPromote().R();
                case Op.NOT:
                    if (!t.IsScalar)
                        throw new Error(expr.Pos, string.Format("invalid argument type '{0}' to unary expression", t));
                    return TInt.Instance.None(T.LR.R);
                default:
                    throw new InvalidOperationException("Unrecognized unary operator");
            }
        }

        public readonly Expr expr;
        public readonly Op op;
    }


    /// <summary>
    /// sizeof unary-expression
    /// sizeof ( type-name )
    /// </summary>
    public sealed class STSizeOf : Expr, IEquatable<STSizeOf> {

        public STSizeOf(Expr expr) {
            this.expr = expr;
        }

        public STSizeOf(TypeName name) {
            this.name = name;
        }

        public override Position Pos => expr == null ? name.Pos : expr.Pos;


        public bool Equals(STSizeOf x) {
            return x != null && NullableEquals(x.name, name) && NullableEquals(x.expr, expr);
        }

        public override bool Equals(object obj) {
            return Equals(obj as STSizeOf);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
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
    public sealed class STArrSub : Expr, IEquatable<STArrSub> {

        public STArrSub(Expr arr, Expr idx) {
            this.arr = arr;
            this.idx = idx;
        }

        public override Position Pos => arr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STArrSub);
        }

        public bool Equals(STArrSub x) {
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
        /// Returns T
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override T TypeCheck(Env env) {
            T arrType = arr.TypeCheck(env);
            if (!arrType.IsPointer && !arrType.IsArray) {
                throw new Error(arr.Pos, "subscripting none pointer type");
            }

            T idxType = idx.TypeCheck(env);
            if (!idxType.IsInteger) {
                throw new Error(idx.Pos, "subscripting none integer type");
            }

            if (arrType.IsPointer)
                return (arrType.nake as TPointer).element;
            else
                return (arrType.nake as TArray).element;
        }

        public readonly Expr arr;
        public readonly Expr idx;
    }

    /// <summary>
    /// s.i
    /// p->i
    /// </summary>
    public sealed class STAccess : Expr, IEquatable<STAccess> {

        public enum Kind {
            DOT,
            PTR
        }

        public STAccess(Expr aggregation, T_IDENTIFIER token, Kind type) {
            this.agg = aggregation;
            this.field = token.name;
            this.kind = type;
        }

        public override Position Pos => agg.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STAccess);
        }

        public bool Equals(STAccess x) {
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
        public override T TypeCheck(Env env) {
            T aggType = agg.TypeCheck(env);

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

            if (kind == Kind.DOT) {
                return aggType.Unnest(m, aggType.lr);
            } else {
                return aggType.Unnest(m, T.LR.L);
            }
        }

        public readonly Expr agg;
        public readonly string field;
        public readonly Kind kind;
    }

    /// <summary>
    /// i++
    /// i--
    /// </summary>
    public sealed class STPostStep : Expr, IEquatable<STPostStep> {

        public enum Kind {
            INC,
            DEC
        }

        public STPostStep(Expr expr, Kind kind) {
            this.expr = expr;
            this.kind = kind;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STPostStep);
        }

        public bool Equals(STPostStep x) {
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
        public override T TypeCheck(Env env) {

            T t = expr.TypeCheck(env);
            if (!t.IsPointer && !t.IsArithmetic) {
                throw new Error(expr.Pos, string.Format("{0} on type {1}", kind, t));
            }

            if (!t.IsModifiable) {
                throw new Error(expr.Pos, string.Format("cannot assign to type {0}", t));
            }

            return t.R();
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

    public sealed class STFuncCall : Expr, IEquatable<STFuncCall> {

        public STFuncCall(Expr expr, IEnumerable<Expr> args) {
            this.expr = expr;
            this.args = args;
        }

        public override Position Pos => expr.Pos;

        public bool Equals(STFuncCall x) {
            return x != null && x.expr.Equals(expr) && x.args.SequenceEqual(args);
        }

        public override bool Equals(object obj) {
            return Equals(obj as STFuncCall);
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
            name = token.name;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as Id);
        }

        public bool Equals(Id x) {
            return x != null && x.name.Equals(name) && x.pos.Equals(pos);
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        /// <summary>
        /// Check that the identfifier is defined as variable.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override T TypeCheck(Env env) {
            var entry = env.GetSymbol(name);
            if (entry == null) throw new ErrUndefinedIdentifier(pos, name);
            else return entry.type;
        }

        public readonly string name;

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

        public override AST.Expr ToAST(Env env) {
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
                throw new ErrUnknownType(pos, "multi-character");
            }

            values = Evaluate(pos, token.text);

            // Do not support multi-character characer.
            if (values.Count() > 1) {
                throw new ErrUnknownType(pos, "multi-character");
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

        public override AST.Expr ToAST(Env env) {
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
                    t = TFloat.Instance;
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

        public override AST.Expr ToAST(Env env) {
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
            var arrType = new TArray(TChar.Instance.None(T.LR.L), values.Count());
            type = arrType.None(T.LR.L);
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

        public override T TypeCheck(Env env) {
            return type;
        }

        public static IEnumerable<ushort> Evaluate(LinkedList<T_STRING_LITERAL> tokens) {

            IEnumerable<ushort> values = new LinkedList<ushort>();

            foreach (var t in tokens) {
                Position pos = new Position { line = t.line };
                if (t.prefix == T_STRING_LITERAL.Prefix.L) {
                    throw new ErrUnknownType(pos, "multi-character");
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
