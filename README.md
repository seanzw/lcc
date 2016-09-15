# lcc
Light C-Compiler in C#.

## Introduction
Light C-Compiler is an entertainment project for me to practice my programming skill and reach into some deep corners of C language. It is based on C99 and works ONLY on Windows 32bits + Visual Studio 2015.

Like many other compilers, lcc divides the whole compilation into 4 stages: lexical analysis, syntax checking, semantic checking and code generation. For the first two stages, three simple tools are developed: RegEx, Lexer and Parserc (see below). The target platform is x86.

## How to use it
The compiler reads in multiple `.c` files and generates x86 assembly code to corresponding `.s` file. You can link it later with clang.

### Example
Inside `\LC-CompilerTests\ASTTests\code\doubly_linked_list` you can find an implementation of doubly linked list.

```C
typedef unsigned int size_t;
void* malloc(size_t size);
void free(void* ptr);

typedef unsigned short KeyT;

/// Introduction to Algorithms, 3rd edition
/// 10.2.8: np = next ^ prev.
typedef struct LinkedListNode {
    KeyT key;
    struct LinkedListNode* np;
} LinkedListNode;


/// Doubly circular linked list.
typedef struct LinkedList {
    LinkedListNode* head;
    LinkedListNode* tail;
} LinkedList;

LinkedListNode* linked_list_next(LinkedListNode* curr, LinkedListNode* prev) {
    return (LinkedListNode*)((int)curr->np ^ (int)prev);
}

LinkedList* linked_list_init() {
    LinkedList* list;
    list = malloc(sizeof(LinkedList));
    if (list != 0) {
        list->head = malloc(sizeof(LinkedListNode));
        list->tail = malloc(sizeof(LinkedListNode));
        if (list->head != 0 && list->tail != 0) {
            list->head->np = list->tail;
            list->tail->np = list->head;
            return list;
        }
        else {
            if (list->head != 0) {
                free(list->head);
            }
            if (list->tail != 0) {
                free(list->tail);
            }
            free(list);
        }
    }
    return 0;
}

void linked_list_free(LinkedList* list) {
    LinkedListNode* curr;
    LinkedListNode* prev;
    LinkedListNode* next;
    curr = list->head->np;
    prev = list->head;
    while (curr != list->tail) {
        next = linked_list_next(curr, prev);
        prev = curr;
        curr = next;
        free(prev);
    }
    free(list->head);
    free(list->tail);
    free(list);
}

int linked_list_is_empty(LinkedList* list) {
    return list->head->np == list->tail;
}

/// Insert at head.
int linked_list_insert(LinkedList* list, KeyT key) {
    LinkedListNode* i;
    i = malloc(sizeof(LinkedListNode));
    if (i != 0) {
        i->key = key;
        i->np = (LinkedListNode*)((int)list->head ^ (int)list->head->np);
        list->head->np->np = (LinkedListNode*)((int)list->head->np->np ^ (int)i ^ (int)list->head);
        list->head->np = i;
        return 0;
    }
    return -1;
}

/// Search a key, return list->tail if not found.
LinkedListNode* linked_list_search(LinkedList* list, KeyT key) {
    LinkedListNode* curr;
    LinkedListNode* prev;
    LinkedListNode* next;
    list->tail->key = key;
    for (prev = list->head, curr = list->head->np; curr->key != key; prev = curr, curr = next) {
        next = linked_list_next(curr, prev);
    }
    return curr;
}

/// Delete a node, return -1 if not found.
int linked_list_delete(LinkedList* list, LinkedListNode* x) {
    LinkedListNode* curr;
    LinkedListNode* prev;
    LinkedListNode* next;
    for (prev = list->head, curr = list->head->np; curr != list->tail; prev = curr, curr = next) {
        next = linked_list_next(curr, prev);
        if (curr == x) {
            prev->np = (LinkedListNode*)((int)prev->np ^ (int)curr ^ (int)next);
            next->np = (LinkedListNode*)((int)next->np ^ (int)curr ^ (int)prev);
            free(curr);
            return 0;
        }
    }
    return -1;
}

/// Reverse a linked list (nonrecursive, O(1)).
void linked_list_reverse(LinkedList* list) {
    LinkedListNode* temp;
    temp = list->head;
    list->head = list->tail;
    list->tail = temp;
}
```

To test this implementation, build the project and type the follow commands in `\LC-Compiler\bin\Debug`:
```shell
> cp ../../../LC-CompilerTests/ASTTests/code/doubly_linked_list/*.c .
> clang -S doubly_linked_list_main.c -masm=intel -o doubly_linked_list_main.s
> ./lcc.exe doubly_linked_list.c
> clang doubly_linked_list.s doubly_linked_list_main.s -o a.exe
> ./a.exe
```

