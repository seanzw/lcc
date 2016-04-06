using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTFuncDef : ASTNode, IEquatable<ASTFuncDef> {

        public ASTFuncDef(
            ASTDeclSpecs specifiers,
            ASTDecl declarator,
            IEnumerable<ASTDeclaration> declarations,
            ASTStmt statement
            ) {
            this.specifiers = specifiers;
            this.declarator = declarator;
            this.declarations = declarations;
            this.statement = statement;
        }

        public override Position Pos => declarator.Pos;

        public override bool Equals(object obj) {
            ASTFuncDef x = obj as ASTFuncDef;
            return Equals(x);
        }

        public bool Equals(ASTFuncDef x) {
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
        public readonly ASTDecl declarator;
        public readonly IEnumerable<ASTDeclaration> declarations;
        public readonly ASTStmt statement;
    }
}
