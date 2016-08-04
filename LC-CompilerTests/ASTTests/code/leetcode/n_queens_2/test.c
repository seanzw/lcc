#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    /*exit(-1);                               \*/\
} 

int totalNQueens(int n);

int main(int argc, char* argv[]) {

    assert(totalNQueens(4) == 2, "size == 2");
    assert(totalNQueens(5) == 10, "size == 2");

    printf("everything is fine!\n");
    return 0;
}