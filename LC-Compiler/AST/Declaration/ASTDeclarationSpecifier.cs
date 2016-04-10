using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.Token;
using lcc.TypeSystem;

namespace lcc.AST {
    public abstract class ASTDeclSpec : ASTNode { }

    public sealed class ASTStoreSpec : ASTDeclSpec, IEquatable<ASTStoreSpec> {

        public enum Kind {
            NONE,           // Represent no storage-specifier
            TYPEDEF,
            EXTERN,
            STATIC,
            AUTO,
            REGISTER
        }

        public ASTStoreSpec(int line, Kind type) {
            this.pos = new Position { line = line };
            this.kind = type;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTStoreSpec);
        }

        public bool Equals(ASTStoreSpec x) {
            return x != null && x.pos.Equals(pos) && x.kind == kind;
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public readonly Kind kind;
        public readonly Position pos;
    }

    public abstract class ASTTypeSpecQual : ASTDeclSpec {}

    public abstract class ASTTypeSpec : ASTTypeSpecQual {

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

        public readonly Kind kind;
    }

    public sealed class ASTTypeKeySpecifier : ASTTypeSpec, IEquatable<ASTTypeKeySpecifier> {

        public ASTTypeKeySpecifier(int line, Kind kind) : base(kind) {
            this.pos = new Position { line = line };
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTTypeKeySpecifier);
        }

        public bool Equals(ASTTypeKeySpecifier x) {
            return x != null && x.pos.Equals(pos) && x.kind == kind;
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public readonly Position pos;
    }

    /// <summary>
    /// Represents a user-defined type specifier.
    /// </summary>
    public abstract class ASTTypeUserSpec : ASTTypeSpec {
        public ASTTypeUserSpec(Kind kind) : base(kind) { }
        public abstract TUnqualified GetTUnqualified(ASTEnv env);
    }

    public sealed class ASTTypedefName : ASTTypeUserSpec, IEquatable<ASTTypedefName> {

        public ASTTypedefName(ASTId identifier) : base(Kind.TYPEDEF) {
            name = identifier.name;
            pos = identifier.Pos;
        }

        public bool Equals(ASTTypedefName x) {
            return x != null && x.name.Equals(name) && x.pos.Equals(pos);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTTypedefName);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override TUnqualified GetTUnqualified(ASTEnv env) {
            throw new NotImplementedException();
        }

        public override Position Pos => pos;

        public readonly string name;
        private readonly Position pos;
    }

    public sealed class ASTTypeQual : ASTTypeSpecQual, IEquatable<ASTTypeQual> {

        public enum Kind {
            CONST,
            RESTRICT,
            VOLATILE
        }

        public ASTTypeQual(int line, Kind kind) {
            this.pos = new Position { line = line };
            this.kind = kind;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTTypeQual);
        }

        public bool Equals(ASTTypeQual x) {
            return x != null && x.pos.Equals(pos) && x.kind == kind;
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public readonly Kind kind;
        private readonly Position pos;
    }

    public sealed class ASTFuncSpec : ASTDeclSpec, IEquatable<ASTFuncSpec> {

        public enum Kind {
            NONE,
            INLINE
        }

        public ASTFuncSpec(int line, Kind kind) {
            this.pos = new Position { line = line };
            this.kind = kind;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTFuncSpec);
        }

        public bool Equals(ASTFuncSpec x) {
            return x != null && x.pos.Equals(pos) && x.kind == kind;
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public readonly Kind kind;
        private readonly Position pos;

    }

}
