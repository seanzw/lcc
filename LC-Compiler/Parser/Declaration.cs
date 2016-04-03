using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Parserc.Parserc;
using lcc.Token;
using lcc.AST;

// To simplify all the Token.Token.
using T = lcc.Token.Token;

namespace lcc.Parser {
    public static partial class Parser {

        /// <summary>
        /// declaration
        ///     : declaration-specifiers init-declarator-list_opt ;
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTDeclaration> Declaration() {
            return DeclarationSpecifiers()
                .Bind(specifiers => InitDeclaratorList()
                .Bind(declarators => Match<T_PUNC_SEMICOLON>()
                .Return(new ASTDeclaration(specifiers, declarators))));
        }

        /// <summary>
        /// declaration-specifiers
        ///     : declaration-specifier declaration-specifiers_opt
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, LinkedList<ASTDeclarationSpecifier>> DeclarationSpecifiers() {
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
        public static Parserc.Parser<T, ASTDeclarationSpecifier> DeclarationSpecifier() {
            return StorageClassSpecifier().Cast<T, ASTDeclarationSpecifier, ASTStorageSpecifier>()
                .Or(TypeSpecifier())
                .Or(TypeQualifier())
                .Or(FunctionSpecifier());
        }

        /// <summary>
        /// init-declarator-list
        ///     : init-declarator
        ///     | init-declarator-list , init-declarator
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, LinkedList<ASTInitDeclarator>> InitDeclaratorList() {
            return InitDeclarator().PlusSeperatedBy(Match<T_PUNC_COMMA>());
        }

        /// <summary>
        /// init-declarator
        ///     : declarator
        ///     | declarator = initializer
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTInitDeclarator> InitDeclarator() {
            return Declarator().Bind(declarator => Match<T_PUNC_ASSIGN>().Then(Initializer()).ElseNull()
                .Select(initializer => new ASTInitDeclarator(declarator, initializer)));
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
        public static Parserc.Parser<T, ASTStorageSpecifier> StorageClassSpecifier() {
            return Get<T_KEY_TYPEDEF>().Select(t => new ASTStorageSpecifier(t.line, ASTStorageSpecifier.Type.TYPEDEF))
                .Else(Get<T_KEY_EXTERN>().Select(t => new ASTStorageSpecifier(t.line, ASTStorageSpecifier.Type.EXTERN)))
                .Else(Get<T_KEY_STATIC>().Select(t => new ASTStorageSpecifier(t.line, ASTStorageSpecifier.Type.STATIC)))
                .Else(Get<T_KEY_AUTO>().Select(t => new ASTStorageSpecifier(t.line, ASTStorageSpecifier.Type.AUTO)))
                .Else(Get<T_KEY_REGISTER>().Select(t => new ASTStorageSpecifier(t.line, ASTStorageSpecifier.Type.REGISTER)));
        }

        /// <summary>
        /// type-specifier
        ///     : type-key-specifier
        ///     | struct-or-union-specifier
        ///     | enum-specifier
        ///     // TODO | typedef-name
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTTypeSpecifier> TypeSpecifier() {
            return TypeKeySpecifier().Cast<T, ASTTypeSpecifier, ASTTypeKeySpecifier>()
                .Or(StructUnionSpecifier())
                .Or(EnumSpecifier());
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
        ///     | _Bool
        ///     // TODO | _Complex
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTTypeKeySpecifier> TypeKeySpecifier() {
            return Get<T_KEY_VOID>().Select(t => new ASTTypeKeySpecifier(t.line, ASTTypeSpecifier.Type.VOID))
                .Else(Get<T_KEY_CHAR>().Select(t => new ASTTypeKeySpecifier(t.line, ASTTypeSpecifier.Type.CHAR)))
                .Else(Get<T_KEY_SHORT>().Select(t => new ASTTypeKeySpecifier(t.line, ASTTypeSpecifier.Type.SHORT)))
                .Else(Get<T_KEY_INT>().Select(t => new ASTTypeKeySpecifier(t.line, ASTTypeSpecifier.Type.INT)))
                .Else(Get<T_KEY_LONG>().Select(t => new ASTTypeKeySpecifier(t.line, ASTTypeSpecifier.Type.LONG)))
                .Else(Get<T_KEY_FLOAT>().Select(t => new ASTTypeKeySpecifier(t.line, ASTTypeSpecifier.Type.FLOAT)))
                .Else(Get<T_KEY_DOUBLE>().Select(t => new ASTTypeKeySpecifier(t.line, ASTTypeSpecifier.Type.DOUBLE)))
                .Else(Get<T_KEY_SIGNED>().Select(t => new ASTTypeKeySpecifier(t.line, ASTTypeSpecifier.Type.SIGNED)))
                .Else(Get<T_KEY_UNSIGNED>().Select(t => new ASTTypeKeySpecifier(t.line, ASTTypeSpecifier.Type.UNSIGNED)))
                .Else(Get<T_KEY__BOOL>().Select(t => new ASTTypeKeySpecifier(t.line, ASTTypeSpecifier.Type.BOOL)));
        }

        /// <summary>
        /// struct-union-specifier
        ///     : struct struct-union-specifier-tail
        ///     | union struct-union-specifier-tail
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTStructUnionSpecifier> StructUnionSpecifier() {
            return Get<T_KEY_STRUCT>().Bind(t => StructUnionSpecifierTail(true, t.line))
                .Else(Get<T_KEY_UNION>().Bind(t => StructUnionSpecifierTail(false, t.line)));
        }

        /// <summary>
        /// struct-union-specifier-tail
        ///     : identifier { struct-declaration-list }
        ///     | identifier
        ///     | { struct-declaration-list }
        ///     ;
        /// </summary>
        /// <param name="structOrUnion"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTStructUnionSpecifier> StructUnionSpecifierTail(bool structOrUnion, int line) {
            Func<ASTIdentifier, LinkedList<ASTStructDeclaration>, ASTStructUnionSpecifier> aux =
                (identifier, declarations) => structOrUnion
                    ? new ASTStructSpecifier(line, identifier, declarations) as ASTStructUnionSpecifier
                    : new ASTUnionSpecifier(line, identifier, declarations) as ASTStructUnionSpecifier;
            return Identifier()
                    .Bind(identifier => StructDeclarationList().BracelLR()
                    .Select(declarations => aux(identifier, declarations))
                    .Or(Result<T, ASTStructUnionSpecifier>(aux(identifier, null))))
                .Or(StructDeclarationList().BracelLR()
                    .Select(declarations => aux(null, declarations)));
        }

        /// <summary>
        /// struct-declaration-list
        ///     : struct-declaration
        ///     | struct-declaration-list struct-declaration
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, LinkedList<ASTStructDeclaration>> StructDeclarationList() {
            return StructDeclaration().Plus();
        }

        /// <summary>
        /// struct-declaration
        ///     : specifier-qualifier-list struct-declarator-list ;
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTStructDeclaration> StructDeclaration() {
            return SpecifierQualifierList()
                .Bind(spcifierQualifierList => StructDeclaratorList()
                .Bind(declarators => Match<T_PUNC_SEMICOLON>()
                .Return(new ASTStructDeclaration(spcifierQualifierList, declarators))));
        }

        /// <summary>
        /// specifier-qualifier-list
        ///     : type-specifier specifier-qualifier-list_opt
        ///     | type-qualifier specifier-qualifier-list_opt
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, LinkedList<ASTTypeSpecifierQualifier>> SpecifierQualifierList() {
            return Ref(TypeSpecifier).Cast<T, ASTTypeSpecifierQualifier, ASTTypeSpecifier>()
                .Else(TypeQualifier())
                .Plus();
        }

        /// <summary>
        /// struct-declarator-list
        ///     : struct-declarator
        ///     | struct-declarator-list , struct-declarator
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, LinkedList<ASTStructDeclarator>> StructDeclaratorList() {
            return StructDeclarator().PlusSeperatedBy(Match<T_PUNC_COMMA>());
        }

        /// <summary>
        /// struct-declarator
        ///     : declarator
        ///     | declarator : constant-expression
        ///     | : constant-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTStructDeclarator> StructDeclarator() {
            return Declarator()
                .Bind(declarator => Match<T_PUNC_COLON>()
                    .Then(ConstantExpression())
                    .Select(expr => new ASTStructDeclarator(declarator, expr))
                    .Else(Result<T, ASTStructDeclarator>(new ASTStructDeclarator(declarator, null))))
                .Or(Match<T_PUNC_COLON>()
                    .Then(ConstantExpression())
                    .Select(expr => new ASTStructDeclarator(null, expr)));
        }

        /// <summary>
        /// enum-specifier
        ///     : enum identifier_opt { enumerator-list }
        ///     | enum identifier_opt { enumerator-list , }
        ///     | enum identifier
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTEnumSpecifier> EnumSpecifier() {
            return Get<T_KEY_ENUM>().Bind(e =>
                Identifier().Bind(identifier =>
                    EnumeratorList().Option(Match<T_PUNC_COMMA>()).BracelLR()
                        .Select(enumerators => new ASTEnumSpecifier(e.line, identifier, enumerators))
                    .Or(Result<T, ASTEnumSpecifier>(new ASTEnumSpecifier(e.line, identifier, null))))
                .Or(EnumeratorList().Option(Match<T_PUNC_COMMA>()).Bracket(
                        Match<T_PUNC_BRACEL>(),
                        Match<T_PUNC_BRACER>()).Select(enumerators => new ASTEnumSpecifier(e.line, null, enumerators))));
        }

        /// <summary>
        /// enumeration-list
        ///     : enumerator
        ///     | enumerator-list , enumerator
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, LinkedList<ASTEnumerator>> EnumeratorList() {
            return Enumerator().PlusSeperatedBy(Match<T_PUNC_COMMA>());
        }

        /// <summary>
        /// enumerator
        ///     : identifier
        ///     | identifier = const-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTEnumerator> Enumerator() {
            return Identifier().Bind(i => Match<T_PUNC_ASSIGN>().Then(ConstantExpression()).ElseNull()
                .Select(expr => new ASTEnumerator(i, expr)));
        }

