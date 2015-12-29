
using Lexer;

%%

$\$(\\[^\n\r\t]|[^\n\r\t\$\\])*\$$  
    tokens.Add(new T_REGEX(text.Substring(1, text.Length - 2)));

$[^\$ \n\r\t%][^\r\n]*$
    tokens.Add(new T_CODE(text));

$[ \n\r\t]+$
    
$%%$
    tokens.Add(new T_SPLITER());
%%
