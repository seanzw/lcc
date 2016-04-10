using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Store = lcc.AST.ASTStoreSpec.Kind;

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
                    - variable length array
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
                                - character
                                    - char
                                    - [signed/unsigned] char
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

        public virtual void CompleteArr(int n) {
            throw new InvalidOperationException("Can't complete an array which is not an incomplete array!");
        }

        public virtual void CompleteStruct(IEnumerable<TStructUnion.Field> fields) {
            throw new InvalidOperationException("Can't complete a struct which is not an incomplete struct!");
        }

        public virtual void CompleteUnion(IEnumerable<TStructUnion.Field> fields) {
            throw new InvalidOperationException("Can't complete a union which is not an incomplete union!");
        }

        public virtual bool IsComplete => false;
        public virtual bool IsFunc => false;
        public virtual bool IsObject => false;
        public virtual bool IsCharacter => false;
        public virtual bool IsInteger => false;
        public virtual bool IsReal => false;
        public virtual bool IsArithmetic => false;
        public virtual bool IsScalar => false;
        public virtual bool IsAggregate => false;
        public virtual bool IsPointer => false;
        public virtual bool IsArray => false;
        public virtual bool IsVarArray => false;
        public virtual bool IsStruct => false;
        public virtual bool IsUnion => false;

        public abstract int Size { get; }

        /// <summary>
        /// Qualify this type.
        /// </summary>
        /// <param name="qualifiers"></param>
        /// <param name="lr"></param>
        /// <returns></returns>
        public T Qualify(TQualifiers qualifiers, T.LR lr, Store store) {
            return new T(this, qualifiers, lr, store);
        }

        /// <summary>
        /// Qualify this type with const.
        /// </summary>
        /// <param name="lr"></param>
        /// <returns></returns>
        public T Const(T.LR lr = T.LR.L, Store store = Store.NONE) {
            return Qualify(TQualifiers.C, lr, store);
        }

        /// <summary>
        /// Qualify this type without qualifier.
        /// </summary>
        /// <param name="lr"></param>
        /// <returns></returns>
        public T None(T.LR lr = T.LR.L, Store store = Store.NONE) {
            return Qualify(TQualifiers.N, lr, store);
        }
    }

    public abstract class TObject : TUnqualified {
        public override bool IsObject => true;
    }

    public abstract class TScalar : TObject {
        public override bool IsComplete => true;
        public override bool IsScalar => true;
    }

    public abstract class TArithmetic : TScalar {
        public override bool IsArithmetic => true;
        public abstract int Rank { get; }
    }

    public abstract class TReal : TArithmetic {
        public override bool IsReal => true;
    }

    public abstract class TInteger : TReal {
        public override bool IsInteger => true;
        public abstract BigInteger MAX { get; }
        public abstract BigInteger MIN { get; }
    }

    public abstract class TCharacter : TInteger {
        public override int Size => 1;
        public override bool IsCharacter => true;
    }

    /// <summary>
    /// Holds all the three qualifiers.
    /// TODO: Support restrict and volatile.
    /// </summary>
    public class TQualifiers {
        public readonly bool isConstant;
        public readonly bool isRestrict;
        public readonly bool isVolatile;
        public override string ToString() {
            return string.Format("{0}{1}{2}", isConstant ? "const " : "", isRestrict ? " restrict " : "", isVolatile ? " volatile " : "");
        }
        public static TQualifiers operator |(TQualifiers q1, TQualifiers q2) {
            var tuple = new Tuple<bool, bool, bool>(q1.isConstant || q2.isConstant, q1.isRestrict || q2.isRestrict, q1.isVolatile || q2.isVolatile);
            return dict[tuple];
        }

        /// <summary>
        /// Static constructor to build all the instance and the dictionary.
        /// </summary>
        static TQualifiers() {
            N = new TQualifiers(false, false, false);
            C = new TQualifiers(true, false, false);
            R = new TQualifiers(false, true, false);
            V = new TQualifiers(false, false, true);
            CR = new TQualifiers(true, true, false);
            CV = new TQualifiers(true, false, true);
            RV = new TQualifiers(false, true, true);
            CRV = new TQualifiers(true, true, true);
            dict = new Dictionary<Tuple<bool, bool, bool>, TQualifiers> {
                { new Tuple<bool, bool, bool>(false, false, false), N },
                { new Tuple<bool, bool, bool>(true, false, false), C },
                { new Tuple<bool, bool, bool>(false, true, false), R },
                { new Tuple<bool, bool, bool>(false, false, true), V },
                { new Tuple<bool, bool, bool>(true, true, false), CR },
                { new Tuple<bool, bool, bool>(true, false, true), CV },
                { new Tuple<bool, bool, bool>(false, true, true), RV },
                { new Tuple<bool, bool, bool>(true, true, true), CRV }
            };
        }

        public static readonly TQualifiers N;
        public static readonly TQualifiers C;
        public static readonly TQualifiers R;
        public static readonly TQualifiers V;
        public static readonly TQualifiers CR;
        public static readonly TQualifiers CV;
        public static readonly TQualifiers RV;
        public static readonly TQualifiers CRV;
        public static readonly Dictionary<Tuple<bool, bool, bool>, TQualifiers> dict;

        /// <summary>
        /// Private constructor to make sure that there is only one instance for every combination of qualifiers.
        /// </summary>
        /// <param name="isConstant"></param>
        /// <param name="isRestrict"></param>
        /// <param name="isVolatile"></param>
        private TQualifiers(bool isConstant, bool isRestrict, bool isVolatile) {
            this.isConstant = isConstant;
            this.isRestrict = isRestrict;
            this.isVolatile = isVolatile;
        }
    }

    /// <summary>
    /// A type is composed with an unqualified type, qualifiers and storage specifier.
    /// </summary>
    public sealed class T {

        public enum LR {
            L,
            R
        }

        public T(TUnqualified baseType, TQualifiers qualifiers, LR lr, Store store) {
            this.nake = baseType;
            this.qualifiers = qualifiers;
            this.lr = lr;
            this.store = store;
        }

        public override bool Equals(object obj) {
            return Equals(obj as T);
        }

        public bool Equals(T t) {
            return t == null ? false : t.nake.Equals(nake)
                && t.qualifiers.Equals(qualifiers)
                && t.store == store
                && t.lr == lr;
        }

        public override int GetHashCode() {
            return nake.GetHashCode();
        }

        public override string ToString() {
            string storeStr;
            switch (store) {
                case Store.AUTO:        storeStr = "auto ";     break;
                case Store.EXTERN:      storeStr = "extern ";   break;
                case Store.REGISTER:    storeStr = "register "; break;
                case Store.STATIC:      storeStr = "static ";   break;
                default:                storeStr = "";          break;
            }
            return storeStr + qualifiers.ToString() + nake.ToString();
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
            return new T(nested.nake, qualifiers | nested.qualifiers, lr, store);
        }

        /// <summary>
        /// Return the same type but as an rvalue.
        /// 
        /// Notice for RValue, there is no storage specifier.
        /// </summary>
        /// <returns></returns>
        public T R() {
            return IsRValue ? this : new T(nake, qualifiers, LR.R, Store.NONE);
        }

        /// <summary>
        /// Pointer derivation.
        /// </summary>
        /// <param name="qualifier"></param>
        /// <param name="lr"></param>
        /// <returns></returns>
        public T Ptr(TQualifiers qualifiers = null, Store store = Store.NONE) {
            qualifiers = qualifiers ?? TQualifiers.N;
            return new T(new TPointer(this), qualifiers, LR.L, store);
        }

        /// <summary>
        /// Complete array derivation.
        /// </summary>
        /// <param name="qualifiers"></param>
        /// <param name="lr"></param>
        /// <returns></returns>
        public T Arr(int n, TQualifiers qualifiers = null, Store store = Store.NONE) {
            qualifiers = qualifiers ?? TQualifiers.N;
            return new T(new TArray(this, n), qualifiers, LR.L, store);
        }

        /// <summary>
        /// Incomplete array derivation.
        /// </summary>
        /// <param name="qualifiers"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public T IArr(TQualifiers qualifiers = null, Store store = Store.NONE) {
            qualifiers = qualifiers ?? TQualifiers.N;
            return new T(new TArray(this), qualifiers, LR.L, store);
        }

        /// <summary>
        /// Variable length array derivation.
        /// </summary>
        /// <param name="qualifiers"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public T VArr(TQualifiers qualifiers = null, Store store = Store.NONE) {
            qualifiers = qualifiers ?? TQualifiers.N;
            return new T(new TVarArray(this), qualifiers, LR.L, store);
        }

        public void CompleteArr(int n) { nake.CompleteArr(n); }
        public void CompleteStruct(IEnumerable<TStructUnion.Field> fields) { nake.CompleteStruct(fields); }
        public void CompleteUnion(IEnumerable<TStructUnion.Field> fields) { nake.CompleteUnion(fields); }

        public bool IsComplete => nake.IsComplete;
        public bool IsFunc => nake.IsFunc;
        public bool IsObject => nake.IsObject;
        public bool IsCharacter => nake.IsCharacter;
        public bool IsInteger => nake.IsInteger;
        public bool IsReal => nake.IsReal;
        public bool IsArithmetic => nake.IsArithmetic;
        public bool IsScalar => nake.IsScalar;
        public bool IsAggregate => nake.IsAggregate;
        public bool IsPointer => nake.IsPointer;
        public bool IsArray => nake.IsArray;
        public bool IsVarArray => nake.IsVarArray;
        public bool IsStruct => nake.IsStruct;
        public bool IsUnion => nake.IsUnion;

        public int Size => nake.Size;

        public bool IsLValue => lr == LR.L;
        public bool IsRValue => lr == LR.R;
        public bool IsModifiable => IsLValue && (!qualifiers.isConstant);

        public readonly TUnqualified nake;
        public readonly TQualifiers qualifiers;
        public readonly LR lr;
        public readonly Store store;

    }

    public static class TPromotion {

        /// <summary>
        /// Integer promotion.
        /// Those type with rank less than int will be promoted to int.
        /// 
        /// The promoted type should be rvalue.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T IntPromote(this T t) {
            TArithmetic ta = t.nake as TArithmetic;
            if (ta.Rank < TInt.Instance.Rank) {
                return TInt.Instance.Qualify(t.qualifiers, T.LR.R, Store.NONE);
            } else {
                return t;
            }
        }
     
    }
}
