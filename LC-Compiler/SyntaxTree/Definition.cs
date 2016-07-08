using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lcc.AST;
using lcc.TypeSystem;

namespace lcc.SyntaxTree {
    public sealed class Program : Node, IEquatable<Program> {

        public Program(IEnumerable<Node> nodes) {
            this.nodes = nodes;
        }

        public override Position Pos => nodes.First().Pos;

        public override bool Equals(object obj) {
            return Equals(obj as Program);
        }

        public bool Equals(Program x) {
            return x.nodes.SequenceEqual(nodes);
        }

        public override int GetHashCode() {
            return nodes.Aggregate(0, (hash, node) => hash ^ node.GetHashCode());
        }

        public override AST.Node ToAST(Env env) {
            var ns = new LinkedList<AST.Node>();
            foreach (var node in nodes) {
                ns.AddLast(node.ToAST(env));
            }
            return new AST.Program(ns);
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
            this.body = statement;
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
                && x.body.Equals(body);
        }

        public override int GetHashCode() {
            return specifiers.GetHashCode();
        }

        public override AST.Node ToAST(Env env) {

            T baseType = specifiers.GetT(env);
            var result = declarator.Declare(env, baseType);

            if (declarations != null) {
                throw new Error(Pos, "sorry we donot support old-style function definition");
            }

            /// The identifier declared in a function definition (which is the name of the function) shall
            /// have a function type
            if (!result.Item2.IsFunc) {
                throw new ETypeError(Pos, "function definition should have function type");
            }

            /// The storage-class specifier, if any, shall be either extern or static.
            if (specifiers.storage != StoreSpec.Kind.EXTERN &&
                specifiers.storage != StoreSpec.Kind.NONE &&
                specifiers.storage != StoreSpec.Kind.STATIC) {
                throw new Error(Pos, string.Format("illegal storage specifier {0} in function definition", specifiers.storage));
            }

            /// Determine the linkage.
            var link = specifiers.storage == StoreSpec.Kind.STATIC ? SymbolEntry.Link.INTERNAL : SymbolEntry.Link.EXTERNAL;

            var entry = env.GetSymbol(result.Item1);
            if (entry == null) {
                /// This identifier has not been used.
                env.AddFunc(result.Item1, result.Item2, link, Pos);
            } else {
                /// This identifier has been declared.
                /// Check if it's a function type.
                if (!entry.type.Equals(result.Item2)) {
                    throw new ERedefineSymbolTypeConflict(Pos, result.Item1, entry.type, result.Item2);
                }

                /// Check if the function has already been defined.
                if (entry.type.IsDefined) {
                    throw new ERedefineFunction(Pos, result.Item1);
                }

                /// Define the function.
                entry.type.DefFunc();
            }

            TFunc type = result.Item2.nake as TFunc;

            /// Push a new scope.
            env.PushFuncScope(result.Item1, type, result.Item3, Pos);

            /// Semantic check the function body.
            AST.CompoundStmt b = body.ToCompoundStmt(env);

            string returnLabel = env.GetReturnLabel();

            env.PopScope();

            return new AST.FuncDef(
                result.Item1, 
                returnLabel, 
                type, 
                result.Item3, 
                b, 
                env.ASTEnv, 
                specifiers.storage != StoreSpec.Kind.STATIC
                );
        }

        public readonly DeclSpecs specifiers;
        public readonly Declarator declarator;
        public readonly IEnumerable<Declaration> declarations;
        public readonly CompoundStmt body;
    }
}
