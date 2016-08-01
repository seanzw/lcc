int search(int* nums, int numsSize, int target) {
    int lhs, rhs, mid;
    lhs = 0;
    rhs = numsSize - 1;
    
    while (lhs <= rhs) {
        mid = (lhs + rhs) / 2;
        if (nums[mid] == target) return mid;
        if (nums[mid] < nums[lhs]) {
            if (target > nums[mid] && target <= nums[rhs]) {
                lhs = mid + 1;
            }
            else {
                rhs = mid - 1;
            }
        }
        else {
            if (target >= nums[lhs] && target < nums[mid]) {
                rhs = mid - 1;
            }
            else {
                lhs = mid + 1;
            }
        }
    }
    return -1;
}