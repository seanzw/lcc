using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.SyntaxTree {
    public class STException : Exception {

        public STException(Position pos, string msg) {
            this.pos = pos;
            this.msg = msg;
        }

        public override string ToString() {
            return "TypeError: " + pos + " " + msg;
        }

        public readonly string msg;
        public readonly Position pos;

    }

    public sealed class ASTErrUnknownType : STException {
        public ASTErrUnknownType(Position pos, string name) 
            : base(pos, "unknown type " + name) { }
    }

    public sealed class ASTErrEscapedSequenceOutOfRange : STException {
        public ASTErrEscapedSequenceOutOfRange(Position pos, string sequence)
            : base(pos, string.Format("escaped sequence out of range.\n\t{0}", sequence)) { }
    }

    public sealed class ASTErrIntegerLiteralOutOfRange : STException {
        public ASTErrIntegerLiteralOutOfRange(Position pos)
            : base(pos, "integer literal is too large to be represented in any integer type.") { }
    }

    public sealed class ASTErrUndefinedIdentifier : STException {
        public ASTErrUndefinedIdentifier(Position pos, string name)
            : base(pos, string.Format("undefined identfifier {0}", name)) { }
    }

}
