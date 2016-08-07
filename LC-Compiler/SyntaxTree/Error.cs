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

    public sealed class EReservedIdentifier : Error {
        public EReservedIdentifier(Position pos, string id)
            : base(pos, string.Format("reserved identifier {0}", id)) { }
    }

    public sealed class ETypeError : Error {
        public ETypeError(Position pos, string msg)
            : base(pos, string.Format("type error: {0}", msg)) { }
    }

    public sealed class ERedfineLabel : Error {
        public ERedfineLabel(Position pos, string label)
            : base(pos, string.Format("redefine lable {0}", label)) { }
    }

    public sealed class ERedefineTag : Error {
        public ERedefineTag(Position pos, string tag, Position previous)
            : base(pos, string.Format("redefine tag {0}, previous defined at {1}", tag, previous)) { }
    }

    public sealed class ERedefineSymbolAsDiffKind : Error {
        public ERedefineSymbolAsDiffKind(Position pos, string symbol, Position previous)
            : base(pos, string.Format("redefine symbol {0} as different kind, previous defined at {1}", symbol, previous)) { }
    }

    public sealed class ERedefineObject : Error {
        public ERedefineObject(Position pos, string symbol, Position previous)
            : base(pos, string.Format("redefinition of object '{0}', previous at {1}.", symbol, previous)) { }
    }

    public sealed class ErrDeclareTagAsDifferentType : Error {
        public ErrDeclareTagAsDifferentType(Position pos, string tag, Position previous, TUnqualified previoudType)
            : base(pos, string.Format("declare tag {0} as different type, previous declared at {1} as {2}", tag, previous, previoudType)) { }
    }

    public sealed class EIllegalInitializer : Error {
        public EIllegalInitializer(Position pos) : base(pos, "illegal initializer (only variables can be initialized.") { }
    }

    public sealed class EIllegalStorageSpecifier : Error {
        public EIllegalStorageSpecifier(Position pos) : base(pos, "illegal storage specifier.") { }
    }

    public sealed class EInternalAfterExternal : Error {
        public EInternalAfterExternal(Position pos, Position previous)
            : base(pos, string.Format("internal linkage after prior external linkage declaration at {0}", previous)) { }
    }

    public sealed class ErrTypeRedefinition : Error {
        public ErrTypeRedefinition(Position pos, T previous, T now)
            : base(pos, string.Format("typedef redefinition with different types ({0} vs {1}).", previous, now)) { }
    }

    public sealed class ERedefineSymbolTypeConflict : Error {
        public ERedefineSymbolTypeConflict(Position pos, string symbol, T previous, T now)
            : base(pos, string.Format("conflict type for '{0}', previous {1}, now {2}", symbol, previous, now)) { }
    }

    public sealed class EUnknownType : Error {
        public EUnknownType(Position pos, string name) 
            : base(pos, "unknown type " + name) { }
    }

    public sealed class ERedefineFunction : Error {
        public ERedefineFunction(Position pos, string name)
            : base(pos, string.Format("redefine function {0}", name)) { }
    }

    public sealed class EEscapedSequenceOutOfRange : Error {
        public EEscapedSequenceOutOfRange(Position pos, string sequence)
            : base(pos, string.Format("escaped sequence out of range.\n\t{0}", sequence)) { }
    }

    public sealed class ErrIntegerLiteralOutOfRange : Error {
        public ErrIntegerLiteralOutOfRange(Position pos)
            : base(pos, "integer literal is too large to be represented in any integer type.") { }
    }

    public sealed class EUndefinedIdentifier : Error {
        public EUndefinedIdentifier(Position pos, string name)
            : base(pos, string.Format("undefined identfifier {0}", name)) { }
    }

}
