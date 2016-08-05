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
        public override void ToX86(X86Gen gen) {
            gen.Tag(X86Gen.Seg.TEXT, label);
            stmt.ToX86(gen);
        }
    }

    public sealed class CompoundStmt : Node {
        public readonly IEnumerable<Node> stmts;
        public CompoundStmt(IEnumerable<Node> stmts) {
            this.stmts = stmts;
        }
        public override void ToX86(X86Gen gen) {
            foreach (var stmt in stmts) {
                stmt.ToX86(gen);
            }
        }

        /// <summary>
        /// Generate the code for this compound statement,
        /// with label placed at the end of the block.
        /// {
        ///     ...
        ///     label:
        /// }
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="label"></param>
        public void ToX86WithLabel(X86Gen gen, string label) {
            foreach (var stmt in stmts) {
                stmt.ToX86(gen);
            }
            gen.Tag(X86Gen.Seg.TEXT, label);
        }
    }

    public sealed class If : Node {
        public readonly Expr expr;
        public readonly Node then;
        /// <summary>
        /// Null if there is no associate else statement.
        /// </summary>
        public readonly Node other;
        /// <summary>
        /// Else label.
        /// </summary>
        public readonly string elseLabel;
        /// <summary>
        /// End label.
        /// </summary>
        public readonly string endIfLabel;
        public If(Expr expr, Node then, Node other, string elseLabel, string endIfLabel) {
            this.expr = expr;
            this.then = then;
            this.other = other;
            this.elseLabel = elseLabel;
            this.endIfLabel = endIfLabel;
        }
        public override void ToX86(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, "if");
            gen.Branch(expr, other != null ? elseLabel : endIfLabel, false);

            /// Generate code for then branch.
            /// Remember to jump to endif label since
            /// "If the first substatement is reached via a label, the second substatement is not executed.
            gen.Comment(X86Gen.Seg.TEXT, "then");
            then.ToX86(gen);
            gen.Inst(X86Gen.jmp, endIfLabel);
            if (other != null) {
                gen.Comment(X86Gen.Seg.TEXT, "else");
                gen.Tag(X86Gen.Seg.TEXT, elseLabel);
                other.ToX86(gen);
            }
            gen.Tag(X86Gen.Seg.TEXT, endIfLabel);
        }
        
    }

    public abstract class Breakable : Node {
        public readonly string breakLabel;
        public Breakable(string breakLabel) {
            this.breakLabel = breakLabel;
        }
    }

    public abstract class Loop : Breakable {
        /// <summary>
        /// Body is a block.
        /// </summary>
        public readonly CompoundStmt body;
        /// <summary>
        /// Should be placed at the end of the body.
        /// </summary>
        public readonly string continueLabel;
        /// <summary>
        /// Second or more iteration start label.
        /// </summary>
        public readonly string secondPlusLabel;
        /// <summary>
        /// The first iteration start label.
        /// </summary>
        public readonly string firstLabel;
        public Loop(
            string breakLabel,
            string continueLabel,
            string secondPlusLabel,
            string firstLabel,
            CompoundStmt body
            ) : base(breakLabel) {
            this.continueLabel = continueLabel;
            this.secondPlusLabel = secondPlusLabel;
            this.firstLabel = firstLabel;
            this.body = body;
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
        public While(
            string breakLabel, 
            string continueLabel,
            string secondPlusLabel, 
            string firstLabel,
            Expr expr,
            CompoundStmt body
            ) : base(breakLabel, continueLabel, secondPlusLabel, firstLabel, body) {
            this.expr = expr;
        }
        /// <summary>
        /// Basic structure:
        /// 
        /// first_label:
        ///     # pred
        ///     je break_label
        ///     # body
        ///     jmp first_label
        /// break_label:
        ///     
        /// </summary>
        /// <param name="gen"></param>
        public override void ToX86(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, "while pred");
            gen.Tag(X86Gen.Seg.TEXT, firstLabel);
            gen.Branch(expr, breakLabel, false);

            gen.Comment(X86Gen.Seg.TEXT, "while body");
            body.ToX86WithLabel(gen, continueLabel);
            gen.Inst(X86Gen.jmp, firstLabel);

            gen.Comment(X86Gen.Seg.TEXT, "while end");
            gen.Tag(X86Gen.Seg.TEXT, breakLabel);
        }
    }

    public sealed class Do : Loop {
        public readonly Expr expr;
        public Do(
            string breakLabel, 
            string continueLabel, 
            string secondPlusLabel,
            string firstLabel,
            Expr expr, 
            CompoundStmt body
            ) : base(breakLabel, continueLabel, secondPlusLabel, firstLabel, body) {
            this.expr = expr;
        }
        /// <summary>
        /// structure:
        /// first_label:
        ///     # body {
        ///     ...
        ///     continue_label: ;
        ///     }
        ///     # pred
        ///     jmp first_label    
        /// break_label:
        /// 
        /// </summary>
        /// <param name="gen"></param>
        public override void ToX86(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, "do body");
            gen.Tag(X86Gen.Seg.TEXT, firstLabel);
            body.ToX86WithLabel(gen, continueLabel);
            gen.Comment(X86Gen.Seg.TEXT, "do pred");
            gen.Branch(expr, firstLabel, true);
            gen.Comment(X86Gen.Seg.TEXT, "do end");
            gen.Tag(X86Gen.Seg.TEXT, breakLabel);
        }
    }

    public sealed class For : Loop {
        /// <summary>
        /// This is a node since it can be an expression or a declaration with initializer.
        /// Nullable.
        /// </summary>
        public readonly Node init;
        /// <summary>
        /// Never be null.
        /// </summary>
        public readonly Expr pred;
        /// <summary>
        /// This is evaluated as a void expression.
        /// Nullable.
        /// </summary>
        public readonly Node iter;
        public For(
            string breakLabel,
            string continueLabel,
            string secondPlusLabel,
            string firstLabel,
            Node init, 
            Expr pred, 
            Node iter, 
            CompoundStmt body
            ) : base(breakLabel, continueLabel, secondPlusLabel, firstLabel, body) {
            this.init = init;
            this.pred = pred;
            this.iter = iter;
        }
        /// <summary>
        /// The basic structure is like:
        ///     
        ///     # init
        ///     jmp first_label
        /// second_plus_label:
        ///     # iter
        /// first_label:
        ///     # pred
        ///     je break_label
        ///     # body
        ///     jmp second_plus_label
        /// break_label:
        ///     
        /// </summary>
        /// <param name="gen"></param>
        public override void ToX86(X86Gen gen) {

            /// Generate the initialize code.
            /// Jump to first_label if iter is not omitted.
            gen.Comment(X86Gen.Seg.TEXT, "for init");
            if (init != null) {
                init.ToX86(gen);
                if (iter != null) gen.Inst(X86Gen.jmp, firstLabel);
            }

            /// Generate the iterate code.
            gen.Comment(X86Gen.Seg.TEXT, "for iter");
            if (iter != null) {
                gen.Tag(X86Gen.Seg.TEXT, secondPlusLabel);
                iter.ToX86(gen);
            }

            /// Generate the controlling (predicating) code.
            gen.Comment(X86Gen.Seg.TEXT, "for pred");
            gen.Tag(X86Gen.Seg.TEXT, firstLabel);
            gen.Branch(pred, breakLabel, false);

            /// Generate body code.
            gen.Comment(X86Gen.Seg.TEXT, "for body");
            body.ToX86WithLabel(gen, continueLabel);
            gen.Inst(X86Gen.jmp, iter != null ? secondPlusLabel : firstLabel);

            /// Generate break labe.
            gen.Comment(X86Gen.Seg.TEXT, "for end");
            gen.Tag(X86Gen.Seg.TEXT, breakLabel);
        }
    }

    public sealed class Return : Node {
        public readonly string label;
        public readonly Expr expr;
        public Return(string label, Expr expr) {
            this.label = label;
            this.expr = expr;
        }
        public override string ToString() {
            return string.Format("return {0}", expr == null ? "" : expr.ToString());
        }
        public override void ToX86(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, ToString());
            if (expr == null) {
                gen.Inst(X86Gen.jmp, label);
                return;
            }
            var ret = expr.ToX86Expr(gen);
            switch (expr.Type.Kind) {
                case TKind.CHAR:
                case TKind.SCHAR:
                case TKind.UCHAR:
                    if (ret == X86Gen.Ret.PTR) {
                        gen.Inst(X86Gen.mov, X86Gen.al, X86Gen.eax.Addr(X86Gen.Size.BYTE));
                    }
                    gen.Inst(X86Gen.jmp, label);
                    return;
                case TKind.SHORT:
                case TKind.USHORT:
                    if (ret == X86Gen.Ret.PTR) {
                        gen.Inst(X86Gen.mov, X86Gen.ax, X86Gen.eax.Addr(X86Gen.Size.WORD));
                    }
                    gen.Inst(X86Gen.jmp, label);
                    return;
                case TKind.PTR:
                case TKind.UINT:
                case TKind.ULONG:
                case TKind.INT:
                case TKind.LONG:
                    if (ret == X86Gen.Ret.PTR) {
                        gen.Inst(X86Gen.mov, X86Gen.eax, X86Gen.eax.Addr());
                    }
                    gen.Inst(X86Gen.jmp, label);
                    return;
                case TKind.DOUBLE:
                    if (ret == X86Gen.Ret.PTR) {
                        gen.Inst(X86Gen.movsd, X86Gen.xmm0, X86Gen.eax.Addr(X86Gen.Size.QWORD));
                    }
                    gen.Inst(X86Gen.sub, X86Gen.esp, 8);
                    gen.Inst(X86Gen.movsd, X86Gen.esp.Addr(X86Gen.Size.QWORD), X86Gen.xmm0);
                    gen.Inst(X86Gen.fld, X86Gen.esp.Addr(X86Gen.Size.QWORD));
                    gen.Inst(X86Gen.add, X86Gen.esp, 8);
                    gen.Inst(X86Gen.jmp, label);
                    return;
            }

            throw new NotImplementedException();
        }
    }

    public sealed class GoTo : Node {
        public readonly string label;
        public GoTo(string label) {
            this.label = label;
        }
        public override string ToString() {
            return string.Format("goto {0}", label);
        }
        public override void ToX86(X86Gen gen) {
            gen.Comment(X86Gen.Seg.TEXT, ToString());
            gen.Inst(X86Gen.jmp, label);
        }
    }

    public sealed class VoidStmt : Node {
        private static VoidStmt instance = new VoidStmt();
        public static VoidStmt Instance => instance;
        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="gen"></param>
        public override void ToX86(X86Gen gen) {
            
        }
    }
}
