int printf(const char* format, ...);

int singleNumber(int* nums, int numsSize) {
    char x[32];
    int i, j, ret;
    for (i = 0; i < 32; ++i) {
        x[i] = 0;
    }
    for (i = 0; i < numsSize; ++i) {
        for (j = 0; j < 32; ++j) {
            x[j] = (x[j] + ((nums[i] >> j) & 1)) % 3;
        }
    }
    ret = 0;
    for (i = 0; i < 32; ++i) {
        ret |= (x[i] & 1) << i;
    }
    return ret;
}