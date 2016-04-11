using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.SyntaxTree {
    public sealed class STProgram : STNode, IEquatable<STProgram> {

        public STProgram(IEnumerable<STNode> nodes) {
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

        public readonly IEnumerable<STNode> nodes;
    }

    public sealed class STFuncDef : STNode, IEquatable<STFuncDef> {

        public STFuncDef(
            STDeclSpecs specifiers,
            STDeclarator declarator,
            IEnumerable<STDeclaration> declarations,
            STStmt statement
            ) {
            this.specifiers = specifiers;
            this.declarator = declarator;
            this.declarations = declarations;
            this.statement = statement;
        }

        public override Position Pos => declarator.Pos;

        public override bool Equals(object obj) {
            STFuncDef x = obj as STFuncDef;
            return Equals(x);
        }

        public bool Equals(STFuncDef x) {
            return x == null ? false : base.Equals(x)
                && x.specifiers.Equals(specifiers)
                && x.declarator.Equals(declarator)
                && x.declarations == null ? declarations == null : x.declarations.SequenceEqual(declarations)
                && x.statement.Equals(statement);
        }

        public override int GetHashCode() {
            return specifiers.GetHashCode();
        }

        public readonly STDeclSpecs specifiers;
        public readonly STDeclarator declarator;
        public readonly IEnumerable<STDeclaration> declarations;
        public readonly STStmt statement;
    }
}