These commands compile `doubly_linked_list.c` with lcc and `doubly_linked_list_main.c` with clang, link them together using clang. You should see the output:
```shell
> everything is fine!
```

## How does it work
### 1. RegEx
* Simple regular expression library.

It supports 16 bit character set and the following regular expression operator:
* `*`: appears 0 or more times.
* `+`: appears 1 or more times.
* `|`: applys rule 0 or rule 1.
* `\`: escapes character.
* `.`: wildcard character.
* `()`: combines elements together.
* `[-]`: character in range.

First a DFA is constructed from the parsing result, and then compressed into a NFA.

### 2. Lexer (with RegEx)
* Flex-like lexer generator.
* The lexer of Lexer is generated by Lexer itself.

Lexer reads in a set of regular expressions and generates a lexer in `C#` that recognizes them. Notice that the lexer of Lexer is generated by Lexer itself. Here is the lexical rule of Lexer as a simple example.

```
%N $[A-Z_]+$

%%

using Lexer;

%%

$\$(\\[^\n\r\t]|[^\n\r\t\$\\])*\$$  
    tokens.Add(new T_REGEX(text.Substring(1, text.Length - 2)));

$[^\$ \n\r\t%][^\r\n]*$
    tokens.Add(new T_CODE(text));

$%{N}$
    tokens.Add(new T_ALIAS(text.Substring(1)));

$[ \n\r\t]+$
    
$%%$
    tokens.Add(new T_SPLITER());
%%
```

The input file contains four sections, separated by `%%`. Notice that:
* All the regular expressions should be wrapped in `$`, which makes `$` itself as a meta character and shoud be escaped.
* You can define a macro via `%` in the first section and use it in the regular expression via `{}`. For example here `N` represents `[A-Z_]+`.
* The order of the rules matters as if more than one rules are successfully matched, only the first rule is applied.
* If you want to use some other library or include your own code, for example using a namespace, you can do this at the second section. This section will be placed at the begining of the output file.
* You have to provide `Token` type as the following `Scan` function retuns `List<Token>`.
* After the rule is matched, you can write C# code to define the action. You can also define your own function by putting it at the last section, while Lexer offers the following APIs:

| Symbol  | Type                    | Meaning |
|:----:   |:----:                   |:------- |
| `text`  | `string`                | The matched text. |
| `tokens`| `List<Token>`           | The matched tokens. |
| `line`  | `int`                   | The line number of `text[0]`. |
| `More`  | `()->bool`              | Checks if the input stream has ended. |
| `Peek`  | `()->char`              | Peeks the next character. |
| `Next`  | `()->char`              | If More(), consumes one character and returns it, otherwise does nothing. |
| `Error` | `string->()`            | Throws a `LexerException`. |
| `Scan`  | `string->List<Token>`   | Scans the input string and returns a list of tokens. |

Lexer generates a `Lexer.cs` file, which the lexer contained in `LLexer` namespace. The lexer is a singleton and to use it, simply write the following code in your project:
```C#
var tokens = LLexer.Lexer.Instance.Scan(src);
```

### 3. Parserc (as parser combinator)
* Parser can generate multiple results (ambiguity).
* Resolves circular reference via lambda expression (lazy evaluation).

Parserc is a parser combinator library in `C#`, where `parser` is defined as:
```C#
public interface IParserResult<I, out V> {
    V Value { get; }
    ITokenStream<I> Remain { get; }
}
public delegate IEnumerable<IParserResult<I, V>> Parser<I, out V>(ITokenStream<I> tokens);
```
A parser reads a token stream and returns zero (when parsing fails) or more parsing results. The parsing result contains the result (usually a syntax tree) in `Value` field and the remaining tokens in `Remain` field.

#### 1. Primitive parsers
Parserc provides three primitive parsers as listed in the following table. You can also define your own.

| Primitive | Type | Meaning |
|:---------:|:----:|:--------|
| `Result`    | `V->Parser<I, V>` | Returns a parser that does not consume tokens and simply returns V. |
| `Zero`      | `()->Parser<I, V>` | Returns a parser that does not consume tokens and always fails. |
| `Item`      | `()->Parser<I, I>` | Returns a parser that consumes a token and returns it. |

#### 2. Combinators
Parserc defines many combinators. Here take the classical `bind` combinator:
```C#
/// <summary>
/// Bind two parsers together.
/// The result list is flatened.
/// </summary>
/// <typeparam name="I"> Input type. </typeparam>
/// <typeparam name="V"> Returned value type. </typeparam>
/// <param name="first"> The first parser. </param>
/// <param name="second"> Takes a value and returns a parser. </param>
/// <returns> A new parser. </returns>
public static Parser<I, V2> Bind<I, V1, V2>(this Parser<I, V1> first, Func<V1, Parser<I, V2>> second) {
    return tokens => {
        var ret = new List<IParserResult<I, V2>>();
        foreach (var r in first(tokens)) {
            foreach (var s in second(r.Value)(r.Remain)) {
                ret.Add(s);
            }
        }
        return ret;
    };
}
```
The `bind` combinator takes a parser `first` and a function `second`. Each result of `first` is feed into `second` to get a parser that parses the remaining tokens.

