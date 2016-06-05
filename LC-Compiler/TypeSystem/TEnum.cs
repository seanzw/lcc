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
    public sealed class TEnum : TUnqualified {

        /// <summary>
        /// Initialize an incomplete enum type with a tag.
        /// </summary>
        /// <param name="isConstant"></param>
        public TEnum(string tag) {
            this.tag = tag;
        }

        /// <summary>
        /// Initialize an incomplete enum type with anonymous but distinct tag.
        /// Notice that the tag name should be illegal for user.
        /// </summary>
        public TEnum() {
            tag = "enum@" + id++;
        }

        /// <summary>
        /// Complete the definition of the enum type.
        /// </summary>
        /// <param name="enums"></param>
        public void Define(IEnumerable<Tuple<string, int>> enums) {
            if (this.enums != null) throw new InvalidOperationException("Can't complete an enum which is already complete.");
            else this.enums = enums;
        }

        /// <summary>
        /// The enumerated type is incomplete untill after the } that terminates
        /// the list of enumerator declarators.
        /// Which means the enumerated type is complete when it is defined.
        /// </summary>
        /// <returns></returns>
        public override bool IsComplete => IsDefined;

        /// <summary>
        /// Whether this is a enum type.
        /// </summary>
        public override bool IsEnum => true;

        /// <summary>
        /// Whether this enum is defined.
        /// </summary>
        public override bool IsDefined => enums != null;

        /// <summary>
        /// Enumerator is represented as int.
        /// </summary>
        public override int Bits => 32;

        /// <summary>
        /// Each enumerated type shall be compatible with char, a signed integer type,
        /// or an unsigned integer type. The choice of type is implementation-defined,
        /// but shall be capable of representing the values of all the members of the
        /// enumeration.
        /// 
        /// Here the enumerated type is represented as int, thus enum should be compatible
        /// with int.
        /// Notice that enum and int are NOT the same type, thus Equals should return false.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Compatible(TUnqualified other) {
            return Equals(other) || other is TInt;
        }

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
        public IEnumerable<Tuple<string, int>> enums;

        public string Dump() {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} {{\n", ToString());
            foreach (var item in enums) {
                sb.AppendFormat("    {0, -20} = {1, -10}\n", item.Item1, item.Item2);
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        public override string ToString() {
            return "enum " + tag;
        }

        private static int id = 0;
    }
}
