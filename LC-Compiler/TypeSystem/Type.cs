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
                        - real
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
                        - complex
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

        /// <summary>
        /// Qualify this type.
        /// </summary>
        /// <param name="qualifiers"></param>
        /// <param name="lr"></param>
        /// <returns></returns>
        public T Qualify(TQualifiers qualifiers, T.LR lr = T.LR.L) {
            return new T(this, qualifiers, lr);
        }

        public T Const(T.LR lr = T.LR.L) {
            return Qualify(TQualifiers.Const, lr);
        }

        public T None(T.LR lr = T.LR.L) {
            return Qualify(TQualifiers.None, lr);
        }
    }

    public abstract class TObject : TUnqualified {
        public override bool Completed => true;
    }

    public abstract class TScalar : TObject { }

    public abstract class TArithmetic : TScalar {

        public TArithmetic(uint size) {
            this.size = size;
        }

        public abstract int RANK { get; }

        /// <summary>
        /// The value returned by sizeof.
        /// </summary>
        public readonly uint size;
    }

    public abstract class TReal : TArithmetic {
        public TReal(uint size) : base(size) { }
    }

    public abstract class TInteger : TReal {

        public TInteger(uint size) : base(size) { }

        public abstract BigInteger MAX { get; }
        
        public abstract BigInteger MIN { get; }
    }

    /// <summary>
    /// Holds all the three qualifiers.
    /// TODO: Support restrict and volatile.
    /// </summary>
    public class TQualifiers {
        public bool isConstant;
        private TQualifiers(bool isConstant, bool isRestrict, bool isVolatile) {
            this.isConstant = isConstant;
        }

        public bool Equals(TQualifiers other) {
            return other.isConstant == isConstant;
        }
        public override string ToString() {
            string str = isConstant ? "const " : "";
            return str;
        }
        public static TQualifiers operator |(TQualifiers q1, TQualifiers q2) {
            return new TQualifiers(
                q1.isConstant || q2.isConstant,
                false,
                false
                );
        }

        public static readonly TQualifiers None = new TQualifiers(false, false, false);
        public static readonly TQualifiers Const = new TQualifiers(true, false, false);
    }

    /// <summary>
    /// A type is composed with an unqualified type and qualifiers.
    /// </summary>
    public sealed class T {

        public enum LR {
            L,
            R
        }

        public T(TUnqualified baseType, TQualifiers qualifiers, LR lr) {
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
            return qualifiers.ToString() + baseType.ToString();
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
        /// </summary>
        /// <param name="nested"> Nested member type. </param>
        /// <param name="lr"> LRValue. </param>
        /// <returns> A new type. </returns>
        public T Unnest(T nested, LR lr = LR.L) {
            return new T(nested.baseType, qualifiers | nested.qualifiers, lr);
        }

        /// <summary>
        /// Return the same type but as an rvalue.
        /// </summary>
        /// <returns></returns>
        public T R() {
            return IsRValue ? this : new T(baseType, qualifiers, LR.R);
        }

        /// <summary>
        /// Pointer derivation.
        /// </summary>
        /// <param name="qualifier"></param>
        /// <param name="lr"></param>
        /// <returns></returns>
        public T Ptr(TQualifiers qualifiers = null) {
            qualifiers = qualifiers ?? TQualifiers.None;
            return new T(new TPointer(this), qualifiers, LR.L);
        }

        /// <summary>
        /// Array derivation.
        /// </summary>
        /// <param name="qualifiers"></param>
        /// <param name="lr"></param>
        /// <returns></returns>
        public T Arr(int n, TQualifiers qualifiers = null) {
            qualifiers = qualifiers ?? TQualifiers.None;
            return new T(new TArray(this, n), qualifiers, LR.L);
        }

        public bool IsObject => baseType is TObject;
        public bool IsArray => baseType is TArray;
        public bool IsScalar => baseType is TScalar;
        public bool IsPointer => baseType is TPointer;
        public bool IsArithmetic => baseType is TArithmetic;
        public bool IsReal => baseType is TReal;
        public bool IsInteger => baseType is TInteger;
        
        public bool IsStructUnion => baseType is TStructUnion;
        public bool IsFunc => baseType is TFunc;


        public bool IsLValue => lr == LR.L;
        public bool IsRValue => lr == LR.R;
        public bool IsModifiable => IsLValue && (!qualifiers.isConstant);

        public readonly TUnqualified baseType;
        public readonly TQualifiers qualifiers;
        public readonly LR lr;

    }

    public static class TPromotion {

        /// <summary>
        /// Integer promotion.
        /// Those type with rank less than int will be promoted to int.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T IntPromote(this T t) {
            TArithmetic ta = t.baseType as TArithmetic;
            if (ta.RANK < TInt.Instance.RANK) {
                return new T(TInt.Instance, t.qualifiers, t.lr);
            } else {
                return t;
            }
        }
     
    }
}
