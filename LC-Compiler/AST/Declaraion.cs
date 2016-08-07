using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {
    public sealed class Declaraions : Node {
        public readonly IEnumerable<Declaration> declarations;
        public Declaraions(IEnumerable<Declaration> declarations) {
            this.declarations = declarations;
        }
        public override void ToX86(X86Gen gen) {
            foreach (var declaration in declarations) {
                declaration.ToX86(gen);
            }
        }
    }

    public abstract class Declaration : Node {
        public readonly Initializer initializer;
        public Declaration(Initializer initializer) {
            this.initializer = initializer;
        }
    }

    public sealed class DynamicDeclaration : Declaration {
        public readonly DynamicObjExpr obj;
        public DynamicDeclaration(DynamicObjExpr obj, Initializer initializer) : base(initializer) {
            this.obj = obj;
        }
        public override void ToX86(X86Gen gen) {
            if (initializer != null) {
                var ret = obj.ToX86Expr(gen);
                Debug.Assert(ret == X86Gen.Ret.PTR);
                gen.Inst(X86Gen.push, X86Gen.eax);
                initializer.Initialize(gen);
                gen.Inst(X86Gen.pop, X86Gen.eax);
            }
        }
    }

    public abstract class Initializer {
        /// <summary>
        /// Assume the pointer to the obj is at the top of the stack.
        /// </summary>
        /// <param name="gen"></param>
        public abstract void Initialize(X86Gen gen);
    }

    public sealed class SimpleInitializer : Initializer {
        T type;
        Expr expr;
        public SimpleInitializer(T type, Expr expr) {
            this.type = type;
            this.expr = expr;
        }
        public override void Initialize(X86Gen gen) {
            var ret = expr.ToX86Expr(gen);
            switch (type.Kind) {
                case TKind.CHAR:
                case TKind.SCHAR:
                case TKind.UCHAR:
                    if (ret == X86Gen.Ret.PTR) gen.Inst(X86Gen.mov, X86Gen.al, X86Gen.eax.Addr(X86Gen.Size.BYTE));
                    gen.Inst(X86Gen.mov, X86Gen.ecx, X86Gen.esp.Addr());
                    gen.Inst(X86Gen.mov, X86Gen.ecx.Addr(X86Gen.Size.BYTE), X86Gen.al);
                    break;
                case TKind.SHORT:
                case TKind.USHORT:
                    if (ret == X86Gen.Ret.PTR) gen.Inst(X86Gen.mov, X86Gen.ax, X86Gen.eax.Addr(X86Gen.Size.WORD));
                    gen.Inst(X86Gen.mov, X86Gen.ecx, X86Gen.esp.Addr());
                    gen.Inst(X86Gen.mov, X86Gen.ecx.Addr(X86Gen.Size.WORD), X86Gen.ax);
                    break;
                case TKind.INT:
                case TKind.UINT:
                case TKind.LONG:
                case TKind.ULONG:
                case TKind.PTR:
                case TKind.ENUM:
                    if (ret == X86Gen.Ret.PTR) gen.Inst(X86Gen.mov, X86Gen.eax, X86Gen.eax.Addr(X86Gen.Size.DWORD));
                    gen.Inst(X86Gen.mov, X86Gen.ecx, X86Gen.esp.Addr());
                    gen.Inst(X86Gen.mov, X86Gen.ecx.Addr(X86Gen.Size.DWORD), X86Gen.eax);
                    break;
                default: throw new NotImplementedException();
            }
        }
    }
}
