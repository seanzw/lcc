using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Token {
    public sealed class T_IDENTIFIER : Token {
        public T_IDENTIFIER(int line, string name) : base(line) {
            this.name = name;
        }

        public override bool Equals(object obj) {
            T_IDENTIFIER i = obj as T_IDENTIFIER;
            return i == null ? false : base.Equals(obj)
                && i.name.Equals(name);
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly string name;
    }
}
