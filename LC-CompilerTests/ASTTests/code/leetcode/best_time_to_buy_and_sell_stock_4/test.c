#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

int maxProfit(int, int*, int);

int main(int argc, char* argv[]) {
    int prices[] = { 2, 3, 1, 5 };
    assert(maxProfit(1, prices, 0) == 0, "0 -> 0");
    assert(maxProfit(1, prices, 1) == 0, "1 -> 0");
    assert(maxProfit(1, prices, 4) == 4, "1 4 -> 5");
    assert(maxProfit(2, prices, 4) == 5, "2 4 -> 5");
    assert(maxProfit(3, prices, 4) == 5, "3 4 -> 5");
    assert(maxProfit(4, prices, 4) == 5, "4 4 -> 5");
    printf("everything is fine!\n");
    return 0;
}