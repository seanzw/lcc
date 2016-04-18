using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.SyntaxTree {
    public class Error : System.Exception {

        public Error(Position pos, string msg) {
            this.pos = pos;
            this.msg = msg;
        }

        public override string ToString() {
            return "TypeError: " + pos + " " + msg;
        }

        public readonly string msg;
        public readonly Position pos;

    }

    public sealed class ErrRedefineTag : Error {
        public ErrRedefineTag(Position pos, string tag, Position previous)
            : base(pos, string.Format("redefine tag {0}, previous defined at {1}", tag, previous)) { }
    }

    public sealed class ErrDeclareTagAsDifferentType : Error {
        public ErrDeclareTagAsDifferentType(Position pos, string tag, Position previous, TUnqualified previoudType)
            : base(pos, string.Format("declare tag {0} as different type, previous declared at {1} as {2}", tag, previous, previoudType)) { }
    }

    public sealed class ErrUnknownType : Error {
        public ErrUnknownType(Position pos, string name) 
            : base(pos, "unknown type " + name) { }
    }

    public sealed class ErrEscapedSequenceOutOfRange : Error {
        public ErrEscapedSequenceOutOfRange(Position pos, string sequence)
            : base(pos, string.Format("escaped sequence out of range.\n\t{0}", sequence)) { }
    }

    public sealed class ErrIntegerLiteralOutOfRange : Error {
        public ErrIntegerLiteralOutOfRange(Position pos)
            : base(pos, "integer literal is too large to be represented in any integer type.") { }
    }

    public sealed class ErrUndefinedIdentifier : Error {
        public ErrUndefinedIdentifier(Position pos, string name)
            : base(pos, string.Format("undefined identfifier {0}", name)) { }
    }

}
