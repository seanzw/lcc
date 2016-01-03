
%L $[a-zA-Z_]$
%D $[0-9]$
%O $[0-7]$
%H $[0-9a-fA-F]$

%%

using lcc.Token;

%%
$/\*$           Comment();
$auto$          tokens.Add(new T_KEY_AUTO(line));
$break$         tokens.Add(new T_KEY_BREAK(line));
$case$          tokens.Add(new T_KEY_CASE(line));
$char$          tokens.Add(new T_KEY_CHAR(line));
$const$         tokens.Add(new T_KEY_CONST(line));
$continue$      tokens.Add(new T_KEY_CONTINUE(line));
$default$       tokens.Add(new T_KEY_DEFAULT(line));
$do$            tokens.Add(new T_KEY_DO(line));
$double$        tokens.Add(new T_KEY_DOUBLE(line));
$else$          tokens.Add(new T_KEY_ELSE(line));
$enum$          tokens.Add(new T_KEY_ENUM(line));
$extern$        tokens.Add(new T_KEY_EXTERN(line));
$float$         tokens.Add(new T_KEY_FLOAT(line));
$for$           tokens.Add(new T_KEY_FOR(line));
$goto$          tokens.Add(new T_KEY_GOTO(line));
$if$            tokens.Add(new T_KEY_IF(line));
$inline$        tokens.Add(new T_KEY_INLINE(line));
$int$           tokens.Add(new T_KEY_INT(line));
$long$          tokens.Add(new T_KEY_LONG(line));
$register$      tokens.Add(new T_KEY_REGISTER(line));
$restrict$      tokens.Add(new T_KEY_RESTRICT(line));
$return$        tokens.Add(new T_KEY_RETURN(line));
$short$         tokens.Add(new T_KEY_SHORT(line));
$signed$        tokens.Add(new T_KEY_SIGNED(line));
$sizeof$        tokens.Add(new T_KEY_SIZEOF(line));
$static$        tokens.Add(new T_KEY_STATIC(line));
$struct$        tokens.Add(new T_KEY_STRUCT(line));
$switch$        tokens.Add(new T_KEY_SWITCH(line));
$typedef$       tokens.Add(new T_KEY_TYPEDEF(line));
$union$         tokens.Add(new T_KEY_UNION(line));
$unsigned$      tokens.Add(new T_KEY_UNSIGNED(line));
$void$          tokens.Add(new T_KEY_VOID(line));
$volatile$      tokens.Add(new T_KEY_VOLATILE(line));
$while$         tokens.Add(new T_KEY_WHILE(line));
$_Bool$         tokens.Add(new T_KEY__BOOL(line));
$_Complex$      tokens.Add(new T_KEY__COMPLEX(line));
$_Imaginary$    tokens.Add(new T_KEY__IMAGINARY(line));

${L}({L}|{D})*$
    tokens.Add(new T_IDENTIFIER(line, text));

$0[xX]{H}*[uU]?(l|L|ll|LL)?$
    tokens.Add(new T_CONST_INT(line, text.Substring(2), 16));
$0[xX]{H}*(l|L|ll|LL)[uU]$
    tokens.Add(new T_CONST_INT(line, text.Substring(2), 16));
$0{O}*[uU]?(l|L|ll|LL)?$
    tokens.Add(new T_CONST_INT(line, text, 8));
$0{O}*(l|L|ll|LL)[uU]$
    tokens.Add(new T_CONST_INT(line, text, 8));
$[1-9]{D}*[uU]?(l|L|ll|LL)?$
    tokens.Add(new T_CONST_INT(line, text, 10));
$[1-9]{D}*(l|L|ll|LL)[uU]$
    tokens.Add(new T_CONST_INT(line, text, 10));

$({D}+)?\.[0-9]+([eE][\+\-]?{D}+)?[fFlL]?$
    tokens.Add(new T_CONST_FLOAT(line, text, 10));
$({D}+)\.([eE][\+\-]?{D}+)?[fFlL]?$
    tokens.Add(new T_CONST_FLOAT(line, text, 10));
$({D}+)[eE][\+\-]?{D}+[fFlL]?$
    tokens.Add(new T_CONST_FLOAT(line, text, 10));
$0[xX]({H}+)?\.{H}+[pP][\+\-]?[0-9]+[fFlL]?$
    tokens.Add(new T_CONST_FLOAT(line, text.Substring(2), 16));
