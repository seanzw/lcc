#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

int minCut(char* s);

int main(int argc, char* argv[]) {

    assert(minCut("aab") == 1, "aab");

    printf("everything is fine!\n");
    return 0;
}