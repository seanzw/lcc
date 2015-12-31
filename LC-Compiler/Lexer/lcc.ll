
using lcc.Token;

%%

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

$[_a-zA-Z][_a-zA-Z0-9]*$
    tokens.Add(new T_IDENTIFIER(line, text));
$[ \n\r\t]+$
    
%%
