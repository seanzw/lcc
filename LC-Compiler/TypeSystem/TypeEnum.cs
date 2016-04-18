using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {

    /// <summary>
    /// Enum type is represented as unsigned int.
    /// This is used for semantic analysis only.
    /// </summary>
    public sealed class TypeEnum : TUnqualified {

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
        /// Complete the definition of the enum type.
        /// </summary>
        /// <param name="enums"></param>
        public override void DefEnum(IDictionary<string, int> enums) {
            if (this.enums != null) throw new InvalidOperationException("Can't complete an enum which is already complete.");
            else this.enums = enums;
        }

        /// <summary>
        /// Notice that enum type is always complete, but not always defined.
        /// </summary>
        /// <returns></returns>
        public override bool IsComplete => true;

        /// <summary>
        /// Whether this enum is defined.
        /// </summary>
        public override bool IsDefined => enums != null;

        /// <summary>
        /// Enumerator is represent as int.
        /// </summary>
        public override int Bits => 32;

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The tag for this enum type.
        /// </summary>
        public readonly string tag;

        /// <summary>
        /// The value for enumerators.
        /// </summary>
        public IDictionary<string, int> enums;

        private static int id = 0;
    }
}
