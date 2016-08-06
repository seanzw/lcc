#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
}

int climbStairs(int n);

int main(int argc, char* argv[]) {
    assert(climbStairs(1) == 1, "1");
    assert(climbStairs(2) == 2, "2");
    printf("everything is fine!\n");
    return 0;
}