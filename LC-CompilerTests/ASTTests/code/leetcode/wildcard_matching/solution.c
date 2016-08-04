int printf(const char* format, ...);

int isMatch(char* s, char* p) {
    /// The key is that when we encounter two '*'s， only the second one matters.
    /// If the second one fails, then the first one will always fail.
    int star, str;
    star = -1;  // The position of previous star.
    str = -1;   // The position of s when backtracking star.

    int ss, pp;
    ss = 0;
    pp = 0;
    while (1) {
        if (p[pp] == '\0' && s[ss] == '\0') return 1;

        if (p[pp] == '*') {
            /// There is a star.
            star = pp;
            str = ss;
            pp++;
        }
        else if ((s[ss] != '\0') && (p[pp] == '?' || s[ss] == p[pp])) {
            /// Successfully matched.
            pp++;
            ss++;
        }
        else {
            /// Failed matching.
            if (star != -1 && s[str] != '\0') {
                /// There is a star before.
                /// Backtracking the star and restart the matching.
                str++;
                ss = str;
                pp = star + 1;
            }
            else {
                /// No star before.
                /// Failed.
                return 0;
            }
        }
    }
}