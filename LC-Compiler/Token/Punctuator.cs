using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.Token {

    /// <summary>
    /// [
    /// </summary>
    sealed class T_PUNC_SUBSCRIPTL : Token {
        public T_PUNC_SUBSCRIPTL(int line) : base(line) { }
    }

    /// <summary>
    /// ]
    /// </summary>
    sealed class T_PUNC_SUBSCRIPTR : Token { // "]"
        public T_PUNC_SUBSCRIPTR(int line) : base(line) { }
    }

    /// <summary>
    /// (
    /// </summary>
    sealed class T_PUNC_PARENTL : Token { // "("
        public T_PUNC_PARENTL(int line) : base(line) { }
    }

    /// <summary>
    /// )
    /// </summary>
    sealed class T_PUNC_PARENTR : Token { // ")"
        public T_PUNC_PARENTR(int line) : base(line) { }
    }

    /// <summary>
    /// {
    /// </summary>
    sealed class T_PUNC_BRACEL : Token { // "{"
        public T_PUNC_BRACEL(int line) : base(line) { }
    }

    /// <summary>
    /// }
    /// </summary>
    sealed class T_PUNC_BRACER : Token { // "}"
        public T_PUNC_BRACER(int line) : base(line) { }
    }

    /// <summary>
    /// .
    /// </summary>
    sealed class T_PUNC_DOT : Token { // "."
        public T_PUNC_DOT(int line) : base(line) { }
    }

    /// <summary>
    /// ->
    /// </summary>
    sealed class T_PUNC_PTRSEL : Token { // "->"
        public T_PUNC_PTRSEL(int line) : base(line) { }
    }

    /// <summary>
    /// ++
    /// </summary>
    sealed class T_PUNC_INCRE : Token { // "++"
        public T_PUNC_INCRE(int line) : base(line) { }
    }

    /// <summary>
    /// --
    /// </summary>
    sealed class T_PUNC_DECRE : Token { // "--"
        public T_PUNC_DECRE(int line) : base(line) { }
    }

    /// <summary>
    /// &
    /// </summary>
    sealed class T_PUNC_REF : Token { // "&"
        public T_PUNC_REF(int line) : base(line) { }
    }

    /// <summary>
    /// *
    /// </summary>
    sealed class T_PUNC_STAR : Token { // "*"
        public T_PUNC_STAR(int line) : base(line) { }
    }

    /// <summary>
    /// +
    /// </summary>
    sealed class T_PUNC_PLUS : Token { // "+"
        public T_PUNC_PLUS(int line) : base(line) { }
    }

    /// <summary>
    /// -
    /// </summary>
    sealed class T_PUNC_MINUS : Token { // "-"
        public T_PUNC_MINUS(int line) : base(line) { }
    }

    /// <summary>
    /// ~
    /// </summary>
    sealed class T_PUNC_BITNOT : Token { // "~"
        public T_PUNC_BITNOT(int line) : base(line) { }
    }

    /// <summary>
    /// !
    /// </summary>
    sealed class T_PUNC_LOGNOT : Token { // "!"
        public T_PUNC_LOGNOT(int line) : base(line) { }
    }

    /// <summary>
    /// /
    /// </summary>
    sealed class T_PUNC_SLASH : Token { // "/"
        public T_PUNC_SLASH(int line) : base(line) { }
    }

    /// <summary>
    /// %
    /// </summary>
    sealed class T_PUNC_MOD : Token { // "%"
        public T_PUNC_MOD(int line) : base(line) { }
    }

    /// <summary>
    /// &lt&lt
    /// </summary>
    sealed class T_PUNC_SHIFTL : Token { // "<<"
        public T_PUNC_SHIFTL(int line) : base(line) { }
    }

    /// <summary>
    /// &gt&gt
    /// </summary>
    sealed class T_PUNC_SHIFTR : Token { // ">>"
        public T_PUNC_SHIFTR(int line) : base(line) { }
    }

    /// <summary>
    /// &lt
    /// </summary>
    sealed class T_PUNC_LT : Token { // "<"
        public T_PUNC_LT(int line) : base(line) { }
    }

    /// <summary>
    /// &gt
    /// </summary>
    sealed class T_PUNC_GT : Token { // ">"
        public T_PUNC_GT(int line) : base(line) { }
    }

    /// <summary>
    /// &lt=
    /// </summary>
    sealed class T_PUNC_LE : Token { // "<="
        public T_PUNC_LE(int line) : base(line) { }
    }

    /// <summary>
    /// &gt=
    /// </summary>
    sealed class T_PUNC_GE : Token { // ">="
        public T_PUNC_GE(int line) : base(line) { }
    }

    /// <summary>
    /// ==
    /// </summary>
    sealed class T_PUNC_EQ : Token { // "=="
        public T_PUNC_EQ(int line) : base(line) { }
    }

    /// <summary>
    /// !=
    /// </summary>
    sealed class T_PUNC_NEQ : Token { // "!="
        public T_PUNC_NEQ(int line) : base(line) { }
    }

    /// <summary>
    /// ^
    /// </summary>
    sealed class T_PUNC_BITXOR : Token { // "^"
        public T_PUNC_BITXOR(int line) : base(line) { }
    }

    /// <summary>
    /// |
    /// </summary>
    sealed class T_PUNC_BITOR : Token { // "|"
        public T_PUNC_BITOR(int line) : base(line) { }
    }

    /// <summary>
    /// &&
    /// </summary>
    sealed class T_PUNC_LOGAND : Token { // "&&"
        public T_PUNC_LOGAND(int line) : base(line) { }
    }

    /// <summary>
    /// ||
    /// </summary>
    sealed class T_PUNC_LOGOR : Token { // "||"
        public T_PUNC_LOGOR(int line) : base(line) { }
    }

    /// <summary>
    /// ?
    /// </summary>
    sealed class T_PUNC_QUESTION : Token { // "?"
        public T_PUNC_QUESTION(int line) : base(line) { }
    }

    /// <summary>
    /// :
    /// </summary>
    sealed class T_PUNC_COLON : Token { // ":"
        public T_PUNC_COLON(int line) : base(line) { }
    }

    /// <summary>
    /// ;
    /// </summary>
    sealed class T_PUNC_SEMICOLON : Token { // ";"
        public T_PUNC_SEMICOLON(int line) : base(line) { }
    }

    /// <summary>
    /// ...
    /// </summary>
    sealed class T_PUNC_ELLIPSIS : Token { // "..."
        public T_PUNC_ELLIPSIS(int line) : base(line) { }
    }

    /// <summary>
    /// =
    /// </summary>
    sealed class T_PUNC_ASSIGN : Token { // "="
        public T_PUNC_ASSIGN(int line) : base(line) { }
    }

    /// <summary>
    /// *=
    /// </summary>
    sealed class T_PUNC_MULEQ : Token { // "*="
        public T_PUNC_MULEQ(int line) : base(line) { }
    }

    /// <summary>
    /// /=
    /// </summary>
    sealed class T_PUNC_DIVEQ : Token { // "/="
        public T_PUNC_DIVEQ(int line) : base(line) { }
    }

    /// <summary>
    /// %=
    /// </summary>
    sealed class T_PUNC_MODEQ : Token { // "%="
        public T_PUNC_MODEQ(int line) : base(line) { }
    }

    /// <summary>
    /// +=
    /// </summary>
    sealed class T_PUNC_PLUSEQ : Token { // "+="
        public T_PUNC_PLUSEQ(int line) : base(line) { }
    }

    /// <summary>
    /// -=
    /// </summary>
    sealed class T_PUNC_MINUSEQ : Token { // "-="
        public T_PUNC_MINUSEQ(int line) : base(line) { }
    }

    /// <summary>
    /// &lt&lt=
    /// </summary>
    sealed class T_PUNC_SHIFTLEQ : Token { // "<<="
        public T_PUNC_SHIFTLEQ(int line) : base(line) { }
    }

    /// <summary>
    /// &gt&gt=
    /// </summary>
    sealed class T_PUNC_SHIFTREQ : Token { // ">>="
        public T_PUNC_SHIFTREQ(int line) : base(line) { }
    }

    /// <summary>
    /// &=
    /// </summary>
    sealed class T_PUNC_BITANDEQ : Token { // "&="
        public T_PUNC_BITANDEQ(int line) : base(line) { }
    }

    /// <summary>
    /// ^=
    /// </summary>
    sealed class T_PUNC_BITXOREQ : Token { // "^="
        public T_PUNC_BITXOREQ(int line) : base(line) { }
    }

    /// <summary>
    /// |=
    /// </summary>
    sealed class T_PUNC_BITOREQ : Token { // "|="
        public T_PUNC_BITOREQ(int line) : base(line) { }
    }

    /// <summary>
    /// ,
    /// </summary>
    sealed class T_PUNC_COMMA : Token { // ","
        public T_PUNC_COMMA(int line) : base(line) { }
    }
}