        /// <summary>
        /// type-qualifier
        ///     : const
        ///     | restrict
        ///     | volatile
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTTypeQualifier> TypeQualifier() {
            return Get<T_KEY_CONST>().Select(t => new ASTTypeQualifier(t.line, ASTTypeQualifier.Type.CONST))
                .Else(Get<T_KEY_RESTRICT>().Select(t => new ASTTypeQualifier(t.line, ASTTypeQualifier.Type.RESTRICT)))
                .Else(Get<T_KEY_VOLATILE>().Select(t => new ASTTypeQualifier(t.line, ASTTypeQualifier.Type.VOLATILE)));
        }

        /// <summary>
        /// function-specifier
        ///     : inline
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTFunctionSpecifier> FunctionSpecifier() {
            return Get<T_KEY_INLINE>().Select(t => new ASTFunctionSpecifier(t.line, ASTFunctionSpecifier.Type.INLINE));
        }


        /// <summary>
        /// declarator
        ///     // TODO : pointer direct-declarator
        ///     | direct-declarator
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTDeclarator> Declarator() {
            return DirectDeclarator();
        }

        /// <summary>
        /// direct-declarator
        ///     : identifier direct-declarator-prime
        ///     | ( declarator ) direct-declarator-prime
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTDeclarator> DirectDeclarator() {
            return Identifier().Bind(identifier => DirectDeclaratorPrime(new ASTDeclaratorIdentifier(identifier)))
                .Else(Ref(Declarator).ParentLR().Bind(declarator => DirectDeclaratorPrime(declarator)));
        }

