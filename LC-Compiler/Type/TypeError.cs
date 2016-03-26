using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeError : Type {

        public TypeError(string msg) : base(true, true, 0) {
            this.msg = msg;
        }

        public override Type Composite(Type other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "TypeError: " + msg;
        }

        public string msg;
    }
}
