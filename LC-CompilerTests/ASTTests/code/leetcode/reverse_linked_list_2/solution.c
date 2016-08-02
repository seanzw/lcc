/**
* Definition for singly-linked list.
* struct ListNode {
*     int val;
*     struct ListNode *next;
* };
*/
int printf(const char* format, ...);

struct ListNode {
    int val;
    struct ListNode *next;
};

struct ListNode* reverseBetween(struct ListNode* head, int m, int n) {

    if (m == n) return head;
    struct ListNode* prev;
    struct ListNode* curr;      //                          m            n
    struct ListNode* n1;        // l1 -> l2 -> ... -> n1 -> n2 -> ... -> n3 -> n4 -> ...
    struct ListNode* n2;
    struct ListNode* n3;
    struct ListNode* n4;
    int i;
    i = 0;
    curr = head;
    prev = 0;
    while (curr) {
        ++i;
        if (i < m) {
            prev = curr;
            curr = curr->next;
        }
        else if (i == m) {
            n1 = prev;
            n2 = curr;
            prev = curr;
            curr = curr->next;
        }
        else if (i < n) {
            struct ListNode* next;
            next = curr->next;
            curr->next = prev;
            prev = curr;
            curr = next;
        }
        else if (i == n) {
            n3 = curr;
            n4 = curr->next;
            curr->next = prev;
            prev = curr;
            curr = n4;
        }
        else {
            break;
        }
    }

    if (n1) {
        n1->next = n3;
        n2->next = n4;
        return head;
    }
    else {
        n2->next = n4;
        return n3;
    }
}