using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTDeclaration : ASTStatement {

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

        public Type.Type TypeCheck() {

            // Process the specifier list.
            var typeSpecifiers = 
                from s in specifiers
                where s is ASTTypeSpecifier
                select s as ASTTypeSpecifier;

            var storageSpecifiers =
                from s in specifiers
                where s is ASTStorageSpecifier
                select s as ASTStorageSpecifier;

            var typeQualifiers =
                from s in specifiers
                where s is ASTTypeQualifier
                select s as ASTTypeQualifier;

            var funcSpecifiers =
                from s in specifiers
                where s is ASTFunctionSpecifier
                select s as ASTFunctionSpecifier;

            return new Type.TypeError("Unimplemented");
        }
    }
}
