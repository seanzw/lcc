using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.Token;

namespace lcc.AST {

    public sealed class ASTDeclEnumerator : ASTNode {

        public ASTDeclEnumerator(ASTIdentifier identifier, ASTExpr expr = null) {
            this.identifier = identifier;
            this.expr = expr;
        }

        public override int GetLine() {
            return identifier.GetLine();
        }

        public override bool Equals(object obj) {
            ASTDeclEnumerator enumerator = obj as ASTDeclEnumerator;
            return enumerator == null ? false : base.Equals(enumerator)
                && enumerator.identifier.Equals(identifier)
                && (enumerator.expr == null ? expr == null : enumerator.expr.Equals(expr));
        }

        public bool Equals(ASTDeclEnumerator enumerator) {
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

    public sealed class ASTDeclEnumSpec : ASTDeclTypeSpec {

        public ASTDeclEnumSpec(ASTIdentifier identifier, LinkedList<ASTDeclEnumerator> enumerators = null) {
            this.identifier = identifier;
            this.enumerators = enumerators;
        }

        public ASTDeclEnumSpec(LinkedList<ASTDeclEnumerator> enumerators) {
            this.identifier = null;
            this.enumerators = enumerators;
        }

        public override int GetLine() {
            return identifier.GetLine();
        }

        public override bool Equals(object obj) {
            ASTDeclEnumSpec e = obj as ASTDeclEnumSpec;
            return e == null ? false : base.Equals(e)
                && (e.identifier == null ? identifier == null : e.identifier.Equals(identifier))
                && (e.enumerators == null ? enumerators == null : e.enumerators.SequenceEqual(enumerators));
        }

        public bool Equals(ASTDeclEnumSpec e) {
            return base.Equals(e)
                && e.identifier.Equals(identifier)
                && e.enumerators.SequenceEqual(enumerators);
        }

        public override int GetHashCode() {
            return identifier.GetHashCode() | enumerators.GetHashCode();
        }

        public readonly ASTIdentifier identifier;
        public readonly LinkedList<ASTDeclEnumerator> enumerators;
    }
}
