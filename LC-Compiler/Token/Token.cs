using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Token {
    abstract class Token {
        public readonly int line;
        protected Token(int line) {
            this.line = line;
        }
    }
}
