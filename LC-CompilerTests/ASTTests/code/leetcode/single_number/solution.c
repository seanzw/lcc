int singleNumber(int* nums, int numsSize) {
    int x, i;
    x = nums[0];
    for (i = 1; i < numsSize; ++i) {
        x ^= nums[i];
    }
    return x;
}