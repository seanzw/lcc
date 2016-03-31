using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {

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
    public abstract class TUnqualified {

        /// <summary>
        /// Return a composite type of this and other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract TUnqualified Composite(TUnqualified other);

        /// <summary>
        /// Whether this is a completed type.
        /// </summary>
        public abstract bool Completed {
            get;
        }

    }

    public abstract class TObject : TUnqualified {
        public override bool Completed => true;
    }

    public abstract class TArithmetic : TObject {

        public TArithmetic(uint size) {
            this.size = size;
        }

        /// <summary>
        /// The value returned by sizeof.
        /// </summary>
        public readonly uint size;
    }

    public abstract class TInteger : TArithmetic {

        public TInteger(uint size) : base(size) { }

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
    public sealed class T {

        public enum LR {
            L,
            R
        }

        public struct Qualifier {
            public bool isConstant;
            //public bool isRestrict;
            //public bool isVolatile;
            public Qualifier(bool isConstant, bool isRestrict, bool isVolatile) {
                this.isConstant = isConstant;
                //this.isRestrict = isRestrict;
                //this.isVolatile = isVolatile;
            }
            public bool Equals(Qualifier other) {
                return other.isConstant == isConstant;
                    //&& other.isRestrict == isRestrict
                    //&& other.isVolatile == isVolatile;
            }
        }

        public T(TUnqualified baseType, Qualifier qualifiers, LR lr) {
            this.baseType = baseType;
            this.qualifiers = qualifiers;
            this.lr = lr;
        }

        public override bool Equals(object obj) {
            return Equals(obj as T);
        }

        public bool Equals(T t) {
            return t == null ? false : t.baseType.Equals(baseType)
                && t.qualifiers.Equals(qualifiers)
                && t.lr == lr;
        }

        public override int GetHashCode() {
            return baseType.GetHashCode();
        }

        public override string ToString() {
            string constantStr = qualifiers.isConstant ? "constant " : "";
            //string restrictStr = qualifiers.isRestrict ? "restrict " : "";
            //string volatileStr = qualifiers.isVolatile ? "volatile " : "";
            //return constantStr + restrictStr + volatileStr + baseType;
            return constantStr + baseType;
        }

        public bool IsObject => baseType is TObject;
        public bool IsPointer => baseType is TPointer;
        public bool IsInteger => baseType is TInteger;
        public bool IsStructUnion => baseType is TStructUnion;
        public bool IsLValue => lr == LR.L;

        public readonly TUnqualified baseType;
        public readonly Qualifier qualifiers;
        public readonly LR lr;
    }

    public static class TypeExtension {

        private class Repo {
            public T none;
            public T constant;
        }

        public static T MakeConst(this TUnqualified type, T.LR lr) {
            var buffer = (lr == T.LR.L) ? LValueBuffer : RValueBuffer;
            if (!buffer.ContainsKey(type))
                buffer.Add(type, new Repo());
            if (buffer[type].constant == null)
                buffer[type].constant = new T(type, new T.Qualifier(true, false, false), lr);
            return buffer[type].constant;
        }

        public static T MakeType(this TUnqualified type, T.LR lr) {
            var buffer = (lr == T.LR.L) ? LValueBuffer : RValueBuffer;
            if (!buffer.ContainsKey(type))
                buffer.Add(type, new Repo());
            if (buffer[type].none == null)
                buffer[type].none = new T(type, new T.Qualifier(false, false, false), lr);
            return buffer[type].none;
        }

        /// <summary>
        /// Take the qualifier from type and apply them to other.
        /// Return a (maybe) new type.
        /// 
        /// Example:
        /// struct t {
        ///     int i;
        ///     const int ci;
        /// };
        /// struct t s;
        /// const struct t cs;
        /// 
        /// s.i     -> int
        /// s.ci    -> const int;
        /// cs.i    -> const int;
        /// cs.ci   -> const int;
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="other"></param>
        /// <param name="isLValue"></param>
        /// <returns></returns>
        public static T MakeQualified(this T type, T other, T.LR lr) {
            var buffer = (lr == T.LR.L) ? LValueBuffer : RValueBuffer;
            if (!buffer.ContainsKey(other.baseType))
                buffer.Add(other.baseType, new Repo());
            if (type.qualifiers.isConstant || other.qualifiers.isConstant) {
                return other.baseType.MakeConst(lr);
            } else {
                return other.baseType.MakeType(lr);
            }
        }

        // Use buffer to avoid creating a lot of types during type check.
        private static Dictionary<TUnqualified, Repo> LValueBuffer = new Dictionary<TUnqualified, Repo>();
        private static Dictionary<TUnqualified, Repo> RValueBuffer = new Dictionary<TUnqualified, Repo>();
    }
}