        /// <summary>
        /// direct-declarator-prime
        ///     : ( parameter-type-list ) direct-declarator-prime
        ///     | epsilon
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTDeclarator> DirectDeclaratorPrime(ASTDeclarator declarator) {
            return ParameterTypeList().ParentLR()
                    .Select(parameterType => new ASTFunctionParameter(declarator, parameterType) as ASTDeclarator)
                .Or(Result<T, ASTDeclarator>(declarator));
        }

        public static Parserc.Parser<T, ASTDeclarator> Pointer() {
            return Zero<T, ASTDeclarator>();
        }

        /// <summary>
        /// parameter-type-list
        ///     : parameter-list
        ///     | parameter-list , ...
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTParameterType> ParameterTypeList() {
            return ParameterList()
                .Bind(parameters => Match<T_PUNC_COMMA>()
                    .Then(Match<T_PUNC_ELLIPSIS>())
                    .Return(new ASTParameterType(parameters, true))
                    .Or(Result<T, ASTParameterType>(new ASTParameterType(parameters, false))));
        }

        /// <summary>
        /// parameter-list
        ///     : parameter-declaration
        ///     | parameter-list , parameter-declaration
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, LinkedList<ASTParameter>> ParameterList() {
            return ParameterDeclaration().PlusSeperatedBy(Match<T_PUNC_COMMA>());
        }

        /// <summary>
        /// parameter-declaration
        ///     : declaration-specifiers declarator
        ///     // TODO | declaration-specifiers abstract-declarator_opt
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTParameter> ParameterDeclaration() {
            return Ref(DeclarationSpecifiers)
                .Bind(specifiers => Ref(Declarator)
                .Select(declarator => new ASTParameterDeclarator(specifiers, declarator) as ASTParameter));
        }

        /// <summary>
        /// initializer
        ///     : assignment-expression
        ///     | { initializer-list }
        ///     | { initializer-list , }
        ///     ;
        ///     
        /// initializer-list
        ///     : init-item
        ///     | initializer-list , init-item
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTInitializer> Initializer() {
            return AssignmentExpression().Select(expr => new ASTInitializer(expr))
                .Else(InitItem()
                    .PlusSeperatedBy(Match<T_PUNC_COMMA>()).Option(Match<T_PUNC_COMMA>())
                    .BracelLR().Select(items => new ASTInitializer(items)));
        }

        /// <summary>
        /// init-item
        ///     : designation_opt initializer
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTInitItem> InitItem() {
            return Designation().ElseNull().Bind(designators => Ref(Initializer)
                .Select(initializer => new ASTInitItem(initializer, designators)));
        }

        /// <summary>
        /// designation
        ///     : designator-list =
        ///     ;
        ///     
        /// designator-list
        ///     : designator
        ///     | designator-list designator
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, LinkedList<ASTDesignator>> Designation() {
            return Designator().Plus().Bind(designators => Match<T_PUNC_ASSIGN>().Return(designators));
        }

        /// <summary>
        /// designator
        ///     : [ constant-expression ]
        ///     | . identifier
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTDesignator> Designator() {
            return ConstantExpression().SubLR().Select(expr => new ASTDesignator(expr))
                .Or(Match<T_PUNC_DOT>().Then(Identifier()).Select(identifier => new ASTDesignator(identifier)));
        }
    }
}
