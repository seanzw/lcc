using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTFunctionDefinition : ASTNode {

        public ASTFunctionDefinition(
            IEnumerable<ASTDeclarationSpecifier> specifiers,
            ASTDeclarator declarator,
            IEnumerable<ASTDeclaration> declarations,
            ASTStatement statement
            ) {
            this.specifiers = specifiers;
            this.declarator = declarator;
            this.declarations = declarations;
            this.statement = statement;
        }

        public override int GetLine() {
            return declarator.GetLine();
        }

        public override bool Equals(object obj) {
            ASTFunctionDefinition x = obj as ASTFunctionDefinition;
            return Equals(x);
        }

        public bool Equals(ASTFunctionDefinition x) {
            return x == null ? false : base.Equals(x)
                && x.specifiers.SequenceEqual(specifiers)
                && x.declarator.Equals(declarator)
                && x.declarations == null ? declarations == null : x.declarations.SequenceEqual(declarations)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return specifiers.GetHashCode();
        }

        public readonly IEnumerable<ASTDeclarationSpecifier> specifiers;
        public readonly ASTDeclarator declarator;
        public readonly IEnumerable<ASTDeclaration> declarations;
        public readonly ASTStatement statement;
    }
}
