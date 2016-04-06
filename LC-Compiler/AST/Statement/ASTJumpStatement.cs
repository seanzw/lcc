using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTContinue : ASTStmt, IEquatable<ASTContinue> {

        public ASTContinue(
            int line
            ) {
            this.pos = new Position { line = line };
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTContinue);
        }

        public bool Equals(ASTContinue x) {
            return x != null && x.pos.Equals(pos);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        private readonly Position pos;
    }

    public sealed class ASTBreak : ASTStmt, IEquatable<ASTBreak> {

        public ASTBreak(
            int line
            ) {
            this.pos = new Position { line = line };
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTBreak);
        }

        public bool Equals(ASTBreak x) {
            return x != null && x.pos.Equals(pos);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        private readonly Position pos;
    }

    public sealed class ASTReturn : ASTStmt, IEquatable<ASTReturn> {

        public ASTReturn(
            int line,
            ASTExpr expr
            ) {
            this.pos = new Position { line = line };
            this.expr = expr;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTReturn);
        }

        public bool Equals(ASTReturn x) {
            return x != null && x.pos.Equals(pos)
                && x.expr == null ? expr == null : x.expr.Equals(expr);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        private readonly Position pos;
        public readonly ASTExpr expr;
    }

    public sealed class ASTGoto : ASTStmt, IEquatable<ASTGoto> {

        public ASTGoto(
            int line,
            ASTId label
            ) {
            this.pos = new Position { line = line };
            this.label = label;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTGoto);
        }

        public bool Equals(ASTGoto x) {
            return x != null && x.pos.Equals(pos)
                && x.label.Equals(label);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        private readonly Position pos;
        public readonly ASTId label;
    }
}
