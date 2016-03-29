using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {

    /**
    
        A type is composed of an unqualified type and variable qualifiers.
       
        Qualifier
            - const
            - volatile
            - restrict
      
        type
            - incomplete
            - void
            - function
            - unqualified
                - aggregate
                    - array
                    - structure
                    - union
                - scalar
                    - pointer
                    - arithmetic
                        - floating
                            - float
                            - double
                            - long double
                        - integer
                            - enumerated
                            - [signed/unsigned] short
                            - [signed/unsigned] int
                            - [signed/unsigned] long
                            - [signed/unsigned] long long
     */



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
            if (!ConstQualifierBuffer.ContainsKey(type))
                ConstQualifierBuffer.Add(type, new Type(type, new Type.Qualifier(true, false, false)));
            return ConstQualifierBuffer[type];
        }

        public static Type MakeType(this UnqualifiedType type) {
            if (!NoneQualifierBuffer.ContainsKey(type))
                NoneQualifierBuffer.Add(type, new Type(type, new Type.Qualifier(false, false, false)));
            return NoneQualifierBuffer[type];
        }

        // Use buffer to avoid creating a lot of types during type check.
        private static Dictionary<UnqualifiedType, Type> NoneQualifierBuffer = new Dictionary<UnqualifiedType, Type>();
        private static Dictionary<UnqualifiedType, Type> ConstQualifierBuffer = new Dictionary<UnqualifiedType, Type>();
    }
}
