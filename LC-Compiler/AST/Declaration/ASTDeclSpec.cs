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

    public sealed class ASTDeclStroageSpec : ASTDeclSpec {

        public enum Type {
            TYPEDEF,
            EXTERN,
            STATIC,
            AUTO,
            REGISTER
        }

        public ASTDeclStroageSpec(int line, Type type) {
            this.line = line;
            this.type = type;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTDeclStroageSpec spec = obj as ASTDeclStroageSpec;
            return spec == null ? false : base.Equals(spec)
                && spec.line == line
                && spec.type == type;
        }

        public bool Equals(ASTDeclStroageSpec spec) {
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

    public abstract class ASTDeclTypeSpec : ASTDeclSpec {
        public abstract override int GetLine();

        public override bool Equals(object obj) {
            return obj is ASTDeclTypeSpec;
        }

        public bool Equals(ASTDeclTypeSpec spec) {
            return true;
        }

        public override int GetHashCode() {
            return GetLine();
        }
    }

    public sealed class ASTDeclTypeKeySpec : ASTDeclTypeSpec {

        public enum Type {
            VOID,
            CHAR,
            SHORT,
            INT,
            LONG,
            FLOAT,
            DOUBLE,
            SIGNED,
            UNSIGNED
        }

        public ASTDeclTypeKeySpec(int line, Type type) {
            this.line = line;
            this.type = type;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTDeclTypeKeySpec spec = obj as ASTDeclTypeKeySpec;
            return spec == null ? false : base.Equals(spec)
                && spec.line == line
                && spec.type == type;
        }

        public bool Equals(ASTDeclTypeKeySpec spec) {
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

    public sealed class ASTDeclTypeQual : ASTDeclSpec {

        public enum Type {
            CONST,
            RESTRICT,
            VOLATILE
        }

        public ASTDeclTypeQual(int line, Type type) {
            this.line = line;
            this.type = type;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTDeclTypeQual spec = obj as ASTDeclTypeQual;
            return spec == null ? false : base.Equals(spec)
                && spec.line == line
                && spec.type == type;
        }

        public bool Equals(ASTDeclTypeQual spec) {
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

    public sealed class ASTDeclFuncSpec : ASTDeclSpec {

        public enum Type {
            INLINE
        }

        public ASTDeclFuncSpec(int line, Type type) {
            this.line = line;
            this.type = type;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTDeclFuncSpec spec = obj as ASTDeclFuncSpec;
            return spec == null ? false : base.Equals(spec)
                && spec.line == line
                && spec.type == type;
        }

        public bool Equals(ASTDeclFuncSpec spec) {
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
