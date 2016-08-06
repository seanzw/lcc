#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
}

int largestRectangleArea(int* heights, int heightsSize);

int main(int argc, char* argv[]) {
    int h[] = { 0, 9 };
    int h2[] = { 9, 0 };
    int h3[] = { 2, 1, 2 };
    assert(largestRectangleArea(h, 2) == 9, "0 9");
    assert(largestRectangleArea(h2, 2) == 9, "9 0");
    assert(largestRectangleArea(h3, 3) == 3, "2 1 2");
    printf("everything is fine!\n");
    return 0;
}