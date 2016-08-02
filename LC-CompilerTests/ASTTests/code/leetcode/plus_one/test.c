#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

int* plusOne(int* digits, int digitsSize, int* returnSize);

int main(int argc, char* argv[]) {
    int h1[] = { 9 };
    int h2[] = { 8 };
    int x;
    int* r1 = plusOne(h1, 1, &x);
    assert(x == 2, "carry");
    assert(r1[0] == 1, "r1[0]");
    assert(r1[1] == 0, "r1[1]");
    printf("everything is fine!\n");
    return 0;
}