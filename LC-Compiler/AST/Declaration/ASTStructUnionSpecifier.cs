using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {

    public sealed class ASTStructDeclarator : ASTNode {

        public ASTStructDeclarator(ASTDeclarator declarator, ASTExpr expr) {
            this.declarator = declarator;
            this.expr = expr;
        }

        public override int GetLine() {
            return declarator == null ? expr.GetLine() : declarator.GetLine();
        }

        public override bool Equals(object obj) {
            ASTStructDeclarator x = obj as ASTStructDeclarator;
            return x == null ? false : base.Equals(x)
                && (x.declarator == null ? declarator == null : x.declarator.Equals(declarator))
                && (x.expr == null ? expr == null : x.expr.Equals(expr));
        }

        public bool Equals(ASTStructDeclarator x) {
            return base.Equals(x)
                && (x.declarator == null ? declarator == null : x.declarator.Equals(declarator))
                && (x.expr == null ? expr == null : x.expr.Equals(expr));
        }

        public override int GetHashCode() {
            return GetLine();
        }

        public readonly ASTDeclarator declarator;
        public readonly ASTExpr expr;
    }

    public sealed class ASTStructDeclaration : ASTNode {

        public ASTStructDeclaration(
            IEnumerable<ASTTypeSpecifierQualifier> specifierQualifierList,
            IEnumerable<ASTStructDeclarator> declarators
            ) {
            this.specifierQualifierList = specifierQualifierList;
            this.declarators = declarators;
        }

        public override int GetLine() {
            return specifierQualifierList.First().GetLine();
        }

        public override bool Equals(object obj) {
            ASTStructDeclaration x = obj as ASTStructDeclaration;
            return x == null ? false : base.Equals(x)
                && x.specifierQualifierList.SequenceEqual(specifierQualifierList)
                && x.declarators.SequenceEqual(declarators);
        }

        public bool Equals(ASTStructDeclaration x) {
            return base.Equals(x)
                && x.specifierQualifierList.SequenceEqual(specifierQualifierList)
                && x.declarators.SequenceEqual(declarators);
        }

        public override int GetHashCode() {
            return specifierQualifierList.GetHashCode();
        }

        public readonly IEnumerable<ASTTypeSpecifierQualifier> specifierQualifierList;
        public readonly IEnumerable<ASTStructDeclarator> declarators;
    }

    public sealed class ASTStructUnionSpecifier : ASTTypeSpec {

        public ASTStructUnionSpecifier(
            int line,
            ASTIdentifier identifier,
            IEnumerable<ASTStructDeclaration> declarations,
            Kind kind
            ) : base(kind) {
            this.line = line;
            this.identifier = identifier;
            this.declarations = declarations;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTStructUnionSpecifier);
        }

        public bool Equals(ASTStructUnionSpecifier x) {
            return x != null && base.Equals(x)
                && NullableEquals(x.identifier, identifier)
                && NullableEquals(x.declarations, declarations);
        }

        public override int GetHashCode() {
            return GetLine();
        }

        public readonly int line;

        /// <summary>
        /// Nullable.
        /// </summary>
        public readonly ASTIdentifier identifier;
        /// <summary>
        /// Nullable.
        /// </summary>
        public readonly IEnumerable<ASTStructDeclaration> declarations;
    }
    
}
