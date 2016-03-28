using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {

    /// <summary>
    /// Enum type is represented as unsigned int.
    /// This is used for semantic analysis only.
    /// </summary>
    public sealed class TypeEnum : UnqualifiedType {

        /// <summary>
        /// Initialize an incomplete enum type with a tag.
        /// </summary>
        /// <param name="isConstant"></param>
        public TypeEnum(string tag) {
            this.tag = tag;
        }

        /// <summary>
        /// Initialize an incomplete enum type with anonymous but distinct tag.
        /// Notice that the tag name should be illegal for user.
        /// </summary>
        public TypeEnum() {
            tag = "enum@" + id++;
        }

        /// <summary>
        /// Notice that enum type is always complete.
        /// </summary>
        /// <returns></returns>
        public override bool isCompleted() {
            return true;
        }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The tag for this enum type.
        /// </summary>
        public string tag;

        private static int id = 0;
    }
}
