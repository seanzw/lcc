/// nums is non-increasing array, find the smallest number which is bigger than val,
/// returns its idx.
static int find_ceil(int* nums, int start, int end, int val) {
    int lhs, rhs, mid;
    lhs = start;
    rhs = end;
    /// At least length 3, which means mid > lhs.
    while (lhs < rhs - 1) {
        mid = (lhs + rhs) / 2;
        if (nums[mid] == val) {
            while (mid >= 0) {
                if (nums[mid] > val) return mid;
                --mid;
            }
            return -1;
        }
        else if (nums[mid] < val) rhs = mid - 1;
        else lhs = mid;
    }
    return nums[lhs] > val ? (nums[rhs] > val ? rhs : lhs) : -1;
}

void nextPermutation(int* nums, int numsSize) {
    int i;
    for (i = numsSize - 2; i >= 0; --i) {
        int tmp;
        tmp = nums[i];
        if (tmp < nums[i + 1]) {
            int j;
            j = find_ceil(nums, i + 1, numsSize - 1, tmp);
            /// j will never be -1;
            nums[i] = nums[j];
            nums[j] = tmp;
            /// Reverse nums[j...numsSize - 1].
            for (j = i + 1; j < i + (numsSize + 1 - i) / 2; ++j) {
                int k;
                k = numsSize - (j - i);
                tmp = nums[j];
                nums[j] = nums[k];
                nums[k] = tmp;
            }
            return;
        }
    }
    /// Reverse all.
    for (i = 0; i <= (numsSize - 1) / 2; ++i) {
        int k;
        k = numsSize - 1 - i;
        int tmp;
        tmp = nums[i];
        nums[i] = nums[k];
        nums[k] = tmp;
    }
}