#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
}

struct ListNode {
    int val;
    struct ListNode *next;
};

int minDistance(char* word1, char* word2);

int main(int argc, char* argv[]) {
    assert(minDistance("a", "b") == 1, "change 1");
    assert(minDistance("aabcd", "abcd") == 1, "insert 1");
    assert(minDistance("abcdd", "abcd") == 1, "delete 1");
    printf("everything is fine!\n");
    return 0;
}