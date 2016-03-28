using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public abstract class ASTException : Exception {

        public ASTException(int line, string msg) {
            this.line = line;
            this.msg = msg;
        }

        public override string ToString() {
            return "TypeError: line " + line + " " + msg;
        }

        public readonly string msg;
        public readonly int line;

    }

    public sealed class ASTErrUnknownType : ASTException {
        public ASTErrUnknownType(int line, string name) 
            : base(line, "unknown type " + name) { }
    }

    public sealed class ASTErrEscapedSequenceOutOfRange : ASTException {
        public ASTErrEscapedSequenceOutOfRange(int line, string sequence)
            : base(line, string.Format("escaped sequence out of range.\n\t{0}", sequence)) { }
    }
}
