using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {
    public sealed class ASTDeclaration : ASTStatement {

        public ASTDeclaration(
            IEnumerable<ASTDeclarationSpecifier> specifiers,
            IEnumerable<ASTInitDeclarator> declarators
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

        public readonly IEnumerable<ASTDeclarationSpecifier> specifiers;
        public readonly IEnumerable<ASTInitDeclarator> declarators;

        public T TypeCheck(ASTEnv env) {

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

        private static Dictionary<IEnumerable<ASTTypeSpecifier.Type>, TUnqualified> dict =
            new Dictionary<IEnumerable<ASTTypeSpecifier.Type>, TUnqualified>(new ListComparer()) {
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
                    TChar.Instance
                },
                {
                    // signed char
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.CHAR
                    } orderby s ascending select s,
                    TSChar.Instance
                },
                {
                    // unsigned char
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.CHAR
                    } orderby s ascending select s,
                    TUChar.Instance
                },
                {
                    // short
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SHORT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // signed short
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.SHORT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // short int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SHORT,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // signed short int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.SHORT,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // unsigned short
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.SHORT
                    } orderby s ascending select s,
                    TUShort.Instance
                },
                {
                    // unsigned short int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.SHORT,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TUShort.Instance
                },
                {
                    // int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TInt.Instance
                },
                {
                    // signed
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED
                    } orderby s ascending select s,
                    TInt.Instance
                },
                {
                    // signed int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TInt.Instance
                },
                {
                    // unsigned
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED
                    } orderby s ascending select s,
                    TUInt.Instance
                },
                {
                    // unsigned int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TUInt.Instance
                },
                {
                    // long
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.LONG
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // signed long
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.LONG
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // long int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // signed long int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // unsigned long
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.LONG
                    } orderby s ascending select s,
                    TULong.Instance
                },
                {
                    // unsigned long int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TULong.Instance
                },
                {
                    // long long
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.LONG
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // signed long long
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.LONG
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // long long int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // signed long long int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.SIGNED,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // unsigned long long
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.LONG
                    } orderby s ascending select s,
                    TULLong.Instance
                },
                {
                    // unsigned long long int
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.UNSIGNED,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.INT
                    } orderby s ascending select s,
                    TULLong.Instance
                },
                {
                    // float
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.FLOAT
                    } orderby s ascending select s,
                    TFloat.Instance
                },
                {
                    // double
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.DOUBLE
                    } orderby s ascending select s,
                    TDouble.Instance
                },
                {
                    // long double
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.LONG,
                        ASTTypeSpecifier.Type.DOUBLE
                    } orderby s ascending select s,
                    TLDouble.Instance
                },
                {
                    // double
                    from s in new List<ASTTypeSpecifier.Type> {
                        ASTTypeSpecifier.Type.BOOL
                    } orderby s ascending select s,
                    TBool.Instance
                },
            };
        #endregion
    }
}
