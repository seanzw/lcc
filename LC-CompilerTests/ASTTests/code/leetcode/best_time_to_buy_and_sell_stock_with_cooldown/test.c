#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

int maxProfit(int*, int);

int main(int argc, char* argv[]) {
    int prices[] = { 1, 2, 3, 0, 2 };
    assert(maxProfit(prices, 0) == 0, "0 -> 0");
    assert(maxProfit(prices, 1) == 0, "1 -> 0");
    assert(maxProfit(prices, 5) == 3, "5 -> 3");
    printf("everything is fine!\n");
    return 0;
}