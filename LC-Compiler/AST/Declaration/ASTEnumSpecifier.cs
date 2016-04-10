using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {

    public sealed class ASTEnum : ASTNode, IEquatable<ASTEnum> {

        public ASTEnum(ASTId identifier, ASTExpr expr) {
            this.identifier = identifier;
            this.expr = expr;
        }

        public override Position Pos => identifier.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTEnum);
        }

        public bool Equals(ASTEnum x) {
            return x != null
                && x.identifier.Equals(identifier)
                && NullableEquals(expr, x.expr);
        }

        public override int GetHashCode() {
            return identifier.GetHashCode();
        }

        public readonly ASTId identifier;
        public readonly ASTExpr expr;
    }

    public sealed class ASTEnumSpec : ASTTypeUserSpec, IEquatable<ASTEnumSpec> {

        public ASTEnumSpec(int line, ASTId id, IEnumerable<ASTEnum> enums)
            : base(Kind.ENUM) {
            pos = new Position { line = line };
            this.id = id;
            this.enums = enums;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTEnumSpec);
        }

        public bool Equals(ASTEnumSpec x) {
            return x != null
                && NullableEquals(x.id, id)
                && NullableEquals(x.enums, enums);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override TUnqualified GetTUnqualified(ASTEnv env) {
            throw new NotImplementedException();
            // if (enumerators == null) {
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
            }

        private readonly Position pos;
        public readonly ASTId id;
        public readonly IEnumerable<ASTEnum> enums;
    }
}
