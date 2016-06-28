using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {

    public sealed class Labeled : Node {
        public readonly string label;
        public readonly Node stmt;
        public Labeled(string label, Node stmt) {
            this.label = label;
            this.stmt = stmt;
        }
    }

    public sealed class CompoundStmt : Node {
        public readonly IEnumerable<Node> stmts;
        public CompoundStmt(IEnumerable<Node> stmts) {
            this.stmts = stmts;
        }
    }

    public sealed class If : Node {
        public readonly Expr expr;
        public readonly Node then;
        public readonly Node other;
        public If(Expr expr, Node then, Node other) {
            this.expr = expr;
            this.then = then;
            this.other = other;
        }
    }

    public abstract class Breakable : Node {
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
        public readonly Node stmt;
        public Switch(string breakLabel, LinkedList<Tuple<string, ConstIntExpr>> cases,
            string defaultLabel, Expr expr, Node stmt) : base(breakLabel) {
            this.cases = cases;
            this.defaultLabel = defaultLabel;
            this.expr = expr;
            this.stmt = stmt;
        }
    }

    public sealed class While : Loop {
        public readonly Expr expr;
        public readonly Node stmt;
        public While(string breakLabel, string continueLabel, Expr expr, Node stmt) : base(breakLabel, continueLabel) {
            this.expr = expr;
            this.stmt = stmt;
        }
    }

    public sealed class Do : Loop {
        public readonly Expr expr;
        public readonly Node stmt;
        public Do(string breakLabel, string continueLabel, Expr expr, Node stmt) : base(breakLabel, continueLabel) {
            this.expr = expr;
            this.stmt = stmt;
        }
    }

    public sealed class Return : Node {
        public readonly string label;
        public readonly Expr expr;
        public Return(string label, Expr expr) {
            this.label = label;
            this.expr = expr;
        }
    }

    public sealed class GoTo : Node {
        public readonly string label;
        public GoTo(string label) {
            this.label = label;
        }
    }

    public sealed class VoidStmt : Node {
        private static VoidStmt instance = new VoidStmt();
        public static VoidStmt Instance => instance;
    }
}
