using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.Type;

namespace lcc.AST {
    public sealed class ASTDeclaration : ASTStatement {

        public ASTDeclaration(
            LinkedList<ASTDeclarationSpecifier> specifiers,
            LinkedList<ASTDeclarator> declarators
            ) {
            this.specifiers = specifiers;
            this.declarators = declarators;
        }

        public override int GetLine() {
            return specifiers.First().GetLine();
        }

        public override bool Equals(object obj) {
            ASTDeclaration x = obj as ASTDeclaration;
            return x == null ? false : base.Equals(x)
                && x.specifiers.SequenceEqual(specifiers)
                && x.declarators.SequenceEqual(declarators);
        }

        public bool Equals(ASTDeclaration x) {
            return x == null ? false : base.Equals(x)
                && x.specifiers.SequenceEqual(specifiers)
                && x.declarators.SequenceEqual(declarators);
        }

        public override int GetHashCode() {
            return specifiers.GetHashCode();
        }

        public readonly LinkedList<ASTDeclarationSpecifier> specifiers;
        public readonly LinkedList<ASTDeclarator> declarators;

        public Type.Type TypeCheck(ASTEnv env) {

            // Process the specifier list.
            var typeSpecifiers = 
                from s in specifiers
                where s is ASTTypeSpecifier
                select s as ASTTypeSpecifier;

            var storageSpecifiers =
                from s in specifiers
                where s is ASTStorageSpecifier
                select s as ASTStorageSpecifier;

            var typeQualifiers =
                from s in specifiers
                where s is ASTTypeQualifier
                select s as ASTTypeQualifier;

            var funcSpecifiers =
                from s in specifiers
                where s is ASTFunctionSpecifier
                select s as ASTFunctionSpecifier;



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
        private class ListComparer : IEqualityComparer<IEnumerable<ASTTypeSpecifier.Type>> {
            public bool Equals(IEnumerable<ASTTypeSpecifier.Type> x, IEnumerable<ASTTypeSpecifier.Type> y) {
                return x.SequenceEqual(y);
            }

            public int GetHashCode(IEnumerable<ASTTypeSpecifier.Type> x) {
                int hash = 0;
                foreach (var s in x) {
                    hash |= s.GetHashCode();
                }
                return hash;
            }
        }

        private static Dictionary<IEnumerable<ASTTypeSpecifier.Type>, ArithmeticType> dict =
            new Dictionary<IEnumerable<ASTTypeSpecifier.Type>, ArithmeticType>(new ListComparer()) {
                {
                    // void
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.VOID
                    } orderby s ascending select s,
                    TypeVoid.Instance
                },
                {
                    // char
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.CHAR
                    } orderby s ascending select s,
                    TypeChar.Instance
                },
                {
                    // signed char
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.CHAR
                    } orderby s ascending select s,
                    TypeSignedChar.Instance
                },
                {
                    // unsigned char
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.CHAR
                    } orderby s ascending select s,
                    TypeUnsignedChar.Instance
                },
                {
                    // short
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SHORT
                    } orderby s ascending select s,
                    TypeShort.Instance
                },
                {
                    // signed short
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.SHORT
                    } orderby s ascending select s,
                    TypeShort.Instance
                },
                {
                    // short int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SHORT,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TypeShort.Instance
                },
                {
                    // signed short int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.SHORT,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TypeShort.Instance
                },
                {
                    // unsigned short
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.SHORT
                    } orderby s ascending select s,
                    TypeUnsignedShort.Instance
                },
                {
                    // unsigned short int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.SHORT,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TypeUnsignedShort.Instance
                },
                {
                    // int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TypeInt.Instance
                },
                {
                    // signed
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED
                    } orderby s ascending select s,
                    TypeInt.Instance
                },
                {
                    // signed int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TypeInt.Instance
                },
                {
                    // unsigned
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED
                    } orderby s ascending select s,
                    TypeUnsignedInt.Instance
                },
                {
                    // unsigned int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TypeUnsignedInt.Instance
                },
                {
                    // long
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.LONG
                    } orderby s ascending select s,
                    TypeLong.Instance
                },
                {
                    // signed long
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.LONG
                    } orderby s ascending select s,
                    TypeLong.Instance
                },
                {
                    // long int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TypeLong.Instance
                },
                {
                    // signed long int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TypeLong.Instance
                },
                {
                    // unsigned long
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.LONG
                    } orderby s ascending select s,
                    TypeUnsignedLong.Instance
                },
                {
                    // unsigned long int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TypeUnsignedLong.Instance
                },
                {
                    // long long
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.LONG
                    } orderby s ascending select s,
                    TypeLongLong.Instance
                },
                {
                    // signed long long
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.LONG
                    } orderby s ascending select s,
                    TypeLongLong.Instance
                },
                {
                    // long long int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TypeLongLong.Instance
                },
                {
                    // signed long long int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TypeLongLong.Instance
                },
                {
                    // unsigned long long
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.LONG
                    } orderby s ascending select s,
                    TypeUnsignedLongLong.Instance
                },
                {
                    // unsigned long long int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TypeUnsignedLongLong.Instance
                },
                {
                    // float
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.FLOAT
                    } orderby s ascending select s,
                    TypeFloat.Instance
                },
                {
                    // double
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.DOUBLE
                    } orderby s ascending select s,
                    TypeDouble.Instance
                },
                {
                    // long double
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.DOUBLE
                    } orderby s ascending select s,
                    TypeLongDouble.Instance
                },
                {
                    // double
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.BOOL
                    } orderby s ascending select s,
                    TypeBool.Instance
                },
            };
        #endregion
    }
}