#### 3. Circular reference
Circular reference can happen when parser 1 depends on parser 2 and vice versa, Consider a simple example:
```C#
/// expression
///     : primary-expression
///     ;
/// primary-expression
///     : parser
///     | ( expression )
///     ;
public static Parser<I, V> Expression() {
    /// Depends on primary expression.
    return PrimaryExpression();
}
public static Parser<I, V> PrimaryExression() {
    /// Some other parser is defined.
    /// Apply parser, if fails, try ( expression ).
    return parser.Else(Expression().ParentLR()).
}
```

In this example when the compiler trys to evaluate `Expression` it will come back to itself, which means infinite recursion. Here I solve this problem via lambda expression so that the compiler can evaluate it later.

`Ref` combinator is defined as:
```C#
/// <summary>
/// Allow circular reference by delay the evaluation with lambda expression.
/// </summary>
/// <typeparam name="I"></typeparam>
/// <typeparam name="V"></typeparam>
/// <param name="reference"></param>
/// <returns></returns>
public static Parser<I, V> Ref<I, V>(Func<Parser<I, V>> reference) {
    Parser<I, V> p = null;
    return i => {
        if (p == null) {
            p = reference();
        }
        return p(i);
    };
}
```

Now the `PrimaryExpression` can be changed to:
```C#
public static Parser<I, V> PrimaryExression() {
    /// Some other parser is defined.
    /// Apply parser, if fails, try ( expression ).
    return parser.Else(Ref(Expression).ParentLR()).
}
```

### 4. Stage I: Lexical Analysis (with Lexer)
* Generated by Lexer.

With Lexer, it is trivial to write the lexer for C language. C99 standard defines the following lexical elements:

| Kind              | Example   |
|:----:             |:-------:  |
|keyword            | `int`     |
|identifier         | `foo`     |
|constant           | `0.1e2f`  |
|string literial    | `"hello"` |
|punctuator         | `!`       |

Notice that our simple RegEx cannot recognize block comment since it requires to **NOT** match `*/`. Hence a `Comment` function is defined to fix this.

### 5. Stage II: Syntax Analysis (with Parserc)
* Written using Parserc.
* Resovles identfier/typedef-name ambiguity via a simple environment.

With Parserc, it is intuitive to translate the grammar rule into a parser. For example the rule to parse cast expression:
```C#
/// <summary>
/// cast-expression
///     : unary-expression
///     | ( type-name ) cast-expression
///     ;
/// </summary>
/// <returns></returns>
public static Parserc.Parser<Token.Token, Expr> CastExpression() {
    return UnaryExpression()
        .Else(Ref(TypeName).ParentLR().Bind(name => Ref(CastExpression)
        .Select(expr => new Cast(name, expr))));
}
```

#### Identifier/typedef-name ambiguity
Sometimes it is ambiguous as whether a symbol is an identifier or a typedef name. To resolve this, a small environment is maintained during parsing. When the parser has finished parsing a typedef declaration, it add the typedef name into the environment. 

### 6. Stage III: Semantic Analysis
* A type system for C language.
* Scope-based environment with support for special statements like `break`, `return`, `switch` etc.

In this stage, syntax tree are transformed into abstract syntax tree. Types are checked and implicitly converted if necessary. All the implicit type conversion are done by explicitly insert a cast expression into the output abstract syntax tree.

For type conversions, check [Type Conversions (C)](https://msdn.microsoft.com/en-us/library/k630sk6z.aspx).

In most situation the syntax tree is immutable. The compiler simply maintains the environment and generates the abstract syntax tree for code generation. However, some statements requires special environment. 

For example, `switch` requires some extra information from enclosed `case` and `default` statement. It is hard to pass such information directly via the environment. To fix this, `case` and `default` statement can directly modify their enclosing `switch` statement to pass the necessary information.

### 7. Stage IV: Code Generation
* Generates x86 assembly code.

This is the final phase. X86 assembly code is generated and is able to be sent directly into `clang`'s assembler to generate Windows executable.

For how to convert between arithmetic types, check [Type Conversions (C)](https://msdn.microsoft.com/en-us/library/k630sk6z.aspx).

## Known Issues
- [ ] Support static variable.
- [x] Support simple initializer.
- [ ] Support designation initializer.
- [ ] Support variable length array.