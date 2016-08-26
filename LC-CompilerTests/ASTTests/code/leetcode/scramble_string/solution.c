typedef unsigned int size_t;
void* malloc(size_t);
void free(void*);
void* memcpy(void*, const void*, size_t);
size_t strlen(const char*);
int printf(const char*, ...);

int isScramble(char* s1, char* s2) {
    int l1 = strlen(s1);
    int l2 = strlen(s2);
    if (l1 != l2) return 0;
    if (l1 == 0) return 0;
    int*** mem = malloc(sizeof(int**) * l1);
    for (int i = 0; i < l1; ++i) {
        mem[i] = malloc(sizeof(int*) * l1);
        for (int j = 0; j < l1; ++j) {
            mem[i][j] = malloc(sizeof(int) * l1);
        }
    }

    for (int i = 0; i < l1; ++i) {
        for (int j = 0; j < l1; ++j) {
            mem[0][i][j] = s1[i] == s2[j];
        }
    }

    for (int n = 1; n < l1; ++n) {
        for (int i = 0; i < l1 - n; ++i) {
            for (int j = 0; j < l1 - n; ++j) {
                mem[n][i][j] = 0;
                for (int k = 0; k < n; ++k) {
                    int m = n - k - 1;
                    if ((mem[k][i][j] && mem[m][i + k + 1][j + k + 1]) ||
                        (mem[k][i][j + m + 1] && mem[m][i + k + 1][j])) {
                        mem[n][i][j] = 1;
                        break;
                    }
                }
            }
        }
    }

    int ret = mem[l1 - 1][0][0];
    for (int i = 0; i < l1; ++i) {
        for (int j = 0; j < l1; ++j) {
            free(mem[i][j]);
        }
        free(mem[i]);
    }
    free(mem);
    return ret;
}