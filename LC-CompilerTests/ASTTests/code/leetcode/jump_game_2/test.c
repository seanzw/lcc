#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    /*exit(-1);                               \*/\
} 

int jump(int* nums, int numsSize);

int main(int argc, char* argv[]) {
    
    int n[] = { 2, 3, 1, 1, 4 };
    int b[] = { 2, 3, 0, 1, 4 };

    assert(jump(n, 1) == 0, "boundary 0");
    assert(jump(n, 5) == 2, "2 step");
    assert(jump(b, 5) == 2, "b");

    for (int i = 0; i < 5; ++i) {
        printf("b[%d] = %d\n", i, b[i]);
    }

    printf("everything is fine!\n");
    return 0;
}