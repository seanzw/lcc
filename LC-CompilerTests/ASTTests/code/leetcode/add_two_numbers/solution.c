/**
* Definition for singly-linked list.
* struct ListNode {
*     int val;
*     struct ListNode *next;
* };
*/

struct ListNode {
    int val;
    struct ListNode *next;
};

void* malloc(unsigned int size);

struct ListNode* addTwoNumbers(struct ListNode* l1, struct ListNode* l2) {
    struct ListNode* head;
    struct ListNode* tail;
    int carry;
    carry = 0;
    head = tail = 0;
    while (l1) {
        if (tail == 0) {
            tail = malloc(sizeof(struct ListNode));
            head = tail;
        }
        else {
            tail->next = malloc(sizeof(struct ListNode));
            tail = tail->next;
        }
        tail->next = 0;
        int val;
        val = carry + l1->val + (l2 ? l2->val : 0);
        tail->val = val % 10;
        carry = val / 10;
        l1 = l1->next;
        if (l2) l2 = l2->next;
    }
    while (l2) {
        if (tail == 0) {
            tail = malloc(sizeof(struct ListNode));
            head = tail;
        }
        else {
            tail->next = malloc(sizeof(struct ListNode));
            tail = tail->next;
        }
        tail->next = 0;
        int val;
        val = carry + l2->val;
        tail->val = val % 10;
        carry = val / 10;
        l2 = l2->next;
    }
    if (carry) {
        tail->next = malloc(sizeof(struct ListNode));
        tail = tail->next;
        tail->next = 0;
        tail->val = carry;
    }
    return head;
}