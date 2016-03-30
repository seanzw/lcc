using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lcc.Token;
using lcc.Type;

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
        public override Type.Type TypeCheck(ASTEnv env) {
            Type.Type arrType = arr.TypeCheck(env);
            if (!arrType.IsPointer) {
                throw new ASTException(arr.GetLine(), "subscripting none pointer type");
            }

            Type.Type idxType = idx.TypeCheck(env);
            if (!idxType.IsInteger) {
                throw new ASTException(idx.GetLine(), "subscripting none integer type");
            }

            return (arrType.baseType as TypePointer).element;
        }

        public readonly ASTExpr arr;
        public readonly ASTExpr idx;
    }

    public sealed class ASTAccess : ASTExpr {

        public enum Kind {
            DOT,
            PTR
        }

        public ASTAccess(ASTExpr aggregation, T_IDENTIFIER token, Kind type) {
            this.aggregation = aggregation;
            this.field = token.name;
            this.kind = type;
        }

        public override int GetLine() {
            return aggregation.GetLine();
        }

        public override bool Equals(object obj) {
            ASTAccess x = obj as ASTAccess;
            return x == null ? false : base.Equals(x)
                && x.aggregation.Equals(aggregation)
                && x.field.Equals(field)
                && x.kind == kind;
        }

        public bool Equals(ASTAccess x) {
            return base.Equals(x)
                && x.aggregation.Equals(aggregation)
                && x.field.Equals(field)
                && x.kind == kind;
        }

        public override int GetHashCode() {
            return aggregation.GetHashCode() ^ field.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        //public override Type.Type TypeCheck(ASTEnv env) {
            
        //}

        public readonly ASTExpr aggregation;
        public readonly string field;
        public readonly Kind kind;
    }

    public sealed class ASTPostStep : ASTExpr {

        public enum Type {
            INC,
            DEC
        }

        public ASTPostStep(ASTExpr expr, Type type) {
            this.expr = expr;
            this.type = type;
        }

        public override int GetLine() {
            return expr.GetLine();
        }

        public override bool Equals(object obj) {
            ASTPostStep x = obj as ASTPostStep;
            return x == null ? false : base.Equals(x)
                && x.expr.Equals(expr)
                && x.type == type;
        }

        public bool Equals(ASTPostStep x) {
            return base.Equals(x)
                && x.expr.Equals(expr)
                && x.type == type;
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public readonly ASTExpr expr;
        public readonly Type type;
    }
}
