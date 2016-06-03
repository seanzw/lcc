using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {
    public abstract class Expr : Stmt {
        public abstract T Type { get; }
        public abstract bool IsLValue { get; }
    }

    public abstract class VarExpr : Expr {
        protected readonly T type;
        public readonly Env env;
        public override T Type => type;
        public VarExpr(T type, Env env) {
            this.type = type;
            this.env = env;
        }
    }

    public sealed class BiExpr : VarExpr {
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

    public sealed class Cast : VarExpr {
        public readonly Expr expr;
        public override bool IsLValue => true;
        public Cast(TUnqualified type, Env env, Expr expr) : base(type.None(), env) {
            this.expr = expr;
        }
    }

    public sealed class UnaryOp : VarExpr {
        public readonly Expr expr;
        public readonly SyntaxTree.UnaryOp.Op op;
        public override bool IsLValue => false;
        public UnaryOp(T type, Env env, Expr expr, SyntaxTree.UnaryOp.Op op) : base(type, env) {
            this.expr = expr;
            this.op = op;
        }
    }

    public sealed class PreStep : VarExpr {
        public readonly Expr expr;
        public readonly SyntaxTree.PreStep.Kind kind;
        public override bool IsLValue => false;
        public PreStep(T type, Env env, Expr expr, SyntaxTree.PreStep.Kind kind) : base(type, env) {
            this.expr = expr;
            this.kind = kind;
        }
    }

    public sealed class ArrSub : VarExpr {
        public readonly Expr arr;
        public readonly Expr idx;
        public override bool IsLValue => true;
        public ArrSub(T type, Env env, Expr arr, Expr idx) : base(type, env) {
            this.arr = arr;
            this.idx = idx;
        }
    }

    public sealed class Access : VarExpr {
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

    public sealed class PostStep : VarExpr {
        public readonly Expr expr;
        public readonly SyntaxTree.PostStep.Kind kind;
        public override bool IsLValue => false;
        public PostStep(T type, Env env, Expr expr, SyntaxTree.PostStep.Kind kind) : base(type, env) {
            this.expr = expr;
            this.kind = kind;
        }
    }

    public sealed class FuncDesignator : VarExpr {
        public readonly string symbol;
        public override bool IsLValue => true;
        public FuncDesignator(T type, Env env, string symbol) : base(type, env) {
            this.symbol = symbol;
        }
    }

    public sealed class ObjExpr : VarExpr {
        public readonly string symbol;
        public override bool IsLValue => true;
        public ObjExpr(T type, Env env, string symbol) : base(type, env) {
            this.symbol = symbol;
        }
    }

    public abstract class ConstExpr : Expr {
        public readonly T type;
        public override T Type => type;
        public override bool IsLValue => false;
        public ConstExpr(TUnqualified type) {
            this.type = type.Const();
        }
    }

    /// <summary>
    /// Arithmetic constant expression
    ///     - integer constant expression
    ///     - float constant expression
    /// </summary>
    public abstract class ConstArithExpr : ConstExpr {
        public ConstArithExpr(TUnqualified type) : base(type) { }
    }

    /// <summary>
    /// Integer constant expression.
    /// </summary>
    public sealed class ConstIntExpr : ConstArithExpr {
        public readonly TInteger t;
        public readonly BigInteger value;
        public ConstIntExpr(TInteger t, BigInteger value) : base(t) {
            this.t = t;
            this.value = value;
        }
        public override T Type => t.Const();
    }

    /// <summary>
    /// Float constant expression.
    /// </summary>
    public sealed class ConstFloatExpr : ConstArithExpr {
        public readonly TArithmetic t;
        public readonly double value;
        public ConstFloatExpr(TArithmetic t, double value) : base(t) {
            this.t = t;
            this.value = value;
        }
        public override T Type => t.Const();
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
