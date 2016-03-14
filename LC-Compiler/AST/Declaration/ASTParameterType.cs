using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {

    public abstract class ASTParameter : ASTNode {

        public ASTParameter(LinkedList<ASTDeclarationSpecifier> specifiers) {
            this.specifiers = specifiers;
        }

        public override int GetLine() {
            return specifiers.First().GetLine();
        }

        public override bool Equals(object obj) {
            ASTParameter x = obj as ASTParameter;
            return x == null ? false : base.Equals(x)
                &&x.specifiers.SequenceEqual(specifiers);
        }

        public bool Equals(ASTParameter x) {
            return x == null ? false : base.Equals(x)
                && x.specifiers.SequenceEqual(specifiers);
        }

        public override int GetHashCode() {
            return specifiers.GetHashCode();
        }

        public readonly LinkedList<ASTDeclarationSpecifier> specifiers;
    }

    public sealed class ASTParameterDeclarator : ASTParameter {

        public ASTParameterDeclarator(LinkedList<ASTDeclarationSpecifier> specifiers, ASTDeclarator declarator)
            : base(specifiers) {
            this.declarator = declarator;
        }

        public override bool Equals(object obj) {
            ASTParameterDeclarator x = obj as ASTParameterDeclarator;
            return x == null ? false : base.Equals(x)
                && x.specifiers.SequenceEqual(specifiers)
                && x.declarator.Equals(declarator);
        }

        public bool Equals(ASTParameterDeclarator x) {
            return x == null ? false : base.Equals(x)
                && x.specifiers.SequenceEqual(specifiers)
                && x.declarator.Equals(declarator);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public readonly ASTDeclarator declarator;
    }

    public sealed class ASTParameterType : ASTNode {

        public ASTParameterType(LinkedList<ASTParameter> parameters, bool ellipsis) {
            this.parameters = parameters;
            this.ellipsis = ellipsis;
        }

        public override int GetLine() {
            return parameters.First().GetLine();
        }

        public override bool Equals(object obj) {
            ASTParameterType x = obj as ASTParameterType;
            return x == null ? false : base.Equals(x)
                && x.parameters.SequenceEqual(parameters)
                && x.ellipsis.Equals(ellipsis);
        }

        public bool Equals(ASTParameterType x) {
            return x == null ? false : base.Equals(x)
                && x.parameters.SequenceEqual(parameters)
                && x.ellipsis.Equals(ellipsis);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public readonly bool ellipsis;
        public readonly LinkedList<ASTParameter> parameters;
    }
}
