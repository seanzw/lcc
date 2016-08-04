int printf(const char* format, ...);

int firstMissingPositive(int* nums, int numsSize) {

    int i;
    for (i = 0; i < numsSize; ++i) {
        int x;
        x = nums[i];
        if (x <= 0 || x > numsSize) nums[i] = -1;
        else if (x < i + 1) {
            nums[x - 1] = x;
            nums[i] = -1;
        }
        else if (x == i + 1);
        else if (x == nums[x - 1]) nums[i] = -1;
        else {
            nums[i] = nums[x - 1];
            nums[x - 1] = x;
            --i;
        } 
    }

    for (i = 0; i < numsSize; ++i) {
        if (nums[i] == -1) return i + 1;
    }
    return numsSize + 1;
}