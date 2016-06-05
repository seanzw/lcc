using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {
    public abstract class Expr : Stmt {
        public T Type => type;
        public Env Envrionment => env;
        public abstract bool IsLValue { get; }

        protected readonly T type;
        protected readonly Env env;

        public Expr(T type, Env env) {
            this.type = type;
            this.env = env;
        }

        /// <summary>
        /// Performs all the three implicit conversions by explicitly using cast operator.
        /// </summary>
        /// <returns></returns>
        public Expr ImplicitCast() {
            return CastLValue().CastArr().CastFunc();
        }

        /// <summary>
        /// 
        /// 
        /// 1. An lvalue that does not have array type is converted to the value stored in the designated
        ///    object (and is no longer an lvalue).
        ///    If the lvalue has qualified type, the values has unqualified version of the type of the lvalue.
        ///    Otherwise, the value has the type of the lvalue.
        ///    EXCEPT: sizeof, unary &, ++, --, left operand of ., left operand of assignment operator.
        /// </summary>
        /// <returns></returns>
        public Expr CastLValue() {
            return (IsLValue && !type.IsArray) ? new Cast(type.nake, env, this) : this;
        }

        /// <summary>
        /// 2. An expression that has type "array of type" is converted to an expression with type "pointer to type"
        ///    that points to the initial element of the array object and is not an lvalue.
        ///    If the array object has register storage class, the behavior is undefined.
        ///    EXCEPT: sizeof, unary &, a string literal used to initialize an array.
        /// </summary>
        /// <returns></returns>
        public Expr CastArr() {
            return type.IsArray ? new Cast(new TPtr((type.nake as TArr).element), env, this) : this;
        }

        /// <summary>
        /// 3. A function designator is an expression that has function type. A function designator with type
        ///    "function returning type" is converted to an expression that has type "pointer to function returning type".
        ///    EXCEPT: sizeof, unary &.
        /// </summary>
        /// <returns></returns>
        public Expr CastFunc() {
            return type.IsFunc ? new Cast(new TPtr(type), env, this) : this;
        }
    }

    public sealed class BiExpr : Expr {
        public readonly Expr lhs;
        public readonly Expr rhs;
        public readonly SyntaxTree.BiExpr.Op op;
        public override bool IsLValue => false;
        public BiExpr(T type, Env env, Expr lhs, Expr rhs, SyntaxTree.BiExpr.Op op) : base(type, env) {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }
    }

    public sealed class Cast : Expr {
        public readonly Expr expr;
        public override bool IsLValue => false;
        public Cast(TUnqualified type, Env env, Expr expr) : base(type.None(), env) {
            this.expr = expr;
        }
    }

    public sealed class UnaryOp : Expr {
        public readonly Expr expr;
        public readonly SyntaxTree.UnaryOp.Op op;
        public override bool IsLValue => false;
        public UnaryOp(T type, Env env, Expr expr, SyntaxTree.UnaryOp.Op op) : base(type, env) {
            this.expr = expr;
            this.op = op;
        }
    }

    public sealed class PreStep : Expr {
        public readonly Expr expr;
        public readonly SyntaxTree.PreStep.Kind kind;
        public override bool IsLValue => false;
        public PreStep(T type, Env env, Expr expr, SyntaxTree.PreStep.Kind kind) : base(type, env) {
            this.expr = expr;
            this.kind = kind;
        }
    }

    public sealed class ArrSub : Expr {
        public readonly Expr arr;
        public readonly Expr idx;
        public override bool IsLValue => true;
        public ArrSub(T type, Env env, Expr arr, Expr idx) : base(type, env) {
            this.arr = arr;
            this.idx = idx;
        }
    }

    public sealed class Access : Expr {
        public readonly Expr agg;
        public readonly string field;
        public readonly SyntaxTree.Access.Kind kind;
        public override bool IsLValue => kind == SyntaxTree.Access.Kind.DOT ? agg.IsLValue : true;
        public Access(T type, Env env, Expr agg, string field, SyntaxTree.Access.Kind kind)
            : base(type, env) {
            this.agg = agg;
            this.field = field;
            this.kind = kind;
        }
    }

    public sealed class PostStep : Expr {
        public readonly Expr expr;
        public readonly SyntaxTree.PostStep.Kind kind;
        public override bool IsLValue => false;
        public PostStep(T type, Env env, Expr expr, SyntaxTree.PostStep.Kind kind) : base(type, env) {
            this.expr = expr;
            this.kind = kind;
        }
    }

    public sealed class FuncDesignator : Expr {
        public readonly string symbol;
        public override bool IsLValue => true;
        public FuncDesignator(T type, Env env, string symbol) : base(type, env) {
            this.symbol = symbol;
        }
    }

    public sealed class ObjExpr : Expr {
        public readonly string symbol;
        public override bool IsLValue => true;
        public ObjExpr(T type, Env env, string symbol) : base(type, env) {
            this.symbol = symbol;
        }
    }

    public abstract class ConstExpr : Expr {
        public override bool IsLValue => false;
        public ConstExpr(TUnqualified type, Env env) : base(type.None(), env) { }
    }

    /// <summary>
    /// Arithmetic constant expression
    ///     - integer constant expression
    ///     - float constant expression
    /// </summary>
    public abstract class ConstArithExpr : ConstExpr {
        public ConstArithExpr(TUnqualified type, Env env) : base(type, env) { }
    }

    /// <summary>
    /// Integer constant expression.
    /// </summary>
    public sealed class ConstIntExpr : ConstArithExpr {
        public readonly TInteger t;
        public readonly BigInteger value;
        public ConstIntExpr(TInteger t, BigInteger value, Env env) : base(t, env) {
            this.t = t;
            this.value = value;
        }
    }

    /// <summary>
    /// Float constant expression.
    /// </summary>
    public sealed class ConstFloatExpr : ConstArithExpr {
        public readonly TArithmetic t;
        public readonly double value;
        public ConstFloatExpr(TArithmetic t, double value, Env env) : base(t, env) {
            this.t = t;
            this.value = value;
        }
    }

    ///// <summary>
    ///// An address constant is a null pointer, a pointer to an lvalue designating an object of static storage duration,
    ///// or a pointer to a function.
    ///// </summary>
    //public abstract class ConstAddrExpr : ConstExpr {
    //}

    ///// <summary>
    ///// A pointer to an lvalue designating an object of static storage duration.
    ///// </summary>
    //public sealed class ConstAddrObj : ConstAddrExpr {
    //    public readonly Obj entry;
    //    public ConstAddrObj(Obj entry) {
    //        this.entry = entry;
    //    }
    //    public override TUnqualified Type => t;
    //}
}
