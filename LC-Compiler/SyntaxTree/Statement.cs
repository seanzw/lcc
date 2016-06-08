using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lcc.AST;
using lcc.TypeSystem;

namespace lcc.SyntaxTree {
    public abstract class Stmt : Node {
        public virtual AST.Stmt ToAST(Env env) {
            throw new NotImplementedException();
        }
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

        public override AST.Stmt ToAST(Env env) {
            if (Id.IsReservedIdentifier(id.symbol)) throw new EReservedIdentifier(Pos, id.symbol);
            if (env.GetLable(id.symbol) != null) throw new ERedfineLabel(Pos, id.symbol);
            string transformed = env.AddLabel(id.symbol);
            return new AST.Labeled(transformed, stmt.ToAST(env));
        }

        public readonly Id id;
        public readonly Stmt stmt;
    }

    public sealed class STCase : Stmt, IEquatable<STCase> {

        public STCase(Expr expr, Stmt statement) {
            this.expr = expr;
            this.statement = statement;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STCase);
        }

        public bool Equals(STCase x) {
            return x != null
                && x.expr.Equals(expr)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        public readonly Expr expr;
        public readonly Stmt statement;
    }

    public sealed class STDefault : Stmt, IEquatable<STDefault> {

        public STDefault(Stmt statement) {
            this.statement = statement;
        }

        public override Position Pos => statement.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STDefault);
        }

        public bool Equals(STDefault x) {
            return x != null && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return statement.GetHashCode();
        }

        public readonly Stmt statement;
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

        public override AST.Stmt ToAST(Env env) {
            env.PushScope(ScopeKind.BLOCK);
            LinkedList<AST.Stmt> results = new LinkedList<AST.Stmt>();
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
        public override AST.Stmt ToAST(Env env) {
            env.PushScope(ScopeKind.BLOCK);
            AST.Expr e = expr.GetASTExpr(env);
            if (!e.Type.IsScalar) {
                throw new ETypeError(Pos, string.Format("expecting scalar type, given {0}", e.Type));
            }

            AST.Stmt t = then.ToAST(env);
            AST.Stmt o = other != null ? other.ToAST(env) : null;

            env.PopScope();
            return new AST.If(e, t, o);
        }

        private readonly Position pos;
        public readonly Expr expr;
        public readonly Stmt then;
        public readonly Stmt other;
    }

    public sealed class STSwitch : Stmt, IEquatable<STSwitch> {

        public STSwitch(
            int line,
            Expr expr,
            Stmt statement
            ) {
            this.pos = new Position { line = line };
            this.expr = expr;
            this.statement = statement;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as STSwitch);
        }

        public bool Equals(STSwitch x) {
            return x != null
                && x.pos.Equals(pos)
                && x.expr.Equals(expr)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
        }

        private readonly Position pos;
        public readonly Expr expr;
        public readonly Stmt statement;
    }

    public sealed class STWhile : Stmt, IEquatable<STWhile> {

        public STWhile(Expr expr, Stmt statement) {
            this.expr = expr;
            this.statement = statement;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STWhile);
        }

        public bool Equals(STWhile x) {
            return x != null
                && x.expr.Equals(expr)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return expr.GetHashCode() | statement.GetHashCode();
        }

        public readonly Expr expr;
        public readonly Stmt statement;
    }

    public sealed class STDo : Stmt, IEquatable<STDo> {

        public STDo(Expr expr, Stmt statement) {
            this.expr = expr;
            this.statement = statement;
        }

        public override Position Pos => expr.Pos;

        public override bool Equals(object obj) {
            STDo x = obj as STDo;
            return Equals(x);
        }

        public bool Equals(STDo x) {
            return x != null
                && x.expr.Equals(expr)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return expr.GetHashCode() | statement.GetHashCode();
        }

        public readonly Expr expr;
        public readonly Stmt statement;
    }

    public sealed class STFor : Stmt, IEquatable<STFor> {

        public STFor(
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
            return Equals(obj as STFor);
        }

        public bool Equals(STFor x) {
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

    public sealed class STContinue : Stmt, IEquatable<STContinue> {

        public STContinue(
            int line
            ) {
            this.pos = new Position { line = line };
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as STContinue);
        }

        public bool Equals(STContinue x) {
            return x != null && x.pos.Equals(pos);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        private readonly Position pos;
    }

    public sealed class STBreak : Stmt, IEquatable<STBreak> {

        public STBreak(
            int line
            ) {
            this.pos = new Position { line = line };
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as STBreak);
        }

        public bool Equals(STBreak x) {
            return x != null && x.pos.Equals(pos);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        private readonly Position pos;
    }

    public sealed class STReturn : Stmt, IEquatable<STReturn> {

        public STReturn(
            int line,
            Expr expr
            ) {
            this.pos = new Position { line = line };
            this.expr = expr;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as STReturn);
        }

        public bool Equals(STReturn x) {
            return x != null && x.pos.Equals(pos)
                && x.expr == null ? expr == null : x.expr.Equals(expr);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        private readonly Position pos;
        public readonly Expr expr;
    }

    public sealed class STGoto : Stmt, IEquatable<STGoto> {

        public STGoto(
            int line,
            Id label
            ) {
            this.pos = new Position { line = line };
            this.label = label;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as STGoto);
        }

        public bool Equals(STGoto x) {
            return x != null && x.pos.Equals(pos)
                && x.label.Equals(label);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        private readonly Position pos;
        public readonly Id label;
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

        public override AST.Stmt ToAST(Env env) {
            return AST.VoidStmt.Instance;
        }

        private readonly Position pos;
    }
}
