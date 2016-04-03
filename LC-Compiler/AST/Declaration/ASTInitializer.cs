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
    public sealed class ASTDesignator : ASTNode {

        public ASTDesignator(ASTExpr expr) { this.expr = expr; }
        public ASTDesignator(ASTIdentifier identifier) { this.identifier = identifier; }

        public override int GetLine() {
            return expr != null ? expr.GetLine() : identifier.GetLine();
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTDesignator);
        }

        public bool Equals(ASTDesignator x) {
            return x != null
                && NullableEquals(x.expr, expr)
                && NullableEquals(x.identifier, identifier);
        }

        public override int GetHashCode() {
            return GetLine();
        }

        public readonly ASTExpr expr;
        public readonly ASTIdentifier identifier;
    }

    /// <summary>
    /// init-item : designation_opt initializer;
    /// </summary>
    public sealed class ASTInitItem : ASTNode {

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

        public override int GetLine() {
            return designators == null ? initializer.GetLine() : designators.First().GetLine();
        }

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
    public sealed class ASTInitializer : ASTNode {

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

        public override int GetLine() {
            return expr != null ? expr.GetLine() : items.First().GetLine();
        }

        public readonly ASTExpr expr;
        public readonly IEnumerable<ASTInitItem> items;
    }
}
