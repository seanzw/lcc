#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
} 

typedef unsigned short KeyT;

/// Introduction to Algorithms, 3rd edition
/// 10.2.8: np = next ^ prev.
typedef struct LinkedListNode {
    KeyT key;
    struct LinkedListNode* np;
} LinkedListNode;


/// Doubly linked list with two nil, non-circular.
/// 0 <--> head <--> E0 <--> ... <--> tail <--> 0
typedef struct LinkedList {
    LinkedListNode* head;
    LinkedListNode* tail;
} LinkedList;

/// Initialize an empty singly circular linked list.
/// Return 0 if malloc failed.
LinkedList* linked_list_init();

void linked_list_free(LinkedList* list);

LinkedListNode* linked_list_next(LinkedListNode* curr, LinkedListNode* prev);

int linked_list_is_empty(LinkedList* list);

/// Insert at head.
int linked_list_insert(LinkedList* list, KeyT key);

/// Search a key, return 0 if not found.
LinkedListNode* linked_list_search(LinkedList* list, KeyT key);

/// Delete a node, return -1 if not found.
int linked_list_delete(LinkedList* list, LinkedListNode* x);

/// Reverse a linked list (nonrecursive, O(1)).
void linked_list_reverse(LinkedList* list);

int main(int argc, char* argv[]) {

#define SIZE 5

    KeyT data[SIZE] = { 1, 3, 4, 5, 9 };
    LinkedList* list = linked_list_init();
    assert(list != 0, "init1");
    assert(linked_list_is_empty(list), "empty1");
    linked_list_reverse(list);
    assert(linked_list_is_empty(list), "reverse empty list should be empty");
    assert(linked_list_delete(list, 0) != 0, "delete null node");

    for (int i = 0; i < SIZE; ++i) {
        assert(linked_list_insert(list, data[i]) == 0, "insert1");
        assert(linked_list_is_empty(list) == 0, "should not be empty after insert");
        assert(list->head->np->key == data[i], "new key should be at head");
    }

    assert(linked_list_search(list, 10) == list->tail, "search 10 not found");
    assert(linked_list_search(list, 1) != list->tail, "serach 1 found");
    assert(linked_list_search(list, 9) != list->tail, "serach 9 found");

    LinkedListNode* curr;
    LinkedListNode* prev;
    LinkedListNode* next;
    int i, j;
    for (i = SIZE - 1, prev = list->head, curr = list->head->np; curr != list->tail; --i, prev = curr, curr = next) {
        next = linked_list_next(curr, prev);
        assert(curr->key == data[i], "order should behave like a stack");
    }

    linked_list_reverse(list);
    for (i = 0, prev = list->head, curr = list->head->np; curr != list->tail; ++i, prev = curr, curr = next) {
        next = linked_list_next(curr, prev);
        assert(curr->key == data[i], "order after reverse is the same as a queue");
    }

    for (int i = 0; i < SIZE; ++i) {
        LinkedListNode* x = linked_list_search(list, data[i]);
        assert(x != list->tail, "search x found");
        assert(x->key == data[i], "search key right");
        assert(linked_list_delete(list, x) == 0, "delete right");
        for (j = i + 1, prev = list->head, curr = list->head->np; curr != list->tail; ++j, prev = curr, curr = next) {
            next = linked_list_next(curr, prev);
            assert(curr->key == data[j], "check order after delete");
        }
    }

    assert(linked_list_is_empty(list) == 1, "is empty after all clear");

    linked_list_free(list);

    printf("everything is fine!\n");
    return 0;
}