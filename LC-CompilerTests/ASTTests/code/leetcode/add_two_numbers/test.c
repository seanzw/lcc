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

struct ListNode* addTwoNumbers(struct ListNode* l1, struct ListNode* l2);

int main(int argc, char* argv[]) {
    struct ListNode x1, x2, x3;
    x1.val = 2;
    x2.val = 8;
    x3.val = 3;
    x1.next = &x2;
    x2.next = &x3;
    x3.next = 0;
    struct ListNode* ret = addTwoNumbers(&x1, &x2);
    assert(ret->val == 0, "ret0");
    assert(ret->next->val == 2, "ret1");
    assert(ret->next->next->val == 4, "ret2");
    assert(ret->next->next->next == 0, "ret3");
    printf("everything is fine!\n");
    return 0;
}