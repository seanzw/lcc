void* malloc(unsigned int);
void free(void*);
unsigned int strlen(const char*);

int minDistance(char* word1, char* word2) {
    int n1, n2;
    n1 = strlen(word1) + 1;
    n2 = strlen(word2) + 1;

    int** mem;
    mem = malloc(sizeof(int*) * n2);
    int i, j;
    for (i = 0; i < n2; ++i) {
        mem[i] = malloc(sizeof(int) * n1);
        mem[i][0] = i;
    }

    /// Initialize.
    for (i = 0; i < n1; ++i) {
        mem[0][i] = i;
    }

    char* s1;
    char* s2;
    for (i = 1, s2 = word2; i < n2; ++i, ++s2) {
        for (j = 1, s1 = word1; j < n1; ++j, ++s1) {
            int ins, del, rev;
            ins = mem[i][j - 1] + 1;
            del = mem[i - 1][j] + 1;
            rev = mem[i - 1][j - 1] + (*s1 != *s2);
            mem[i][j] = ins > del ? (del > rev ? rev : del) : (ins > rev ? rev : ins);
        }
    }

    int ret;
    ret = mem[n2 - 1][n1 - 1];
    for (i = 0; i < n2; ++i) {
        free(mem[i]);
    }
    free(mem);
    return ret;
}