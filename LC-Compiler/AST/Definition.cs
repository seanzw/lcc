using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {

    public sealed class Program : Node {
        public readonly IEnumerable<Node> nodes;
        public Program(IEnumerable<Node> nodes) {
            this.nodes = nodes;
        }
        public override void ToX86(X86Gen gen) {
            foreach (var node in nodes) {
                node.ToX86(gen);
            }
        }
    }

    public sealed class FuncDef : Node {
        public readonly string name;
        public readonly string returnLabel;
        public readonly TFunc type;
        public readonly IEnumerable<Tuple<string, T>> parameters;
        public readonly CompoundStmt body;
        public readonly Env env;
        public readonly bool isGlobal;
        public FuncDef(
            string name,
            string returnLabel,
            TFunc type,
            IEnumerable<Tuple<string, T>> parameters,
            CompoundStmt body, 
            Env env,
            bool isGlobal
            ) {
            this.name = name;
            this.returnLabel = returnLabel;
            this.type = type;
            this.parameters = parameters;
            this.body = body;
            this.env = env;
            this.isGlobal = isGlobal;
        }
        public override void ToX86(X86Gen gen) {
            env.Dump(gen);

            gen.Label(X86Gen.Seg.TEXT, "_" + name, isGlobal);
            gen.Inst(X86Gen.push, X86Gen.ebp);
            gen.Inst(X86Gen.mov, X86Gen.ebp, X86Gen.esp);
            gen.Inst(X86Gen.sub, X86Gen.esp, env.size);

            body.ToX86(gen);


            gen.Label(X86Gen.Seg.TEXT, returnLabel);
            gen.Inst(X86Gen.add, X86Gen.esp, env.size);
            gen.Inst(X86Gen.pop, X86Gen.ebp);
            gen.Inst(X86Gen.ret);
        }
    }
}
