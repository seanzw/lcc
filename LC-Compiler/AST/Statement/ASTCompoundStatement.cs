using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTCompoundStmt : ASTStmt, IEquatable<ASTCompoundStmt> {

        public ASTCompoundStmt(IEnumerable<ASTStmt> stmts) {
            this.stmts = stmts;
        }

        public override Position Pos => stmts.First().Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTCompoundStmt);
        }

        public bool Equals(ASTCompoundStmt x) {
            return x != null && x.stmts.SequenceEqual(stmts);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public readonly IEnumerable<ASTStmt> stmts;
    }
}
