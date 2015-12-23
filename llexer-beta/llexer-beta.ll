\{ {
    // Code block.
    llCur = code(llPre);
    if (llCur != -1) {
        llTokens.Add(new Token(TOKEN.CODE, llSrc.Substring(llPre, llCur - llPre)));
        return true;
    } else {
        llError("Unknown token at " + ", maybe CODE?");
        return false;
    }
}
\[ {
    llTokens.Add(new Token(TOKEN.LBRACKET, llSrc.Substring(llPre, llCur - llPre)));
    return true;
}
\] {
    llTokens.Add(new Token(TOKEN.RBRACKET, llSrc.Substring(llPre, llCur - llPre)));
    return true;
}
\* {
    llTokens.Add(new Token(TOKEN.STAR, llSrc.Substring(llPre, llCur - llPre)));
    return true;
}
\+ {
    llTokens.Add(new Token(TOKEN.PLUS, llSrc.Substring(llPre, llCur - llPre)));
    return true;
}
\- {
    llTokens.Add(new Token(TOKEN.AND, llSrc.Substring(llPre, llCur - llPre)));
    return true;
}
\| {
    llTokens.Add(new Token(TOKEN.OR, llSrc.Substring(llPre, llCur - llPre)));
    return true;
}
\\\ {
    llCur = atom(llCur);
    if (llCur != -1) {
        llTokens.Add(new Token(TOKEN.ATOM, llSrc.Substring(llPre, llCur - llPre)));
        return true;
    } else {
        llError("Unknown token, maybe ATOM?");
        return false;
    }
}
[\ |\\n|\\r|\\t]+ {
    // Ignore space.
    return true;
}
\/-\/-\\WLD* {
    // Ignore line comment.
    return true;
}
{
    Func<int, int> code = (i) => {
        while (i < llSrc.Length) {
            if (llSrc[i] == '}' && llSrc[i - 1] == '\n') {
                return i + 1;
            }
            i++;
        }
        return -1;
    };

    Func<int, int> atom = (i) => {
        if (i < llSrc.Length) {
            if (llSrc[i++] == '\\') {
                if (i + 3 < llSrc.Length) {
                    string sub = llSrc.Substring(i, 3);
                    if (sub == "WLD" ||
                        sub == "a-z" ||
                        sub == "a-Z" ||
                        sub == "0-9"
                        ) {
                        return i + 3;
                    }
                }
                if (i + 1 < llSrc.Length) {
                    return i + 1;
                } else {
                    return -1;
                }
            } else {
                return i;
            }
        } else {
            return -1;
        }
    };
}