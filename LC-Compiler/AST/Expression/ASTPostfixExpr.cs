using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lcc.Token;

namespace lcc.AST {
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

        public readonly ASTExpr arr;
        public readonly ASTExpr idx;
    }

    public sealed class ASTAccess : ASTExpr {

        public enum Type {
            DOT,
            PTR
        }

        public ASTAccess(ASTExpr aggregation, T_IDENTIFIER token, Type type) {
            this.aggregation = aggregation;
            this.field = token.name;
            this.type = type;
        }

        public override int GetLine() {
            return aggregation.GetLine();
        }

        public override bool Equals(object obj) {
            ASTAccess x = obj as ASTAccess;
            return x == null ? false : base.Equals(x)
                && x.aggregation.Equals(aggregation)
                && x.field.Equals(field)
                && x.type == type;
        }

        public bool Equals(ASTAccess x) {
            return base.Equals(x)
                && x.aggregation.Equals(aggregation)
                && x.field.Equals(field)
                && x.type == type;
        }

        public override int GetHashCode() {
            return aggregation.GetHashCode() ^ field.GetHashCode();
        }

        public readonly ASTExpr aggregation;
        public readonly string field;
        public readonly Type type;
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
