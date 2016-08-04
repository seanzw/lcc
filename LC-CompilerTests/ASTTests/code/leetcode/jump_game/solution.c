int printf(const char* format, ...);

int canJump(int* nums, int numsSize) {
    int i, j;
    i = j = 0;
    while (i <= j) {
        if (j >= numsSize - 1) return 1;
        int k;
        int jj;
        jj = j;
        for (k = i; k <= j; ++k) {
            if (k + nums[k] > jj) jj = k + nums[k];
        }
        
        i = j + 1;
        j = jj;
    }
    return 0;   // unreachable.
}