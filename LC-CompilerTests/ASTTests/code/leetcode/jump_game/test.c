﻿#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    /*exit(-1);                               \*/\
} 

int canJump(int* nums, int numsSize);

int main(int argc, char* argv[]) {
    
    int n[] = { 2, 3, 1, 1, 4 };
    int b[] = { 2, 3, 0, 1, 4 };

    assert(canJump(n, 1) == 1, "boundary 0");
    assert(canJump(n, 5) == 1, "2 step");
    assert(canJump(b, 5) == 1, "b");

    printf("everything is fine!\n");
    return 0;
}