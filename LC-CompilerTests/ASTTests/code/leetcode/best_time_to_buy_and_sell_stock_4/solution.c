void* malloc(unsigned int);
void free(void*);
void* memcpy(void*, const void*, unsigned int);


int maxProfitInf(int* prices, int pricesSize) {
    int ret = 0;
    for (int i = 0; i < pricesSize - 1; ++i) {
        if (prices[i] < prices[i + 1]) {
            ret += prices[i + 1] - prices[i];
        }
    }
    return ret;
}

int maxProfit(int k, int* prices, int pricesSize) {
    if (pricesSize == 0 || k == 0) return 0;

    if (k > pricesSize / 2) return maxProfitInf(prices, pricesSize);

    int* kth = malloc(sizeof(int) * pricesSize);
    /// generate result for k == 1.
    {
        int profit = 0;
        int min = 0x7FFFFFFF;
        for (int i = 0; i < pricesSize; ++i) {
            if (prices[i] < min) min = prices[i];
            else if (prices[i] - min > profit) profit = prices[i] - min;
            kth[i] = profit;
        }
    }
    while (k > 1) {
        for (int j = pricesSize - 1; j > 0; --j) {
            int tmp = prices[j];
            int max = kth[j];
            int profit = 0;
            for (int split = j - 1; split > 0; --split) {

                if (prices[split + 1] > tmp) tmp = prices[split + 1];
                else if (tmp - prices[split + 1] > profit) profit = tmp - prices[split + 1];

                if (kth[split] + profit > max) {
                    max = kth[split] + profit;
                }
            }
            kth[j] = max;
        }
        k--;
    }

    int ret = kth[pricesSize - 1];
    free(kth);
    return ret;
}
