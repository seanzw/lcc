void* malloc(unsigned int);
void free(void*);

int maxProfit(int* prices, int pricesSize) {
    int* tmp = malloc(sizeof(int) * pricesSize);
    int ret = 0;
    int min = 0x7FFFFFFF;
    int max = 0;
    for (int i = 0; i < pricesSize; ++i) {
        if (prices[i] < min) min = prices[i];
        else if (prices[i] - min > ret) ret = prices[i] - min;
        tmp[i] = ret;
    }
    for (int i = pricesSize - 1; i > 0; --i) {
        if (prices[i] > max) max = prices[i];
        else if (max - prices[i] + tmp[i - 1] > ret) ret = max - prices[i] + tmp[i - 1];
    }
    free(tmp);
    return ret;
}