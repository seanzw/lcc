void* malloc(unsigned int size);
void free(void* ptr);

/**
* Return an array of size *returnSize.
* Note: The returned array must be malloced, assume caller calls free().
*/
int* plusOne(int* digits, int digitsSize, int* returnSize) {
    int* ret;
    ret = malloc(sizeof(int) * digitsSize);
    int i, carry;
    carry = 1;
    for (i = digitsSize - 1; i >= 0; --i) {
        int x;
        if (carry == 0) {
            int j;
            for (j = 0; j <= i; ++j) ret[j] = digits[j];
            break;
        }
        else {
            x = digits[i] + carry;
            ret[i] = x % 10;
            carry = x / 10;
        }
    }
    if (carry != 0) {
        int* expand;
        expand = malloc(sizeof(int) * (digitsSize + 1));
        *returnSize = digitsSize + 1;
        for (i = 0; i < digitsSize; ++i) {
            expand[i + 1] = ret[i];
        }
        expand[0] = 1;
        free(ret);
        return expand;
    }
    *returnSize = digitsSize;
    return ret;
}