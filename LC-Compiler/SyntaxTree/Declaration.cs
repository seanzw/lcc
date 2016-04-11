using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.SyntaxTree {

    public sealed class STDeclaration : STStmt, IEquatable<STDeclaration> {

        public STDeclaration(
            STDeclSpecs specifiers,
            IEnumerable<STInitDeclarator> declarators
            ) {
            this.specifiers = specifiers;
            this.declarators = declarators;
        }

        /// <summary>
        /// Check if this is a typedef declaration.
        /// </summary>
        /// <returns></returns>
        public bool IsTypedef => specifiers.storage == STStoreSpec.Kind.TYPEDEF;

        /// <summary>
        /// Return the names of the all the direct declarators.
        /// </summary>
        public IEnumerable<string> DeclNames => from declarator in declarators select declarator.declarator.direct.Name;

        public override Position Pos => specifiers.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STDeclaration);
        }

        public bool Equals(STDeclaration x) {
            return x != null && x.specifiers.Equals(specifiers)
                && x.declarators.SequenceEqual(declarators);
        }

        public override int GetHashCode() {
            return specifiers.GetHashCode();
        }

        public readonly STDeclSpecs specifiers;
        public readonly IEnumerable<STInitDeclarator> declarators;

        public T TypeCheck(ASTEnv env) {

            return null;
        }
    }

    public sealed class STDeclSpecs : STNode, IEquatable<STDeclSpecs> {

        public STDeclSpecs(
            IEnumerable<STDeclSpec> all,
            STStoreSpec.Kind storage,
            IEnumerable<STTypeSpec.Kind> keys,
            STFuncSpec.Kind function = STFuncSpec.Kind.NONE
            ) {
            this.storage = storage;
            this.keys = keys;
            this.function = function;
            qualifiers = GetQualifiers(from s in all.OfType<STTypeQual>() select s.kind);
            pos = all.First().Pos;
        }

        public STDeclSpecs(
            IEnumerable<STDeclSpec> all,
            STStoreSpec.Kind storage,
            STTypeUserSpec specifier,
            STFuncSpec.Kind function = STFuncSpec.Kind.NONE
            ) {
            this.storage = storage;
            this.function = function;
            this.specifier = specifier;
            qualifiers = GetQualifiers(from s in all.OfType<STTypeQual>() select s.kind);
            pos = all.First().Pos;
        }

        public bool Equals(STDeclSpecs x) {
            return x != null && x.pos.Equals(pos)
                && x.storage == storage
                && x.qualifiers.Equals(qualifiers)
                && keys == null ? x.keys == null : keys.SequenceEqual(x.keys)
                && NullableEquals(x.specifier, specifier)
                && x.function == function;
        }

        public override bool Equals(object obj) {
            return Equals(obj as STDeclSpecs);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override Position Pos => pos;

        /// <summary>
        /// At most one storage specifier.
        /// </summary>
        public readonly STStoreSpec.Kind storage;

        /// <summary>
        /// All the qualifiers.
        /// </summary>
        public readonly TQualifiers qualifiers;

        /// <summary>
        /// At most one function specifier (inline).
        /// </summary>
        public readonly STFuncSpec.Kind function;

        /// <summary>
        /// All the type specifiers EXCEPT struct, union, enum, typedef.
        /// </summary>
        public readonly IEnumerable<STTypeSpec.Kind> keys;

        /// <summary>
        /// All the struct, union, enum, typedef specifier.
        /// </summary>
        public readonly STTypeUserSpec specifier;

        /// <summary>
        /// Evaluate the type specifiers and get the unqualified type.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public TUnqualified GetTUnqualified(ASTEnv env) {
            // Get the unqualified type.
            if (keys != null) {
                // This is built-in type.
                if (dict.ContainsKey(keys)) return dict[keys];
                else throw new STException(pos, string.Format("Unkown type: {0}", keys.Aggregate("", (str, key) => str + " " + key)));
            } else if (specifier != null) {
                // This is a user-defined type.
                return specifier.GetTUnqualified(env);
            } else {
                // Error!
                throw new STException(pos, "At least one type specifier should be given.");
            }
        }

        /// <summary>
        /// Evaluate the type qualifiers.
        /// </summary>
        /// <returns></returns>
        private static TQualifiers GetQualifiers(IEnumerable<STTypeQual.Kind> qualifiers) {
            var tuple = new Tuple<bool, bool, bool>(
                qualifiers.Contains(STTypeQual.Kind.CONST),
                qualifiers.Contains(STTypeQual.Kind.RESTRICT), 
                qualifiers.Contains(STTypeQual.Kind.VOLATILE));
            return TQualifiers.dict[tuple];
        }

        private readonly Position pos;

        #region TypeSpecifier Map
        private class ListComparer : IEqualityComparer<IEnumerable<STTypeSpec.Kind>> {
            public bool Equals(IEnumerable<STTypeSpec.Kind> x, IEnumerable<STTypeSpec.Kind> y) {
                return x.SequenceEqual(y);
            }

            public int GetHashCode(IEnumerable<STTypeSpec.Kind> x) {
                int hash = 0;
                foreach (var s in x) {
                    hash |= s.GetHashCode();
                }
                return hash;
            }
        }

        private static Dictionary<IEnumerable<STTypeSpec.Kind>, TUnqualified> dict =
            new Dictionary<IEnumerable<STTypeSpec.Kind>, TUnqualified>(new ListComparer()) {
                {
                    // void
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.VOID
                    } orderby s ascending select s,
                    TypeVoid.Instance
                },
                {
                    // char
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.CHAR
                    } orderby s ascending select s,
                    TChar.Instance
                },
                {
                    // signed char
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.SIGNED,
                        STTypeSpec.Kind.CHAR
                    } orderby s ascending select s,
                    TSChar.Instance
                },
                {
                    // unsigned char
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.UNSIGNED,
                        STTypeSpec.Kind.CHAR
                    } orderby s ascending select s,
                    TUChar.Instance
                },
                {
                    // short
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.SHORT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // signed short
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.SIGNED,
                        STTypeSpec.Kind.SHORT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // short int
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.SHORT,
                        STTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // signed short int
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.SIGNED,
                        STTypeSpec.Kind.SHORT,
                        STTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // unsigned short
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.UNSIGNED,
                        STTypeSpec.Kind.SHORT
                    } orderby s ascending select s,
                    TUShort.Instance
                },
                {
                    // unsigned short int
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.UNSIGNED,
                        STTypeSpec.Kind.SHORT,
                        STTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TUShort.Instance
                },
                {
                    // int
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TInt.Instance
                },
                {
                    // signed
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.SIGNED
                    } orderby s ascending select s,
                    TInt.Instance
                },
                {
                    // signed int
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.SIGNED,
                        STTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TInt.Instance
                },
                {
                    // unsigned
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.UNSIGNED
                    } orderby s ascending select s,
                    TUInt.Instance
                },
                {
                    // unsigned int
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.UNSIGNED,
                        STTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TUInt.Instance
                },
                {
                    // long
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // signed long
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.SIGNED,
                        STTypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // long int
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.LONG,
                        STTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // signed long int
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.SIGNED,
                        STTypeSpec.Kind.LONG,
                        STTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // unsigned long
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.UNSIGNED,
                        STTypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TULong.Instance
                },
                {
                    // unsigned long int
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.UNSIGNED,
                        STTypeSpec.Kind.LONG,
                        STTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TULong.Instance
                },
                {
                    // long long
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.LONG,
                        STTypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // signed long long
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.SIGNED,
                        STTypeSpec.Kind.LONG,
                        STTypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // long long int
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.LONG,
                        STTypeSpec.Kind.LONG,
                        STTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // signed long long int
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.SIGNED,
                        STTypeSpec.Kind.LONG,
                        STTypeSpec.Kind.LONG,
                        STTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // unsigned long long
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.UNSIGNED,
                        STTypeSpec.Kind.LONG,
                        STTypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TULLong.Instance
                },
                {
                    // unsigned long long int
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.UNSIGNED,
                        STTypeSpec.Kind.LONG,
                        STTypeSpec.Kind.LONG,
                        STTypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TULLong.Instance
                },
                {
                    // float
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.FLOAT
                    } orderby s ascending select s,
                    TFloat.Instance
                },
                {
                    // double
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.DOUBLE
                    } orderby s ascending select s,
                    TDouble.Instance
                },
                {
                    // long double
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.LONG,
                        STTypeSpec.Kind.DOUBLE
                    } orderby s ascending select s,
                    TLDouble.Instance
                },
                {
                    // double
                    from s in new List<STTypeSpec.Kind> {
                        STTypeSpec.Kind.BOOL
                    } orderby s ascending select s,
                    TBool.Instance
                },
            };
        #endregion
    }

    public sealed class STInitDeclarator : STNode, IEquatable<STInitDeclarator> {

        public STInitDeclarator(STDeclarator declarator, STInitializer initializer = null) {
            this.declarator = declarator;
            this.initializer = initializer;
        }

        public bool Equals(STInitDeclarator x) {
            return x != null && x.declarator.Equals(declarator) && NullableEquals(x.initializer, initializer);
        }

        public override bool Equals(object obj) {
            return Equals(obj as STInitDeclarator);
        }

        public override int GetHashCode() {
            return declarator.GetHashCode();
        }

        public override Position Pos => declarator.Pos;

        public readonly STDeclarator declarator;
        public readonly STInitializer initializer;
    }

    public abstract class STDeclSpec : STNode { }

    public sealed class STStoreSpec : STDeclSpec, IEquatable<STStoreSpec> {

        public enum Kind {
            NONE,           // Represent no storage-specifier
            TYPEDEF,
            EXTERN,
            STATIC,
            AUTO,
            REGISTER
        }

        public STStoreSpec(int line, Kind type) {
            this.pos = new Position { line = line };
            this.kind = type;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as STStoreSpec);
        }

        public bool Equals(STStoreSpec x) {
            return x != null && x.pos.Equals(pos) && x.kind == kind;
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public readonly Kind kind;
        public readonly Position pos;
    }

    public abstract class STTypeSpecQual : STDeclSpec { }

    public abstract class STTypeSpec : STTypeSpecQual {

        public enum Kind {
            VOID,
            CHAR,
            SHORT,
            INT,
            LONG,
            FLOAT,
            DOUBLE,
            SIGNED,
            UNSIGNED,
            BOOL,
            COMPLEX,
            STRUCT,
            UNION,
            ENUM,
            TYPEDEF
        }

        public STTypeSpec(Kind kind) {
            this.kind = kind;
        }

        public readonly Kind kind;
    }

    public sealed class STTypeKeySpec : STTypeSpec, IEquatable<STTypeKeySpec> {

        public STTypeKeySpec(int line, Kind kind) : base(kind) {
            this.pos = new Position { line = line };
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as STTypeKeySpec);
        }

        public bool Equals(STTypeKeySpec x) {
            return x != null && x.pos.Equals(pos) && x.kind == kind;
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public readonly Position pos;
    }

    /// <summary>
    /// Represents a user-defined type specifier.
    /// </summary>
    public abstract class STTypeUserSpec : STTypeSpec {
        public STTypeUserSpec(Kind kind) : base(kind) { }
        public abstract TUnqualified GetTUnqualified(ASTEnv env);
    }

    public sealed class STStructUnionSpec : STTypeUserSpec, IEquatable<STStructUnionSpec> {

        public STStructUnionSpec(
            int line,
            STId identifier,
            IEnumerable<STStructDeclaration> declarations,
            Kind kind
            ) : base(kind) {
            this.pos = new Position { line = line };
            this.identifier = identifier;
            this.declarations = declarations;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as STStructUnionSpec);
        }

        public bool Equals(STStructUnionSpec x) {
            return x != null
                && NullableEquals(x.identifier, identifier)
                && NullableEquals(x.declarations, declarations);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override TUnqualified GetTUnqualified(ASTEnv env) {
            throw new NotImplementedException();
        }

        private readonly Position pos;

        /// <summary>
        /// Nullable.
        /// </summary>
        public readonly STId identifier;
        /// <summary>
        /// Nullable.
        /// </summary>
        public readonly IEnumerable<STStructDeclaration> declarations;
    }

    public sealed class STStructDeclaration : STNode, IEquatable<STStructDeclaration> {

        public STStructDeclaration(
            IEnumerable<STTypeSpecQual> specifierQualifierList,
            IEnumerable<STStructDeclarator> declarators
            ) {
            this.specifierQualifierList = specifierQualifierList;
            this.declarators = declarators;
        }

        public override Position Pos => specifierQualifierList.First().Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STStructDeclaration);
        }

        public bool Equals(STStructDeclaration x) {
            return x != null
                && x.specifierQualifierList.SequenceEqual(specifierQualifierList)
                && x.declarators.SequenceEqual(declarators);
        }

        public override int GetHashCode() {
            return specifierQualifierList.GetHashCode();
        }

        public readonly IEnumerable<STTypeSpecQual> specifierQualifierList;
        public readonly IEnumerable<STStructDeclarator> declarators;
    }

    public sealed class STStructDeclarator : STNode, IEquatable<STStructDeclarator> {

        public STStructDeclarator(STDeclarator declarator, STExpr expr) {
            this.declarator = declarator;
            this.expr = expr;
        }

        public override Position Pos => declarator == null ? expr.Pos : declarator.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STStructDeclarator);
        }

        public bool Equals(STStructDeclarator x) {
            return x != null
                && NullableEquals(x.declarator, declarator)
                && NullableEquals(x.expr, expr);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public readonly STDeclarator declarator;
        public readonly STExpr expr;
    }

    public sealed class STEnumSpec : STTypeUserSpec, IEquatable<STEnumSpec> {

        public STEnumSpec(int line, STId id, IEnumerable<STEnum> enums)
            : base(Kind.ENUM) {
            pos = new Position { line = line };
            this.id = id;
            this.enums = enums;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as STEnumSpec);
        }

        public bool Equals(STEnumSpec x) {
            return x != null
                && NullableEquals(x.id, id)
                && NullableEquals(x.enums, enums);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override TUnqualified GetTUnqualified(ASTEnv env) {
            throw new NotImplementedException();
        }

        //public override TUnqualified GetTUnqualified(ASTEnv env) {
        //    if (enums == null) {
        //        // enum identifier
        //    } else {

        //        // If the type has a tag, check if redeclared.
        //        if (id != null && env.ContainsSymbolInCurrentScope(id.name)) {

        //        }

        //        // Make the new enum type.
        //        TypeEnum type = id == null ? new TypeEnum() : new TypeEnum(id.name);

        //        // Make the complete type for all the enums.
        //        T constType = new lcc.Type.Type(type, new lcc.Type.Type.Qualifier(true, false, false));

        //        foreach (var enumerator in enums) {

        //            ASTid id = enumerator.id;

        //            // First check if the name has been defined.
        //            if (env.ContainsSymbolInCurrentScope(id.name)) {
        //                Utility.TCErrRedecl(env.GetDeclaration(id.name), id);
        //            }

        //            // Check if the enumverator has an initializer.
        //            if (enumerator.expr != null) {

        //            }

        //            // Add every enumerator into the environment.
        //            env.AddSymbol(id.name, type, id.line);
        //        }

        //        return type;
        //    }
        //}

        private readonly Position pos;
        public readonly STId id;
        public readonly IEnumerable<STEnum> enums;
    }

    public sealed class STEnum : STNode, IEquatable<STEnum> {

        public STEnum(STId id, STExpr expr) {
            this.id = id;
            this.expr = expr;
        }

        public override Position Pos => id.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STEnum);
        }

        public bool Equals(STEnum x) {
            return x != null
                && x.id.Equals(id)
                && NullableEquals(expr, x.expr);
        }

        public override int GetHashCode() {
            return id.GetHashCode();
        }

        public readonly STId id;
        public readonly STExpr expr;
    }

    public sealed class STTypeQual : STTypeSpecQual, IEquatable<STTypeQual> {

        public enum Kind {
            CONST,
            RESTRICT,
            VOLATILE
        }

        public STTypeQual(int line, Kind kind) {
            this.pos = new Position { line = line };
            this.kind = kind;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as STTypeQual);
        }

        public bool Equals(STTypeQual x) {
            return x != null && x.pos.Equals(pos) && x.kind == kind;
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public readonly Kind kind;
        private readonly Position pos;
    }

    public sealed class STFuncSpec : STDeclSpec, IEquatable<STFuncSpec> {

        public enum Kind {
            NONE,
            INLINE
        }

        public STFuncSpec(int line, Kind kind) {
            this.pos = new Position { line = line };
            this.kind = kind;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as STFuncSpec);
        }

        public bool Equals(STFuncSpec x) {
            return x != null && x.pos.Equals(pos) && x.kind == kind;
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public readonly Kind kind;
        private readonly Position pos;

    }

    public sealed class STDeclarator : STNode, IEquatable<STDeclarator> {

        public STDeclarator(IEnumerable<STPtr> pointers, STDirDeclarator direct) {
            this.pointers = pointers;
            this.direct = direct;
        }

        public bool Equals(STDeclarator x) {
            return x != null && x.pointers.SequenceEqual(pointers) && x.direct.Equals(direct);
        }

        public override bool Equals(object obj) {
            return Equals(obj as STDeclarator);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public override Position Pos => direct.Pos;

        public IEnumerable<STPtr> pointers;
        public STDirDeclarator direct;
    }

    public abstract class STDirDeclarator : STNode {
        public abstract string Name { get; }
    }

    public sealed class STIdDeclarator : STDirDeclarator, IEquatable<STIdDeclarator> {

        public STIdDeclarator(STId id) {
            this.id = id;
        }

        public override string Name => id.name;
        public override Position Pos => id.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STIdDeclarator);
        }

        public bool Equals(STIdDeclarator i) {
            return i != null && i.id.Equals(id);
        }

        public override int GetHashCode() {
            return id.GetHashCode();
        }

        public readonly STId id;
    }

    public sealed class STParDeclarator : STDirDeclarator, IEquatable<STParDeclarator> {

        public STParDeclarator(STDeclarator declarator) {
            this.declarator = declarator;
        }

        public override string Name => declarator.direct.Name;
        public override Position Pos => declarator.Pos;

        public bool Equals(STParDeclarator x) {
            return x != null && x.declarator.Equals(declarator);
        }

        public override bool Equals(object obj) {
            return Equals(obj as STParDeclarator);
        }

        public override int GetHashCode() {
            return declarator.GetHashCode();
        }

        public readonly STDeclarator declarator;
    }

    public sealed class STFuncDeclarator : STDirDeclarator, IEquatable<STFuncDeclarator> {

        public STFuncDeclarator(
            STDirDeclarator direct,
            IEnumerable<STParam> parameters,
            bool isEllipis
            ) {
            this.direct = direct;
            this.parameters = parameters;
            this.isEllipis = isEllipis;
        }

        public STFuncDeclarator(STDirDeclarator direct, IEnumerable<STId> identifiers) {
            this.direct = direct;
            this.identifiers = identifiers;
        }

        public override string Name => direct.Name;

        public override Position Pos => direct.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STFuncDeclarator);
        }

        public bool Equals(STFuncDeclarator x) {
            return x != null && x.direct.Equals(direct)
                && NullableEquals(x.parameters, parameters)
                && x.isEllipis == isEllipis
                && NullableEquals(x.identifiers, identifiers);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public readonly STDirDeclarator direct;

        // ( parameter-type-list )
        public readonly IEnumerable<STParam> parameters;
        public readonly bool isEllipis;

        // ( identifier-list_opt )
        public readonly IEnumerable<STId> identifiers;
    }

    public sealed class STArrDeclarator : STDirDeclarator, IEquatable<STArrDeclarator> {

        public STArrDeclarator(
            STDirDeclarator direct,
            IEnumerable<STTypeQual> qualifiers,
            STExpr expr,
            bool isStatic
            ) {
            this.direct = direct;
            this.qualifiers = qualifiers;
            this.expr = expr;
            this.isStatic = isStatic;
            this.isStar = false;
        }

        public STArrDeclarator(STDirDeclarator direct, IEnumerable<STTypeQual> qualifiers) {
            this.direct = direct;
            this.qualifiers = qualifiers;
            this.expr = null;
            this.isStar = true;
            this.isStatic = false;
        }

        public override string Name => direct.Name;

        public bool Equals(STArrDeclarator x) {
            return x != null && x.direct.Equals(direct)
                && x.qualifiers.SequenceEqual(qualifiers)
                && x.isStatic == isStatic && x.isStar == isStar
                && NullableEquals(x.expr, expr);
        }

        public override bool Equals(object obj) {
            return Equals(obj as STArrDeclarator);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public override Position Pos => direct.Pos;

        public readonly STDirDeclarator direct;

        public readonly STExpr expr;
        public readonly IEnumerable<STTypeQual> qualifiers;
        public readonly bool isStatic;
        public readonly bool isStar;
    }

    public sealed class STPtr : STNode, IEquatable<STPtr> {

        public STPtr(int line, IEnumerable<STTypeQual> qualifiers) {
            this.pos = new Position { line = line };
            this.qualifiers = qualifiers;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as STPtr);
        }

        public bool Equals(STPtr x) {
            return x != null && x.pos.Equals(pos) && x.qualifiers.SequenceEqual(qualifiers);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        private readonly Position pos;
        public readonly IEnumerable<STTypeQual> qualifiers;
    }

    public sealed class STParam : STNode, IEquatable<STParam> {

        public STParam(STDeclSpecs specifiers, STDeclarator declarator) {
            this.specifiers = specifiers;
            this.declarator = declarator;
        }

        public STParam(STDeclSpecs specifiers, STAbsDeclarator absDeclarator = null) {
            this.specifiers = specifiers;
            this.absDeclarator = absDeclarator;
        }

        public override Position Pos => specifiers.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STParam);
        }

        public bool Equals(STParam x) {
            return x != null && specifiers.Equals(x.specifiers) && NullableEquals(x.declarator, declarator)
                && NullableEquals(x.absDeclarator, absDeclarator);
        }

        public override int GetHashCode() {
            return specifiers.GetHashCode();
        }

        public readonly STDeclSpecs specifiers;
        public readonly STDeclarator declarator;
        public readonly STAbsDeclarator absDeclarator;
    }

    public sealed class ASTTypeName : STNode, IEquatable<ASTTypeName> {

        public ASTTypeName(IEnumerable<STTypeSpecQual> specifiers, STAbsDeclarator declarator = null) {
            this.specifiers = specifiers;
            this.declarator = declarator;
        }

        public bool Equals(ASTTypeName x) {
            return x != null && x.specifiers.SequenceEqual(specifiers) && NullableEquals(declarator, x.declarator);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ASTTypeName);
        }

        public override int GetHashCode() {
            return specifiers.First().GetHashCode();
        }

        public override Position Pos => specifiers.First().Pos;

        public readonly IEnumerable<STTypeSpecQual> specifiers;
        public readonly STAbsDeclarator declarator;
    }

    public sealed class STAbsDeclarator : STNode, IEquatable<STAbsDeclarator> {

        public STAbsDeclarator(IEnumerable<STPtr> pointers, STAbsDirDeclarator direct) {
            this.pointers = pointers;
            this.direct = direct;
        }

        public bool Equals(STAbsDeclarator x) {
            return x != null && x.pointers.SequenceEqual(pointers) && NullableEquals(x.direct, direct);
        }

        public override bool Equals(object obj) {
            return Equals(obj as STAbsDeclarator);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override Position Pos => direct == null ? pointers.First().Pos : direct.Pos;

        public IEnumerable<STPtr> pointers;

        /// <summary>
        /// Nullable.
        /// </summary>
        public STAbsDirDeclarator direct;
    }

    public abstract class STAbsDirDeclarator : STNode { }

    public sealed class STAbsDirDeclaratorNil : STAbsDirDeclarator {
        public STAbsDirDeclaratorNil(int line) {
            pos = new Position { line = line };
        }

        public bool Equals(STAbsDirDeclaratorNil x) {
            return x != null && x.pos.Equals(pos);
        }

        public override bool Equals(object obj) {
            return Equals(obj as STAbsDirDeclaratorNil);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override Position Pos => pos;
        private readonly Position pos;
    }

    public sealed class STAbsParDeclarator : STAbsDirDeclarator, IEquatable<STAbsParDeclarator> {

        public STAbsParDeclarator(STAbsDeclarator declarator) {
            this.declarator = declarator;
        }

        public bool Equals(STAbsParDeclarator x) {
            return x != null && x.declarator.Equals(declarator);
        }

        public override bool Equals(object obj) {
            return Equals(obj as STAbsParDeclarator);
        }

        public override int GetHashCode() {
            return declarator.GetHashCode();
        }

        public override Position Pos => declarator.Pos;

        public readonly STAbsDeclarator declarator;
    }

    public sealed class STAbsArrDeclarator : STAbsDirDeclarator, IEquatable<STAbsArrDeclarator> {

        public STAbsArrDeclarator(
            STAbsDirDeclarator direct,
            IEnumerable<STTypeQual> qualifiers,
            STExpr expr,
            bool isStatic
            ) {
            this.direct = direct;
            this.qualifiers = qualifiers;
            this.expr = expr;
            this.isStatic = isStatic;
            this.isStar = false;
        }

        public STAbsArrDeclarator(STAbsDirDeclarator direct) {
            this.direct = direct;
            this.qualifiers = new List<STTypeQual>();
            this.expr = null;
            this.isStar = true;
            this.isStatic = false;
        }

        public bool Equals(STAbsArrDeclarator x) {
            return x != null && NullableEquals(x.direct, direct)
                && x.qualifiers.SequenceEqual(qualifiers)
                && x.isStatic == isStatic && x.isStar == isStar
                && NullableEquals(x.expr, expr);
        }

        public override bool Equals(object obj) {
            return Equals(obj as STAbsArrDeclarator);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public override Position Pos => direct.Pos;

        public readonly STAbsDirDeclarator direct;

        public readonly STExpr expr;
        public readonly IEnumerable<STTypeQual> qualifiers;
        public readonly bool isStatic;
        public readonly bool isStar;
    }

    public sealed class STAbsFuncDeclarator : STAbsDirDeclarator, IEquatable<STAbsFuncDeclarator> {

        public STAbsFuncDeclarator(
            STAbsDirDeclarator direct,
            IEnumerable<STParam> parameters,
            bool isEllipis
            ) {
            this.direct = direct;
            this.parameters = parameters;
            this.isEllipis = isEllipis;
        }

        public override Position Pos => direct.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STAbsFuncDeclarator);
        }

        public bool Equals(STAbsFuncDeclarator x) {
            return x != null && NullableEquals(x.direct, direct)
                && NullableEquals(x.parameters, parameters)
                && x.isEllipis == isEllipis;
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public readonly STAbsDirDeclarator direct;

        // ( parameter-type-list )
        public readonly IEnumerable<STParam> parameters;
        public readonly bool isEllipis;
    }

    public sealed class STTypedefName : STTypeUserSpec, IEquatable<STTypedefName> {

        public STTypedefName(STId identifier) : base(Kind.TYPEDEF) {
            name = identifier.name;
            pos = identifier.Pos;
        }

        public bool Equals(STTypedefName x) {
            return x != null && x.name.Equals(name) && x.pos.Equals(pos);
        }

        public override bool Equals(object obj) {
            return Equals(obj as STTypedefName);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override TUnqualified GetTUnqualified(ASTEnv env) {
            throw new NotImplementedException();
        }

        public override Position Pos => pos;

        public readonly string name;
        private readonly Position pos;
    }

    /// <summary>
    /// initializer
    ///     : assignment-expression
    ///     | { initializer-list }
    ///     | { initializer-list , }
    ///     ;
    /// </summary>
    public sealed class STInitializer : STNode, IEquatable<STInitializer> {

        /// <summary>
        /// initializer : assignment-expression;
        /// </summary>
        /// <param name="expr"></param>
        public STInitializer(STExpr expr) { this.expr = expr; }
        public STInitializer(IEnumerable<STInitItem> items) { this.items = items; }

        public bool Equals(STInitializer x) {
            return x != null
                && NullableEquals(x.expr, expr)
                && NullableEquals(x.items, items);
        }

        public override bool Equals(object obj) {
            return Equals(obj as STInitializer);
        }

        public override int GetHashCode() {
            return expr != null ? expr.GetHashCode() : items.First().GetHashCode();
        }

        public override Position Pos => expr != null ? expr.Pos : items.First().Pos;

        public readonly STExpr expr;
        public readonly IEnumerable<STInitItem> items;
    }

    /// <summary>
    /// init-item : designation_opt initializer;
    /// </summary>
    public sealed class STInitItem : STNode, IEquatable<STInitItem> {

        public STInitItem(STInitializer initializer, IEnumerable<STDesignator> designators = null) {
            this.initializer = initializer;
            this.designators = designators;
        }

        public bool Equals(STInitItem x) {
            return x != null
                && x.initializer.Equals(initializer)
                && NullableEquals(x.designators, designators);
        }

        public override bool Equals(object obj) {
            return Equals(obj as STInitItem);
        }

        public override int GetHashCode() {
            return initializer.GetHashCode();
        }

        public override Position Pos => designators == null ? initializer.Pos : designators.First().Pos;

        public readonly STInitializer initializer;
        public readonly IEnumerable<STDesignator> designators;
    }

    /// <summary>
    /// [ constant-expression ]
    /// .identfier
    /// </summary>
    public sealed class STDesignator : STNode, IEquatable<STDesignator> {

        public STDesignator(STExpr expr) { this.expr = expr; }
        public STDesignator(STId id) { this.id = id; }

        public override Position Pos => expr != null ? expr.Pos : id.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as STDesignator);
        }

        public bool Equals(STDesignator x) {
            return x != null
                && NullableEquals(x.expr, expr)
                && NullableEquals(x.id, id);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public readonly STExpr expr;
        public readonly STId id;
    }

}
