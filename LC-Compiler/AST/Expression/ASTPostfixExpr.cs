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
    public sealed class ASTArrSub : ASTExpr {

        public ASTArrSub(ASTExpr arr, ASTExpr idx) {
            this.arr = arr;
            this.idx = idx;
        }

        public override int GetLine() {
            return arr.GetLine();
        }

        public override bool Equals(object obj) {
            ASTArrSub x = obj as ASTArrSub;
            return x == null ? false : base.Equals(x)
                && x.arr.Equals(arr)
                && x.idx.Equals(idx);
        }

        public bool Equals(ASTArrSub x) {
            return base.Equals(x)
                && x.arr.Equals(arr)
                && x.idx.Equals(idx);
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
                throw new ASTException(arr.GetLine(), "subscripting none pointer type");
            }

            T idxType = idx.TypeCheck(env);
            if (!idxType.IsInteger) {
                throw new ASTException(idx.GetLine(), "subscripting none integer type");
            }

            if (arrType.IsPointer)
                return (arrType.baseType as TPointer).element;
            else
                return (arrType.baseType as TArray).element;
        }

        public readonly ASTExpr arr;
        public readonly ASTExpr idx;
    }

    /// <summary>
    /// s.i
    /// p->i
    /// </summary>
    public sealed class ASTAccess : ASTExpr {

        public enum Kind {
            DOT,
            PTR
        }

        public ASTAccess(ASTExpr aggregation, T_IDENTIFIER token, Kind type) {
            this.agg = aggregation;
            this.field = token.name;
            this.kind = type;
        }

        public override int GetLine() {
            return agg.GetLine();
        }

        public override bool Equals(object obj) {
            ASTAccess x = obj as ASTAccess;
            return x == null ? false : base.Equals(x)
                && x.agg.Equals(agg)
                && x.field.Equals(field)
                && x.kind == kind;
        }

        public bool Equals(ASTAccess x) {
            return base.Equals(x)
                && x.agg.Equals(agg)
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
                    throw new ASTException(agg.GetLine(), "member reference base type is not a pointer");
                aggType = (aggType.baseType as TPointer).element;
            }

            if (!aggType.IsStructUnion)
                throw new ASTException(agg.GetLine(), "member reference base type is not a struct or union");

            TStructUnion s = aggType.baseType as TStructUnion;

            T m = s.GetType(field);
            if (m == null) {
                throw new ASTException(agg.GetLine(), string.Format("no member named {0} in {1}", field, s.ToString()));
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
    public sealed class ASTPostStep : ASTExpr {

        public enum Kind {
            INC,
            DEC
        }

        public ASTPostStep(ASTExpr expr, Kind kind) {
            this.expr = expr;
            this.kind = kind;
        }

        public override int GetLine() {
            return expr.GetLine();
        }

        public override bool Equals(object obj) {
            ASTPostStep x = obj as ASTPostStep;
            return x == null ? false : base.Equals(x)
                && x.expr.Equals(expr)
                && x.kind == kind;
        }

        public bool Equals(ASTPostStep x) {
            return base.Equals(x)
                && x.expr.Equals(expr)
                && x.kind == kind;
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
                throw new ASTException(expr.GetLine(), string.Format("{0} on type {1}", kind, t));
            }

            if (!t.IsModifiable) {
                throw new ASTException(expr.GetLine(), string.Format("cannot assign to type {0}", t));
            }

            return t.R();
        }

        public readonly ASTExpr expr;
        public readonly Kind kind;
    }
}
