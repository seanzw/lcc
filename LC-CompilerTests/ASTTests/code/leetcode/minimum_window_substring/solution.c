void* malloc(unsigned int);
void free(void*);
void* memcpy(void*, const void*, unsigned int);
unsigned int strlen(const char*);

int getIdx(char s) {
    return s;
}

char* minWindow(char* s, char* t) {
    int wc[128], cnt[128], total;
    int i;
    for (i = 0; i < 128; ++i) {
        wc[i] = 0;
        cnt[i] = 0;
    }

    total = 0;
    char* iter;
    for (iter = t; *iter != '\0'; ++iter) {
        wc[getIdx(*iter)]++;
        total++;
    }

    /// Assume total != 0.

    char* ret;
    int len;
    ret = 0;
    len = 0x7FFFFFFF;

    char* lhs;
    char* rhs;
    for (lhs = s, rhs = s; *rhs != '\0'; ++rhs) {
        int idx;
        idx = getIdx(*rhs);
        if (wc[idx] > 0) {
            /// This is a valid character.
            cnt[idx]++;
            total--;
            if (wc[idx] < cnt[idx]) {
                /// Overflow.
                /// Advance until found *rhs.
                do {
                    int x;
                    x = getIdx(*lhs);
                    if (wc[x] > 0) {
                        cnt[x]--;
                        total++;
                    }
                    lhs++;
                } while (*(lhs - 1) != *rhs);
            }
            else if (total == 0) {
                /// Found a window.
                if ((rhs - lhs) + 1 < len) {
                    len = (rhs - lhs) + 1;
                    ret = lhs;
                }
                /// Advance lhs.
                cnt[getIdx(*lhs)]--;
                total++;
                lhs++;
                /// Ignore all the invalid character.
                while (wc[getIdx(*lhs)] == 0) lhs++;
            }
        }
    }

    if (ret != 0) {
        char* window;
        window = malloc(sizeof(char) * len + 1);
        memcpy(window, ret, len);
        window[len] = '\0';

        return window;
    }
    else {
        return "";
    }
}