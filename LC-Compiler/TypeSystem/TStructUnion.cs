using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {

    public abstract class TStructUnion : TObject, IEquatable<TStructUnion> {

        public struct Field : IEquatable<Field> {
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

        public TStructUnion(string tag, IEnumerable<Field> fields = null) {
            this.tag = tag;
            this.fields = fields;
        }

        public override bool IsComplete => fields != null;
        public override bool IsDefined => IsComplete;

        /// <summary>
        /// Get the type of a field.
        /// Returns null if this is not a field name or this is an incomplete type.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetType(string name) {
            if (fields == null) return null;
            foreach (var field in fields) 
                if (field.name.Equals(name)) return field.type;
            return null;
        }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public bool Equals(TStructUnion x) {
            return x != null && x.tag.Equals(tag)
                && fields == null ? x.fields == null : fields.SequenceEqual(x.fields);
        }

        public override bool Equals(object obj) {
            return Equals(obj as TStructUnion);
        }

        public override int GetHashCode() {
            return tag.GetHashCode();
        }

        public virtual void Define(IEnumerable<Tuple<string, T>> fields) {
            throw new InvalidOperationException("Can't define a struct which is not an undefined struct!");
        }

        public string Dump() {
            if (!IsComplete) {
                return "incomplete struct " + tag;
            } else {
                StringBuilder sb = new StringBuilder();
                sb.Append("struct " + tag + " {\n");
                foreach (var field in fields) {
                    sb.Append(string.Format("{0,-20} {1,-20} {2,-20}\n", field.offset, field.type, field.name));
                }
                sb.Append("}\n");
                return sb.ToString();
            }
        }

        public readonly string tag;
        public IEnumerable<Field> fields;
    }

    public sealed class TStruct : TStructUnion {
        public TStruct(string tag) : base(tag, null) {}
        public TStruct() : base("struct@" + (idx++), null) {}
        public override bool IsStruct => true;
        public override int Bits {
            get {
                if (fields == null) throw new InvalidOperationException("Can't take size of an incomplete struct.");
                else return size;
            }
        }

        public override string ToString() {
            return string.Format("struct {0}", tag);
        }

        public override void Define(IEnumerable<Tuple<string, T>> fields) {
            if (this.fields != null)
                throw new InvalidOperationException("Can't complete a complete struct.");
            var tmp = new LinkedList<Field>();

            // The offset.
            int offset = 0;

            foreach (var field in fields) {
                if (field.Item2.IsBitField) {
                    int nextOffset = AlignTo(offset);
                    if (field.Item2.Bits == 0 || ((offset != nextOffset && (field.Item2.Bits + offset) > nextOffset))) {
                        // As a special case, a bit-field with width of 0 indicated that no further bit-field
                        // is to be packed into the unit in which the previous bit-field, if any, was placed.

                        // If insufficient space remains, whether a bit-field that does not fit is put into the next unit
                        // or overlaps adjacent units is implementation-defined.
                        // Here I choose to put into the next unit.
                        offset = nextOffset;
                    }
                    if (field.Item1 != null) {
                        // This is named bit-field.
                        tmp.AddLast(new Field(field.Item1, field.Item2, offset));
                        offset += field.Item2.Bits;
                    }
                } else {
                    offset = AlignTo(offset);
                    tmp.AddLast(new Field(field.Item1, field.Item2, offset));
                    offset += field.Item2.Align;
                }
            }
            this.fields = tmp;
            size = AlignTo(offset);
        }

        private int size;
        private static int idx = 0;
    }

    public sealed class TUnion : TStructUnion {
        public TUnion(string tag) : base(tag, null) {}
        public TUnion() : base("union@" + (idx++), null) {}
        public override bool IsUnion => true;
        public override int Bits {
            get {
                if (fields == null) throw new InvalidOperationException("Can't take size of an incomplete struct.");
                else return size;
            }
        }
        public override void Define(IEnumerable<Tuple<string, T>> fields) {
            if (this.fields != null) throw new InvalidOperationException("Can't define a union which is alread defined.");
            this.fields = from field in fields select new Field(field.Item1, field.Item2, 0);
            size = this.fields.Aggregate(0, (size, field) => Math.Max(size, field.type.Bits));
        }

        public override string ToString() {
            return string.Format("union {0}", tag);
        }

        private int size;
        private static int idx = 0;
    }
}
