using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.Token;

namespace lcc.AST {
    public abstract class ASTDeclarationSpecifier : ASTNode {

        public abstract override int GetLine();

        public override bool Equals(object obj) {
            return obj is ASTDeclarationSpecifier;
        }

        public bool Equals(ASTDeclarationSpecifier spec) {
            return true;
        }

        public override int GetHashCode() {
            return GetLine();
        }
    }

    public sealed class ASTStorageSpecifier : ASTDeclarationSpecifier {

        public enum Type {
            TYPEDEF,
            EXTERN,
            STATIC,
            AUTO,
            REGISTER
        }

        public ASTStorageSpecifier(int line, Type type) {
            this.line = line;
            this.type = type;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTStorageSpecifier spec = obj as ASTStorageSpecifier;
            return spec == null ? false : base.Equals(spec)
                && spec.line == line
                && spec.type == type;
        }

        public bool Equals(ASTStorageSpecifier spec) {
            return base.Equals(spec)
                && spec.line == line
                && spec.type == type;
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly Type type;
        public readonly int line;
    }

    public abstract class ASTTypeSpecifierQualifier : ASTDeclarationSpecifier {
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

    public abstract class ASTTypeSpecifier : ASTTypeSpecifierQualifier {

        public enum Type {
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
            STRUCT,
            UNION,
            ENUM
        }

        public ASTTypeSpecifier(Type type) {
            this.type = type;
        }

        public abstract override int GetLine();

        public override bool Equals(object obj) {
            return obj is ASTTypeSpecifier;
        }

        public bool Equals(ASTTypeSpecifier spec) {
            return true;
        }

        public override int GetHashCode() {
            return GetLine();
        }

        public readonly Type type;
    }

    public sealed class ASTTypeKeySpecifier : ASTTypeSpecifier {

        public ASTTypeKeySpecifier(int line, Type type) : base(type) {
            this.line = line;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTTypeKeySpecifier spec = obj as ASTTypeKeySpecifier;
            return spec == null ? false : base.Equals(spec)
                && spec.line == line
                && spec.type == type;
        }

        public bool Equals(ASTTypeKeySpecifier spec) {
            return base.Equals(spec)
                && spec.line == line
                && spec.type == type;
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly int line;
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

    public sealed class ASTFunctionSpecifier : ASTDeclarationSpecifier {

        public enum Type {
            INLINE
        }

        public ASTFunctionSpecifier(int line, Type type) {
            this.line = line;
            this.type = type;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTFunctionSpecifier spec = obj as ASTFunctionSpecifier;
            return spec == null ? false : base.Equals(spec)
                && spec.line == line
                && spec.type == type;
        }

        public bool Equals(ASTFunctionSpecifier spec) {
            return base.Equals(spec)
                && spec.line == line
                && spec.type == type;
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly Type type;
        public readonly int line;

    }

}
