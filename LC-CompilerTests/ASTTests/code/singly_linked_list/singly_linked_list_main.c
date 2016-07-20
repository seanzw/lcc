#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
} 

typedef short KeyT;

typedef struct LinkedListNode {
    KeyT key;
    struct LinkedListNode* next;
} LinkedListNode;


/// Singly circular linked list.
///  -> Nil -> E1 -> E2 -> ... -> En
/// |                              |
///  --------------<---------------
///
/// For empty list:
///  -> Nil ->
/// |         |
///  ----<----
typedef struct LinkedList {
    LinkedListNode* nil;
} LinkedList;

/// Initialize an empty singly circular linked list.
/// Return 0 if malloc failed.
LinkedList* linked_list_init();

void linked_list_free(LinkedList* list);

int linked_list_is_empty(LinkedList* list);

/// Insert at head.
int linked_list_insert(LinkedList* list, KeyT key);

/// Search a key, return list->nil if not found.
LinkedListNode* linked_list_search(LinkedList* list, KeyT key);

/// Delete a node, return -1 if not found.
int linked_list_delete(LinkedList* list, LinkedListNode* x);

/// Reverse a linked list (nonrecursive).
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
        assert(list->nil->next->key == data[i], "new key should be at head");
    }

    assert(linked_list_search(list, 10) == list->nil, "search 10 not found");
    assert(linked_list_search(list, 1) != list->nil, "serach 1 found");
    assert(linked_list_search(list, 9) != list->nil, "serach 9 found");

    LinkedListNode* iter = list->nil->next;
    int i = SIZE - 1;
    while (iter != list->nil) {
        assert(iter->key == data[i--], "order should behave like a stack");
        iter = iter->next;
    }

    linked_list_reverse(list);
    iter = list->nil->next;
    i = 0;
    while (iter != list->nil) {
        assert(iter->key == data[i++], "order after reverse is the same as a queue");
        iter = iter->next;
    }

    for (int i = 0; i < SIZE; ++i) {
        LinkedListNode* x = linked_list_search(list, data[i]);
        assert(x != 0, "search x found");
        assert(x->key == data[i], "search key right");
        assert(linked_list_delete(list, x) == 0, "delete right");
        iter = list->nil->next;
        int j = i + 1;
        while (iter != list->nil) {
            assert(iter->key == data[j++], "check order after delete");
            iter = iter->next;
        }
    }

    assert(linked_list_is_empty(list) == 1, "is empty after all clear");

    linked_list_free(list);

    printf("everything is fine!\n");
    return 0;
}