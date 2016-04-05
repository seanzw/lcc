using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {

    public sealed class ASTDeclSpecs : ASTNode {

        public ASTDeclSpecs(
            IEnumerable<ASTDeclSpec> all,
            IEnumerable<ASTTypeSpec.Kind> keys
            ) {
            this.line = all.First().GetLine();
            this.storages = from s in all.OfType<ASTStorageSpecifier>() select s.kind;
            this.qualifiers = from s in all.OfType<ASTTypeQualifier>() select s.kind;
            this.keys = keys;
            this.functions = from s in all.OfType<ASTFunctionSpecifier>() select s.kind;
        }

        public ASTDeclSpecs(
            IEnumerable<ASTDeclSpec> all,
            ASTTypeSpec specifier
            ) {
            this.line = all.First().GetLine();
            this.storages = from s in all.OfType<ASTStorageSpecifier>() select s.kind;
            this.qualifiers = from s in all.OfType<ASTTypeQualifier>() select s.kind;
            this.functions = from s in all.OfType<ASTFunctionSpecifier>() select s.kind;
            this.specifier = specifier;
        }

        public bool Equals(ASTDeclSpecs x) {
            return x != null && x.line == line
                && x.storages.SequenceEqual(storages)
                && x.qualifiers.SequenceEqual(qualifiers)
                && keys == null ? x.keys == null : keys.SequenceEqual(x.keys)
                && NullableEquals(x.specifier, specifier)
                && x.functions.SequenceEqual(functions);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTDeclSpecs);
        }

        public override int GetHashCode() {
            return line;
        }

        public override int GetLine() {
            return line;
        }

        public readonly int line;

        public readonly IEnumerable<ASTStorageSpecifier.Kind> storages;
        public readonly IEnumerable<ASTTypeQualifier.Kind> qualifiers;

        /// <summary>
        /// All the type specifiers EXCEPT struct, union, enum, typedef.
        /// </summary>
        public readonly IEnumerable<ASTTypeSpec.Kind> keys;
        public readonly IEnumerable<ASTFunctionSpecifier.Kind> functions;

        /// <summary>
        /// All the struct, union, enum, typedef specifier.
        /// </summary>
        public readonly ASTTypeSpec specifier;
    }

    public sealed class ASTDeclaration : ASTStatement {

        public ASTDeclaration(
            ASTDeclSpecs specifiers,
            IEnumerable<ASTInitDeclarator> declarators
            ) {
            this.specifiers = specifiers;
            this.declarators = declarators;
        }

        /// <summary>
        /// Check if this is a typedef declaration.
        /// </summary>
        /// <returns></returns>
        public bool IsTypedef => specifiers.storages.Contains(ASTStorageSpecifier.Kind.TYPEDEF);

        /// <summary>
        /// Return the names of the all the direct declarators.
        /// </summary>
        public IEnumerable<string> DeclNames => from declarator in declarators select declarator.declarator.direct.Name;

        public override int GetLine() {
            return specifiers.GetLine();
        }

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
        public readonly IEnumerable<ASTInitDeclarator> declarators;

        public T TypeCheck(ASTEnv env) {

            return null;
        }

        //public UnqualifiedType TCTypeSpecifiers(
        //    ASTEnv env,
        //    IEnumerable<ASTTypeSpecifier> typeSpecifiers
        //    ) {

        //    var typeSpecifierType = from s in typeSpecifiers orderby s.type ascending select s.type;

        //    // First check built-in type.
        //    if (dict.ContainsKey(typeSpecifierType)) {
        //        return dict[typeSpecifierType];
        //    }

        //    if (typeSpecifiers.Count() == 1) {

        //        // Handle the enum specifier.
        //        var specifier = typeSpecifiers.First();
        //        if (specifier.type == ASTTypeSpecifier.Type.ENUM) {
        //            return (specifier as ASTEnumSpecifier).TypeCheck(env);
        //        }

        //    }

        //    return new TypeError("Unimplemented Type!");

        //}

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
}
