using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Token {
    sealed class T_IDENTIFIER : Token {
        public T_IDENTIFIER(int line, string name) : base(line) {
            this.name = name;
        }
        public readonly string name;
    }
}
