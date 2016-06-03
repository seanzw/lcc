using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.SyntaxTree {
    public sealed class STProgram : Node, IEquatable<STProgram> {

        public STProgram(IEnumerable<Node> nodes) {
            this.nodes = nodes;
        }

        public override Position Pos => nodes.First().Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STProgram);
        }

        public bool Equals(STProgram x) {
            return x.nodes.SequenceEqual(nodes);
        }

        public override int GetHashCode() {
            return nodes.Aggregate(0, (hash, node) => hash ^ node.GetHashCode());
        }

        public readonly IEnumerable<Node> nodes;
    }

    public sealed class FuncDef : Node, IEquatable<FuncDef> {

        public FuncDef(
            DeclSpecs specifiers,
            Declarator declarator,
            IEnumerable<Declaration> declarations,
            CompoundStmt statement
            ) {
            this.specifiers = specifiers;
            this.declarator = declarator;
            this.declarations = declarations;
            this.statement = statement;
        }

        public override Position Pos => declarator.Pos;

        public override bool Equals(object obj) {
            FuncDef x = obj as FuncDef;
            return Equals(x);
        }

        public bool Equals(FuncDef x) {
            return x == null ? false : base.Equals(x)
                && x.specifiers.Equals(specifiers)
                && x.declarator.Equals(declarator)
                && x.declarations == null ? declarations == null : x.declarations.SequenceEqual(declarations)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return specifiers.GetHashCode();
        }

        public readonly DeclSpecs specifiers;
        public readonly Declarator declarator;
        public readonly IEnumerable<Declaration> declarations;
        public readonly CompoundStmt statement;
    }
}
