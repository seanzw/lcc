typedef unsigned int size_t;
void* malloc(size_t);
void free(void*);
void* memcpy(void*, const void*, size_t);
size_t strlen(const char*);


int minCut(char* s) {

    if (!s || *s == '\0') return 0;

    int n = strlen(s);

    // p[i][j] represents wheter s[i, j] is palindrome.
    int** p = malloc(sizeof(int*) * n);
    for (int i = 0; i < n; ++i) {
        p[i] = malloc(sizeof(int) * n);
    }

    p[0][0] = 0;
    for (int i = 1; i < n; ++i) {
        for (int j = 0; j <= i; ++j) {
            p[j][i] = s[j] == s[i] && (j >= i - 2 || p[j + 1][i - 1]);
        }
    }

    int* mem = malloc(sizeof(int) * n);
    mem[0] = 0;
    for (int i = 1; i < n; ++i) {
        if (p[0][i]) mem[i] = 0;
        else {
            int min = mem[i - 1];
            for (int j = i - 1; j > 0; --j) {
                if (p[j][i] && mem[j - 1] < min) {
                    min = mem[j - 1];
                }
            }
            mem[i] = min + 1;
        }
    }

    for (int i = 0; i < n; ++i) {
        free(p[i]);
    }
    free(p);

    int ret = mem[n - 1];
    free(mem);
    return ret;
}