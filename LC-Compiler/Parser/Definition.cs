using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Parserc.Parserc;
using lcc.SyntaxTree;

namespace lcc.Parser {
    public static partial class Parser {

        /// <summary>
        /// translation-unit
        ///     : external-declaration
        ///     | translation-unit external-declaration
        ///     ;
        ///     
        /// external-declaration
        ///     : functioin-definition
        ///     | declaration
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, SyntaxTree.Program> TranslationUnit() {
            return FunctionDefintion().Cast<Token.Token, Node, FuncDef>()
                .Or(Declaration().Cast<Token.Token, Node, Declaration>()).Plus()
                .Select((LinkedList<Node> nodes) => new SyntaxTree.Program(nodes));
        }

        /// <summary>
        /// function-definition
        ///     : declaration-specifiers declarator declaration-list_opt compound-statement
        ///     ;
        ///     
        /// declaration-list
        ///     : declaration
        ///     | declaration-list declaration
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, FuncDef> FunctionDefintion() {
            return DeclarationSpecifiers()
                .Bind(specifiers => Declarator()
                .Bind(declarator => Declaration().Plus().ElseNull()
                .Bind(declarations => CompoundStatement()
                .Select(statement => new FuncDef(specifiers, declarator, declarations, statement)))));
        }
    }
}
