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

char* minWindow(char* s, char* t);

int main(int argc, char* argv[]) {

    char* ab_a = minWindow("aA", "a");
    printf("everything is fine!\n");
    return 0;
}