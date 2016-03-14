using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public abstract class ASTFunctionDeclarator : ASTDeclarator {

        public ASTFunctionDeclarator(ASTDeclarator declarator) {
            this.declarator = declarator;
        }

        public override int GetLine() {
            return declarator.GetLine();
        }

        public override bool Equals(object obj) {
            ASTFunctionDeclarator x = obj as ASTFunctionDeclarator;
            return x == null ? false : base.Equals(x)
                && x.declarator.Equals(declarator);
        }

        public bool Equals(ASTFunctionDeclarator x) {
            return x == null ? false : base.Equals(x)
                && x.declarator.Equals(declarator);
        }

        public override int GetHashCode() {
            return declarator.GetHashCode();
        }

        ASTDeclarator declarator;
    }

    public sealed class ASTFunctionParameter : ASTFunctionDeclarator {

        public ASTFunctionParameter(ASTDeclarator declarator, ASTParameterType parameterType)
            : base(declarator) {
            this.parameterType = parameterType;
        }

        public override bool Equals(object obj) {
            ASTFunctionParameter x = obj as ASTFunctionParameter;
            return x == null ? false : base.Equals(x)
                && x.parameterType.Equals(parameterType);
        }

        public bool Equals(ASTFunctionParameter x) {
            return x == null ? false : base.Equals(x)
                && x.parameterType.Equals(parameterType);
        }

        public override int GetHashCode() {
            return base.GetHashCode() ^ parameterType.GetHashCode();
        }

        ASTParameterType parameterType;
    }
}
