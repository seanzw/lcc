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

    public abstract class Breakable : Stmt {
        public readonly string breakLabel;
        public Breakable(string breakLabel) {
            this.breakLabel = breakLabel;
        }
    }

    public abstract class Loop : Breakable {
        public readonly string continueLabel;
        public Loop(string breakLabel, string continueLabel) : base(breakLabel) {
            this.continueLabel = continueLabel;
        }
    }

    public sealed class Switch : Breakable {
        public readonly LinkedList<Tuple<string, ConstIntExpr>> cases;
        public readonly string defaultLabel;
        public readonly Expr expr;
        public readonly Stmt stmt;
        public Switch(string breakLabel, LinkedList<Tuple<string, ConstIntExpr>> cases,
            string defaultLabel, Expr expr, Stmt stmt) : base(breakLabel) {
            this.cases = cases;
            this.defaultLabel = defaultLabel;
            this.expr = expr;
            this.stmt = stmt;
        }
    }

    public sealed class While : Loop {
        public readonly Expr expr;
        public readonly Stmt stmt;
        public While(string breakLabel, string continueLabel, Expr expr, Stmt stmt) : base(breakLabel, continueLabel) {
            this.expr = expr;
            this.stmt = stmt;
        }
    }

    public sealed class Do : Loop {
        public readonly Expr expr;
        public readonly Stmt stmt;
        public Do(string breakLabel, string continueLabel, Expr expr, Stmt stmt) : base(breakLabel, continueLabel) {
            this.expr = expr;
            this.stmt = stmt;
        }
    }

    public sealed class GoTo : Stmt {
        public string label;
        public GoTo(string label) {
            this.label = label;
        }
    }

    public sealed class VoidStmt : Stmt {
        private static VoidStmt instance = new VoidStmt();
        public static VoidStmt Instance => instance;
    }
}
