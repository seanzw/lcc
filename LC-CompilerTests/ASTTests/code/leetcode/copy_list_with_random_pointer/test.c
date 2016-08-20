#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

struct RandomListNode {
    int label;
    struct RandomListNode *next;
    struct RandomListNode *random;
};

struct RandomListNode *copyRandomList(struct RandomListNode *head);

int main(int argc, char* argv[]) {

    struct RandomListNode n1, n2, n3;
    n1.label = 1;
    n2.label = 2;
    n3.label = 3;

    n1.next = &n2;
    n2.next = &n3;
    n3.next = 0;

    n1.random = &n1;
    n2.random = &n1;
    n3.random = &n2;

    struct RandomListNode* list = copyRandomList(&n1);
    assert(list->label == 1, "n1 == 1");
    assert(list->random->label == 1, "n1 random == 1");
    assert(list->next->label == 2, "n2 == 2");


    printf("everything is fine!\n");
    return 0;
}