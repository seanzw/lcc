using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {

    /// <summary>
    /// Base type is a type without type qualifier.
    /// </summary>
    public abstract class UnqualifiedType {

        /// <summary>
        /// Return a composite type of this and other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract UnqualifiedType Composite(UnqualifiedType other);

        /// <summary>
        /// Whether this is a completed type.
        /// </summary>
        public abstract bool Completed {
            get;
        }

    }

    public abstract class ArithmeticType : UnqualifiedType {

        public ArithmeticType(uint size) {
            this.size = size;
        }

        public override bool Completed {
            get {
                return true;
            }
        }

        /// <summary>
        /// Equals test.
        /// A little reflect to save code...
        /// </summary>
        /// <param name="obj"> Object to be compared. </param>
        /// <returns> True if they are the same type. </returns>
        public override bool Equals(object obj) {
            ArithmeticType t = obj as ArithmeticType;
            return t == null ? false : GetType().Equals(obj.GetType());
        }

        public override int GetHashCode() {
            return (int)size;
        }

        /// <summary>
        /// The value returned by sizeof.
        /// </summary>
        public readonly uint size;
    }

    public abstract class IntegerType : ArithmeticType {

        public IntegerType(uint size) : base(size) { }

        public abstract BigInteger MAX {
            get;
        }
        
        public abstract BigInteger MIN {
            get;
        }
    }

    /// <summary>
    /// A type is composed with an unqualified type and qualifiers.
    /// </summary>
    public sealed class Type {

        public struct Qualifier {
            public bool isConstant;
            public bool isRestrict;
            public bool isVolatile;
            public Qualifier(bool isConstant, bool isRestrict, bool isVolatile) {
                this.isConstant = isConstant;
                this.isRestrict = isRestrict;
                this.isVolatile = isVolatile;
            }
            public bool Equals(Qualifier other) {
                return other.isConstant == isConstant
                    && other.isRestrict == isRestrict
                    && other.isVolatile == isVolatile;
            }
        }

        public Type(UnqualifiedType baseType, Qualifier qualifiers) {
            this.baseType = baseType;
            this.qualifiers = qualifiers;
        }

        public override bool Equals(object obj) {
            Type t = obj as Type;
            return t == null ? false : t.baseType.Equals(baseType)
                && t.qualifiers.Equals(qualifiers);
        }

        public override int GetHashCode() {
            return baseType.GetHashCode();
        }

        public override string ToString() {
            string constantStr = qualifiers.isConstant ? "constant " : "";
            string restrictStr = qualifiers.isRestrict ? "restrict " : "";
            string volatileStr = qualifiers.isVolatile ? "volatile " : "";
            return constantStr + restrictStr + volatileStr + baseType;
        }

        public UnqualifiedType baseType;
        public Qualifier qualifiers;
    }

    public static class TypeExtension {
        public static Type MakeConst(this UnqualifiedType type) {
            return new Type(type, new Type.Qualifier(true, false, false));
        }

        public static Type MakeType(this UnqualifiedType type) {
            return new Type(type, new Type.Qualifier(false, false, false));
        }
    }
}
