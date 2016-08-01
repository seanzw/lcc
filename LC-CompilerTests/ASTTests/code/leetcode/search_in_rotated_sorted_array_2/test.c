#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
} 

int search(int* nums, int numsSize, int target);

int main(int argc, char* argv[]) {
    int nums[] = { 4, 5, 4, 4, 4 };
    for (int i = 0; i < 5; ++i) {
        assert(search(nums, 5, nums[i]) == 1, "search");
    }
    printf("everything is fine!\n");
    return 0;
}