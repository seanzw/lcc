#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    /*exit(-1);                               \*/\
} 

char*** solveNQueens(int n, int* returnSize);

int main(int argc, char* argv[]) {

    char solutions[2][4][5] = {
        {
            ".Q..",
            "...Q",
            "Q...",
            "..Q."
        },
        {
            "..Q.",
            "Q...",
            "...Q",
            ".Q..",
        }
    };

    int size;
    char*** answer = solveNQueens(4, &size);

    assert(size == 2, "size == 2");

    for (int i = 0; i < size; ++i) {
        for (int j = 0; j < 4; ++j) {
            for (int k = 0; k < 4; ++k) {
                assert(answer[i][j][k] == solutions[i][j][k], "test");
            }
        }
    }

    


    printf("everything is fine!\n");
    return 0;
}