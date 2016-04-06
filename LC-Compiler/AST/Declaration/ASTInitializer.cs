using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.Token;

namespace lcc.AST {

    /// <summary>
    /// [ constant-expression ]
    /// .identfier
    /// </summary>
    public sealed class ASTDesignator : ASTNode, IEquatable<ASTDesignator> {

        public ASTDesignator(ASTExpr expr) { this.expr = expr; }
        public ASTDesignator(ASTId id) { this.id = id; }

        public override Position Pos => expr != null ? expr.Pos : id.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as ASTDesignator);
        }

        public bool Equals(ASTDesignator x) {
            return x != null
                && NullableEquals(x.expr, expr)
                && NullableEquals(x.id, id);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public readonly ASTExpr expr;
        public readonly ASTId id;
    }

    /// <summary>
    /// init-item : designation_opt initializer;
    /// </summary>
    public sealed class ASTInitItem : ASTNode, IEquatable<ASTInitItem> {

        public ASTInitItem(ASTInitializer initializer, IEnumerable<ASTDesignator> designators = null) {
            this.initializer = initializer;
            this.designators = designators;
        } 

        public bool Equals(ASTInitItem x) {
            return x != null
                && x.initializer.Equals(initializer)
                && NullableEquals(x.designators, designators);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTInitItem);
        }

        public override int GetHashCode() {
            return initializer.GetHashCode();
        }

        public override Position Pos => designators == null ? initializer.Pos : designators.First().Pos;

        public readonly ASTInitializer initializer;
        public readonly IEnumerable<ASTDesignator> designators;
    }

    /// <summary>
    /// initializer
    ///     : assignment-expression
    ///     | { initializer-list }
    ///     | { initializer-list , }
    ///     ;
    /// </summary>
    public sealed class ASTInitializer : ASTNode, IEquatable<ASTInitializer> {

        /// <summary>
        /// initializer : assignment-expression;
        /// </summary>
        /// <param name="expr"></param>
        public ASTInitializer(ASTExpr expr) { this.expr = expr; }
        public ASTInitializer(IEnumerable<ASTInitItem> items) { this.items = items; }

        public bool Equals(ASTInitializer x) {
            return x != null
                && NullableEquals(x.expr, expr)
                && NullableEquals(x.items, items);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTInitializer);
        }

        public override int GetHashCode() {
            return expr != null ? expr.GetHashCode() : items.First().GetHashCode();
        }

        public override Position Pos => expr != null ? expr.Pos : items.First().Pos;

        public readonly ASTExpr expr;
        public readonly IEnumerable<ASTInitItem> items;
    }
}
