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
        public static Parserc.Parser<Token.Token, ASTDecl> Declaration() {
            return Zero<Token.Token, ASTDecl>();
        }

        /// <summary>
        /// declaration-specifiers
        ///     : declaration-specifier declaration-specifiers_opt
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, LinkedList<ASTDeclSpec>> DeclarationSpecifiers() {
            return DeclarationSpecifier().Plus();
        }

        /// <summary>
        /// declaration-specifier
        ///     : storage-class-specifier
        ///     | type-specifier
        ///     | type-qualifier
        ///     | function-specifier
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTDeclSpec> DeclarationSpecifier() {
            return StorageClassSpecifier().Cast<Token.Token, ASTDeclSpec, ASTDeclStroageSpec>()
                .Or(TypeSpecifier().Cast<Token.Token, ASTDeclSpec, ASTDeclTypeSpec>())
                .Or(TypeQualifier().Cast<Token.Token, ASTDeclSpec, ASTDeclTypeQual>())
                .Or(FunctionSpecifier().Cast<Token.Token, ASTDeclSpec, ASTDeclFuncSpec>());
        }

        /// <summary>
        /// storage-class-specifier
        ///     : typedef
        ///     | extern
        ///     | static
        ///     | auto
        ///     | register
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTDeclStroageSpec> StorageClassSpecifier() {
            return Get<T_KEY_TYPEDEF>().Select(t => new ASTDeclStroageSpec(t.line, ASTDeclStroageSpec.Type.TYPEDEF))
                .Else(Get<T_KEY_EXTERN>().Select(t => new ASTDeclStroageSpec(t.line, ASTDeclStroageSpec.Type.EXTERN)))
                .Else(Get<T_KEY_STATIC>().Select(t => new ASTDeclStroageSpec(t.line, ASTDeclStroageSpec.Type.STATIC)))
                .Else(Get<T_KEY_AUTO>().Select(t => new ASTDeclStroageSpec(t.line, ASTDeclStroageSpec.Type.AUTO)))
                .Else(Get<T_KEY_REGISTER>().Select(t => new ASTDeclStroageSpec(t.line, ASTDeclStroageSpec.Type.REGISTER)));
        }

        /// <summary>
        /// type-specifier
        ///     : type-key-specifier
        ///     // TODO | struct-or-union-specifier
        ///     // TODO | enum-specifier
        ///     // TODO | typedef-name
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTDeclTypeSpec> TypeSpecifier() {
            return TypeKeySpecifier().Cast<Token.Token, ASTDeclTypeSpec, ASTDeclTypeKeySpec>();
        }

        /// <summary>
        /// type-key-specifier
        ///     : void
        ///     | char
        ///     | short
        ///     | int
        ///     | long
        ///     | float
        ///     | double
        ///     | signed
        ///     | unsigned
        ///     // TODO | _Bool
        ///     // TODO | _Complex
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTDeclTypeKeySpec> TypeKeySpecifier() {
            return Get<T_KEY_VOID>().Select(t => new ASTDeclTypeKeySpec(t.line, ASTDeclTypeKeySpec.Type.VOID))
                .Else(Get<T_KEY_CHAR>().Select(t => new ASTDeclTypeKeySpec(t.line, ASTDeclTypeKeySpec.Type.CHAR)))
                .Else(Get<T_KEY_SHORT>().Select(t => new ASTDeclTypeKeySpec(t.line, ASTDeclTypeKeySpec.Type.SHORT)))
                .Else(Get<T_KEY_INT>().Select(t => new ASTDeclTypeKeySpec(t.line, ASTDeclTypeKeySpec.Type.INT)))
                .Else(Get<T_KEY_LONG>().Select(t => new ASTDeclTypeKeySpec(t.line, ASTDeclTypeKeySpec.Type.LONG)))
                .Else(Get<T_KEY_FLOAT>().Select(t => new ASTDeclTypeKeySpec(t.line, ASTDeclTypeKeySpec.Type.FLOAT)))
                .Else(Get<T_KEY_DOUBLE>().Select(t => new ASTDeclTypeKeySpec(t.line, ASTDeclTypeKeySpec.Type.DOUBLE)))
                .Else(Get<T_KEY_SIGNED>().Select(t => new ASTDeclTypeKeySpec(t.line, ASTDeclTypeKeySpec.Type.SIGNED)))
                .Else(Get<T_KEY_UNSIGNED>().Select(t => new ASTDeclTypeKeySpec(t.line, ASTDeclTypeKeySpec.Type.UNSIGNED)));
        }

        /// <summary>
        /// enum-specifier
        ///     : enum identifier_opt { enumerator-list }
        ///     | enum identifier_opt { enumerator-list , }
        ///     | enum identifier
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTDeclEnumSpec> EnumSpecifier() {
            return Match<T_KEY_ENUM>().Then(
                Get<T_IDENTIFIER>().Select(t => new ASTIdentifier(t)).Bind(identifier =>
                    EnumeratorList().Option(Match<T_PUNC_COMMA>()).Bracket(
                        Match<T_PUNC_BRACEL>(),
                        Match<T_PUNC_BRACER>()).Select(enumerators => new ASTDeclEnumSpec(identifier, enumerators))
                    .Or(Result<Token.Token, ASTDeclEnumSpec>(new ASTDeclEnumSpec(identifier))))
                .Or(EnumeratorList().Option(Match<T_PUNC_COMMA>()).Bracket(
                        Match<T_PUNC_BRACEL>(),
                        Match<T_PUNC_BRACER>()).Select(enumerators => new ASTDeclEnumSpec(enumerators))));
        }

        /// <summary>
        /// enumeration-list
        ///     : enumerator
        ///     | enumerator-list , enumerator
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, LinkedList<ASTDeclEnumerator>> EnumeratorList() {
            return Enumerator().PlusSeperatedBy(Match<T_PUNC_COMMA>());
        }

        /// <summary>
        /// enumerator
        ///     : identifier
        ///     | identifier = const-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTDeclEnumerator> Enumerator() {
            return Get<T_IDENTIFIER>().Select(t => new ASTDeclEnumerator(new ASTIdentifier(t)))
                .Or(Get<T_IDENTIFIER>().Bind(t => Match<T_PUNC_ASSIGN>()
                    .Then(ConstantExpression())
                    .Select(expr => new ASTDeclEnumerator(new ASTIdentifier(t), expr))));
        }

        /// <summary>
        /// type-qualifier
        ///     : const
        ///     | restrict
        ///     | volatile
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTDeclTypeQual> TypeQualifier() {
            return Get<T_KEY_CONST>().Select(t => new ASTDeclTypeQual(t.line, ASTDeclTypeQual.Type.CONST))
                .Else(Get<T_KEY_RESTRICT>().Select(t => new ASTDeclTypeQual(t.line, ASTDeclTypeQual.Type.RESTRICT)))
                .Else(Get<T_KEY_VOLATILE>().Select(t => new ASTDeclTypeQual(t.line, ASTDeclTypeQual.Type.VOLATILE)));
        }

        /// <summary>
        /// function-specifier
        ///     : inline
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<Token.Token, ASTDeclFuncSpec> FunctionSpecifier() {
            return Get<T_KEY_INLINE>().Select(t => new ASTDeclFuncSpec(t.line, ASTDeclFuncSpec.Type.INLINE));
        }

    }
}
