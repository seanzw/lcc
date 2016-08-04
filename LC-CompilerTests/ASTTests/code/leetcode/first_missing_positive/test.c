#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

int firstMissingPositive(int* nums, int numsSize);

int main(int argc, char* argv[]) {
    
    int n1[] = { -1, -3 };
    int n2[] = { 1, 2, 3 };
    int n3[] = { 0, 2, 1, 3 };

    for (int i = 0; i < 3; ++i) {
        printf("n2[%d] = %d\n", i, n2[i]);
    }

    assert(firstMissingPositive(n1, 2) == 1, "all negative");

    for (int i = 0; i < 3; ++i) {
        printf("n2[%d] = %d\n", i, n2[i]);
    }

    assert(firstMissingPositive(n2, 3) == 4, "1 2 3");
    assert(firstMissingPositive(n1, 0) == 1, "zero element");
    assert(firstMissingPositive(n3, 4) == 4, "out of order");

    printf("everything is fine!\n");
    return 0;
}