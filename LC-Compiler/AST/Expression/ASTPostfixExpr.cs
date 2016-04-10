using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.Token;
using lcc.TypeSystem;

namespace lcc.AST {
    /// <summary>
    /// arr[idx].
    /// </summary>
    public sealed class ASTArrSub : ASTExpr, IEquatable<ASTArrSub> {

        public ASTArrSub(ASTExpr arr, ASTExpr idx) {
            this.arr = arr;
            this.idx = idx;
        }

        public override Position Pos => arr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTArrSub);
        }

        public bool Equals(ASTArrSub x) {
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
        public override T TypeCheck(ASTEnv env) {
            T arrType = arr.TypeCheck(env);
            if (!arrType.IsPointer && !arrType.IsArray) {
                throw new ASTException(arr.Pos, "subscripting none pointer type");
            }

            T idxType = idx.TypeCheck(env);
            if (!idxType.IsInteger) {
                throw new ASTException(idx.Pos, "subscripting none integer type");
            }

            if (arrType.IsPointer)
                return (arrType.nake as TPointer).element;
            else
                return (arrType.nake as TArray).element;
        }

        public readonly ASTExpr arr;
        public readonly ASTExpr idx;
    }

    /// <summary>
    /// s.i
    /// p->i
    /// </summary>
    public sealed class ASTAccess : ASTExpr, IEquatable<ASTAccess> {

        public enum Kind {
            DOT,
            PTR
        }

        public ASTAccess(ASTExpr aggregation, T_IDENTIFIER token, Kind type) {
            this.agg = aggregation;
            this.field = token.name;
            this.kind = type;
        }

        public override Position Pos => agg.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTAccess);
        }

        public bool Equals(ASTAccess x) {
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
        public override T TypeCheck(ASTEnv env) {
            T aggType = agg.TypeCheck(env);

            if (kind == Kind.PTR) {
                if (!aggType.IsPointer)
                    throw new ASTException(agg.Pos, "member reference base type is not a pointer");
                aggType = (aggType.nake as TPointer).element;
            }

            if (!aggType.IsStruct && !aggType.IsUnion)
                throw new ASTException(agg.Pos, "member reference base type is not a struct or union");

            TStructUnion s = aggType.nake as TStructUnion;

            T m = s.GetType(field);
            if (m == null) {
                throw new ASTException(agg.Pos, string.Format("no member named {0} in {1}", field, s.ToString()));
            }

            if (kind == Kind.DOT) {
                return aggType.Unnest(m, aggType.lr);
            } else {
                return aggType.Unnest(m, T.LR.L);
            }
        }

        public readonly ASTExpr agg;
        public readonly string field;
        public readonly Kind kind;
    }

    /// <summary>
    /// i++
    /// i--
    /// </summary>
    public sealed class ASTPostStep : ASTExpr, IEquatable<ASTPostStep> {

        public enum Kind {
            INC,
            DEC
        }

        public ASTPostStep(ASTExpr expr, Kind kind) {
            this.expr = expr;
            this.kind = kind;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTPostStep);
        }

        public bool Equals(ASTPostStep x) {
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
        public override T TypeCheck(ASTEnv env) {

            T t = expr.TypeCheck(env);
            if (!t.IsPointer && !t.IsArithmetic) {
                throw new ASTException(expr.Pos, string.Format("{0} on type {1}", kind, t));
            }

            if (!t.IsModifiable) {
                throw new ASTException(expr.Pos, string.Format("cannot assign to type {0}", t));
            }

            return t.R();
        }

        public readonly ASTExpr expr;
        public readonly Kind kind;
    }

    /// <summary>
    /// ( type-name ) { initializer-list [,] }
    /// </summary>
    public sealed class ASTCompound : ASTExpr, IEquatable<ASTCompound> {

        public ASTCompound(ASTTypeName name, IEnumerable<ASTInitItem> inits) {
            this.name = name;
            this.inits = inits;
        }

        public bool Equals(ASTCompound x) {
            return x != null && x.name.Equals(name) && x.inits.SequenceEqual(inits);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTCompound);
        }

        public override int GetHashCode() {
            return name.GetHashCode();
        }

        public override Position Pos => name.Pos;

        public readonly ASTTypeName name;
        public readonly IEnumerable<ASTInitItem> inits; 
    }

    public sealed class ASTFuncCall : ASTExpr, IEquatable<ASTFuncCall> {

        public ASTFuncCall(ASTExpr expr, IEnumerable<ASTExpr> args) {
            this.expr = expr;
            this.args = args;
        }

        public override Position Pos => expr.Pos;

        public bool Equals(ASTFuncCall x) {
            return x != null && x.expr.Equals(expr) && x.args.SequenceEqual(args);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTFuncCall);
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public readonly ASTExpr expr;
        public readonly IEnumerable<ASTExpr> args;
    }
}
