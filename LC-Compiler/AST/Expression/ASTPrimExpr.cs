using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.Token;

namespace lcc.AST {

    sealed class ASTIdentifier : ASTExpr {

        public ASTIdentifier(T_IDENTIFIER token) {
            line = token.line;
            name = token.name;
        }

        public override int GetLine() {
            return line;
        }

        public readonly string name;
        public readonly int line;
    }

    abstract class ASTConstant : ASTExpr {

        public ASTConstant(int line) {
            this.line = line;
        }

        public override int GetLine() {
            return line;
        }

        public readonly int line;
    }

    sealed class ASTConstInt : ASTConstant {

        public ASTConstInt(T_CONST_INT token) : base(token.line) {

        }
    }

    sealed class ASTConstChar : ASTConstant {
        public ASTConstChar(T_CONST_CHAR token) : base(token.line) {

        }
    }

    sealed class ASTConstFloat : ASTConstant {
        public ASTConstFloat(T_CONST_FLOAT token) : base(token.line) {

        }
    }

    sealed class ASTString : ASTExpr {
        public ASTString(T_STRING_LITERAL token) {
            line = token.line;
            text = token.text;
        }

        public override int GetLine() {
            return line;
        }

        public readonly int line;
        public readonly string text;
    }
}
