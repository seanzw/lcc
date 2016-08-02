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

struct ListNode* reverseKGroup(struct ListNode* head, int k);

int main(int argc, char* argv[]) {
    struct ListNode x1, x2, x3, x4, x5;
    x1.val = 1;
    x2.val = 2;
    x3.val = 3;
    x4.val = 4;
    x5.val = 5;
    x1.next = &x2;
    x2.next = &x3;
    x3.next = &x4;
    x4.next = &x5;
    x5.next = 0;
    struct ListNode* ret = reverseKGroup(&x1, 2);
    struct ListNode* x = ret;
    while (x) {
        printf("%d \n", x->val);
        x = x->next;
    }
    assert(ret->val == 2, "ret0");
    assert(ret->next->val == 1, "ret1");
    assert(ret->next->next->val == 4, "ret2");
    assert(ret->next->next->next->val == 3, "ret3");
    assert(ret->next->next->next->next->val == 5, "ret4");
    assert(ret->next->next->next->next->next == 0, "ret5");
    printf("everything is fine!\n");
    return 0;
}