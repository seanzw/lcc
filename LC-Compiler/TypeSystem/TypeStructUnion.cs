using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public sealed class TStructUnion : TObject {

        public enum Kind {
            STRUCT,
            UNION
        }

        public struct Field {
            public string name;
            public T type;
            public int offset;

            public Field(string name, T type, int offset) {
                this.name = name;
                this.type = type;
                this.offset = offset;
            }

            public override bool Equals(object obj) {
                return obj is Field && Equals((Field)obj);
            }

            public override int GetHashCode() {
                return offset;
            }

            public bool Equals(Field o) {
                return o.name.Equals(name) && o.type.Equals(type) && o.offset.Equals(offset);
            }
        }

        public TStructUnion(string tag, IEnumerable<Field> fields, Kind kind) {
            this.tag = tag;
            this.fields = fields;
            this.kind = kind;
        }

        public TStructUnion(IEnumerable<Field> fields, Kind kind) {
            this.fields = fields;
            this.kind = kind;
            tag = (kind == Kind.STRUCT ? "struct@" : "union@") + (idx++);
        }

        /// <summary>
        /// Get the type of a field.
        /// Returns null if this is not a field name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetType(string name) {
            foreach (var field in fields) 
                if (field.name.Equals(name)) return field.type;
            return null;
        }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj) {
            var o = obj as TStructUnion;
            return o == null ? false : o.kind == kind
                && o.fields.SequenceEqual(fields);
        }

        public override int GetHashCode() {
            return kind.GetHashCode();
        }

        public override string ToString() {
            return "struct " + tag;
        }

        public readonly string tag;

        public readonly IEnumerable<Field> fields;

        public readonly Kind kind;

        private static int idx = 0;
    }

}
