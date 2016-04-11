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

        public readonly string tag;
        public IEnumerable<Field> fields;
    }

    public sealed class TStruct : TStructUnion {
        public TStruct(string tag, IEnumerable<Field> fields = null) : base(tag, fields) {
            if (fields != null) size = Padding(fields);
        }
        public TStruct(IEnumerable<Field> fields = null) : base("struct@" + (idx++), fields) {
            if (fields != null) size = Padding(fields);
        }
        public override bool IsStruct => true;
        public override int Size {
            get {
                if (fields == null) throw new InvalidOperationException("Can't take size of an incomplete struct.");
                else return size;
            }
        }

        public override string ToString() {
            return string.Format("struct {0}", tag);
        }

        public override void DefStruct(IEnumerable<Field> fields) {
            if (this.fields != null) throw new InvalidOperationException("Can't complete a complete struct.");
            else this.fields = fields;
        }

        /// <summary>
        /// Determine the size of the struct.
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static int Padding(IEnumerable<Field> fields) {
            return 0;
        }
        private readonly int size;
        private static int idx = 0;
    }

    public sealed class TUnion : TStructUnion {
        public TUnion(string tag, IEnumerable<Field> fields = null) : base(tag, fields) {
            if (fields != null) size = fields.Aggregate(0, (size, field) => Math.Max(size, field.type.Size));
        }
        public TUnion(IEnumerable<Field> fields = null) : base("union@" + (idx++), fields) {
            if (fields != null) size = fields.Aggregate(0, (size, field) => Math.Max(size, field.type.Size));
        }
        public override bool IsUnion => true;
        public override int Size {
            get {
                if (fields == null) throw new InvalidOperationException("Can't take size of an incomplete struct.");
                else return size;
            }
        }
        public override void DefUnion(IEnumerable<Field> fields) {
            if (this.fields != null) base.DefUnion(fields);
            else this.fields = fields;
        }

        public override string ToString() {
            return string.Format("union {0}", tag);
        }

        private readonly int size;
        private static int idx = 0;
    }
}
