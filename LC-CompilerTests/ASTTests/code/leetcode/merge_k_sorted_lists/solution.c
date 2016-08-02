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

static struct ListNode* merge2Lists(struct ListNode* l1, struct ListNode* l2) {
    struct ListNode* head;
    struct ListNode* tail;
    struct ListNode nil;
    nil.next = l1;
    tail = &nil;
    while (l1 && l2) {
        if (l1->val < l2->val) {
            l1 = l1->next;
            tail = tail->next;
        }
        else {
            tail->next = l2;
            l2 = l2->next;
            tail->next->next = l1;
            tail = tail->next;
        }
    }
    if (l2) {
        tail->next = l2;
    }
    return nil.next;
}

struct ListNode* mergeKLists(struct ListNode** lists, int listsSize) {
    struct ListNode* head;
    struct ListNode* tail;
    head = tail = 0;
    int min;
    int i;

    if (listsSize == 0) return 0;
    if (listsSize == 1) return lists[0];
    for (i = 1; i < listsSize; ++i) {
        lists[i] = merge2Lists(lists[i - 1], lists[i]);
    }
    
    return lists[listsSize - 1];

    /*int idx;
    while (1) {
        idx = -1;
        for (i = 0; i < listsSize; ++i) {
            if (lists[i]) {
                if (idx == -1 || lists[i]->val < min) {
                    idx = i;
                    min = lists[i]->val;
                }
            }
        }
        if (idx == -1) {
            return head;
        }
        else {
            if (head == 0) {
                head = tail = lists[idx];
                lists[idx] = tail->next;
            }
            else {
                tail->next = lists[idx];
                tail = tail->next;
                lists[idx] = tail->next;
            }
        }
    }*/
}