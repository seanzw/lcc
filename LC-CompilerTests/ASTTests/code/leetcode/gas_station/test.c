#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
}

int canCompleteCircuit(int* gas, int gasSize, int* cost, int costSize);

int main(int argc, char* argv[]) {
    int gas[] = { 1,2,3 };
    int cost[] = { 3, 3, 1 };

    assert(canCompleteCircuit(gas, 3, cost, 3) == -1, "-1");
    printf("everything is fine!\n");
    return 0;
}