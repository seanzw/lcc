#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

int uniquePathsWithObstacles(int**, int, int);

int main(int argc, char* argv[]) {
    int g1[] = { 0, 0, 0 };
    int g2[] = { 0, 1, 0 };
    int g3[] = { 0, 0, 0 };
    int* grid[] = { g1, g2, g3 };

    assert(uniquePathsWithObstacles(grid, 3, 3) == 2, "3 3 = 2");
    printf("everything is fine!\n");
    return 0;
}