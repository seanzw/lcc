#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

char* getPermutation(int n, int k);

void test(int n, int k, char* ret) {
    char* result;
    result = getPermutation(n, k);
    printf("n = %d, k = %d, result = %s\n", n, k, result);
    for (int i = 0; i < n; ++i) {
        assert(result[i] == ret[i], "test");
    }
}

int main(int argc, char* argv[]) {
    test(3, 1, "123");
    test(3, 2, "132");
    test(3, 3, "213");
    test(3, 4, "231");
    test(3, 5, "312");
    test(3, 6, "321");
    printf("everything is fine!\n");
    return 0;
}