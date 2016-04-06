using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {

    public sealed class ASTInitDecl : ASTNode, IEquatable<ASTInitDecl> {

        public ASTInitDecl(ASTDecl declarator, ASTInitializer initializer = null) {
            this.declarator = declarator;
            this.initializer = initializer;
        }

        public bool Equals(ASTInitDecl x) {
            return x != null && x.declarator.Equals(declarator) && NullableEquals(x.initializer, initializer);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTInitDecl);
        }

        public override int GetHashCode() {
            return declarator.GetHashCode();
        }

        public override Position Pos => declarator.Pos;

        public readonly ASTDecl declarator;
        public readonly ASTInitializer initializer;
    }

    public sealed class ASTPtr : ASTNode, IEquatable<ASTPtr> {

        public ASTPtr(int line, IEnumerable<ASTTypeQual> qualifiers) {
            this.pos = new Position { line = line };
            this.qualifiers = qualifiers;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTPtr);
        }

        public bool Equals(ASTPtr x) {
            return x != null && x.pos.Equals(pos) && x.qualifiers.SequenceEqual(qualifiers);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        private readonly Position pos;
        public readonly IEnumerable<ASTTypeQual> qualifiers;
    }

    public sealed class ASTDecl : ASTNode, IEquatable<ASTDecl> {

        public ASTDecl(IEnumerable<ASTPtr> pointers, ASTDirDecl direct) {
            this.pointers = pointers;
            this.direct = direct;
        }

        public bool Equals(ASTDecl x) {
            return x != null && x.pointers.SequenceEqual(pointers) && x.direct.Equals(direct);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTDecl);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public override Position Pos => direct.Pos;

        public IEnumerable<ASTPtr> pointers;
        public ASTDirDecl direct;
    }

    public abstract class ASTDirDecl : ASTNode {
        public abstract string Name { get; }
    }

    public sealed class ASTIdDecl : ASTDirDecl, IEquatable<ASTIdDecl> {

        public ASTIdDecl(ASTId id) {
            this.id = id;
        }

        public override string Name => id.name;
        public override Position Pos => id.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTIdDecl);
        }

        public bool Equals(ASTIdDecl i) {
            return i != null && i.id.Equals(id);
        }

        public override int GetHashCode() {
            return id.GetHashCode();
        }

        public readonly ASTId id;
    }

    public sealed class ASTParDecl : ASTDirDecl, IEquatable<ASTParDecl> {

        public ASTParDecl(ASTDecl declarator) {
            this.declarator = declarator;
        }

        public override string Name => declarator.direct.Name;
        public override Position Pos => declarator.Pos;

        public bool Equals(ASTParDecl x) {
            return x != null && x.declarator.Equals(declarator);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTParDecl);
        }

        public override int GetHashCode() {
            return declarator.GetHashCode();
        }

        public readonly ASTDecl declarator;
    }

    public sealed class ASTParam : ASTNode, IEquatable<ASTParam> {

        public ASTParam(ASTDeclSpecs specifiers, ASTDecl declarator) {
            this.specifiers = specifiers;
            this.declarator = declarator;
        }

        public ASTParam(ASTDeclSpecs specifiers, ASTAbsDecl absDeclarator = null) {
            this.specifiers = specifiers;
            this.absDeclarator = absDeclarator;
        }

        public override Position Pos => specifiers.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTParam);
        }

        public bool Equals(ASTParam x) {
            return x != null && specifiers.Equals(x.specifiers) && NullableEquals(x.declarator, declarator)
                && NullableEquals(x.absDeclarator, absDeclarator);
        }

        public override int GetHashCode() {
            return specifiers.GetHashCode();
        }

        public readonly ASTDeclSpecs specifiers;
        public readonly ASTDecl declarator;
        public readonly ASTAbsDecl absDeclarator;
    }

    public sealed class ASTFuncDecl : ASTDirDecl, IEquatable<ASTFuncDecl> {

        public ASTFuncDecl(
            ASTDirDecl direct,
            IEnumerable<ASTParam> parameters,
            bool isEllipis
            ) {
            this.direct = direct;
            this.parameters = parameters;
            this.isEllipis = isEllipis;
        }

        public ASTFuncDecl(ASTDirDecl direct, IEnumerable<ASTId> identifiers) {
            this.direct = direct;
            this.identifiers = identifiers;
        }

        public override string Name => direct.Name;

        public override Position Pos => direct.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTFuncDecl);
        }

        public bool Equals(ASTFuncDecl x) {
            return x != null && x.direct.Equals(direct)
                && NullableEquals(x.parameters, parameters)
                && x.isEllipis == isEllipis
                && NullableEquals(x.identifiers, identifiers);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public readonly ASTDirDecl direct;

        // ( parameter-type-list )
        public readonly IEnumerable<ASTParam> parameters;
        public readonly bool isEllipis;

        // ( identifier-list_opt )
        public readonly IEnumerable<ASTId> identifiers;
    }

    public sealed class ASTArrDecl : ASTDirDecl, IEquatable<ASTArrDecl> {

        public ASTArrDecl(
            ASTDirDecl direct,
            IEnumerable<ASTTypeQual> qualifiers,
            ASTExpr expr,
            bool isStatic
            ) {
            this.direct = direct;
            this.qualifiers = qualifiers;
            this.expr = expr;
            this.isStatic = isStatic;
            this.isStar = false;
        }

        public ASTArrDecl(ASTDirDecl direct, IEnumerable<ASTTypeQual> qualifiers) {
            this.direct = direct;
            this.qualifiers = qualifiers;
            this.expr = null;
            this.isStar = true;
            this.isStatic = false;
        }

        public override string Name => direct.Name;

        public bool Equals(ASTArrDecl x) {
            return x != null && x.direct.Equals(direct)
                && x.qualifiers.SequenceEqual(qualifiers)
                && x.isStatic == isStatic && x.isStar == isStar
                && NullableEquals(x.expr, expr);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTArrDecl);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public override Position Pos => direct.Pos;

        public readonly ASTDirDecl direct;

        public readonly ASTExpr expr;
        public readonly IEnumerable<ASTTypeQual> qualifiers;
        public readonly bool isStatic;
        public readonly bool isStar;
    }

    public sealed class ASTAbsDecl : ASTNode, IEquatable<ASTAbsDecl> {

        public ASTAbsDecl(IEnumerable<ASTPtr> pointers, ASTAbsDirDecl direct) {
            this.pointers = pointers;
            this.direct = direct;
        }

        public bool Equals(ASTAbsDecl x) {
            return x != null && x.pointers.SequenceEqual(pointers) && NullableEquals(x.direct, direct);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTAbsDecl);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override Position Pos => direct == null ? pointers.First().Pos : direct.Pos;

        public IEnumerable<ASTPtr> pointers;

        /// <summary>
        /// Nullable.
        /// </summary>
        public ASTAbsDirDecl direct;
    }

    public abstract class ASTAbsDirDecl : ASTNode {}

    public sealed class ASTAbsDirDeclNil : ASTAbsDirDecl {
        public ASTAbsDirDeclNil(int line) {
            pos = new Position { line = line };
        }

        public bool Equals(ASTAbsDirDeclNil x) {
            return x != null && x.pos.Equals(pos);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTAbsDirDeclNil);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override Position Pos => pos;
        private readonly Position pos;
    }

    public sealed class ASTAbsParDecl : ASTAbsDirDecl, IEquatable<ASTAbsParDecl> {

        public ASTAbsParDecl(ASTAbsDecl declarator) {
            this.declarator = declarator;
        }

        public bool Equals(ASTAbsParDecl x) {
            return x != null && x.declarator.Equals(declarator);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTAbsParDecl);
        }

        public override int GetHashCode() {
            return declarator.GetHashCode();
        }

        public override Position Pos => declarator.Pos;

        public readonly ASTAbsDecl declarator;
    }

    public sealed class ASTAbsArrDecl : ASTAbsDirDecl, IEquatable<ASTAbsArrDecl> {

        public ASTAbsArrDecl(
            ASTAbsDirDecl direct,
            IEnumerable<ASTTypeQual> qualifiers,
            ASTExpr expr,
            bool isStatic
            ) {
            this.direct = direct;
            this.qualifiers = qualifiers;
            this.expr = expr;
            this.isStatic = isStatic;
            this.isStar = false;
        }

        public ASTAbsArrDecl(ASTAbsDirDecl direct) {
            this.direct = direct;
            this.qualifiers = new List<ASTTypeQual>();
            this.expr = null;
            this.isStar = true;
            this.isStatic = false;
        }

        public bool Equals(ASTAbsArrDecl x) {
            return x != null && NullableEquals(x.direct, direct)
                && x.qualifiers.SequenceEqual(qualifiers)
                && x.isStatic == isStatic && x.isStar == isStar
                && NullableEquals(x.expr, expr);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTAbsArrDecl);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public override Position Pos => direct.Pos;

        public readonly ASTAbsDirDecl direct;

        public readonly ASTExpr expr;
        public readonly IEnumerable<ASTTypeQual> qualifiers;
        public readonly bool isStatic;
        public readonly bool isStar;
    }

    public sealed class ASTAbsFuncDecl : ASTAbsDirDecl, IEquatable<ASTAbsFuncDecl> {

        public ASTAbsFuncDecl(
            ASTAbsDirDecl direct,
            IEnumerable<ASTParam> parameters,
            bool isEllipis
            ) {
            this.direct = direct;
            this.parameters = parameters;
            this.isEllipis = isEllipis;
        }

        public override Position Pos => direct.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTAbsFuncDecl);
        }

        public bool Equals(ASTAbsFuncDecl x) {
            return x != null && NullableEquals(x.direct, direct)
                && NullableEquals(x.parameters, parameters)
                && x.isEllipis == isEllipis;
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public readonly ASTAbsDirDecl direct;

        // ( parameter-type-list )
        public readonly IEnumerable<ASTParam> parameters;
        public readonly bool isEllipis;
    }

    public sealed class ASTTypeName : ASTNode, IEquatable<ASTTypeName> {

        public ASTTypeName(IEnumerable<ASTTypeSpecQual> specifiers, ASTAbsDecl declarator = null) {
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

        public override Position Pos => specifiers.First().Pos;

        public readonly IEnumerable<ASTTypeSpecQual> specifiers;
        public readonly ASTAbsDecl declarator;
    }
}
