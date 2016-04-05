using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.Token;

namespace lcc.AST {
    public abstract class ASTDeclSpec : ASTNode {

        public abstract override int GetLine();

        public override bool Equals(object obj) {
            return obj is ASTDeclSpec;
        }

        public bool Equals(ASTDeclSpec spec) {
            return true;
        }

        public override int GetHashCode() {
            return GetLine();
        }
    }

    public sealed class ASTStorageSpecifier : ASTDeclSpec {

        public enum Kind {
            TYPEDEF,
            EXTERN,
            STATIC,
            AUTO,
            REGISTER
        }

        public ASTStorageSpecifier(int line, Kind type) {
            this.line = line;
            this.kind = type;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTStorageSpecifier spec = obj as ASTStorageSpecifier;
            return spec == null ? false : base.Equals(spec)
                && spec.line == line
                && spec.kind == kind;
        }

        public bool Equals(ASTStorageSpecifier spec) {
            return base.Equals(spec)
                && spec.line == line
                && spec.kind == kind;
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly Kind kind;
        public readonly int line;
    }

    public abstract class ASTTypeSpecifierQualifier : ASTDeclSpec {
        public abstract override int GetLine();

        public override bool Equals(object obj) {
            return obj is ASTTypeSpecifierQualifier;
        }

        public bool Equals(ASTTypeSpecifierQualifier spec) {
            return true;
        }

        public override int GetHashCode() {
            return GetLine();
        }
    }

    public abstract class ASTTypeSpec : ASTTypeSpecifierQualifier {

        public enum Kind {
            VOID,
            CHAR,
            SHORT,
            INT,
            LONG,
            FLOAT,
            DOUBLE,
            SIGNED,
            UNSIGNED,
            BOOL,
            COMPLEX,
            STRUCT,
            UNION,
            ENUM,
            TYPEDEF
        }

        public ASTTypeSpec(Kind kind) {
            this.kind = kind;
        }

        public abstract override int GetLine();

        public override bool Equals(object obj) {
            return obj is ASTTypeSpec;
        }

        public bool Equals(ASTTypeSpec spec) {
            return true;
        }

        public override int GetHashCode() {
            return GetLine();
        }

        public readonly Kind kind;
    }

    public sealed class ASTTypeKeySpecifier : ASTTypeSpec {

        public ASTTypeKeySpecifier(int line, Kind kind) : base(kind) {
            this.line = line;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTTypeKeySpecifier spec = obj as ASTTypeKeySpecifier;
            return spec == null ? false : base.Equals(spec)
                && spec.line == line
                && spec.kind == kind;
        }

        public bool Equals(ASTTypeKeySpecifier spec) {
            return base.Equals(spec)
                && spec.line == line
                && spec.kind == kind;
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly int line;
    }

    public sealed class ASTTypedefName : ASTTypeSpec {

        public ASTTypedefName(ASTIdentifier identifier) : base(Kind.TYPEDEF) {
            this.identifier = identifier;
        }

        public bool Equals(ASTTypedefName x) {
            return x != null && x.identifier.Equals(identifier);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTTypedefName);
        }

        public override int GetHashCode() {
            return identifier.GetHashCode();
        }

        public override int GetLine() {
            return identifier.GetLine();
        }

        public readonly ASTIdentifier identifier;
    }

    public sealed class ASTTypeQualifier : ASTTypeSpecifierQualifier {

        public enum Kind {
            CONST,
            RESTRICT,
            VOLATILE
        }

        public ASTTypeQualifier(int line, Kind kind) {
            this.line = line;
            this.kind = kind;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTTypeQualifier spec = obj as ASTTypeQualifier;
            return spec == null ? false : base.Equals(spec)
                && spec.line == line
                && spec.kind == kind;
        }

        public bool Equals(ASTTypeQualifier spec) {
            return base.Equals(spec)
                && spec.line == line
                && spec.kind == kind;
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly Kind kind;
        public readonly int line;
    }

    public sealed class ASTFunctionSpecifier : ASTDeclSpec {

        public enum Kind {
            INLINE
        }

        public ASTFunctionSpecifier(int line, Kind type) {
            this.line = line;
            this.kind = type;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTFunctionSpecifier spec = obj as ASTFunctionSpecifier;
            return spec == null ? false : base.Equals(spec)
                && spec.line == line
                && spec.kind == kind;
        }

        public bool Equals(ASTFunctionSpecifier spec) {
            return base.Equals(spec)
                && spec.line == line
                && spec.kind == kind;
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly Kind kind;
        public readonly int line;

    }

}
