int removeElement(int* nums, int numsSize, int val) {
    int i, j;
    j = 0;
    for (i = 0; i < numsSize; ++i) {
        if (nums[i] != val) {
            nums[j++] = nums[i];
        }
    }
    return j;
}