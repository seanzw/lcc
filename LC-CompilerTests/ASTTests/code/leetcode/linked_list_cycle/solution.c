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
/**
* Definition for singly-linked list.
* struct ListNode {
*     int val;
*     struct ListNode *next;
* };
*/
int hasCycle(struct ListNode *head) {
    struct ListNode* fast;
    struct ListNode* slow;
    fast = head;
    slow = head;
    while (fast && slow) {
        if (fast->next) {
            slow = slow->next;
            fast = fast->next->next;
            if (fast == slow) return 1;
        }
        else {
            return 0;
        }
    }
    return 0;
}