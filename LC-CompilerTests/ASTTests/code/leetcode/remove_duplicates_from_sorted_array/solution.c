int removeDuplicates(int* nums, int numsSize) {
    if (numsSize < 2) return numsSize;
    int i, j;
    i = 1;
    j = 0;
    while (i < numsSize) {
        if (nums[i] != nums[j]) {
            nums[++j] = nums[i++];
        }
        else {
            i++;
        }
    }
    return j + 1;
}