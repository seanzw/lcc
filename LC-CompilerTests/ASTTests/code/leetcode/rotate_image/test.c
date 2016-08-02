#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

void rotate(int** matrix, int matrixRowSize, int matrixColSize);

int main(int argc, char* argv[]) {
    int row1[2] = { 1, 2 };
    int row2[2] = { 3, 4 };
    int* image[2];
    image[0] = row1;
    image[1] = row2;
    int p[][2] = { {3, 1}, {4, 2} };
    rotate(image, 2, 2);
    for (int i = 0; i < 2; ++i) {
        for (int j = 0; j < 2; ++j) {
            assert(image[i][j] == p[i][j], "test");
        }
    }
    printf("everything is fine!\n");
    return 0;
}