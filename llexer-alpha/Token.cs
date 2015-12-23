using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOKEN = System.String;

namespace llexer {
    public class Token {
        public Token(TOKEN type, string src) {
            this.type = type;
            this.src = src;
        }
        public override string ToString() {
            return type + ": " + src;
        }
        public readonly TOKEN type;
        public readonly string src;
    }
}
