using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {

    public sealed class Labeled : Stmt {
        public readonly string label;
        public readonly Stmt stmt;
        public Labeled(string label, Stmt stmt) {
            this.label = label;
            this.stmt = stmt;
        }
    }

    public sealed class CompoundStmt : Stmt {
        public readonly IEnumerable<Stmt> stmts;
        public CompoundStmt(IEnumerable<Stmt> stmts) {
            this.stmts = stmts;
        }
    }

    public sealed class If : Stmt {
        public readonly Expr expr;
        public readonly Stmt then;
        public readonly Stmt other;
        public If(Expr expr, Stmt then, Stmt other) {
            this.expr = expr;
            this.then = then;
            this.other = other;
        }
    }

    public sealed class VoidStmt : Stmt {
        private static VoidStmt instance = new VoidStmt();
        public static VoidStmt Instance => instance;
    }
}
