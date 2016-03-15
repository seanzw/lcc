using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTVoidStatement : ASTStatement {

        public ASTVoidStatement(int line) {
            this.line = line;
        }

        public override int GetLine() {
            return line;
        }

        public override bool Equals(object obj) {
            ASTVoidStatement x = obj as ASTVoidStatement;
            return Equals(x);
        }

        public bool Equals(ASTVoidStatement x) {
            return x == null ? false : x.line == line;
        }

        public override int GetHashCode() {
            return line;
        }

        public readonly int line;
    }
}
