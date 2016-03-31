using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {

    public sealed class ASTEnumerator : ASTNode {

        public ASTEnumerator(ASTIdentifier identifier, ASTExpr expr) {
            this.identifier = identifier;
            this.expr = expr;
        }

        public override int GetLine() {
            return identifier.GetLine();
        }

        public override bool Equals(object obj) {
            ASTEnumerator enumerator = obj as ASTEnumerator;
            return enumerator == null ? false : base.Equals(enumerator)
                && enumerator.identifier.Equals(identifier)
                && (enumerator.expr == null ? expr == null : enumerator.expr.Equals(expr));
        }

        public bool Equals(ASTEnumerator enumerator) {
            return base.Equals(enumerator)
                && enumerator.identifier.Equals(identifier)
                && enumerator.expr.Equals(expr);
        }

        public override int GetHashCode() {
            return identifier.GetHashCode();
        }

        public readonly ASTIdentifier identifier;
        public readonly ASTExpr expr;
    }

    public sealed class ASTEnumSpecifier : ASTTypeSpecifier {

        public ASTEnumSpecifier(int line, ASTIdentifier identifier, LinkedList<ASTEnumerator> enumerators)
            : base(Type.ENUM) {
            this.line = line;
            this.identifier = identifier;
            this.enumerators = enumerators;
        }

        public override int GetLine() {
            return line;
        }

        //public TypeEnum TypeCheck(ASTEnv env) {
        //    if (enumerators == null) {
        //        // enum identifier
        //    } else {

        //        // If the type has a tag, check if redeclared.
        //        if (identifier != null && env.ContainsSymbolInCurrentScope(identifier.name)) {
        //            Utility.TCErrRedecl(env.GetDeclaration(identifier.name), identifier);
        //        }

        //        // Make the new enum type.
        //        TypeEnum unqualifiedType = identifier == null ?
        //            new TypeEnum() : new TypeEnum(identifier.name);

        //        // Make the complete type for all the enumerators.
        //        lcc.Type.Type type = new lcc.Type.Type(unqualifiedType, new lcc.Type.Type.Qualifier(true, false, false));

        //        foreach (var enumerator in enumerators) {

        //            ASTIdentifier identifier = enumerator.identifier;

        //            // First check if the name has been defined.
        //            if (env.ContainsSymbolInCurrentScope(identifier.name)) {
        //                Utility.TCErrRedecl(env.GetDeclaration(identifier.name), identifier);
        //            }

        //            // Check if the enumverator has an initializer.
        //            if (enumerator.expr != null) {

        //            }

        //            // Add every enumerator into the environment.
        //            env.AddSymbol(identifier.name, type, identifier.line);
        //        }

        //        return unqualifiedType;
        //    }
        //}

        public override bool Equals(object obj) {
            ASTEnumSpecifier e = obj as ASTEnumSpecifier;
            return e == null ? false : base.Equals(e)
                && (e.identifier == null ? identifier == null : e.identifier.Equals(identifier))
                && (e.enumerators == null ? enumerators == null : e.enumerators.SequenceEqual(enumerators));
        }

        public bool Equals(ASTEnumSpecifier e) {
            return base.Equals(e)
                && (e.identifier == null ? identifier == null : e.identifier.Equals(identifier))
                && (e.enumerators == null ? enumerators == null : e.enumerators.SequenceEqual(enumerators));
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly int line;
        public readonly ASTIdentifier identifier;
        public readonly LinkedList<ASTEnumerator> enumerators;
    }
}
