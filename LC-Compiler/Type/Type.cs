using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public abstract class Type {

        public Type(
            bool isConstant,
            bool isCompleted,
            uint size
            ) {
            this.isConstant = isConstant;
            this.isCompleted = isCompleted;
            this.size = size;
        }

        /// <summary>
        /// Return a composite type of this and other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract Type Composite(Type other);

        public override string ToString() {
            string constStr = isConstant ? "const " : "";
            return constStr;
        }

        /// <summary>
        /// Whether this type is qualified with const.
        /// </summary>
        public readonly bool isConstant;

        /// <summary>
        /// Whether this is a completed type.
        /// </summary>
        public readonly bool isCompleted;

        /// <summary>
        /// The value returned by sizeof.
        /// </summary>
        public readonly uint size;

        // Unsupported.
        //public readonly bool isRestrict;
        //public readonly bool isVolatile;
    }

    public abstract class TypeBuiltIn : Type {

        public TypeBuiltIn(
            bool isConstant,
            uint size
            ) : base(isConstant, true, size) {

        }
    }
}
