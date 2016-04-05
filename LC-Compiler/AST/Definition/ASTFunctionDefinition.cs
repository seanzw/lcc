using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTFuncDefinition : ASTNode {

        public ASTFuncDefinition(
            ASTDeclSpecs specifiers,
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
            ASTFuncDefinition x = obj as ASTFuncDefinition;
            return Equals(x);
        }

        public bool Equals(ASTFuncDefinition x) {
            return x == null ? false : base.Equals(x)
                && x.specifiers.Equals(specifiers)
                && x.declarator.Equals(declarator)
                && x.declarations == null ? declarations == null : x.declarations.SequenceEqual(declarations)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return specifiers.GetHashCode();
        }

        public readonly ASTDeclSpecs specifiers;
        public readonly ASTDeclarator declarator;
        public readonly IEnumerable<ASTDeclaration> declarations;
        public readonly ASTStatement statement;
    }
}
