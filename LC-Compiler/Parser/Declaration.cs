using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Parserc.Parserc;
using lcc.Token;
using lcc.AST;
namespace lcc.Parser {
    public static partial class Parser {

        /// <summary>
        /// declaration
        ///     : declaration-specifiers init-declarator-list_opt
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTDeclaration> Declaration() {
            return Zero<Token.Token, ASTDeclaration>();
        }

        /// <summary>
        /// declaration-specifiers
        ///     : storage-class-specifier declaration-specifiers_opt
        ///     | type-specifier declaration-specifiers_opt
        ///     | type-qualifier declaration-specifiers_opt
        ///     | function-specifier declaration-specifiers_opt
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTDeclaration> DeclarationSpecifiers() {
            return Zero<Token.Token, ASTDeclaration>();
        }

        /// <summary>
        /// init-declarator
        ///     : declarator
        ///     | declarator = initializer
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTDeclaration> InitDeclarator() {
            return Zero<Token.Token, ASTDeclaration>();
        }

        public static Parserc.Parser<Token.Token, ASTDeclaration> StorageClassSpecifier() {
            return Zero<Token.Token, ASTDeclaration>();
        }


        public static Parserc.Parser<Token.Token, ASTDeclaration> TypeSpecifier() {
            return Zero<Token.Token, ASTDeclaration>();
        }

        public static Parserc.Parser<Token.Token, ASTDeclaration> StructOrUnionSpecifier() {
            return Zero<Token.Token, ASTDeclaration>();
        }

        public static Parserc.Parser<Token.Token, ASTDeclaration> StructOrUnion() {
            return Zero<Token.Token, ASTDeclaration>();
        }

        public static Parserc.Parser<Token.Token, ASTDeclaration> StructDeclaration() {
            return Zero<Token.Token, ASTDeclaration>();
        }

        /// <summary>
        /// initializer
        ///     : assignment-expression
        ///     | { initializer-list }
        ///     | { initializer-list , }
        /// </summary>
        /// <returns></returns>
        //public static Parserc.Parser<Token.Token, ASTInitializer> Initializer() {

        //}

        /// <summary>
        /// initializer-list
        ///     : designation_opt initializer
        ///     | initializer-list designation_opt initializer
        ///     ;
        /// </summary>
        //public static Parserc.Parserc<Token.Token, AS>

        /// <summary>
        /// designation
        ///     : designator-list =
        ///     ;
        /// </summary>
        /// <returns></returns>
        //public static Parserc.Parser<Token.Token, LinkedList<ASTDesignator>> Designation() {
        //    return DesignatorList().Bind(list => Match<T_PUNC_ASSIGN>().Return(list));
        //}

        /// <summary>
        /// designator-list
        ///     : designator
        ///     | designator-list designator
        ///     ;
        /// </summary>
        /// <returns></returns>
        //public static Parserc.Parser<Token.Token, LinkedList<ASTDesignator>> DesignatorList() {
        //    return Designator().Plus();
        //}

        /// <summary>
        /// designator
        ///     : [ const-expression ]
        ///     | . identifier
        ///     ;
        /// </summary>
        /// <returns></returns>
        //public static Parserc.Parser<Token.Token, ASTDesignator> Designator() {
        //    return ConstantExpression()
        //            .Bracket(Match<T_PUNC_SUBSCRIPTL>(), Match<T_PUNC_SUBSCRIPTR>())
        //            .Select(expr => new ASTDesignatorExpr(expr) as ASTDesignator)
        //        .Or(Match<T_PUNC_DOT>()
        //            .Then(Get<T_IDENTIFIER>()
        //            .Select(iden => new ASTDesignatorIden(iden) as ASTDesignator)));
        //}

    }
}
