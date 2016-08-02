static int fact(int n) {
    int r;
    r = 1;
    while (n > 1) {
        r *= n;
        n--;
    }
    return r;
}

void* malloc(unsigned int size);
void free(void* ptr);

char* getPermutation(int n, int k) {
    int* used;
    used = malloc(sizeof(int) * n);
    int i;
    for (i = 0; i < n; ++i) {
        used[i] = 0;
    }
    char* ret;
    ret = malloc(sizeof(char) * (n + 1));
    for (i = 0; i < n; ++i) {
        int j, num;
        num = fact(n - 1 - i);
        for (j = 0; j < n; ++j) {
            if (used[j]) continue;
            if (k <= num) {
                ret[i] = '1' + j;
                used[j] = 1;
                break;
            }
            else {
                k -= num;
            }
        }
    }
    ret[n] = 0;
    free(used);
    return ret;
}