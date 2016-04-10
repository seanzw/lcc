using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {

    public sealed class ASTDeclSpecs : ASTNode, IEquatable<ASTDeclSpecs> {

        public ASTDeclSpecs(
            IEnumerable<ASTDeclSpec> all,
            ASTStoreSpec.Kind storage,
            IEnumerable<ASTTypeSpec.Kind> keys,
            ASTFuncSpec.Kind function = ASTFuncSpec.Kind.NONE
            ) {
            this.storage = storage;
            this.keys = keys;
            this.function = function;
            qualifiers = GetQualifiers(from s in all.OfType<ASTTypeQual>() select s.kind);
            pos = all.First().Pos;
        }

        public ASTDeclSpecs(
            IEnumerable<ASTDeclSpec> all,
            ASTStoreSpec.Kind storage,
            ASTTypeUserSpec specifier,
            ASTFuncSpec.Kind function = ASTFuncSpec.Kind.NONE
            ) {
            this.storage = storage;
            this.function = function;
            this.specifier = specifier;
            qualifiers = GetQualifiers(from s in all.OfType<ASTTypeQual>() select s.kind);
            pos = all.First().Pos;
        }

        public bool Equals(ASTDeclSpecs x) {
            return x != null && x.pos.Equals(pos)
                && x.storage == storage
                && x.qualifiers.Equals(qualifiers)
                && keys == null ? x.keys == null : keys.SequenceEqual(x.keys)
                && NullableEquals(x.specifier, specifier)
                && x.function == function;
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTDeclSpecs);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override Position Pos => pos;

        /// <summary>
        /// At most one storage specifier.
        /// </summary>
        public readonly ASTStoreSpec.Kind storage;

        /// <summary>
        /// All the qualifiers.
        /// </summary>
        public readonly TQualifiers qualifiers;

        /// <summary>
        /// At most one function specifier (inline).
        /// </summary>
        public readonly ASTFuncSpec.Kind function;

        /// <summary>
        /// All the type specifiers EXCEPT struct, union, enum, typedef.
        /// </summary>
        public readonly IEnumerable<ASTTypeSpec.Kind> keys;

        /// <summary>
        /// All the struct, union, enum, typedef specifier.
        /// </summary>
        public readonly ASTTypeUserSpec specifier;

        /// <summary>
        /// Evaluate the type specifiers and get the unqualified type.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public TUnqualified GetTUnqualified(ASTEnv env) {
            // Get the unqualified type.
            if (keys != null) {
                // This is built-in type.
                if (dict.ContainsKey(keys)) return dict[keys];
                else throw new ASTException(pos, string.Format("Unkown type: {0}", keys.Aggregate("", (str, key) => str + " " + key)));
            } else if (specifier != null) {
                // This is a user-defined type.
                return specifier.GetTUnqualified(env);
            } else {
                // Error!
                throw new ASTException(pos, "At least one type specifier should be given.");
            }
        }

        /// <summary>
        /// Evaluate the type qualifiers.
        /// </summary>
        /// <returns></returns>
        private static TQualifiers GetQualifiers(IEnumerable<ASTTypeQual.Kind> qualifiers) {
            var tuple = new Tuple<bool, bool, bool>(
                qualifiers.Contains(ASTTypeQual.Kind.CONST),
                qualifiers.Contains(ASTTypeQual.Kind.RESTRICT), 
                qualifiers.Contains(ASTTypeQual.Kind.VOLATILE));
            return TQualifiers.dict[tuple];
        }

        private readonly Position pos;

        #region TypeSpecifier Map
        private class ListComparer : IEqualityComparer<IEnumerable<ASTTypeSpec.Kind>> {
            public bool Equals(IEnumerable<ASTTypeSpec.Kind> x, IEnumerable<ASTTypeSpec.Kind> y) {
                return x.SequenceEqual(y);
            }

            public int GetHashCode(IEnumerable<ASTTypeSpec.Kind> x) {
                int hash = 0;
                foreach (var s in x) {
                    hash |= s.GetHashCode();
                }
                return hash;
            }
        }

        private static Dictionary<IEnumerable<ASTTypeSpec.Kind>, TUnqualified> dict =
            new Dictionary<IEnumerable<ASTTypeSpec.Kind>, TUnqualified>(new ListComparer()) {
                {
                    // void
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.VOID
                    } orderby s ascending select s,
                    TypeVoid.Instance
                },
                {
                    // char
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.CHAR
                    } orderby s ascending select s,
                    TChar.Instance
                },
                {
                    // signed char
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.SIGNED,
                        ASTTypeSpec.Kind.CHAR
                    } orderby s ascending select s,
                    TSChar.Instance
                },
                {
                    // unsigned char
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.UNSIGNED,
                        ASTTypeSpec.Kind.CHAR
                    } orderby s ascending select s,
                    TUChar.Instance
                },
                {
                    // short
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.SHORT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // signed short
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.SIGNED,
                        ASTTypeSpec.Kind.SHORT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // short int
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.SHORT,
                        ASTTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // signed short int
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.SIGNED,
                        ASTTypeSpec.Kind.SHORT,
                        ASTTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // unsigned short
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.UNSIGNED,
                        ASTTypeSpec.Kind.SHORT
                    } orderby s ascending select s,
                    TUShort.Instance
                },
                {
                    // unsigned short int
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.UNSIGNED,
                        ASTTypeSpec.Kind.SHORT,
                        ASTTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TUShort.Instance
                },
                {
                    // int
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TInt.Instance
                },
                {
                    // signed
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.SIGNED
                    } orderby s ascending select s,
                    TInt.Instance
                },
                {
                    // signed int
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.SIGNED,
                        ASTTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TInt.Instance
                },
                {
                    // unsigned
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.UNSIGNED
                    } orderby s ascending select s,
                    TUInt.Instance
                },
                {
                    // unsigned int
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.UNSIGNED,
                        ASTTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TUInt.Instance
                },
                {
                    // long
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // signed long
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.SIGNED,
                        ASTTypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // long int
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.LONG,
                        ASTTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // signed long int
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.SIGNED,
                        ASTTypeSpec.Kind.LONG,
                        ASTTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // unsigned long
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.UNSIGNED,
                        ASTTypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TULong.Instance
                },
                {
                    // unsigned long int
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.UNSIGNED,
                        ASTTypeSpec.Kind.LONG,
                        ASTTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TULong.Instance
                },
                {
                    // long long
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.LONG,
                        ASTTypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // signed long long
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.SIGNED,
                        ASTTypeSpec.Kind.LONG,
                        ASTTypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // long long int
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.LONG,
                        ASTTypeSpec.Kind.LONG,
                        ASTTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // signed long long int
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.SIGNED,
                        ASTTypeSpec.Kind.LONG,
                        ASTTypeSpec.Kind.LONG,
                        ASTTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // unsigned long long
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.UNSIGNED,
                        ASTTypeSpec.Kind.LONG,
                        ASTTypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TULLong.Instance
                },
                {
                    // unsigned long long int
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.UNSIGNED,
                        ASTTypeSpec.Kind.LONG,
                        ASTTypeSpec.Kind.LONG,
                        ASTTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TULLong.Instance
                },
                {
                    // float
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.FLOAT
                    } orderby s ascending select s,
                    TFloat.Instance
                },
                {
                    // double
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.DOUBLE
                    } orderby s ascending select s,
                    TDouble.Instance
                },
                {
                    // long double
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.LONG,
                        ASTTypeSpec.Kind.DOUBLE
                    } orderby s ascending select s,
                    TLDouble.Instance
                },
                {
                    // double
                    from s in new List<ASTTypeSpec.Kind> {
                        ASTTypeSpec.Kind.BOOL
                    } orderby s ascending select s,
                    TBool.Instance
                },
            };
        #endregion
    }

    public sealed class ASTDeclaration : ASTStmt, IEquatable<ASTDeclaration> {

        public ASTDeclaration(
            ASTDeclSpecs specifiers,
            IEnumerable<ASTInitDecl> declarators
            ) {
            this.specifiers = specifiers;
            this.declarators = declarators;
        }

        /// <summary>
        /// Check if this is a typedef declaration.
        /// </summary>
        /// <returns></returns>
        public bool IsTypedef => specifiers.storage == ASTStoreSpec.Kind.TYPEDEF;

        /// <summary>
        /// Return the names of the all the direct declarators.
        /// </summary>
        public IEnumerable<string> DeclNames => from declarator in declarators select declarator.declarator.direct.Name;

        public override Position Pos => specifiers.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTDeclaration);
        }

        public bool Equals(ASTDeclaration x) {
            return x != null && x.specifiers.Equals(specifiers)
                && x.declarators.SequenceEqual(declarators);
        }

        public override int GetHashCode() {
            return specifiers.GetHashCode();
        }

        public readonly ASTDeclSpecs specifiers;
        public readonly IEnumerable<ASTInitDecl> declarators;

        public T TypeCheck(ASTEnv env) {

            return null;
        }
    }
}
