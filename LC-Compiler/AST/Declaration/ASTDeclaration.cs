using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTDeclaration : ASTNode {

        public ASTDeclaration(
            LinkedList<ASTDeclarationSpecifier> specifiers,
            LinkedList<ASTDeclarator> declarators
            ) {
            this.specifiers = specifiers;
            this.declarators = declarators;
        }

        public override int GetLine() {
            return specifiers.First().GetLine();
        }

        public override bool Equals(object obj) {
            ASTDeclaration x = obj as ASTDeclaration;
            return x == null ? false : base.Equals(x)
                && x.specifiers.SequenceEqual(specifiers)
                && x.declarators.SequenceEqual(declarators);
        }

        public bool Equals(ASTDeclaration x) {
            return x == null ? false : base.Equals(x)
                && x.specifiers.SequenceEqual(specifiers)
                && x.declarators.SequenceEqual(declarators);
        }

        public override int GetHashCode() {
            return specifiers.GetHashCode();
        }

        public readonly LinkedList<ASTDeclarationSpecifier> specifiers;
        public readonly LinkedList<ASTDeclarator> declarators;
    }
}
