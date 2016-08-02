#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

int trap(int* height, int heightSize);

int main(int argc, char* argv[]) {
    int h1[] = { 2, 0, 2 };
    int h2[] = { 0, 1, 0, 2, 1, 0, 1, 3, 2, 1, 2, 1 };
    assert(trap(h1, 3) == 2, "h1 == 2");
    assert(trap(h2, 12) == 6, "h2 == 6");
    printf("everything is fine!\n");
    return 0;
}