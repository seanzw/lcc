#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
}

int isMatch(char* s, char* p);

int main(int argc, char* argv[]) {
    assert(isMatch("aa", "a") == 0, "t1");
    assert(isMatch("aa", "aa") == 1, "t1");
    assert(isMatch("aaa", "aa") == 0, "t1");
    assert(isMatch("aa", "a*") == 1, "t1");
    assert(isMatch("aa", ".*") == 1, "t1");
    assert(isMatch("ab", ".*") == 1, "t1");
    assert(isMatch("aab", "c*a*b") == 1, "t2");
    printf("everything is fine!\n");
    return 0;
}