$0[xX]{H}+\.?[pP][\+\-]?{D}+[fFlL]?$
    tokens.Add(new T_CONST_FLOAT(line, text.Substring(2), 16));

$L?'([^'\\\r\n]|\\(.|{O}{O}?{O}?|x{H}+))'$
    tokens.Add(new T_CONST_CHAR(line, text));

$L?"([^"\\\r\n]|\\(.|{O}{O}?{O}?|x{H}+|\r\n))*"$
    tokens.Add(new T_STRING_LITERAL(line, text));

$<:$        tokens.Add(new T_PUNC_SUBSCRIPTL(line));
$:>$        tokens.Add(new T_PUNC_SUBSCRIPTR(line));
$<%$        tokens.Add(new T_PUNC_BRACEL(line));
$%>$        tokens.Add(new T_PUNC_BRACER(line));
$\[$        tokens.Add(new T_PUNC_SUBSCRIPTL(line));
$\]$        tokens.Add(new T_PUNC_SUBSCRIPTR(line));
$\($        tokens.Add(new T_PUNC_PARENTL(line));
$\)$        tokens.Add(new T_PUNC_PARENTR(line));
$\{$        tokens.Add(new T_PUNC_BRACEL(line));
$\}$        tokens.Add(new T_PUNC_BRACER(line));
$\.$        tokens.Add(new T_PUNC_DOT(line));
$\->$       tokens.Add(new T_PUNC_PTRSEL(line));
$\+\+$      tokens.Add(new T_PUNC_INCRE(line));
$\-\-$      tokens.Add(new T_PUNC_DECRE(line));
$&$         tokens.Add(new T_PUNC_REF(line));
$\*$        tokens.Add(new T_PUNC_STAR(line));
$\+$        tokens.Add(new T_PUNC_PLUS(line));
$\-$        tokens.Add(new T_PUNC_MINUS(line));
$~$         tokens.Add(new T_PUNC_BITNOT(line));
$!$         tokens.Add(new T_PUNC_LOGNOT(line));
$/$         tokens.Add(new T_PUNC_SLASH(line));
$%$         tokens.Add(new T_PUNC_MOD(line));
$<<$        tokens.Add(new T_PUNC_SHIFTL(line));
$>>$        tokens.Add(new T_PUNC_SHIFTR(line));
$<$         tokens.Add(new T_PUNC_LT(line));
$>$         tokens.Add(new T_PUNC_GT(line));
$<=$        tokens.Add(new T_PUNC_LE(line));
$>=$        tokens.Add(new T_PUNC_GE(line));
$==$        tokens.Add(new T_PUNC_EQ(line));
$!=$        tokens.Add(new T_PUNC_NEQ(line));
$\^$        tokens.Add(new T_PUNC_BITXOR(line));
$\|$        tokens.Add(new T_PUNC_BITOR(line));
$&&$        tokens.Add(new T_PUNC_LOGAND(line));
$\|\|$      tokens.Add(new T_PUNC_LOGOR(line));
$\?$        tokens.Add(new T_PUNC_QUESTION(line));
$:$         tokens.Add(new T_PUNC_COLON(line));
$;$         tokens.Add(new T_PUNC_SEMICOLON(line));
$\.\.\.$    tokens.Add(new T_PUNC_ELLIPSIS(line));
$=$         tokens.Add(new T_PUNC_ASSIGN(line));
$\*=$       tokens.Add(new T_PUNC_MULEQ(line));
$/=$        tokens.Add(new T_PUNC_DIVEQ(line));
$%=$        tokens.Add(new T_PUNC_MODEQ(line));
$\+=$       tokens.Add(new T_PUNC_PLUSEQ(line));
$\-=$       tokens.Add(new T_PUNC_MINUSEQ(line));
$<<=$       tokens.Add(new T_PUNC_SHIFTLEQ(line));
$>>=$       tokens.Add(new T_PUNC_SHIFTREQ(line));
$&=$        tokens.Add(new T_PUNC_BITANDEQ(line));
$\^=$       tokens.Add(new T_PUNC_BITXOREQ(line));
$\|=$       tokens.Add(new T_PUNC_BITOREQ(line));
$,$         tokens.Add(new T_PUNC_COMMA(line));

$[ \n\r\t]+$
$//[^\n]*$
    
%%

void Comment() {
    bool prevStar = false;
    while (More()) {
        if (prevStar && Peek() == '/') {
            Next();
            return;
        }
        prevStar = Next() == '*';
    }
    Error("Unclosed block comment at line " + line);
}