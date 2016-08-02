void* malloc(unsigned int size);
void free(void* ptr);

int candy(int* ratings, int ratingsSize) {
    if (ratingsSize < 1) return 0;
    int* mem;
    mem = malloc(ratingsSize * sizeof(int));
    int i;
    for (i = 0; i < ratingsSize; ++i) {
        mem[i] = 0;
    }
    int inc;
    inc = 0;
    for (i = 1; i < ratingsSize; ++i) {
        if (ratings[i] > ratings[i - 1]) {
            ++inc;
            if (mem[i] < inc) mem[i] = inc;
        }
        else {
            inc = 0;
        }
    }
    inc = 0;
    for (i = ratingsSize - 2; i >= 0; --i) {
        if (ratings[i] > ratings[i + 1]) {
            ++inc;
            if (mem[i] < inc) mem[i] = inc;
        }
        else {
            inc = 0;
        }
    }
    int total;
    total = ratingsSize;
    for (i = 0; i < ratingsSize; ++i) {
        total += mem[i];
    }
    free(mem);
    return total;
}