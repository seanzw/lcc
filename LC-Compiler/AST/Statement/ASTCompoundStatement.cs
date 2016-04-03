using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.AST {
    public sealed class ASTCompoundStatement : ASTStatement {

        public ASTCompoundStatement(IEnumerable<ASTStatement> statements) {
            this.statements = statements;
        }

        public override int GetLine() {
            return statements.First().GetLine();
        }

        public override bool Equals(object obj) {
            ASTCompoundStatement x = obj as ASTCompoundStatement;
            return Equals(x);
        }

        public bool Equals(ASTCompoundStatement x) {
            return x == null ? false : x.statements.SequenceEqual(statements);
        }

        public override int GetHashCode() {
            return statements.GetHashCode();
        }

        public readonly IEnumerable<ASTStatement> statements;
    }
}
