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

struct ListNode* reverseKGroup(struct ListNode* head, int k) {

    if (k <= 1) return head;

    ///                      mk         mk + k
    /// head -> ... -> n1 -> n2 -> ... -> n3 -> n4 -> ...

    struct ListNode* iter;
    struct ListNode* n1;
    struct ListNode* n2;
    struct ListNode* n3;
    struct ListNode* n4;

    struct ListNode nil;
    nil.next = head;

    n1 = &nil;
    n2 = head;

    iter = head;

    int i;
    i = 0;
    while (iter) {
        i++;
        if (i % k == 0) {
            n3 = iter;
            n4 = iter->next;
            /// Reverse.
            struct ListNode* curr;
            struct ListNode* prev;
            struct ListNode* next;

            prev = n1;
            curr = n2;
            while (curr != n4) {
                next = curr->next;
                curr->next = prev;
                prev = curr;
                curr = next;
            }
            n1->next = n3;
            n2->next = n4;
            n1 = n2;
            n2 = n4;
            
            /// Update iter.
            iter = n2;
        }
        else {
            iter = iter->next;
        }
    }
    return nil.next;
}