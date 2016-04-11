using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Parserc.Parserc;
using lcc.Token;
using lcc.SyntaxTree;

// To simplify all the Token.Token.
using T = lcc.Token.Token;

namespace lcc.Parser {
    public static partial class Parser {

        /// <summary>
        /// declaration
        ///     : declaration-specifiers init-declarator-list_opt ;
        ///     ;
        ///     
        /// If the declaration is a typedef, add the name into the environment.
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STDeclaration> Declaration() {
            return DeclarationSpecifiers()
                .Bind(specifiers => InitDeclaratorList()
                .Bind(declarators => Match<T_PUNC_SEMICOLON>()
                .Bind(_ => {
                    var decl = new STDeclaration(specifiers, declarators);
                    if (decl.IsTypedef)
                        foreach (var name in decl.DeclNames)
                            Env.AddTypedefName(decl.Pos.line, name);
                    return Result<T, STDeclaration>(decl);
                })));
        }

        /// <summary>
        /// declaration-specifiers
        ///     : declaration-specifier declaration-specifiers_opt
        ///     ;
        ///     
        /// Notice this parser will do some semantic analysis to solve typedef and variable ambiguity.
        /// Make sure that 
        /// 1. struct/union/enum/typedef type specifier appear only once.
        /// 2. at least one type specifier shall be given in the declaration.
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STDeclSpecs> DeclarationSpecifiers() {
            return DeclarationSpecifier().Plus().Bind(ss => {

                // At most one storage-class specifier may be given in the declaration specifiers.
                var stors = ss.OfType<STStoreSpec>();
                STStoreSpec.Kind storage = STStoreSpec.Kind.NONE;
                if (stors.Count() > 1) return Zero<T, STDeclSpecs>();
                else if (stors.Count() == 1) storage = stors.First().kind;

                // At most one function specifier.
                var funcs = ss.OfType<STFuncSpec>();
                STFuncSpec.Kind function = STFuncSpec.Kind.NONE;
                if (funcs.Count() > 1) return Zero<T, STDeclSpecs>();
                else if (funcs.Count() == 1) function = funcs.First().kind;

                // At least one type specifier shall be given in the declaration specifiers.
                var specs = ss.OfType<STTypeSpec>();
                if (specs.Count() == 0) return Zero<T, STDeclSpecs>();

                foreach (var spec in specs) {
                    if (spec.kind == STTypeSpec.Kind.TYPEDEF ||
                        spec.kind == STTypeSpec.Kind.STRUCT ||
                        spec.kind == STTypeSpec.Kind.UNION ||
                        spec.kind == STTypeSpec.Kind.ENUM)
                        if (specs.Count() != 1) return Zero<T, STDeclSpecs>();
                        // User-defined types.
                        else return Result<T, STDeclSpecs>(new STDeclSpecs(ss, storage, spec as STTypeUserSpec, function));
                }

                // Built-in type.
                var keys = from s in specs select s.kind;
                return Result<T, STDeclSpecs>(new STDeclSpecs(ss, storage, keys, function));
            });
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
        public static Parserc.Parser<T, STDeclSpec> DeclarationSpecifier() {
            return StorageClassSpecifier().Cast<T, STDeclSpec, STStoreSpec>()
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
        public static Parserc.Parser<T, LinkedList<STInitDeclarator>> InitDeclaratorList() {
            return InitDeclarator().PlusSeperatedBy(Match<T_PUNC_COMMA>());
        }

        /// <summary>
        /// init-declarator
        ///     : declarator
        ///     | declarator = initializer
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STInitDeclarator> InitDeclarator() {
            return Declarator().Bind(declarator => Match<T_PUNC_ASSIGN>().Then(Initializer()).ElseNull()
                .Select(initializer => new STInitDeclarator(declarator, initializer)));
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
        public static Parserc.Parser<T, STStoreSpec> StorageClassSpecifier() {
            return Get<T_KEY_TYPEDEF>().Select(t => new STStoreSpec(t.line, STStoreSpec.Kind.TYPEDEF))
                .Else(Get<T_KEY_EXTERN>().Select(t => new STStoreSpec(t.line, STStoreSpec.Kind.EXTERN)))
                .Else(Get<T_KEY_STATIC>().Select(t => new STStoreSpec(t.line, STStoreSpec.Kind.STATIC)))
                .Else(Get<T_KEY_AUTO>().Select(t => new STStoreSpec(t.line, STStoreSpec.Kind.AUTO)))
                .Else(Get<T_KEY_REGISTER>().Select(t => new STStoreSpec(t.line, STStoreSpec.Kind.REGISTER)));
        }

        /// <summary>
        /// type-specifier
        ///     : type-key-specifier
        ///     | struct-or-union-specifier
        ///     | enum-specifier
        ///     | typedef-name
        ///     ;
        ///     
        /// typedef-name
        ///     : identifier
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STTypeSpec> TypeSpecifier() {
            return TypeKeySpecifier().Cast<T, STTypeSpec, STTypeKeySpec>()
                .Else(StructUnionSpecifier())
                .Else(EnumSpecifier())
                .Else(Identifier().Bind(identifier => {
                    return Env.IsTypedefName(identifier.name) ? Result<T, STTypeSpec>(new STTypedefName(identifier))
                        : Zero<T, STTypeSpec>();
                }));
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
        ///     | _Complex
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STTypeKeySpec> TypeKeySpecifier() {
            return Get<T_KEY_VOID>().Select(t => new STTypeKeySpec(t.line, STTypeSpec.Kind.VOID))
                .Else(Get<T_KEY_CHAR>().Select(t => new STTypeKeySpec(t.line, STTypeSpec.Kind.CHAR)))
                .Else(Get<T_KEY_SHORT>().Select(t => new STTypeKeySpec(t.line, STTypeSpec.Kind.SHORT)))
                .Else(Get<T_KEY_INT>().Select(t => new STTypeKeySpec(t.line, STTypeSpec.Kind.INT)))
                .Else(Get<T_KEY_LONG>().Select(t => new STTypeKeySpec(t.line, STTypeSpec.Kind.LONG)))
                .Else(Get<T_KEY_FLOAT>().Select(t => new STTypeKeySpec(t.line, STTypeSpec.Kind.FLOAT)))
                .Else(Get<T_KEY_DOUBLE>().Select(t => new STTypeKeySpec(t.line, STTypeSpec.Kind.DOUBLE)))
                .Else(Get<T_KEY_SIGNED>().Select(t => new STTypeKeySpec(t.line, STTypeSpec.Kind.SIGNED)))
                .Else(Get<T_KEY_UNSIGNED>().Select(t => new STTypeKeySpec(t.line, STTypeSpec.Kind.UNSIGNED)))
                .Else(Get<T_KEY__BOOL>().Select(t => new STTypeKeySpec(t.line, STTypeSpec.Kind.BOOL)))
                .Else(Get<T_KEY__COMPLEX>().Select(t => new STTypeKeySpec(t.line, STTypeSpec.Kind.COMPLEX)));
        }

        /// <summary>
        /// struct-union-specifier
        ///     : struct struct-union-specifier-tail
        ///     | union struct-union-specifier-tail
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STStructUnionSpec> StructUnionSpecifier() {
            return Get<T_KEY_STRUCT>().Bind(t => StructUnionSpecifierTail(STTypeSpec.Kind.STRUCT, t.line))
                .Else(Get<T_KEY_UNION>().Bind(t => StructUnionSpecifierTail(STTypeSpec.Kind.UNION, t.line)));
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
        public static Parserc.Parser<T, STStructUnionSpec> StructUnionSpecifierTail(STTypeSpec.Kind kind, int line) {
            return Identifier()
                    .Bind(identifier => StructDeclarationList().BracelLR().ElseNull()
                    .Select(declarations => new STStructUnionSpec(line, identifier, declarations, kind)))
                .Or(StructDeclarationList().BracelLR()
                    .Select(declarations => new STStructUnionSpec(line, null, declarations, kind)));
        }

        /// <summary>
        /// struct-declaration-list
        ///     : struct-declaration
        ///     | struct-declaration-list struct-declaration
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, LinkedList<STStructDeclaration>> StructDeclarationList() {
            return StructDeclaration().Plus();
        }

        /// <summary>
        /// struct-declaration
        ///     : specifier-qualifier-list struct-declarator-list ;
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STStructDeclaration> StructDeclaration() {
            return SpecifierQualifierList()
                .Bind(spcifierQualifierList => StructDeclaratorList()
                .Bind(declarators => Match<T_PUNC_SEMICOLON>()
                .Return(new STStructDeclaration(spcifierQualifierList, declarators))));
        }

        /// <summary>
        /// specifier-qualifier-list
        ///     : type-specifier specifier-qualifier-list_opt
        ///     | type-qualifier specifier-qualifier-list_opt
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, IEnumerable<STTypeSpecQual>> SpecifierQualifierList() {
            return Ref(TypeSpecifier).Cast<T, STTypeSpecQual, STTypeSpec>()
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
        public static Parserc.Parser<T, LinkedList<STStructDeclarator>> StructDeclaratorList() {
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
        public static Parserc.Parser<T, STStructDeclarator> StructDeclarator() {
            return Declarator()
                .Bind(declarator => Match<T_PUNC_COLON>()
                    .Then(ConstantExpression())
                    .Select(expr => new STStructDeclarator(declarator, expr))
                    .Else(Result<T, STStructDeclarator>(new STStructDeclarator(declarator, null))))
                .Or(Match<T_PUNC_COLON>()
                    .Then(ConstantExpression())
                    .Select(expr => new STStructDeclarator(null, expr)));
        }

        /// <summary>
        /// enum-specifier
        ///     : enum identifier_opt { enumerator-list }
        ///     | enum identifier_opt { enumerator-list , }
        ///     | enum identifier
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STEnumSpec> EnumSpecifier() {
            return Get<T_KEY_ENUM>().Bind(e =>
                Identifier().Bind(identifier =>
                    EnumeratorList().Option(Match<T_PUNC_COMMA>()).BracelLR()
                        .Select(enumerators => new STEnumSpec(e.line, identifier, enumerators))
                    .Or(Result<T, STEnumSpec>(new STEnumSpec(e.line, identifier, null))))
                .Or(EnumeratorList().Option(Match<T_PUNC_COMMA>()).Bracket(
                        Match<T_PUNC_BRACEL>(),
                        Match<T_PUNC_BRACER>()).Select(enumerators => new STEnumSpec(e.line, null, enumerators))));
        }

        /// <summary>
        /// enumeration-list
        ///     : enumerator
        ///     | enumerator-list , enumerator
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, IEnumerable<STEnum>> EnumeratorList() {
            return Enumerator().PlusSeperatedBy(Match<T_PUNC_COMMA>());
        }

        /// <summary>
        /// enumerator
        ///     : identifier
        ///     | identifier = const-expression
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STEnum> Enumerator() {
            return Identifier().Bind(i => Match<T_PUNC_ASSIGN>().Then(ConstantExpression()).ElseNull()
                .Select(expr => new STEnum(i, expr)));
        }

        /// <summary>
        /// type-qualifier
        ///     : const
        ///     | restrict
        ///     | volatile
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STTypeQual> TypeQualifier() {
            return Get<T_KEY_CONST>().Select(t => new STTypeQual(t.line, STTypeQual.Kind.CONST))
                .Else(Get<T_KEY_RESTRICT>().Select(t => new STTypeQual(t.line, STTypeQual.Kind.RESTRICT)))
                .Else(Get<T_KEY_VOLATILE>().Select(t => new STTypeQual(t.line, STTypeQual.Kind.VOLATILE)));
        }

        /// <summary>
        /// function-specifier
        ///     : inline
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STFuncSpec> FunctionSpecifier() {
            return Get<T_KEY_INLINE>().Select(t => new STFuncSpec(t.line, STFuncSpec.Kind.INLINE));
        }


        /// <summary>
        /// declarator
        ///     : pointer-list_opt direct-declarator
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STDeclarator> Declarator() {
            return Pointer().Many().Bind(pointers => DirectDeclarator()
                .Select(direct => new STDeclarator(pointers, direct)));
        }

        /// <summary>
        /// direct-declarator
        ///     : identifier direct-declarator-prime
        ///     | ( declarator ) direct-declarator-prime
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STDirDeclarator> DirectDeclarator() {
            return Identifier().Bind(identifier => DirectDeclaratorPrime(new STIdDeclarator(identifier)))
                .Else(Ref(Declarator).ParentLR().Bind(declarator => DirectDeclaratorPrime(new STParDeclarator(declarator))));
        }

        /// <summary>
        /// direct-declarator-prime
        ///     : ( parameter-type-list ) direct-declarator-prime
        ///     | ( identifier-list_opt ) direct-declarator-prime
        ///     | [ type-qualifier-list_opt assignment-expression_opt ]
        ///     | [ static type-qualifier-list_opt assignment-expression ]
        ///     | [ type-qualifier-list static assignment-expression ]
        ///     | [ type-qualifier-list_opt * ]
        ///     | epsilon
        ///     ;
        ///     
        /// identfier-list
        ///     : identifier
        ///     | identifier , identifier-list
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STDirDeclarator> DirectDeclaratorPrime(STDirDeclarator direct) {
            return ParameterTypeList().ParentLR()
                    .Bind(tuple => DirectDeclaratorPrime(new STFuncDeclarator(direct, tuple.Item1, tuple.Item2)))
                .Or(Identifier().ManySeperatedBy(Match<T_PUNC_COMMA>()).ParentLR()
                    .Bind(identifiers => DirectDeclaratorPrime(new STFuncDeclarator(direct, identifiers))))
                .Or(Match<T_PUNC_SUBSCRIPTL>().Then(TypeQualifier().Many())
                    .Bind(qualifiers => AssignmentExpression().ElseNull()
                    .Bind(expr => Match<T_PUNC_SUBSCRIPTR>()
                    .Then(DirectDeclaratorPrime(new STArrDeclarator(direct, qualifiers, expr, false))))))
                .Or(Match<T_PUNC_SUBSCRIPTL>().Then(Match<T_KEY_STATIC>())
                    .Then(TypeQualifier().Many())
                    .Bind(qualifiers => AssignmentExpression()
                    .Bind(expr => Match<T_PUNC_SUBSCRIPTR>()
                    .Then(DirectDeclaratorPrime(new STArrDeclarator(direct, qualifiers, expr, true))))))
                .Or(Match<T_PUNC_SUBSCRIPTL>().Then(TypeQualifier().Plus())
                    .Bind(qualifiers => Match<T_KEY_STATIC>()
                    .Then(AssignmentExpression())
                    .Bind(expr => Match<T_PUNC_SUBSCRIPTR>()
                    .Then(DirectDeclaratorPrime(new STArrDeclarator(direct, qualifiers, expr, true))))))
                .Or(Match<T_PUNC_SUBSCRIPTL>().Then(TypeQualifier().Many())
                    .Bind(qualifiers => Match<T_PUNC_STAR>()
                    .Then(Match<T_PUNC_SUBSCRIPTR>())
                    .Then(DirectDeclaratorPrime(new STArrDeclarator(direct, qualifiers)))))
                .ElseReturn(direct);
        }

        /// <summary>
        /// pointer
        ///     : * type-qualifier-list_opt
        ///     | * type-qualifier-list_opt
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STPtr> Pointer() {
            return Get<T_PUNC_STAR>().Bind(t => TypeQualifier().Many()
                .Select(qualifiers => new STPtr(t.line, qualifiers)));
        }

        /// <summary>
        /// parameter-type-list
        ///     : parameter-list
        ///     | parameter-list , ...
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, Tuple<IEnumerable<STParam>, bool>> ParameterTypeList() {
            return ParameterList().Bind(parameters => Match<T_PUNC_COMMA>()
                .Then(Match<T_PUNC_ELLIPSIS>()).Return(true).ElseReturn(false)
                .Select(isEllipis => new Tuple<IEnumerable<STParam>, bool>(parameters, isEllipis)));
        }

        /// <summary>
        /// parameter-list
        ///     : parameter-declaration
        ///     | parameter-list , parameter-declaration
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, IEnumerable<STParam>> ParameterList() {
            return ParameterDeclaration().PlusSeperatedBy(Match<T_PUNC_COMMA>());
        }

        /// <summary>
        /// parameter-declaration
        ///     : declaration-specifiers declarator
        ///     | declaration-specifiers abstract-declarator_opt
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STParam> ParameterDeclaration() {
            return Ref(DeclarationSpecifiers)
                .Bind(specifiers => Ref(Declarator)
                .Select(declarator => new STParam(specifiers, declarator))
                .Else(AbstractDeclarator().ElseNull().Select(absDeclarator => new STParam(specifiers, absDeclarator))));
        }

        /// <summary>
        /// type-name
        ///     : specifier-qualifier-list abstract-declarator_opt
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, ASTTypeName> TypeName() {
            return Ref(SpecifierQualifierList).Bind(specifiers => AbstractDeclarator().ElseNull()
                .Select(declarator => new ASTTypeName(specifiers, declarator)));
        }

        /// <summary>
        /// abstract-declarator
        ///     : pointer-list
        ///     | pointer-list_opt direct-abstract-declarator
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STAbsDeclarator> AbstractDeclarator() {
            return Pointer().Plus().Select(pointers => new STAbsDeclarator(pointers, null))
                .Or(Pointer().Many().Bind(pointers => DirectAbstractDeclarator()
                .Select(direct => new STAbsDeclarator(pointers, direct))));
        }


        /// <summary>
        /// direct-abstract-declarator
        ///     : ( abstract-declarator ) direct-abstract-declarator-prime
        ///     | epsilon
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STAbsDirDeclarator> DirectAbstractDeclarator() {
            return Ref(AbstractDeclarator).ParentLR().Bind(declarator => DirectAbstractDeclaratorPrime(new STAbsParDeclarator(declarator)))
                .Else(Peek<T>()
                    .Bind(t => DirectAbstractDeclaratorPrime(new STAbsDirDeclaratorNil(t.line))));
        }

        /// <summary>
        /// direct-abstract-declarator-prime
        ///     : ( parameter-type-list_opt ) direct-abstract-declarator-prime
        ///     | [ type-qualifier-list_opt assignment-expression_opt ] direct-abstract-declarator-prime
        ///     | [ static type-qualifier-list_opt assignment-expression ] direct-abstract-declarator-prime
        ///     | [ type-qualifer-list static assignment-expression ] direct-abstract-declarator-prime
        ///     | [ * ] direct-abstract-declarator-prime
        ///     | epsilon (not null)
        ///     ;
        /// </summary>
        /// <param name="direct"></param>
        /// <returns></returns>
        public static Parserc.Parser<T, STAbsDirDeclarator> DirectAbstractDeclaratorPrime(STAbsDirDeclarator direct) {
            return Ref(ParameterTypeList).ParentLR()
                    .Bind(tuple => DirectAbstractDeclaratorPrime(new STAbsFuncDeclarator(direct, tuple.Item1, tuple.Item2)))
                .Or(Match<T_PUNC_PARENTL>().Then(Match<T_PUNC_PARENTR>())
                    .Bind(_ => DirectAbstractDeclaratorPrime(new STAbsFuncDeclarator(direct, null, false))))
                .Or(Match<T_PUNC_SUBSCRIPTL>().Then(TypeQualifier().Many())
                    .Bind(qualifiers => AssignmentExpression().ElseNull()
                    .Bind(expr => Match<T_PUNC_SUBSCRIPTR>()
                    .Then(DirectAbstractDeclaratorPrime(new STAbsArrDeclarator(direct, qualifiers, expr, false))))))
                .Or(Match<T_PUNC_SUBSCRIPTL>().Then(Match<T_KEY_STATIC>())
                    .Then(TypeQualifier().Many())
                    .Bind(qualifiers => AssignmentExpression()
                    .Bind(expr => Match<T_PUNC_SUBSCRIPTR>()
                    .Then(DirectAbstractDeclaratorPrime(new STAbsArrDeclarator(direct, qualifiers, expr, true))))))
                .Or(Match<T_PUNC_SUBSCRIPTL>().Then(TypeQualifier().Plus())
                    .Bind(qualifiers => Match<T_KEY_STATIC>()
                    .Then(AssignmentExpression())
                    .Bind(expr => Match<T_PUNC_SUBSCRIPTR>()
                    .Then(DirectAbstractDeclaratorPrime(new STAbsArrDeclarator(direct, qualifiers, expr, true))))))
                .Or(Match<T_PUNC_SUBSCRIPTL>().Then(Match<T_PUNC_STAR>())
                    .Then(Match<T_PUNC_SUBSCRIPTR>())
                    .Bind(_ => DirectAbstractDeclaratorPrime(new STAbsArrDeclarator(direct))))
                .Else(direct is STAbsDirDeclaratorNil ? Zero<T, STAbsDirDeclarator>() : Result<T, STAbsDirDeclarator>(direct));
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
        public static Parserc.Parser<T, STInitializer> Initializer() {
            return AssignmentExpression().Select(expr => new STInitializer(expr))
                .Else(InitItem()
                    .PlusSeperatedBy(Match<T_PUNC_COMMA>()).Option(Match<T_PUNC_COMMA>())
                    .BracelLR().Select(items => new STInitializer(items)));
        }

        /// <summary>
        /// init-item
        ///     : designation_opt initializer
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STInitItem> InitItem() {
            return Designation().ElseNull().Bind(designators => Ref(Initializer)
                .Select(initializer => new STInitItem(initializer, designators)));
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
        public static Parserc.Parser<T, LinkedList<STDesignator>> Designation() {
            return Designator().Plus().Bind(designators => Match<T_PUNC_ASSIGN>().Return(designators));
        }

        /// <summary>
        /// designator
        ///     : [ constant-expression ]
        ///     | . identifier
        ///     ;
        /// </summary>
        /// <returns></returns>
        public static Parserc.Parser<T, STDesignator> Designator() {
            return ConstantExpression().SubLR().Select(expr => new STDesignator(expr))
                .Or(Match<T_PUNC_DOT>().Then(Identifier()).Select(identifier => new STDesignator(identifier)));
        }
    }
}
