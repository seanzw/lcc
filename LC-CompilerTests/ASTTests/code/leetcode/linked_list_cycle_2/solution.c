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
struct ListNode* detectCycle(struct ListNode *head) {
    struct ListNode* fast;
    struct ListNode* slow;
    struct ListNode* late;
    fast = slow = late = head;
    while (fast && slow) {
        if (fast->next) {
            fast = fast->next->next;
            slow = slow->next;
            if (fast == slow) {
                while (slow != late) {
                    slow = slow->next;
                    late = late->next;
                }
                return late;
            }
        }
        else {
            return 0;
        }
    }
    return 0;
}