using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Type {
    public sealed class TypeError : UnqualifiedType {

        public TypeError(string msg) {
            this.msg = msg;
        }

        public override UnqualifiedType Composite(UnqualifiedType other) {
            throw new NotImplementedException();
        }

        public override bool isCompleted() {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "TypeError: " + msg;
        }

        public string msg;
    }
}
