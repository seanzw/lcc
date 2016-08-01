int search(int* nums, int numsSize, int target) {
    int lhs, rhs, mid;
    lhs = 0;
    rhs = numsSize - 1;
    
    while (lhs <= rhs) {
        mid = (lhs + rhs) / 2;
        if (nums[mid] == target) return 1;
        if (nums[mid] < nums[lhs]) {
            if (target > nums[mid] && target <= nums[rhs]) {
                lhs = mid + 1;
            }
            else {
                rhs = mid - 1;
            }
        }
        else if (nums[mid] > nums[lhs]) {
            if (target >= nums[lhs] && target < nums[mid]) {
                rhs = mid - 1;
            }
            else {
                lhs = mid + 1;
            }
        }
        else {
            lhs++;
        }
    }
    return 0;
}