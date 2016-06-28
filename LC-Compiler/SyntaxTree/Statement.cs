using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lcc.AST;
using lcc.TypeSystem;

namespace lcc.SyntaxTree {
    public abstract class Stmt : Node {

    }

    public sealed class Labeled : Stmt, IEquatable<Labeled> {

        public Labeled(Id id, Stmt statement) {
            this.id = id;
            this.stmt = statement;
        }

        public override Position Pos => id.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as Labeled);
        }

        public bool Equals(Labeled x) {
            return x != null
                && x.id.Equals(id)
                && x.stmt.Equals(stmt);
        }

        public override int GetHashCode() {
            return id.GetHashCode();
        }

        public override AST.Node ToAST(Env env) {
            if (Id.IsReservedIdentifier(id.symbol)) throw new EReservedIdentifier(Pos, id.symbol);
            if (env.GetLable(id.symbol) != null) throw new ERedfineLabel(Pos, id.symbol);
            string transformed = env.AddLabel(id.symbol);
            return new AST.Labeled(transformed, stmt.ToAST(env));
        }

        public readonly Id id;
        public readonly Stmt stmt;
    }

    public sealed class Case : Stmt, IEquatable<Case> {

        public Case(Expr expr, Stmt statement) {
            this.expr = expr;
            this.stmt = statement;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as Case);
        }

        public bool Equals(Case x) {
            return x != null
                && x.expr.Equals(expr)
                && x.stmt.Equals(stmt);
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public override AST.Node ToAST(Env env) {
            /// Check the switch statement.
            Switch sw = env.GetSwitch();
            if (sw == null) {
                throw new Error(Pos, "default statement not in switch statement");
            }

            /// The expression of a case should be constant integer expression.
            AST.ConstIntExpr c = expr.GetASTExpr(env) as AST.ConstIntExpr;
            if (c == null) {
                throw new Error(Pos, "the expression of a case should be constant integer expression");
            }

            /// No two of the case constant expressions shall have the same value.
            /// TODO: The conversion.
            foreach (var e in sw.cases) {
                if (c.value == e.Item2.value) {
                    throw new Error(Pos, string.Format("duplicate value {0} in case", c.value));
                }
            }

            string label = env.AllocCaseLabel();

            sw.cases.AddLast(new Tuple<string, ConstIntExpr>(label, c));

            AST.Node s = stmt.ToAST(env);

            return new AST.Labeled(label, s);
        }

        public readonly Expr expr;
        public readonly Stmt stmt;
    }

    public sealed class Default : Stmt, IEquatable<Default> {

        public Default(Stmt statement) {
            this.stmt = statement;
        }

        public override Position Pos => stmt.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as Default);
        }

        public bool Equals(Default x) {
            return x != null && x.stmt.Equals(stmt);
        }

        public override int GetHashCode() {
            return stmt.GetHashCode();
        }

        public override AST.Node ToAST(Env env) {

            /// Check the switch statement.
            Switch sw = env.GetSwitch();
            if (sw == null) {
                throw new Error(Pos, "default statement not in switch statement");
            }

            /// At most one default statement in a switch.
            if (sw.defaultLabel != null) {
                throw new Error(Pos, "at most one default label in a switch statement");
            }

            string label = env.AllocDefaultLabel();
            sw.defaultLabel = label;

            AST.Node s = stmt.ToAST(env);

            return new AST.Labeled(label, s);
        }

        public readonly Stmt stmt;
    }

    public sealed class CompoundStmt : Stmt, IEquatable<CompoundStmt> {

        public CompoundStmt(IEnumerable<Stmt> stmts) {
            this.stmts = stmts;
        }

        public override Position Pos => stmts.First().Pos;

        public override bool Equals(object obj) {
            return Equals(obj as CompoundStmt);
        }

        public bool Equals(CompoundStmt x) {
            return x != null && x.stmts.SequenceEqual(stmts);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override AST.Node ToAST(Env env) {
            env.PushBlockScope();
            LinkedList<AST.Node> results = new LinkedList<AST.Node>();
            foreach (var stmt in stmts) {
                results.AddLast(stmt.ToAST(env));
            }
            env.PopScope();
            return new AST.CompoundStmt(results);
        }

        //public IEnumerable<AST.Stmt> FuncBody(Env env, T type, IEnumerable<)

        public readonly IEnumerable<Stmt> stmts;
    }

    public sealed class If : Stmt, IEquatable<If> {

        public If(
            int line,
            Expr expr,
            Stmt then,
            Stmt other
            ) {
            this.pos = new Position { line = line };
            this.expr = expr;
            this.then = then;
            this.other = other;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as If);
        }

        public bool Equals(If x) {
            return x != null
                && x.pos.Equals(pos)
                && x.expr.Equals(expr)
                && x.then.Equals(then)
                && x.other == null ? other == null : x.other.Equals(other);
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        /// <summary>
        /// A selection statement is a block whose scope is a strict subset
        /// of the scope of its enclosing block.
        /// 
        /// The controlling expression of an if statement shall have scalar type.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override AST.Node ToAST(Env env) {
            env.PushBlockScope();
            AST.Expr e = expr.GetASTExpr(env);
            if (!e.Type.IsScalar) {
                throw new ETypeError(Pos, string.Format("expecting scalar type, given {0}", e.Type));
            }

            AST.Node t = then.ToAST(env);
            AST.Node o = other != null ? other.ToAST(env) : null;

            env.PopScope();
            return new AST.If(e, t, o);
        }

        private readonly Position pos;
        public readonly Expr expr;
        public readonly Stmt then;
        public readonly Stmt other;
    }

    public abstract class Breakable : Stmt {
        public string breakLabel;
    }

    public sealed class Switch : Breakable, IEquatable<Switch> {

        public Switch(
            int line,
            Expr expr,
            Stmt statement
            ) {
            this.pos = new Position { line = line };
            this.expr = expr;
            this.stmt = statement;
            this.cases = new LinkedList<Tuple<string, ConstIntExpr>>();
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as Switch);
        }

        public bool Equals(Switch x) {
            return x != null
                && x.pos.Equals(pos)
                && x.expr.Equals(expr)
                && x.stmt.Equals(stmt);
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public override AST.Node ToAST(Env env) {

            /// The controlling expression shall have integer type.
            e = expr.GetASTExpr(env);
            if (!e.Type.IsInteger) {
                throw new ETypeError(Pos, "the controlling expression of switch statement shall have integer type");
            }

            /// Integer promotions are performed on the controlling expression.
            e = e.IntPromote();

            env.PushSwitch(this);

            /// Semantic check the statment.
            AST.Node s = stmt.ToAST(env);

            env.PopBreakable();

            return new AST.Switch(breakLabel, cases, defaultLabel, e, s);
        }

        /// <summary>
        /// For case and default statement.
        /// </summary>
        public LinkedList<Tuple<string, AST.ConstIntExpr>> cases;
        public string defaultLabel;
        public AST.Expr e {
            get;
            private set;
        }

        public readonly Expr expr;
        public readonly Stmt stmt;

        private readonly Position pos;
    }

    public abstract class Iteration : Breakable {
        public string continueLabel;
    }

    public sealed class While : Iteration, IEquatable<While> {

        public While(Expr expr, Stmt statement) {
            this.expr = expr;
            this.stmt = statement;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as While);
        }

        public bool Equals(While x) {
            return x != null
                && x.expr.Equals(expr)
                && x.stmt.Equals(stmt);
        }

        public override int GetHashCode() {
            return expr.GetHashCode() | stmt.GetHashCode();
        }

        public override AST.Node ToAST(Env env) {

            /// An iteration statement is a block.
            env.PushBlockScope();

            /// The controlling expression should have scalar type.
            AST.Expr e = expr.GetASTExpr(env);
            if (!e.Type.IsScalar) {
                throw new ETypeError(Pos, "the controlling expression of iteration statement should have scalar type");
            }

            /// The loop body is also a block.
            env.PushBlockScope();

            /// Add this loop to the environemnt.
            env.PushLoop(this);

            AST.Node s = stmt.ToAST(env);

            env.PopBreakable();

            env.PopScope();
            env.PopScope();

            return new AST.While(breakLabel, continueLabel, e, s);
        }

        public readonly Expr expr;
        public readonly Stmt stmt;
    }

    public sealed class Do : Iteration, IEquatable<Do> {

        public Do(Expr expr, Stmt statement) {
            this.expr = expr;
            this.stmt = statement;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            Do x = obj as Do;
            return Equals(x);
        }

        public bool Equals(Do x) {
            return x != null
                && x.expr.Equals(expr)
                && x.stmt.Equals(stmt);
        }

        public override int GetHashCode() {
            return expr.GetHashCode() | stmt.GetHashCode();
        }

        public override AST.Node ToAST(Env env) {
            env.PushBlockScope();
            env.PushBlockScope();
            env.PushLoop(this);

            AST.Node s = stmt.ToAST(env);

            env.PopBreakable();
            env.PopScope();

            AST.Expr e = expr.GetASTExpr(env);
            if (!e.Type.IsScalar) {
                throw new ETypeError(Pos, "the controlling expression of iteration statement should be have scalar type");
            }

            env.PopScope();
            return new AST.Do(breakLabel, continueLabel, e, s);
        }

        public readonly Expr expr;
        public readonly Stmt stmt;
    }

    public sealed class For : Iteration, IEquatable<For> {

        public For(
            int line,
            Expr init,
            Expr pred,
            Expr iter,
            Stmt statement) {
            this.pos = new Position { line = line };
            this.init = init;
            this.pred = pred;
            this.iter = iter;
            this.statement = statement;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as For);
        }

        public bool Equals(For x) {
            return x != null
                && x.pos.Equals(pos)
                && NullableEquals(x.init, init)
                && NullableEquals(x.pred, pred)
                && NullableEquals(x.iter, iter)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return statement.GetHashCode();
        }

        private readonly Position pos;
        public readonly Expr init;
        public readonly Expr pred;
        public readonly Expr iter;
        public readonly Stmt statement;
    }

    public sealed class Continue : Stmt, IEquatable<Continue> {

        public Continue(
            int line
            ) {
            this.pos = new Position { line = line };
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as Continue);
        }

        public bool Equals(Continue x) {
            return x != null && x.pos.Equals(pos);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override AST.Node ToAST(Env env) {

            Iteration i = env.GetLoop();
            if (i == null) {
                throw new Error(Pos, "continue statement not in an iteration statement");
            }

            return new AST.GoTo(i.continueLabel);
        }

        private readonly Position pos;
    }

    public sealed class Break : Stmt, IEquatable<Break> {

        public Break(
            int line
            ) {
            this.pos = new Position { line = line };
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as Break);
        }

        public bool Equals(Break x) {
            return x != null && x.pos.Equals(pos);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override AST.Node ToAST(Env env) {
            Breakable b = env.GetBreakable();
            if (b == null) {
                throw new Error(Pos, "break statement not in breakable statement");
            }

            return new AST.GoTo(b.breakLabel);
        }

        private readonly Position pos;
    }

    public sealed class Return : Stmt, IEquatable<Return> {

        public Return(
            int line,
            Expr expr
            ) {
            this.pos = new Position { line = line };
            this.expr = expr;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as Return);
        }

        public bool Equals(Return x) {
            return x != null && x.pos.Equals(pos)
                && x.expr == null ? expr == null : x.expr.Equals(expr);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override AST.Node ToAST(Env env) {

            /// A return statement with an expression shall not appear in a function whose return type is void. 
            /// A return statement without an expression shall only appear in a function whose return type is void.
            TFunc f = env.GetFuncType();
            string returnLabel = env.GetReturnLabel();

            if (expr != null) {
                if (f.ret.IsVoid) {
                    throw new Error(Pos, "a return statement with an expression shall not appear a function whose return type is void");
                }
                AST.Expr e = expr.GetASTExpr(env);
                if (!Assign.SimpleAssignable(f.ret, e)) {
                    throw new ETypeError(Pos, string.Format("cannot assign {0} to {1}", e.Type, f.ret));
                }
                return new AST.Return(returnLabel, e.ImplicitConvert(f.ret));
            } else {
                if (!f.ret.IsVoid) {
                    throw new Error(Pos, "a return statement without an expression shall only appear in a function whose return type is void");
                }
                return new AST.Return(returnLabel, null);
            }
        }

        private readonly Position pos;
        public readonly Expr expr;
    }

    public sealed class GoTo : Stmt, IEquatable<GoTo> {

        public GoTo(
            int line,
            string label
            ) {
            this.pos = new Position { line = line };
            this.label = label;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as GoTo);
        }

        public bool Equals(GoTo x) {
            return x != null && x.pos.Equals(pos)
                && x.label.Equals(label);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override AST.Node ToAST(Env env) {
            if (Id.IsReservedIdentifier(label)) {
                throw new EReservedIdentifier(Pos, label);
            }

            string rename = env.GetLable(label);
            if (rename == null) {
                throw new ErrUndefinedIdentifier(Pos, label);
            }

            return new AST.GoTo(rename);
        }

        private readonly Position pos;
        public readonly string label;
    }

    public sealed class VoidStmt : Stmt {

        public VoidStmt(int line) {
            this.pos = new Position { line = line };
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as VoidStmt);
        }

        public bool Equals(VoidStmt x) {
            return x != null && x.pos.Equals(pos);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override AST.Node ToAST(Env env) {
            return AST.VoidStmt.Instance;
        }

        private readonly Position pos;
    }
}
