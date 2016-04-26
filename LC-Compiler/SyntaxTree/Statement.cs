using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.SyntaxTree {
    public abstract class Stmt : Node {}

    public sealed class STLabeled : Stmt, IEquatable<STLabeled> {

        public STLabeled(Id identifier, Stmt statement) {
            this.identifier = identifier;
            this.statement = statement;
        }

        public override Position Pos => identifier.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STLabeled);
        }

        public bool Equals(STLabeled x) {
            return x != null
                && x.identifier.Equals(identifier)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return identifier.GetHashCode();
        }

        public readonly Id identifier;
        public readonly Stmt statement;
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

    public sealed class STCompoundStmt : Stmt, IEquatable<STCompoundStmt> {

        public STCompoundStmt(IEnumerable<Stmt> stmts) {
            this.stmts = stmts;
        }

        public override Position Pos => stmts.First().Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STCompoundStmt);
        }

        public bool Equals(STCompoundStmt x) {
            return x != null && x.stmts.SequenceEqual(stmts);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public readonly IEnumerable<Stmt> stmts;
    }

    public sealed class STIf : Stmt, IEquatable<STIf> {


        public STIf(
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
            return Equals(obj as STIf);
        }

        public bool Equals(STIf x) {
            return x != null
                && x.pos.Equals(pos)
                && x.expr.Equals(expr)
                && x.then.Equals(then)
                && x.other == null ? other == null : x.other.Equals(other);
        }

        public override int GetHashCode() {
            return expr.GetHashCode();
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

    public sealed class STVoidStmt : Stmt {

        public STVoidStmt(int line) {
            this.pos = new Position { line = line };
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as STVoidStmt);
        }

        public bool Equals(STVoidStmt x) {
            return x != null && x.pos.Equals(pos);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        private readonly Position pos;
    }
}
