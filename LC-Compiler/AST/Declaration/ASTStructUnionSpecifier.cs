using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {

    public sealed class ASTStructDecl : ASTNode, IEquatable<ASTStructDecl> {

        public ASTStructDecl(ASTDecl declarator, ASTExpr expr) {
            this.declarator = declarator;
            this.expr = expr;
        }

        public override Position Pos => declarator == null ? expr.Pos : declarator.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTStructDecl);
        }

        public bool Equals(ASTStructDecl x) {
            return x != null
                && NullableEquals(x.declarator, declarator)
                && NullableEquals(x.expr, expr);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public readonly ASTDecl declarator;
        public readonly ASTExpr expr;
    }

    public sealed class ASTStructDeclaration : ASTNode, IEquatable<ASTStructDeclaration> {

        public ASTStructDeclaration(
            IEnumerable<ASTTypeSpecQual> specifierQualifierList,
            IEnumerable<ASTStructDecl> declarators
            ) {
            this.specifierQualifierList = specifierQualifierList;
            this.declarators = declarators;
        }

        public override Position Pos => specifierQualifierList.First().Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTStructDeclaration);
        }

        public bool Equals(ASTStructDeclaration x) {
            return x != null
                && x.specifierQualifierList.SequenceEqual(specifierQualifierList)
                && x.declarators.SequenceEqual(declarators);
        }

        public override int GetHashCode() {
            return specifierQualifierList.GetHashCode();
        }

        public readonly IEnumerable<ASTTypeSpecQual> specifierQualifierList;
        public readonly IEnumerable<ASTStructDecl> declarators;
    }

    public sealed class ASTStructUnionSpec : ASTTypeSpec, IEquatable<ASTStructUnionSpec> {

        public ASTStructUnionSpec(
            int line,
            ASTId identifier,
            IEnumerable<ASTStructDeclaration> declarations,
            Kind kind
            ) : base(kind) {
            this.pos = new Position { line = line };
            this.identifier = identifier;
            this.declarations = declarations;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTStructUnionSpec);
        }

        public bool Equals(ASTStructUnionSpec x) {
            return x != null
                && NullableEquals(x.identifier, identifier)
                && NullableEquals(x.declarations, declarations);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        private readonly Position pos;

        /// <summary>
        /// Nullable.
        /// </summary>
        public readonly ASTId identifier;
        /// <summary>
        /// Nullable.
        /// </summary>
        public readonly IEnumerable<ASTStructDeclaration> declarations;
    }
    
}
