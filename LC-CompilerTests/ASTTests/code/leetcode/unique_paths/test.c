#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

int uniquePaths(int m, int n);

int main(int argc, char* argv[]) {
    assert(uniquePaths(3, 3) == 6, "3 3 = 1");
    printf("everything is fine!\n");
    return 0;
}