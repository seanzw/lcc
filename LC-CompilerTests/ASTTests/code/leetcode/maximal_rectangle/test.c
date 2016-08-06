#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
}

int maximalRectangle(char** matrix, int matrixRowSize, int matrixColSize);

int main(int argc, char* argv[]) {
    char* h[] = {
        "10100",
        "10111",
        "11111",
        "10010"
    };

    assert(maximalRectangle(h, 4, 5) == 6, "6");
    printf("everything is fine!\n");
    return 0;
}