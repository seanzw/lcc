using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    /// <summary>
    /// ( type-name ) expr
    /// </summary>
    public sealed class ASTCast : ASTExpr, IEquatable<ASTCast> {

        public ASTCast(ASTTypeName name, ASTExpr expr) {
            this.name = name;
            this.expr = expr;
        }

        public override Position Pos => name.Pos;
        public bool Equals(ASTCast x) {
            return x != null && x.name.Equals(name) && x.expr.Equals(expr);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTCast);
        }

        public override int GetHashCode() {
            return name.GetHashCode() ^ expr.GetHashCode();
        }

        public readonly ASTTypeName name;
        public readonly ASTExpr expr;
    }
}
