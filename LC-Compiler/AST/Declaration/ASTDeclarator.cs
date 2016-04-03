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

    public sealed class ASTPointer : ASTNode {

        public ASTPointer(int line, IEnumerable<ASTTypeQualifier> qualifiers) {
            this.line = line;
            this.qualifiers = qualifiers;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTPointer);
        }

        public bool Equals(ASTPointer x) {
            return x != null && x.line == line && x.qualifiers.SequenceEqual(qualifiers);
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly int line;
        public readonly IEnumerable<ASTTypeQualifier> qualifiers;
    }

    public sealed class ASTDeclarator : ASTNode {

        public ASTDeclarator(IEnumerable<ASTPointer> pointers, ASTDirDeclarator direct) {
            this.pointers = pointers;
            this.direct = direct;
        }

        public bool Equals(ASTDeclarator x) {
            return x != null && x.pointers.SequenceEqual(pointers) && x.direct.Equals(direct);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTDeclarator);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public override int GetLine() {
            return direct.GetLine();
        }

        public IEnumerable<ASTPointer> pointers;
        public ASTDirDeclarator direct;
    }

    public abstract class ASTDirDeclarator : ASTNode {}

    public sealed class ASTIdentifierDeclarator : ASTDirDeclarator {

        public ASTIdentifierDeclarator(ASTIdentifier identifier) {
            this.identifier = identifier;
        }

        public override int GetLine() {
            return identifier.GetLine();
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTIdentifierDeclarator);
        }

        public bool Equals(ASTIdentifierDeclarator i) {
            return i != null && i.identifier.Equals(identifier);
        }

        public override int GetHashCode() {
            return identifier.GetHashCode();
        }

        public readonly ASTIdentifier identifier;
    }

    public sealed class ASTParentDeclarator : ASTDirDeclarator {

        public ASTParentDeclarator(ASTDeclarator declarator) {
            this.declarator = declarator;
        }

        public bool Equals(ASTParentDeclarator x) {
            return x != null && x.declarator.Equals(declarator);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTParentDeclarator);
        }

        public override int GetHashCode() {
            return declarator.GetHashCode();
        }

        public override int GetLine() {
            return declarator.GetLine();
        }

        public readonly ASTDeclarator declarator;
    }

    public sealed class ASTParameter : ASTNode {

        public ASTParameter(IEnumerable<ASTDeclarationSpecifier> specifiers, ASTDeclarator declarator) {
            this.specifiers = specifiers;
            this.declarator = declarator;
        }

        public ASTParameter(IEnumerable<ASTDeclarationSpecifier> specifiers, ASTAbsDeclarator absDeclarator = null) {
            this.specifiers = specifiers;
            this.absDeclarator = absDeclarator;
        }

        public override int GetLine() {
            return specifiers.First().GetLine();
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTParameter);
        }

        public bool Equals(ASTParameter x) {
            return x != null && specifiers.SequenceEqual(x.specifiers) && NullableEquals(x.declarator, declarator)
                && NullableEquals(x.absDeclarator, absDeclarator);
        }

        public override int GetHashCode() {
            return specifiers.GetHashCode();
        }

        public readonly IEnumerable<ASTDeclarationSpecifier> specifiers;
        public readonly ASTDeclarator declarator;
        public readonly ASTAbsDeclarator absDeclarator;
    }

    public sealed class ASTFuncDeclarator : ASTDirDeclarator {

        public ASTFuncDeclarator(
            ASTDirDeclarator direct,
            IEnumerable<ASTParameter> parameters,
            bool isEllipis
            ) {
            this.direct = direct;
            this.parameters = parameters;
            this.isEllipis = isEllipis;
        }

        public ASTFuncDeclarator(ASTDirDeclarator direct, IEnumerable<ASTIdentifier> identifiers) {
            this.direct = direct;
            this.identifiers = identifiers;
        }

        public override int GetLine() {
            return direct.GetLine();
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTFuncDeclarator);
        }

        public bool Equals(ASTFuncDeclarator x) {
            return x != null && x.direct.Equals(direct)
                && NullableEquals(x.parameters, parameters)
                && x.isEllipis == isEllipis
                && NullableEquals(x.identifiers, identifiers);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public readonly ASTDirDeclarator direct;

        // ( parameter-type-list )
        public readonly IEnumerable<ASTParameter> parameters;
        public readonly bool isEllipis;

        // ( identifier-list_opt )
        public readonly IEnumerable<ASTIdentifier> identifiers;
    }

    public sealed class ASTArrDeclarator : ASTDirDeclarator {

        public ASTArrDeclarator(
            ASTDirDeclarator direct,
            IEnumerable<ASTTypeQualifier> qualifiers,
            ASTExpr expr,
            bool isStatic
            ) {
            this.direct = direct;
            this.qualifiers = qualifiers;
            this.expr = expr;
            this.isStatic = isStatic;
            this.isStar = false;
        }

        public ASTArrDeclarator(ASTDirDeclarator direct, IEnumerable<ASTTypeQualifier> qualifiers) {
            this.direct = direct;
            this.qualifiers = qualifiers;
            this.expr = null;
            this.isStar = true;
            this.isStatic = false;
        }

        public bool Equals(ASTArrDeclarator x) {
            return x != null && x.direct.Equals(direct)
                && x.qualifiers.SequenceEqual(qualifiers)
                && x.isStatic == isStatic && x.isStar == isStar
                && NullableEquals(x.expr, expr);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTArrDeclarator);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public override int GetLine() {
            return direct.GetLine();
        }

        public readonly ASTDirDeclarator direct;

        public readonly ASTExpr expr;
        public readonly IEnumerable<ASTTypeQualifier> qualifiers;
        public readonly bool isStatic;
        public readonly bool isStar;
    }

    public sealed class ASTAbsDeclarator : ASTNode {

        public ASTAbsDeclarator(IEnumerable<ASTPointer> pointers, ASTAbsDirDeclarator direct = null) {
            this.pointers = pointers;
            this.direct = direct;
        }

        public bool Equals(ASTAbsDeclarator x) {
            return x != null && x.pointers.SequenceEqual(pointers) && NullableEquals(x.direct, direct);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTAbsDeclarator);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public override int GetLine() {
            return direct.GetLine();
        }

        public IEnumerable<ASTPointer> pointers;
        public ASTAbsDirDeclarator direct;
    }

    public abstract class ASTAbsDirDeclarator : ASTNode { }

    public sealed class ASTAbsParentDeclarator : ASTAbsDirDeclarator {

        public ASTAbsParentDeclarator(ASTAbsDeclarator declarator) {
            this.declarator = declarator;
        }

        public bool Equals(ASTAbsParentDeclarator x) {
            return x != null && x.declarator.Equals(declarator);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTAbsParentDeclarator);
        }

        public override int GetHashCode() {
            return declarator.GetHashCode();
        }

        public override int GetLine() {
            return declarator.GetLine();
        }

        public readonly ASTAbsDeclarator declarator;
    }

    public sealed class ASTAbsArrDeclarator : ASTAbsDirDeclarator {

        public ASTAbsArrDeclarator(
            ASTAbsDirDeclarator direct,
            IEnumerable<ASTTypeQualifier> qualifiers,
            ASTExpr expr,
            bool isStatic
            ) {
            this.direct = direct;
            this.qualifiers = qualifiers;
            this.expr = expr;
            this.isStatic = isStatic;
            this.isStar = false;
        }

        public ASTAbsArrDeclarator(ASTAbsDirDeclarator direct) {
            this.direct = direct;
            this.qualifiers = new List<ASTTypeQualifier>();
            this.expr = null;
            this.isStar = true;
            this.isStatic = false;
        }

        public bool Equals(ASTAbsArrDeclarator x) {
            return x != null && NullableEquals(x.direct, direct)
                && x.qualifiers.SequenceEqual(qualifiers)
                && x.isStatic == isStatic && x.isStar == isStar
                && NullableEquals(x.expr, expr);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTAbsArrDeclarator);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public override int GetLine() {
            return direct.GetLine();
        }

        public readonly ASTAbsDirDeclarator direct;

        public readonly ASTExpr expr;
        public readonly IEnumerable<ASTTypeQualifier> qualifiers;
        public readonly bool isStatic;
        public readonly bool isStar;
    }

    public sealed class ASTAbsFuncDeclarator : ASTAbsDirDeclarator {

        public ASTAbsFuncDeclarator(
            ASTAbsDirDeclarator direct,
            IEnumerable<ASTParameter> parameters,
            bool isEllipis
            ) {
            this.direct = direct;
            this.parameters = parameters;
            this.isEllipis = isEllipis;
        }

        public override int GetLine() {
            return direct.GetLine();
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTAbsFuncDeclarator);
        }

        public bool Equals(ASTAbsFuncDeclarator x) {
            return x != null && NullableEquals(x.direct, direct)
                && NullableEquals(x.parameters, parameters)
                && x.isEllipis == isEllipis;
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public readonly ASTAbsDirDeclarator direct;

        // ( parameter-type-list )
        public readonly IEnumerable<ASTParameter> parameters;
        public readonly bool isEllipis;
    }

    public sealed class ASTTypeName : ASTNode {

        public ASTTypeName(IEnumerable<ASTTypeSpecifierQualifier> specifiers, ASTAbsDeclarator declarator = null) {
            this.specifiers = specifiers;
            this.declarator = declarator;
        }

        public bool Equals(ASTTypeName x) {
            return x != null && x.specifiers.SequenceEqual(specifiers) && NullableEquals(declarator, x.declarator);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTTypeName);
        }

        public override int GetHashCode() {
            return specifiers.First().GetHashCode();
        }

        public override int GetLine() {
            return specifiers.First().GetLine();
        }

        public readonly IEnumerable<ASTTypeSpecifierQualifier> specifiers;
        public readonly ASTAbsDeclarator declarator;
    }
}
