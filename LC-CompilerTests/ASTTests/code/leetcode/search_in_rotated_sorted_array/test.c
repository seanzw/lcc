#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
} 

int search(int* nums, int numsSize, int target);

int main(int argc, char* argv[]) {
    int nums[] = { 4, 5, 6, 0, 1, 2 };
    for (int i = 0; i < 6; ++i) {
        assert(search(nums, 6, nums[i]) == i, "search");
    }
    printf("everything is fine!\n");
    return 0;
}