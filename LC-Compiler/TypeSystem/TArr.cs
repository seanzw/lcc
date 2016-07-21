using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {

    public abstract class TArr : TObject {
        public readonly T element;
        public override bool IsArray => true;
        public override bool IsAggregate => true;
        public TArr(TKind Kind, T element) : base(Kind) {
            this.element = element;
        }
    }

    /// <summary>
    /// Represent an incomplete array.
    /// </summary>
    public sealed class TIArr : TArr, IEquatable<TIArr> {
        public TIArr(T element) : base(TKind.IARR, element) { }
        public override bool IsComplete => false;
        public override bool IsDefined => false;
        public override int Bits {
            get {
                throw new InvalidOperationException("Can't take size of an incomplete array.");
            }
        }

        /// <summary>
        /// Complete this array with length n.
        /// </summary>
        /// <param name="n"></param>
        public override TCArr DefArr(int n) {
            if (n <= 0) throw new ArgumentException("Illegal length of an array!");
            else return new TCArr(element, n);
        }

        public bool Equals(TIArr t) {
            return t != null && t.element.Equals(element);
        }
        public override bool Equals(object obj) {
            return Equals(obj as TIArr);
        }
        public override int GetHashCode() {
            return element.GetHashCode();
        }

        /// <summary>
        /// Two array types are compatible if:
        /// 1. Their element types are compatible.
        /// 2. If both have constant size, that size is the same.
        /// 
        /// Note:
        /// arrays of unknown bound is compatible with any array of compatible element type.
        /// arrays of variable length is compatible with any array of compatible element type.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Compatible(TUnqualified other) {
            if (other.IsArray) {
                TArr a = other as TArr;
                if (element.Compatible(a.element)) {
                    // They points to compatible type.
                    // Check if other is VLA.
                    return true;
                }
            }
            return false;
        }


        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return string.Format("({0})[]", element);
        }
    }

    /// <summary>
    /// Represent a complete array with constant size.
    /// </summary>
    public sealed class TCArr : TArr, IEquatable<TCArr> {

        public TCArr(T element, int n) : base(TKind.CARR, element) {
            this.n = n;
        }
        public override bool IsComplete => n != -1;
        public override bool IsDefined => IsComplete;
        public override int Bits {
            get {
                if (n == -1) throw new InvalidOperationException("Can't take size of an incomplete array.");
                else return n * element.Bits;
            }
        }


        public bool Equals(TCArr t) {
            return t != null && t.n == n && t.element.Equals(element);
        }
        public override bool Equals(object obj) {
            return Equals(obj as TCArr);
        }
        public override int GetHashCode() {
            return element.GetHashCode();
        }

        /// <summary>
        /// Two array types are compatible if:
        /// 1. Their element types are compatible.
        /// 2. If both have constant size, that size is the same.
        /// 
        /// Note:
        /// arrays of unknown bound is compatible with any array of compatible element type.
        /// arrays of variable length is compatible with any array of compatible element type.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Compatible(TUnqualified other) {
            if (other.IsArray) {
                TArr a = other as TArr;
                if (element.Compatible(a.element)) {
                    // They points to compatible type.
                    // Check if other is VLA.
                    if (a is TVArr) return true;
                    if (IsComplete && a.IsComplete) return n == (a as TCArr).n;
                    else return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            if (IsComplete) return string.Format("({0})[{1}]", element, n);
            else return string.Format("({0})[]", element);
        }

        private int n;
    }

    /// <summary>
    /// Represents a variable length array.
    /// </summary>
    public sealed class TVArr : TArr, IEquatable<TVArr> {

        public TVArr(T element) : base(TKind.VARR, element) {
        }
        public override bool IsArray => true;
        public override bool IsComplete => true;
        public override bool IsDefined => true;
        public override int Bits {
            get {
                throw new InvalidOperationException("Can't take the bits of a variable length array.");
            }
        }

        /// <summary>
        /// Arrays with variable length is compatible with any array of compatible element type.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Compatible(TUnqualified other) {
            if (other.IsArray) {
                TArr a = other as TArr;
                return element.Compatible(a.element);
            } else {
                return false;
            }
        }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public bool Equals(TVArr x) {
            return x != null && x.element.Equals(element);
        }

        public override bool Equals(object obj) {
            return Equals(obj as TVArr);
        }

        public override int GetHashCode() {
            return element.GetHashCode();
        }
        public override string ToString() {
            return string.Format("({0})[x]", element);
        }
    }

}
