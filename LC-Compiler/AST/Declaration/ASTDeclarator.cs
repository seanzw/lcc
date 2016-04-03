using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {

    public sealed class ASTInitDeclarator : ASTNode {

        public ASTInitDeclarator(ASTDeclarator declarator, ASTInitializer initializer = null) {
            this.declarator = declarator;
            this.initializer = initializer;
        }

        public bool Equals(ASTInitDeclarator x) {
            return x != null && x.declarator.Equals(declarator) && NullableEquals(x.initializer, initializer);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTInitDeclarator);
        }

        public override int GetHashCode() {
            return declarator.GetHashCode();
        }

        public override int GetLine() {
            return declarator.GetLine();
        }

        public readonly ASTDeclarator declarator;
        public readonly ASTInitializer initializer;
    }

    public abstract class ASTDeclarator : ASTNode { }

    public sealed class ASTDeclaratorIdentifier : ASTDeclarator {

        public ASTDeclaratorIdentifier(ASTIdentifier identifier) {
            this.identifier = identifier;
        }

        public override int GetLine() {
            return identifier.GetLine();
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTDeclaratorIdentifier);
        }

        public bool Equals(ASTDeclaratorIdentifier i) {
            return i != null && i.identifier.Equals(identifier);
        }

        public override int GetHashCode() {
            return identifier.GetHashCode();
        }

        public readonly ASTIdentifier identifier;
    }
}
