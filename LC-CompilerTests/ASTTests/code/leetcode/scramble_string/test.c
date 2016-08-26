#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

int isScramble(char* s1, char* s2);

int main(int argc, char* argv[]) {
    assert(isScramble("ab", "aa") == 0, "ab != aa");
    printf("everything is fine!\n");
    return 0;
}