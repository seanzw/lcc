typedef unsigned int size_t;
void* malloc(size_t);
void free(void*);
void* memcpy(void*, const void*, size_t);
/**
* Definition for singly-linked list with a random pointer.
* struct RandomListNode {
*     int label;
*     struct RandomListNode *next;
*     struct RandomListNode *random;
* };
*/

struct RandomListNode {
    int label;
    struct RandomListNode *next;
    struct RandomListNode *random;
};

struct RandomListNode *copyRandomList(struct RandomListNode *head) {

    if (!head) return 0;

    struct RandomListNode* iter = head;
    while (iter) {
        struct RandomListNode* node = malloc(sizeof(*iter));
        node->label = iter->label;
        node->next = iter->next;
        iter->next = node;
        iter = node->next;
    }

    iter = head;
    while (iter) {
        if (iter->random)
            iter->next->random = iter->random->next;
        else
            iter->next->random = 0;
        iter = iter->next->next;
    }

    struct RandomListNode* ret;
    struct RandomListNode* t1;
    struct RandomListNode* t2;

    ret = head->next;
    t1 = head;
    t2 = head->next;
    
    iter = ret->next;
    while (iter) {
        t1->next = iter;
        t2->next = iter->next;
        t1 = t1->next;
        t2 = t2->next;
        iter = iter->next->next;
    }

    t1->next = 0;
    t2->next = 0;

    return ret;
}