#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

int isMatch(char* s, char* p);

int main(int argc, char* argv[]) {
    
    assert(isMatch("aa", "a") == 0, "aa a");
    assert(isMatch("aa", "aa") == 1, "aa aa");
    assert(isMatch("aaa", "aa") == 0, "aaa aa");
    assert(isMatch("aa", "*") == 1, "aa *");
    assert(isMatch("aa", "a*") == 1, "aa a*");
    assert(isMatch("ab", "?*") == 1, "ab ?*");
    assert(isMatch("", "?") == 0, " ?");
    assert(isMatch("aab", "c*a*b") == 0, "aab c*a*b");

    printf("everything is fine!\n");
    return 0;
}