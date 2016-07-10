using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {
    public abstract class Expr : Node {
        public T Type => type;
        public Env Envrionment => env;
        public virtual bool IsLValue => false;

        public virtual bool IsConstZero => false;
        public virtual bool IsNullPtr => false;

        protected readonly T type;
        protected readonly Env env;

        public Expr(T type, Env env) {
            this.type = type;
            this.env = env;
        }

        /// <summary>
        /// Performs integer promotion by explicitly using cast operator.
        /// </summary>
        /// <returns></returns>
        public Expr IntPromote() {
            T type = this.type.IntPromote();
            return type.Equals(this.type) ? this : new Cast(type.nake, env, this);
        }

        /// <summary>
        /// Perform usual arithmetic conversion by explicitly using cast operator.
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        public static Tuple<T, Expr, Expr> UsualArithConvert(Expr e1, Expr e2) {
            T type = e1.Type.UsualArithConversion(e2.Type);
            Expr c1 = type.Equals(e1.Type) ? e1 : new Cast(type.nake, e1.Envrionment, e1);
            Expr c2 = type.Equals(e2.type) ? e2 : new Cast(type.nake, e2.Envrionment, e2);
            return new Tuple<T, Expr, Expr>(type, c1, c2);
        }

        /// <summary>
        /// Whether this type can be implicitly convertible to target without generating an warning.
        /// Returns null if it cannot be implicitly converted to the target.
        /// 
        /// One of the following situation. All other situations are either illegal or requiring explicit cast.
        /// 
        /// 0. The same types.
        /// 
        /// 1. Compatible types.
        /// 
        /// 2. Boolean conversion.
        ///    A value of any scalar type can be implicitly converted to _Bool.
        ///    
        /// 3. Arithmetic conversion.
        ///    A value of arithmetic type can be implicitly converted to another arithmetic type without warning.
        ///    No matter real or complex.
        ///    
        /// 4. Pointer conversion.
        ///    a. A pointer to void can be implicitly converted to and from a pointer to any incomplete or object type.
        ///    NOTE: I checked clang and void* can be implicitly converted to a pointer to function type and vice verse.
        ///    Here I stick with the standard.
        ///    
        ///    b. For any qualifier q, a pointer to a non-q-qualified type may be converted to a pointer to the q-qualified
        ///    version of the type.
        ///    
        ///    c. An integer constant expression with value 0 can be converted to a null pointer to any type.
        ///    
        ///    d. A null pointer can be converted another null pointer to any type.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Expr ImplicitConvert(T target) {
            if (Type.Equals(target)) {
                return this;
            }
            if (Type.Compatible(target)) {
                return new Cast(target.nake, env, this);
            }
            if (Type.IsScalar && target.Kind == TKind.BOOL) {
                return new Cast(target.nake, env, this);
            }
            if (Type.IsArithmetic && target.IsArithmetic) {
                return new Cast(target.nake, env, this);
            }
            if (Type.IsPtr) {
                T e1 = (Type.nake as TPtr).element;
                if (target.IsPtr) {
                    T e2 = (target.nake as TPtr).element;
                    if (e1.IsVoid && !e2.IsFunc) {
                        return new Cast(target.nake, env, this);
                    }
                    if (e2.IsVoid && !e1.IsFunc) {
                        return new Cast(target.nake, env, this);
                    }
                    if ((e1.qualifiers | e2.qualifiers).Equals(e2.qualifiers)) {
                        return new Cast(target.nake, env, this);
                    }
                }
            }
            if ((IsNullPtr || IsConstZero) && target.IsPtr) {
                return new ConstNullPtr((target.nake as TPtr).element, env);
            }

            return null;
        }

        /// <summary>
        /// Performs all the three value transforms by explicitly using cast operator.
        /// Notice that these three implicit conversions will be perfromed automatically and
        /// there is only one possible conversion result.
        /// </summary>
        /// <returns></returns>
        public Expr ValueTransform() {
            return VTLValue().VTArr().VTFunc();
        }

        /// <summary>
        /// 1. An lvalue that does not have array type is converted to the value stored in the designated
        ///    object (and is no longer an lvalue).
        ///    If the lvalue has qualified type, the values has unqualified version of the type of the lvalue.
        ///    Otherwise, the value has the type of the lvalue.
        ///    EXCEPT: sizeof, unary &, ++, --, left operand of ., left operand of assignment operator.
        /// </summary>
        /// <returns></returns>
        public Expr VTLValue() {
            return (IsLValue && !type.IsArray) ? new Cast(type.nake, env, this) : this;
        }

        /// <summary>
        /// 2. An expression that has type "array of type" is converted to an expression with type "pointer to type"
        ///    that points to the initial element of the array object and is not an lvalue.
        ///    If the array object has register storage class, the behavior is undefined.
        ///    EXCEPT: sizeof, unary &, a string literal used to initialize an array.
        /// </summary>
        /// <returns></returns>
        public Expr VTArr() {
            return type.IsArray ? new Cast(new TPtr((type.nake as TArr).element), env, this) : this;
        }

        /// <summary>
        /// 3. A function designator is an expression that has function type. A function designator with type
        ///    "function returning type" is converted to an expression that has type "pointer to function returning type".
        ///    EXCEPT: sizeof, unary &.
        /// </summary>
        /// <returns></returns>
        public Expr VTFunc() {
            return type.IsFunc ? new Cast(new TPtr(type), env, this) : this;
        }

        public virtual X86Gen.Ret ToX86Expr(X86Gen gen) {
            throw new NotImplementedException();
        }

        public sealed override void ToX86(X86Gen gen) {
            ToX86Expr(gen);
        }

    }

    public sealed class CommaExpr : Expr {
        public readonly IEnumerable<Expr> exprs;
        public CommaExpr(T type, Env env, IEnumerable<Expr> exprs) : base(type, env) {
            this.exprs = exprs;
        }
    }

    public sealed class Assign : Expr {
        public readonly Expr lhs;
        public readonly Expr rhs;
        public readonly SyntaxTree.Assign.Op op;
        public Assign(T type, Env env, Expr lhs, Expr rhs, SyntaxTree.Assign.Op op) : base(type, env) {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }
    }

    public sealed class CondExpr : Expr {
        public readonly Expr p;
        public readonly Expr t;
        public readonly Expr f;
        public CondExpr(T type, Env env, Expr p, Expr t, Expr f) : base(type, env) {
            this.p = p;
            this.t = t;
            this.f = f;
        }
    }

    public sealed class BiExpr : Expr {
        public readonly Expr lhs;
        public readonly Expr rhs;
        public readonly SyntaxTree.BiExpr.Op op;
        public BiExpr(T type, Env env, Expr lhs, Expr rhs, SyntaxTree.BiExpr.Op op) : base(type, env) {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }
    }

    public sealed class Cast : Expr {
        public readonly Expr expr;
        public Cast(TUnqualified type, Env env, Expr expr) : base(type.None(), env) {
            this.expr = expr;
        }
    }

    public sealed class UnaryOp : Expr {
        public readonly Expr expr;
        public readonly SyntaxTree.UnaryOp.Op op;
        public UnaryOp(T type, Env env, Expr expr, SyntaxTree.UnaryOp.Op op) : base(type, env) {
            this.expr = expr;
            this.op = op;
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
        public PostStep(T type, Env env, Expr expr, SyntaxTree.PostStep.Kind kind) : base(type, env) {
            this.expr = expr;
            this.kind = kind;
        }
    }

    public sealed class FuncCall : Expr {
        public readonly Expr f;
        public readonly IEnumerable<Expr> args;
        public FuncCall(T type, Env env, Expr f, IEnumerable<Expr> args) : base(type, env) {
            this.f = f;
            this.args = args;
        }
    }

    public sealed class FuncDesignator : Expr {
        public readonly string symbol;
        public override bool IsLValue => true;
        public FuncDesignator(T type, Env env, string symbol) : base(type, env) {
            this.symbol = symbol;
        }
    }

    /// <summary>
    /// Represent an object.
    /// </summary>
    public sealed class DynamicObjExpr : Expr {
        public readonly string uid;
        public readonly string symbol;
        public override bool IsLValue => true;
        public DynamicObjExpr(T type, Env env, string uid, string symbol) : base(type, env) {
            this.uid = uid;
            this.symbol = symbol;
        }
        public override string ToString() {
            return symbol;
        }
        public override X86Gen.Ret ToX86Expr(X86Gen gen) {
            /// Get the offset to ebp.
            int ebp = env.GetEBP(uid);
            gen.Comment(X86Gen.Seg.TEXT, ToString());
            gen.Inst(X86Gen.lea, X86Gen.eax, new X86Gen.Address(X86Gen.ebp, ebp));
            return X86Gen.Ret.PTR;
        }
    }

    public abstract class ConstExpr : Expr {
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
        public override bool IsConstZero => value == 0;
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

    /// <summary>
    /// Null pointer constant.
    /// </summary>
    public sealed class ConstNullPtr : ConstExpr {
        public override bool IsNullPtr => false;
        public ConstNullPtr(T element, Env env) : base(new TPtr(element), env) { }
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
