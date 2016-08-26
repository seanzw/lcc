typedef unsigned int size_t;
void* malloc(size_t);
void free(void*);
void* memcpy(void*, const void*, size_t);
size_t strlen(const char*);
int printf(const char*, ...);

int combination(int n, int m) {
    int* ret = malloc(sizeof(int) * n);
    for (int i = 0; i < n; ++i) {
        ret[i] = 1;
    }
    for (int i = 1; i < m; ++i) {
        for (int j = 1; j < n; ++j) {
            ret[j] += ret[j - 1];
        }
    }
    int num = ret[n - 1];
    free(ret);
    return num;
}

int uniquePaths(int m, int n) {
    if (m > n) return combination(m, n);
    else return combination(n, m);
}