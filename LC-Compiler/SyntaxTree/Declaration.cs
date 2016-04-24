using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.SyntaxTree {

    public sealed class STDeclaration : STStmt, IEquatable<STDeclaration> {

        public STDeclaration(
            DeclSpecs specifiers,
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

        public readonly DeclSpecs specifiers;
        public readonly IEnumerable<STInitDeclarator> declarators;

        public T TypeCheck(Env env) {

            return null;
        }
    }

    public sealed class DeclSpecs : Node, IEquatable<DeclSpecs> {

        public DeclSpecs(
            IEnumerable<DeclSpec> all,
            STStoreSpec.Kind storage,
            IEnumerable<TypeSpec.Kind> keys,
            STFuncSpec.Kind function = STFuncSpec.Kind.NONE
            ) {
            this.storage = storage;
            this.keys = keys;
            this.function = function;
            qualifiers = GetQualifiers(from s in all.OfType<TypeQual>() select s.kind);
            pos = all.First().Pos;
        }

        public DeclSpecs(
            IEnumerable<DeclSpec> all,
            STStoreSpec.Kind storage,
            TypeUserSpec specifier,
            STFuncSpec.Kind function = STFuncSpec.Kind.NONE
            ) {
            this.storage = storage;
            this.function = function;
            this.specifier = specifier;
            qualifiers = GetQualifiers(from s in all.OfType<TypeQual>() select s.kind);
            pos = all.First().Pos;
        }

        public bool Equals(DeclSpecs x) {
            return x != null && x.pos.Equals(pos)
                && x.storage == storage
                && x.qualifiers.Equals(qualifiers)
                && keys == null ? x.keys == null : keys.SequenceEqual(x.keys)
                && NullableEquals(x.specifier, specifier)
                && x.function == function;
        }

        public override bool Equals(object obj) {
            return Equals(obj as DeclSpecs);
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
        public readonly IEnumerable<TypeSpec.Kind> keys;

        /// <summary>
        /// All the struct, union, enum, typedef specifier.
        /// </summary>
        public readonly TypeUserSpec specifier;

        /// <summary>
        /// Get the qualified type as LValue.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public T GetT(Env env) {
            return GetTUnqualified(env).Qualify(qualifiers, T.LR.L, storage);
        }

        /// <summary>
        /// Evaluate the type specifiers and get the unqualified type.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        private TUnqualified GetTUnqualified(Env env) {
            if (keys != null) {
                // This is built-in type.
                if (dict.ContainsKey(keys)) return dict[keys];
                else throw new Error(pos, string.Format("Unkown type: {0}", keys.Aggregate("", (str, key) => str + " " + key)));
            } else if (specifier != null) {
                // This is a user-defined type.
                return specifier.GetTUnqualified(env);
            } else {
                // Error!
                throw new Error(pos, "At least one type specifier should be given.");
            }
        }

        /// <summary>
        /// Evaluate the type qualifiers.
        /// </summary>
        /// <returns></returns>
        public static TQualifiers GetQualifiers(IEnumerable<TypeQual.Kind> qualifiers) {
            var tuple = new Tuple<bool, bool, bool>(
                qualifiers.Contains(TypeQual.Kind.CONST),
                qualifiers.Contains(TypeQual.Kind.RESTRICT), 
                qualifiers.Contains(TypeQual.Kind.VOLATILE));
            return TQualifiers.dict[tuple];
        }

        private readonly Position pos;

        #region TypeSpecifier Map
        private class ListComparer : IEqualityComparer<IEnumerable<TypeSpec.Kind>> {
            public bool Equals(IEnumerable<TypeSpec.Kind> x, IEnumerable<TypeSpec.Kind> y) {
                return x.SequenceEqual(y);
            }

            public int GetHashCode(IEnumerable<TypeSpec.Kind> x) {
                int hash = 0;
                foreach (var s in x) {
                    hash |= s.GetHashCode();
                }
                return hash;
            }
        }

        private static Dictionary<IEnumerable<TypeSpec.Kind>, TUnqualified> dict =
            new Dictionary<IEnumerable<TypeSpec.Kind>, TUnqualified>(new ListComparer()) {
                {
                    // void
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.VOID
                    } orderby s ascending select s,
                    TypeVoid.Instance
                },
                {
                    // char
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.CHAR
                    } orderby s ascending select s,
                    TChar.Instance
                },
                {
                    // signed char
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.SIGNED,
                        TypeSpec.Kind.CHAR
                    } orderby s ascending select s,
                    TSChar.Instance
                },
                {
                    // unsigned char
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.UNSIGNED,
                        TypeSpec.Kind.CHAR
                    } orderby s ascending select s,
                    TUChar.Instance
                },
                {
                    // short
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.SHORT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // signed short
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.SIGNED,
                        TypeSpec.Kind.SHORT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // short int
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.SHORT,
                        TypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // signed short int
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.SIGNED,
                        TypeSpec.Kind.SHORT,
                        TypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TShort.Instance
                },
                {
                    // unsigned short
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.UNSIGNED,
                        TypeSpec.Kind.SHORT
                    } orderby s ascending select s,
                    TUShort.Instance
                },
                {
                    // unsigned short int
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.UNSIGNED,
                        TypeSpec.Kind.SHORT,
                        TypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TUShort.Instance
                },
                {
                    // int
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TInt.Instance
                },
                {
                    // signed
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.SIGNED
                    } orderby s ascending select s,
                    TInt.Instance
                },
                {
                    // signed int
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.SIGNED,
                        TypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TInt.Instance
                },
                {
                    // unsigned
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.UNSIGNED
                    } orderby s ascending select s,
                    TUInt.Instance
                },
                {
                    // unsigned int
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.UNSIGNED,
                        TypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TUInt.Instance
                },
                {
                    // long
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // signed long
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.SIGNED,
                        TypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // long int
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.LONG,
                        TypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // signed long int
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.SIGNED,
                        TypeSpec.Kind.LONG,
                        TypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TLong.Instance
                },
                {
                    // unsigned long
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.UNSIGNED,
                        TypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TULong.Instance
                },
                {
                    // unsigned long int
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.UNSIGNED,
                        TypeSpec.Kind.LONG,
                        TypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TULong.Instance
                },
                {
                    // long long
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.LONG,
                        TypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // signed long long
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.SIGNED,
                        TypeSpec.Kind.LONG,
                        TypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // long long int
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.LONG,
                        TypeSpec.Kind.LONG,
                        TypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // signed long long int
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.SIGNED,
                        TypeSpec.Kind.LONG,
                        TypeSpec.Kind.LONG,
                        TypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TLLong.Instance
                },
                {
                    // unsigned long long
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.UNSIGNED,
                        TypeSpec.Kind.LONG,
                        TypeSpec.Kind.LONG
                    } orderby s ascending select s,
                    TULLong.Instance
                },
                {
                    // unsigned long long int
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.UNSIGNED,
                        TypeSpec.Kind.LONG,
                        TypeSpec.Kind.LONG,
                        TypeSpec.Kind.INT
                    } orderby s ascending select s,
                    TULLong.Instance
                },
                {
                    // float
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.FLOAT
                    } orderby s ascending select s,
                    TFloat.Instance
                },
                {
                    // double
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.DOUBLE
                    } orderby s ascending select s,
                    TDouble.Instance
                },
                {
                    // long double
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.LONG,
                        TypeSpec.Kind.DOUBLE
                    } orderby s ascending select s,
                    TLDouble.Instance
                },
                {
                    // double
                    from s in new List<TypeSpec.Kind> {
                        TypeSpec.Kind.BOOL
                    } orderby s ascending select s,
                    TBool.Instance
                },
            };
        #endregion
    }

    public sealed class STInitDeclarator : Node, IEquatable<STInitDeclarator> {

        public STInitDeclarator(Declarator declarator, STInitializer initializer = null) {
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

        public readonly Declarator declarator;
        public readonly STInitializer initializer;
    }

    public abstract class DeclSpec : Node { }

    public sealed class STStoreSpec : DeclSpec, IEquatable<STStoreSpec> {

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

    public abstract class TypeSpecQual : DeclSpec { }

    public abstract class TypeSpec : TypeSpecQual {

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

        public TypeSpec(Kind kind) {
            this.kind = kind;
        }

        public readonly Kind kind;
    }

    public sealed class STTypeKeySpec : TypeSpec, IEquatable<STTypeKeySpec> {

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
    public abstract class TypeUserSpec : TypeSpec {
        public TypeUserSpec(Kind kind) : base(kind) { }
        public abstract TUnqualified GetTUnqualified(Env env);
    }

    public sealed class StructUnionSpec : TypeUserSpec, IEquatable<StructUnionSpec> {

        public StructUnionSpec(
            int line,
            Id identifier,
            IEnumerable<StructDeclaration> declarations,
            Kind kind
            ) : base(kind) {
            if (kind != Kind.STRUCT && kind != Kind.UNION)
                throw new ArgumentException("This must be either struct or union!");
            pos = new Position { line = line };
            this.identifier = identifier;
            this.declarations = declarations;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as StructUnionSpec);
        }

        public bool Equals(StructUnionSpec x) {
            return x != null
                && NullableEquals(x.identifier, identifier)
                && NullableEquals(x.declarations, declarations);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        /// <summary>
        /// Evaluate the struct or union specifier and get the unqualified type.
        /// 
        /// struct/union tag
        ///     - check if the tag has been declaraed in the current scope
        ///     - yes -> return the type
        ///     - no  -> make this an incomplete type in the current scope
        ///     
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override TUnqualified GetTUnqualified(Env env) {

            Func<TagEntry, bool> IsSameType = (tagEntry) => {
                if (kind == Kind.STRUCT) return tagEntry.type.IsStruct;
                else return tagEntry.type.IsUnion;
            };

            if (declarations == null) {
                // struct/union tag
                var tag = identifier.name;
                
                // Check the tag in all scopes.
                var tagEntry = env.GetTag(tag, false);
                if (tagEntry != null) {
                    // This tag has been declaraed, check if this is the same type.
                    if (IsSameType(tagEntry.Value))
                        return tagEntry.Value.type;
                    else
                        throw new ErrDeclareTagAsDifferentType(pos, tag, tagEntry.Value.node.Pos, tagEntry.Value.type);
                } else {
                    // This tag has not been declaraed, make it an incomplete type.
                    TStructUnion type = kind == Kind.STRUCT ? new TStruct(tag) as TStructUnion : new TUnion(tag);
                    env.AddTag(identifier.name, type, this);
                    return type;
                }
            } else {
                TStructUnion type;
                TagEntry entry;

                if (identifier == null) {
                    // struct/union { ... }
                    // should declare a unique new type.
                    type = kind == Kind.STRUCT ? new TStruct() as TStructUnion : new TUnion();
                    entry = env.AddTag(type.tag, type, this);
                } else {
                    // struct/union tag { ... }
                    // Check if this tag has been declared in the current scope.
                    var tag = identifier.name;
                    var tmp = env.GetTag(tag, true);
                    if (tmp != null) {
                        // This tag has been declaraed, check if this is the same type
                        if (!IsSameType(tmp.Value))
                            throw new ErrDeclareTagAsDifferentType(pos, tag, tmp.Value.node.Pos, tmp.Value.type);
                        // Check if this type is already complete.
                        if (tmp.Value.type.IsComplete)
                            throw new ErrRedefineTag(pos, tag, tmp.Value.node.Pos);
                        entry = tmp.Value;
                        type = entry.type as TStructUnion;
                    } else {
                        type = kind == Kind.STRUCT ? new TStruct(tag) as TStructUnion : new TUnion(tag);
                        entry = env.AddTag(type.tag, type, this);
                    }
                }

                // Push a new scope.
                env.PushScope(ScopeKind.BLOCK);

                // Get all the fields.
                var fields = declarations.Aggregate(
                    Enumerable.Empty<Tuple<string, T>>(),
                    (acc, decl) => acc.Concat(decl.GetFields(env)));

                // Complete the definition of the struct/union with all these structs.
                if (kind == Kind.STRUCT) {
                    type.DefStruct(fields);
                } else {
                    type.DefUnion(fields);
                }

                // Update the entry and set the node to this syntax tree node.
                entry.node = this;

                // Pop a new scope.
                env.PopScope();

                return type;
            }
        }

        private readonly Position pos;

        /// <summary>
        /// Nullable.
        /// </summary>
        public readonly Id identifier;
        /// <summary>
        /// Nullable.
        /// </summary>
        public readonly IEnumerable<StructDeclaration> declarations;
    }

    public sealed class StructDeclaration : Node, IEquatable<StructDeclaration> {

        public StructDeclaration(
            DeclSpecs specifiers,
            IEnumerable<StructDeclarator> declarators
            ) {
            this.specifiers = specifiers;
            this.declarators = declarators;
        }

        public override Position Pos => specifiers.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as StructDeclaration);
        }

        public bool Equals(StructDeclaration x) {
            return x != null
                && x.specifiers.Equals(specifiers)
                && x.declarators.SequenceEqual(declarators);
        }

        public override int GetHashCode() {
            return specifiers.GetHashCode();
        }

        /// <summary>
        /// Evaluate the fields and get all the fields.
        /// </summary>
        /// <param name="env"> Environment. </param>
        /// <returns> A list of tuples, containing the field name and its type. </returns>
        public IEnumerable<Tuple<string, T>> GetFields(Env env) {

            // First get the type.
            T type = specifiers.GetT(env);

            // Get all the fields.
            return declarators.Aggregate(
                new LinkedList<Tuple<string, T>>(),
                (acc, decl) => {
                    acc.AddLast(decl.GetField(env, type));
                    return acc;
                });
        }

        public readonly DeclSpecs specifiers;
        public readonly IEnumerable<StructDeclarator> declarators;
    }

    /// <summary>
    /// struct-declarator
    ///     : declarator
    ///     | declarator_opt : constant-expression
    ///     ;
    /// </summary>
    public sealed class StructDeclarator : Node, IEquatable<StructDeclarator> {

        public StructDeclarator(Declarator declarator, Expr expr) {
            this.declarator = declarator;
            this.expr = expr;
        }

        public override Position Pos => declarator == null ? expr.Pos : declarator.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as StructDeclarator);
        }

        public bool Equals(StructDeclarator x) {
            return x != null
                && NullableEquals(x.declarator, declarator)
                && NullableEquals(x.expr, expr);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        /// <summary>
        /// Get the field and the type.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Tuple<string, T> GetField(Env env, T type) {


            if (declarator != null) {

            }

            // Whether a bit field.
            if (expr == null) {

            } else {

            }

            throw new NotImplementedException();
        }

        public readonly Declarator declarator;
        public readonly Expr expr;
    }

    public sealed class STEnumSpec : TypeUserSpec, IEquatable<STEnumSpec> {

        public STEnumSpec(int line, Id id, IEnumerable<STEnum> enums)
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

        public override TUnqualified GetTUnqualified(Env env) {
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
        public readonly Id id;
        public readonly IEnumerable<STEnum> enums;
    }

    public sealed class STEnum : Node, IEquatable<STEnum> {

        public STEnum(Id id, Expr expr) {
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

        public readonly Id id;
        public readonly Expr expr;
    }

    public sealed class TypeQual : TypeSpecQual, IEquatable<TypeQual> {

        public enum Kind {
            CONST,
            RESTRICT,
            VOLATILE
        }

        public TypeQual(int line, Kind kind) {
            this.pos = new Position { line = line };
            this.kind = kind;
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as TypeQual);
        }

        public bool Equals(TypeQual x) {
            return x != null && x.pos.Equals(pos) && x.kind == kind;
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public readonly Kind kind;
        private readonly Position pos;
    }

    public sealed class STFuncSpec : DeclSpec, IEquatable<STFuncSpec> {

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

    public sealed class Declarator : Node, IEquatable<Declarator> {

        public Declarator(IEnumerable<Ptr> pointers, DirDeclarator direct) {
            this.pointers = pointers;
            this.direct = direct;
        }

        public bool Equals(Declarator x) {
            return x != null && x.pointers.SequenceEqual(pointers) && x.direct.Equals(direct);
        }

        public override bool Equals(object obj) {
            return Equals(obj as Declarator);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        /// <summary>
        /// Get the identifier and the type of the declarator.
        /// </summary>
        /// <param name="env"> Environment </param>
        /// <param name="type"> Type specified by declaration specifiers. </param>
        /// <returns></returns>
        public Tuple<string, T> Declare(Env env, T type) {

            // Add the pointer derivation.
            foreach (var pointer in pointers) {
                type = pointer.GetPtr(type);
            }

            // Get the declaration from direct declarator.
            return direct.Declare(env, type);
        }

        public override Position Pos => direct.Pos;

        /// <summary>
        /// Zero or more pointers.
        /// </summary>
        public IEnumerable<Ptr> pointers;
        public DirDeclarator direct;
    }

    public abstract class DirDeclarator : Node {
        public abstract string Name { get; }
        public abstract Tuple<string, T> Declare(Env env, T type);
    }

    public sealed class IdDeclarator : DirDeclarator, IEquatable<IdDeclarator> {

        public IdDeclarator(Id id) {
            this.id = id;
        }

        public override string Name => id.name;
        public override Position Pos => id.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as IdDeclarator);
        }

        public bool Equals(IdDeclarator i) {
            return i != null && i.id.Equals(id);
        }

        public override int GetHashCode() {
            return id.GetHashCode();
        }

        public override Tuple<string, T> Declare(Env env, T type) {
            return new Tuple<string, T>(id.name, type);
        }

        public readonly Id id;
    }

    public sealed class ParDeclarator : DirDeclarator, IEquatable<ParDeclarator> {

        public ParDeclarator(Declarator declarator) {
            this.declarator = declarator;
        }

        public override string Name => declarator.direct.Name;
        public override Position Pos => declarator.Pos;

        public bool Equals(ParDeclarator x) {
            return x != null && x.declarator.Equals(declarator);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ParDeclarator);
        }

        public override int GetHashCode() {
            return declarator.GetHashCode();
        }
        public override Tuple<string, T> Declare(Env env, T type) {
            return declarator.Declare(env, type);
        }

        public readonly Declarator declarator;
    }

    public sealed class FuncDeclarator : DirDeclarator, IEquatable<FuncDeclarator> {

        public FuncDeclarator(
            DirDeclarator direct,
            IEnumerable<STParam> parameters,
            bool isEllipis
            ) {
            this.direct = direct;
            this.parameters = parameters;
            this.isEllipis = isEllipis;
        }

        public FuncDeclarator(DirDeclarator direct, IEnumerable<Id> identifiers) {
            this.direct = direct;
            this.identifiers = identifiers;
        }

        public override string Name => direct.Name;

        public override Position Pos => direct.Pos;

        public override bool Equals(object obj) {
            return Equals(obj as FuncDeclarator);
        }

        public bool Equals(FuncDeclarator x) {
            return x != null && x.direct.Equals(direct)
                && NullableEquals(x.parameters, parameters)
                && x.isEllipis == isEllipis
                && NullableEquals(x.identifiers, identifiers);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        public override Tuple<string, T> Declare(Env env, T type) {
            throw new NotImplementedException();
        }

        public readonly DirDeclarator direct;

        // ( parameter-type-list )
        public readonly IEnumerable<STParam> parameters;
        public readonly bool isEllipis;

        // ( identifier-list_opt )
        public readonly IEnumerable<Id> identifiers;
    }

    public sealed class ArrDeclarator : DirDeclarator, IEquatable<ArrDeclarator> {

        public ArrDeclarator(
            DirDeclarator direct,
            IEnumerable<TypeQual> qualifiers,
            Expr expr,
            bool isStatic
            ) {
            this.direct = direct;
            this.qualifiers = qualifiers;
            this.expr = expr;
            this.isStatic = isStatic;
            this.isStar = false;
        }

        public ArrDeclarator(DirDeclarator direct, IEnumerable<TypeQual> qualifiers) {
            this.direct = direct;
            this.qualifiers = qualifiers;
            this.expr = null;
            this.isStar = true;
            this.isStatic = false;
        }

        public override string Name => direct.Name;

        public bool Equals(ArrDeclarator x) {
            return x != null && x.direct.Equals(direct)
                && x.qualifiers.SequenceEqual(qualifiers)
                && x.isStatic == isStatic && x.isStar == isStar
                && NullableEquals(x.expr, expr);
        }

        public override bool Equals(object obj) {
            return Equals(obj as ArrDeclarator);
        }

        public override int GetHashCode() {
            return direct.GetHashCode();
        }

        /// <summary>
        /// Array declarator.
        /// For now only support complete array and incomplete array.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override Tuple<string, T> Declare(Env env, T type) {

            // The element type shall not be an incomplete type.
            if (!type.IsComplete) {
                throw new Error(Pos, string.Format("use incomplete type as array element: {0}.", type));
            }

            // The element type shall not be a function type.
            if (type.IsFunc) {
                throw new Error(Pos, string.Format("use function type as array element: {0}.", type));
            }

            // Check the expression is const integer expression.
            if (expr != null) {
                AST.ConstIntExpr expr = this.expr.ToAST(env) as AST.ConstIntExpr;
                if (expr == null) {
                    throw new Error(Pos, "use non-constant expression as array length.");
                }
                if (expr.value <= 0) {
                    throw new Error(Pos, "array length should be greater than zero.");
                }
                if (expr.value > TInt.Instance.MAX) {
                    throw new Error(Pos, string.Format("sorry we can't handle such long array: {0}.", expr.value));
                }
                return direct.Declare(env, type.Arr((int)expr.value));
            } else {
                // Set this as incomplete array.
                return direct.Declare(env, type.IArr());
            }
        }

        public override Position Pos => direct.Pos;

        public readonly DirDeclarator direct;

        public readonly Expr expr;
        public readonly IEnumerable<TypeQual> qualifiers;
        public readonly bool isStatic;
        public readonly bool isStar;
    }

    public sealed class Ptr : Node, IEquatable<Ptr> {

        public Ptr(int line, IEnumerable<TypeQual> qualifiers) {
            this.pos = new Position { line = line };
            this.qualifiers = DeclSpecs.GetQualifiers(qualifiers.Select(_ => _.kind));
        }

        public override Position Pos => pos;

        public override bool Equals(object obj) {
            return Equals(obj as Ptr);
        }

        public bool Equals(Ptr x) {
            return x != null && x.pos.Equals(pos) && x.qualifiers.Equals(qualifiers);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public T GetPtr(T type) {
            return type.Ptr(qualifiers);
        }

        private readonly Position pos;
        public readonly TQualifiers qualifiers;
    }

    public sealed class STParam : Node, IEquatable<STParam> {

        public STParam(DeclSpecs specifiers, Declarator declarator) {
            this.specifiers = specifiers;
            this.declarator = declarator;
        }

        public STParam(DeclSpecs specifiers, AbsDeclarator absDeclarator = null) {
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

        public readonly DeclSpecs specifiers;
        public readonly Declarator declarator;
        public readonly AbsDeclarator absDeclarator;
    }

    public sealed class TypeName : Node, IEquatable<TypeName> {

        public TypeName(DeclSpecs specifiers, AbsDeclarator declarator = null) {
            this.specifiers = specifiers;
            this.declarator = declarator;
        }

        public bool Equals(TypeName x) {
            return x != null && x.specifiers.Equals(specifiers) && NullableEquals(declarator, x.declarator);
        }

        public override bool Equals(object obj) {
            return Equals(obj as TypeName);
        }

        public override int GetHashCode() {
            return specifiers.GetHashCode();
        }

        public override Position Pos => specifiers.Pos;

        public readonly DeclSpecs specifiers;
        public readonly AbsDeclarator declarator;
    }

    public sealed class AbsDeclarator : Node, IEquatable<AbsDeclarator> {

        public AbsDeclarator(IEnumerable<Ptr> pointers, STAbsDirDeclarator direct) {
            this.pointers = pointers;
            this.direct = direct;
        }

        public bool Equals(AbsDeclarator x) {
            return x != null && x.pointers.SequenceEqual(pointers) && NullableEquals(x.direct, direct);
        }

        public override bool Equals(object obj) {
            return Equals(obj as AbsDeclarator);
        }

        public override int GetHashCode() {
            return Pos.GetHashCode();
        }

        public override Position Pos => direct == null ? pointers.First().Pos : direct.Pos;

        public IEnumerable<Ptr> pointers;

        /// <summary>
        /// Nullable.
        /// </summary>
        public STAbsDirDeclarator direct;
    }

    public abstract class STAbsDirDeclarator : Node { }

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

        public STAbsParDeclarator(AbsDeclarator declarator) {
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

        public readonly AbsDeclarator declarator;
    }

    public sealed class STAbsArrDeclarator : STAbsDirDeclarator, IEquatable<STAbsArrDeclarator> {

        public STAbsArrDeclarator(
            STAbsDirDeclarator direct,
            IEnumerable<TypeQual> qualifiers,
            Expr expr,
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
            this.qualifiers = new List<TypeQual>();
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

        public readonly Expr expr;
        public readonly IEnumerable<TypeQual> qualifiers;
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

    public sealed class STTypedefName : TypeUserSpec, IEquatable<STTypedefName> {

        public STTypedefName(Id identifier) : base(Kind.TYPEDEF) {
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

        public override TUnqualified GetTUnqualified(Env env) {
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
    public sealed class STInitializer : Node, IEquatable<STInitializer> {

        /// <summary>
        /// initializer : assignment-expression;
        /// </summary>
        /// <param name="expr"></param>
        public STInitializer(Expr expr) { this.expr = expr; }
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

        public readonly Expr expr;
        public readonly IEnumerable<STInitItem> items;
    }

    /// <summary>
    /// init-item : designation_opt initializer;
    /// </summary>
    public sealed class STInitItem : Node, IEquatable<STInitItem> {

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
    public sealed class STDesignator : Node, IEquatable<STDesignator> {

        public STDesignator(Expr expr) { this.expr = expr; }
        public STDesignator(Id id) { this.id = id; }

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

        public readonly Expr expr;
        public readonly Id id;
    }

}
