#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
} 

void nextPermutation(int* nums, int numsSize);

int main(int argc, char* argv[]) {
    int n1[] = { 1, 2, 3 };
    int p1[] = { 1, 3, 2 };
    int n2[] = { 3, 2, 1 };
    int p2[] = { 1, 2, 3 };
    int n3[] = { 1, 5, 5 };
    int p3[] = { 5, 1, 5 };
    nextPermutation(n1, 3);
    for (int i = 0; i < 3; ++i) {
        assert(p1[i] == n1[i], "n1");
    }
    nextPermutation(n2, 3);
    for (int i = 0; i < 3; ++i) {
        assert(p2[i] == n2[i], "n2");
    }
    nextPermutation(n3, 3);
    for (int i = 0; i < 3; ++i) {
        assert(p3[i] == n3[i], "n3");
    }
    printf("everything is fine!\n");
    return 0;
}