
using llexer_beta;

%%

$\$(\\[^\n\r\t]|[^\n\r\t\$\\])*\$$  
    lltokens.Add(new T_REGEX(lltext.Substring(1, lltext.Length - 2)));

$[^\$ \n\r\t%][^\r\n]*$
    lltokens.Add(new T_CODE(lltext));

$[ \n\r\t]+$
    
$%%$
    lltokens.Add(new T_SPLITER());
%%
