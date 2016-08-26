int max(int a, int b) {
    return a > b ? a : b;
}

int maxProfit(int* prices, int pricesSize) {
    if (pricesSize < 2) return 0;

    if (pricesSize == 2) return max(0, prices[1] - prices[0]);

    int sold = 0;
    int hold = max(prices[0], prices[1]) - prices[0] - prices[1];
    int cool = prices[1] - prices[0];
    for (int i = 2; i < pricesSize; ++i) {
        int old_sold = sold;
        sold = max(sold, cool);
        cool = hold + prices[i];
        hold = max(old_sold - prices[i], hold);
    }

    return max(sold, cool);
}