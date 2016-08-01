int removeDuplicates(int* nums, int numsSize) {
    if (numsSize < 2) return numsSize;
    int i, j, cnt;
    i = 1;
    j = 0;
    cnt = 1;
    while (i < numsSize) {
        if (nums[i] != nums[j]) {
            nums[++j] = nums[i++];
            cnt = 1;
        }
        else if (cnt < 2) {
            nums[++j] = nums[i++];
            cnt++;
        }
        else {
            i++;
        }
    }
    return j + 1;
}