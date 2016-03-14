using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public abstract class ASTDeclarator : ASTNode {

        public override bool Equals(object obj) {
            return obj is ASTDeclarator;
        }

        public bool Equals(ASTDeclarator i) {
            return true;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }

    public sealed class ASTDeclaratorIdentifier : ASTDeclarator {

        public ASTDeclaratorIdentifier(ASTIdentifier identifier) {
            this.identifier = identifier;
        }

        public override int GetLine() {
            return identifier.GetLine();
        }

        public override bool Equals(object obj) {
            ASTDeclaratorIdentifier i = obj as ASTDeclaratorIdentifier;
            return i == null ? false : base.Equals(i)
                && i.identifier.Equals(identifier);
        }

        public bool Equals(ASTDeclaratorIdentifier i) {
            return i == null ? false : base.Equals(i)
                && i.identifier.Equals(identifier);
        }

        public override int GetHashCode() {
            return identifier.GetHashCode();
        }

        public readonly ASTIdentifier identifier;
    }
}
