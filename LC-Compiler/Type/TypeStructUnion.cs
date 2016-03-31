using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeStructUnion : ObjectType {

        public enum Kind {
            STRUCT,
            UNION
        }

        public struct Field {
            public string name;
            public Type type;
            public int offset;
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

        public TypeStructUnion(string tag, LinkedList<Field> fields, Kind kind) {
            this.tag = tag;
            this.fields = fields;
            this.kind = kind;
        }

        public TypeStructUnion(LinkedList<Field> fields, Kind kind) {
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
        public Type GetType(string name) {
            foreach (var field in fields) 
                if (field.name.Equals(name)) return field.type;
            return null;
        }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj) {
            var o = obj as TypeStructUnion;
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

        public readonly LinkedList<Field> fields;

        public readonly Kind kind;

        private static int idx = 0;
    }

}
