using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Parserc.Parserc;
using lcc.AST;

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
        public static Parserc.Parser<Token.Token, ASTProgram> TranslationUnit() {
            return FunctionDefintion().Cast<Token.Token, ASTNode, ASTFuncDefinition>()
                .Or(Declaration().Cast<Token.Token, ASTNode, ASTDeclaration>()).Plus()
                .Select(nodes => new ASTProgram(nodes));
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
        public static Parserc.Parser<Token.Token, ASTFuncDefinition> FunctionDefintion() {
            return DeclarationSpecifiers()
                .Bind(specifiers => Declarator()
                .Bind(declarator => Declaration().Plus().ElseNull()
                .Bind(declarations => CompoundStatement()
                .Select(statement => new ASTFuncDefinition(specifiers, declarator, declarations, statement)))));
        }
    }
}
