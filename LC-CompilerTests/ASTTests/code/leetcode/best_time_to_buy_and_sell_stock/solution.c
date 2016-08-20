int maxProfit(int* prices, int pricesSize) {
    int ret = 0, min = 0x7FFFFFFF;
    for (int i = 0; i < pricesSize; ++i) {
        if (prices[i] < min) {
            min = prices[i];
        }
        else if (prices[i] - min > ret) {
            ret = prices[i] - min;
        }
    }
    return ret;
}