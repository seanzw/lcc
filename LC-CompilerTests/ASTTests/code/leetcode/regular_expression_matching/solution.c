int isMatch(char* s, char* p) {
    if (*s == '\0' && *p == '\0') return 1;
    if (*s != '\0' && *p == '\0') return 0;

    if (*(p + 1) == '*') {
        return (*s != '\0' && (*s == *p || *p == '.') && isMatch(s + 1, p)) || isMatch(s, p + 2);
    }
    else {
        return (*s != '\0' && (*s == *p || *p == '.')) && isMatch(s + 1, p + 1);
    }
}