#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
} 

int removeElement(int* nums, int numsSize, int val);

int main(int argc, char* argv[]) {
    int nums[] = { 1, 1, 1, 2, 2, 4 };
    assert(removeElement(nums, 0, 1) == 0, "boundary 0");
    assert(removeElement(nums, 1, 1) == 0, "boundary 1");
    assert(removeElement(nums, 1, 2) == 1, "boundary 1 for not exist");
    assert(removeElement(nums, 6, 1) == 3, "test for length 6");
    printf("everything is fine!\n");
    return 0;
}