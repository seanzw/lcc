#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
} 

int matrix_chain(const int* p, int n);

int main(int argc, char* argv[]) {
    int p1[] = { 10, 100, 5, 50 };
    assert(matrix_chain(p1, 1) == 0, "boundary test 1");
    assert(matrix_chain(p1, 2) == 5000, "boundary test 2");
    assert(matrix_chain(p1, 3) == 7500, "should be ((A)x(B))x(C)");

    return 0;
}