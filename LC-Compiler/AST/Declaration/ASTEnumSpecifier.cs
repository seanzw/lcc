using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.Token;

namespace lcc.AST {

    public sealed class ASTEnumerator : ASTNode {

        public ASTEnumerator(ASTIdentifier identifier, ASTExpr expr) {
            this.identifier = identifier;
            this.expr = expr;
        }

        public override int GetLine() {
            return identifier.GetLine();
        }

        public override bool Equals(object obj) {
            ASTEnumerator enumerator = obj as ASTEnumerator;
            return enumerator == null ? false : base.Equals(enumerator)
                && enumerator.identifier.Equals(identifier)
                && (enumerator.expr == null ? expr == null : enumerator.expr.Equals(expr));
        }

        public bool Equals(ASTEnumerator enumerator) {
            return base.Equals(enumerator)
                && enumerator.identifier.Equals(identifier)
                && enumerator.expr.Equals(expr);
        }

        public override int GetHashCode() {
            return identifier.GetHashCode();
        }

        public readonly ASTIdentifier identifier;
        public readonly ASTExpr expr;
    }

    public sealed class ASTEnumSpecifier : ASTTypeSpecifier {

        public ASTEnumSpecifier(int line, ASTIdentifier identifier, LinkedList<ASTEnumerator> enumerators)
            : base(Type.ENUM) {
            this.line = line;
            this.identifier = identifier;
            this.enumerators = enumerators;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTEnumSpecifier e = obj as ASTEnumSpecifier;
            return e == null ? false : base.Equals(e)
                && (e.identifier == null ? identifier == null : e.identifier.Equals(identifier))
                && (e.enumerators == null ? enumerators == null : e.enumerators.SequenceEqual(enumerators));
        }

        public bool Equals(ASTEnumSpecifier e) {
            return base.Equals(e)
                && (e.identifier == null ? identifier == null : e.identifier.Equals(identifier))
                && (e.enumerators == null ? enumerators == null : e.enumerators.SequenceEqual(enumerators));
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly int line;
        public readonly ASTIdentifier identifier;
        public readonly LinkedList<ASTEnumerator> enumerators;
    }
}
