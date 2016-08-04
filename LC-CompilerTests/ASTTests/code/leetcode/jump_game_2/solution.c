int printf(const char* format, ...);

int jump(int* nums, int numsSize) {
    int i, j;
    int step;
    i = j = 0;
    step = 0;
    while (i <= j) {
        if (j >= numsSize - 1) return step;
        int k;
        int jj;
        jj = j;
        for (k = i; k <= j; ++k) {
            if (k + nums[k] > jj) jj = k + nums[k];
        }
        
        i = j + 1;
        j = jj;
        step++;
    }
    return -1;   // unreachable.
}