int maxProfit(int* prices, int pricesSize) {
    int ret = 0;
    for (int i = 0; i < pricesSize - 1; ++i) {
        if (prices[i] < prices[i + 1]) {
            ret += prices[i + 1] - prices[i];
        }
    }
    return ret;
}