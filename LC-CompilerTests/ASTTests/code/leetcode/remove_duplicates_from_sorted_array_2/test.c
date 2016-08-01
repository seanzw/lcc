#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
} 

int removeDuplicates(int* nums, int numsSize);

int main(int argc, char* argv[]) {
    int nums[] = { 1, 1, 1, 2, 2, 4 };
    assert(removeDuplicates(nums, 0) == 0, "boundary 0");
    assert(removeDuplicates(nums, 1) == 1, "boundary 1");
    assert(removeDuplicates(nums, 2) == 2, "test for length 2");
    assert(nums[0] == 1, "nums[0]");
    assert(nums[1] == 1, "nums[1] == 1");
    assert(removeDuplicates(nums, 3) == 2, "test for length 3");
    assert(nums[0] == 1, "nums[0] 2");
    assert(nums[1] == 1, "nums[1] == 1");
    assert(removeDuplicates(nums, 6) == 5, "test for length 5");
    assert(nums[2] == 2, "nums[2] == 2");
    printf("everything is fine!\n");
    return 0;
}