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
            LinkedList<ASTTypeSpecifierQualifier> specifierQualifierList,
            LinkedList<ASTStructDeclarator> declarators
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

        public readonly LinkedList<ASTTypeSpecifierQualifier> specifierQualifierList;
        public readonly LinkedList<ASTStructDeclarator> declarators;
    }

    public abstract class ASTStructUnionSpecifier : ASTTypeSpecifier {

        public ASTStructUnionSpecifier(
            int line,
            ASTIdentifier identifier,
            LinkedList<ASTStructDeclaration> declarations,
            Type type
            ) : base(type) {
            this.line = line;
            this.identifier = identifier;
            this.declarations = declarations;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTStructUnionSpecifier x = obj as ASTStructUnionSpecifier;
            return x == null ? false : base.Equals(x)
                && (x.identifier == null ? identifier == null : x.identifier.Equals(identifier))
                && (x.declarations == null ? declarations == null : x.declarations.SequenceEqual(declarations));
        }

        public bool Equals(ASTStructUnionSpecifier x) {
            return base.Equals(x)
                && (x.identifier == null ? identifier == null : x.identifier.Equals(identifier))
                && (x.declarations == null ? declarations == null : x.declarations.SequenceEqual(declarations));
        }

        public override int GetHashCode() {
            return GetLine();
        }

        public readonly int line;
        public readonly ASTIdentifier identifier;
        public readonly LinkedList<ASTStructDeclaration> declarations;
    }

    public sealed class ASTStructSpecifier : ASTStructUnionSpecifier {

        public ASTStructSpecifier(
            int line,
            ASTIdentifier identifier,
            LinkedList<ASTStructDeclaration> declarations = null
            ) : base(line, identifier, declarations, Type.STRUCT) { }

    }

    public sealed class ASTUnionSpecifier : ASTStructUnionSpecifier {

        public ASTUnionSpecifier(
            int line,
            ASTIdentifier identifier,
            LinkedList<ASTStructDeclaration> declarations = null
            ) : base(line, identifier, declarations, Type.UNION) { }

    }
}
