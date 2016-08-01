#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
}

int close(double x, double y) {
    if (x > y) return x - y < 0.000001;
    else return y - x < 0.000001;
}

int longestConsecutive(int* nums, int numsSize);

int main(int argc, char* argv[]) {
    int n[] = { 100, 4, 200, 1, 3, 2 };
    assert(longestConsecutive(n, 0) == 0, "boundary test0");
    assert(longestConsecutive(n, 1) == 1, "boundary test1");
    assert(longestConsecutive(n, 6) == 4, "test");
    printf("everything is fine!\n");
    return 0;
}