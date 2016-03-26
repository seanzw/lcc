using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.AST;

namespace lcc.Type {
    public sealed class Utility {

        public static Type GetType(
            IEnumerable<ASTTypeSpecifier> typeSpecifiers,
            bool isConstant
            ) {

            var typeSpecifierType = from s in typeSpecifiers orderby s.type ascending select s.type;
            
            // First check built-in type.
            foreach (var s in builtInTypeMap) {
                if (s.list.SequenceEqual(typeSpecifierType)) {
                    return isConstant ? s.constant : s.variable;
                }
            }

            return new TypeError("Unimplemented Type!");

            // Otherwise check if this is a user-defined type.
            //if (typeSpecifiers.Count() != 1) {
            //    return new TypeError("Unknown type!");
            //}
            
        }

        private struct SetToType {
            public IEnumerable<ASTTypeSpecifier.Type> list;
            public Type constant;
            public Type variable;
            public SetToType(List<ASTTypeSpecifier.Type> list, Type constant, Type variable) {
                // Sort the list so that the order does not matter.
                this.list = from t in list orderby t ascending select t;
                this.constant = constant;
                this.variable = variable;
            }
        }

        private static List<SetToType> builtInTypeMap = new List<SetToType> {
            // void
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.VOID
                },
                TypeVoid.Constant,
                TypeVoid.Variable
            ),
            // char
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.CHAR
                },
                TypeChar.Constant,
                TypeChar.Variable
            ),
            // signed char
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.SIGNED,
                    ASTTypeSpecifier.Type.CHAR
                },
                TypeSignedChar.Constant,
                TypeSignedChar.Variable
            ),
            // unsigned char
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.UNSIGNED,
                    ASTTypeSpecifier.Type.CHAR
                },
                TypeUnsignedChar.Constant,
                TypeUnsignedChar.Variable
            ),
            // short
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.SHORT
                },
                TypeShort.Constant,
                TypeShort.Variable
            ),
            // signed short
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.SIGNED,
                    ASTTypeSpecifier.Type.SHORT
                },
                TypeShort.Constant,
                TypeShort.Variable
            ),
            // short int
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.SHORT,
                    ASTTypeSpecifier.Type.INT
                },
                TypeShort.Constant,
                TypeShort.Variable
            ),
            // signed short int
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.SIGNED,
                    ASTTypeSpecifier.Type.SHORT,
                    ASTTypeSpecifier.Type.INT
                },
                TypeShort.Constant,
                TypeShort.Variable
            ),
            // unsigned short
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.UNSIGNED,
                    ASTTypeSpecifier.Type.SHORT
                },
                TypeUnsignedShort.Constant,
                TypeUnsignedShort.Variable
            ),
            // unsigned short int
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.UNSIGNED,
                    ASTTypeSpecifier.Type.SHORT,
                    ASTTypeSpecifier.Type.INT
                },
                TypeUnsignedShort.Constant,
                TypeUnsignedShort.Variable
            ),
            // int
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.INT
                },
                TypeInt.Constant,
                TypeInt.Variable
            ),
            // signed
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.SIGNED
                },
                TypeInt.Constant,
                TypeInt.Variable
            ),
            // signed int
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.SIGNED,
                    ASTTypeSpecifier.Type.INT
                },
                TypeInt.Constant,
                TypeInt.Variable
            ),
            // unsigned
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.UNSIGNED
                },
                TypeUnsignedInt.Constant,
                TypeUnsignedInt.Variable
            ),
            // unsigned int
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.UNSIGNED,
                    ASTTypeSpecifier.Type.INT
                },
                TypeUnsignedInt.Constant,
                TypeUnsignedInt.Variable
            ),
            // long
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.LONG
                },
                TypeLong.Constant,
                TypeLong.Variable
            ),
            // signed long
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.SIGNED,
                    ASTTypeSpecifier.Type.LONG
                },
                TypeLong.Constant,
                TypeLong.Variable
            ),
            // long int
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.LONG,
                    ASTTypeSpecifier.Type.INT
                },
                TypeLong.Constant,
                TypeLong.Variable
            ),
            // signed long int
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.SIGNED,
                    ASTTypeSpecifier.Type.LONG,
                    ASTTypeSpecifier.Type.INT
                },
                TypeLong.Constant,
                TypeLong.Variable
            ),
            // unsigned long
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.UNSIGNED,
                    ASTTypeSpecifier.Type.LONG
                },
                TypeUnsignedLong.Constant,
                TypeUnsignedLong.Variable
            ),
            // unsigned long int
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.UNSIGNED,
                    ASTTypeSpecifier.Type.LONG,
                    ASTTypeSpecifier.Type.INT
                },
                TypeUnsignedLong.Constant,
                TypeUnsignedLong.Variable
            ),
            // long long
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.LONG,
                    ASTTypeSpecifier.Type.LONG
                },
                TypeLongLong.Constant,
                TypeLongLong.Variable
            ),
            // signed long long
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.SIGNED,
                    ASTTypeSpecifier.Type.LONG,
                    ASTTypeSpecifier.Type.LONG
                },
                TypeLongLong.Constant,
                TypeLongLong.Variable
            ),
            // long long int
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.LONG,
                    ASTTypeSpecifier.Type.LONG,
                    ASTTypeSpecifier.Type.INT
                },
                TypeLongLong.Constant,
                TypeLongLong.Variable
            ),
            // signed long long int
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.SIGNED,
                    ASTTypeSpecifier.Type.LONG,
                    ASTTypeSpecifier.Type.LONG,
                    ASTTypeSpecifier.Type.INT
                },
                TypeLongLong.Constant,
                TypeLongLong.Variable
            ),
            // unsigned long long
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.UNSIGNED,
                    ASTTypeSpecifier.Type.LONG,
                    ASTTypeSpecifier.Type.LONG
                },
                TypeUnsignedLongLong.Constant,
                TypeUnsignedLongLong.Variable
            ),
            // unsigned long long int
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.UNSIGNED,
                    ASTTypeSpecifier.Type.LONG,
                    ASTTypeSpecifier.Type.LONG,
                    ASTTypeSpecifier.Type.INT
                },
                TypeUnsignedLongLong.Constant,
                TypeUnsignedLongLong.Variable
            ),
            // float
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.FLOAT
                },
                TypeFloat.Constant,
                TypeFloat.Variable
            ),
            // double
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.DOUBLE
                },
                TypeDouble.Constant,
                TypeDouble.Variable
            ),
            // long double
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.LONG,
                    ASTTypeSpecifier.Type.DOUBLE
                },
                TypeLongDouble.Constant,
                TypeLongDouble.Variable
            ),
            // _Bool
            new SetToType(
                new List<ASTTypeSpecifier.Type> {
                    ASTTypeSpecifier.Type.BOOL
                },
                TypeBool.Constant,
                TypeBool.Variable
            ),
        };




    }
}
