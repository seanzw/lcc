#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

int singleNumber(int* nums, int numsSize);

int main(int argc, char* argv[]) {
    int h1[] = { 9 };
    int h2[] = { 8, 7, 8 };
    int x;
    assert(singleNumber(h1, 1) == 9, "candy 1");
    assert(singleNumber(h2, 3) == 7, "candy 3");
    printf("everything is fine!\n");
    return 0;
}