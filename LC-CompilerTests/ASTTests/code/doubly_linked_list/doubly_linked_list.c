typedef unsigned int size_t;
void* malloc(size_t size);
void free(void* ptr);

typedef unsigned short KeyT;

/// Introduction to Algorithms, 3rd edition
/// 10.2.8: np = next ^ prev.
typedef struct LinkedListNode {
    KeyT key;
    struct LinkedListNode* np;
} LinkedListNode;


/// Doubly circular linked list.
typedef struct LinkedList {
    LinkedListNode* head;
    LinkedListNode* tail;
} LinkedList;

LinkedListNode* linked_list_next(LinkedListNode* curr, LinkedListNode* prev) {
    return (LinkedListNode*)((int)curr->np ^ (int)prev);
}

LinkedList* linked_list_init() {
    LinkedList* list;
    list = malloc(sizeof(LinkedList));
    if (list != 0) {
        list->head = malloc(sizeof(LinkedListNode));
        list->tail = malloc(sizeof(LinkedListNode));
        if (list->head != 0 && list->tail != 0) {
            list->head->np = list->tail;
            list->tail->np = list->head;
            return list;
        }
        else {
            if (list->head != 0) {
                free(list->head);
            }
            if (list->tail != 0) {
                free(list->tail);
            }
            free(list);
        }
    }
    return 0;
}

void linked_list_free(LinkedList* list) {
    LinkedListNode* curr;
    LinkedListNode* prev;
    LinkedListNode* next;
    curr = list->head->np;
    prev = list->head;
    while (curr != list->tail) {
        next = linked_list_next(curr, prev);
        prev = curr;
        curr = next;
        free(prev);
    }
    free(list->head);
    free(list->tail);
    free(list);
}

int linked_list_is_empty(LinkedList* list) {
    return list->head->np == list->tail;
}

/// Insert at head.
int linked_list_insert(LinkedList* list, KeyT key) {
    LinkedListNode* i;
    i = malloc(sizeof(LinkedListNode));
    if (i != 0) {
        i->key = key;
        i->np = (LinkedListNode*)((int)list->head ^ (int)list->head->np);
        list->head->np->np = (LinkedListNode*)((int)list->head->np->np ^ (int)i ^ (int)list->head);
        list->head->np = i;
        return 0;
    }
    return -1;
}

/// Search a key, return list->tail if not found.
LinkedListNode* linked_list_search(LinkedList* list, KeyT key) {
    LinkedListNode* curr;
    LinkedListNode* prev;
    LinkedListNode* next;
    list->tail->key = key;
    for (prev = list->head, curr = list->head->np; curr->key != key; prev = curr, curr = next) {
        next = linked_list_next(curr, prev);
    }
    return curr;
}

/// Delete a node, return -1 if not found.
int linked_list_delete(LinkedList* list, LinkedListNode* x) {
    LinkedListNode* curr;
    LinkedListNode* prev;
    LinkedListNode* next;
    for (prev = list->head, curr = list->head->np; curr != list->tail; prev = curr, curr = next) {
        next = linked_list_next(curr, prev);
        if (curr == x) {
            prev->np = (LinkedListNode*)((int)prev->np ^ (int)curr ^ (int)next);
            next->np = (LinkedListNode*)((int)next->np ^ (int)curr ^ (int)prev);
            free(curr);
            return 0;
        }
    }
    return -1;
}

/// Reverse a linked list (nonrecursive, O(1)).
void linked_list_reverse(LinkedList* list) {
    LinkedListNode* temp;
    temp = list->head;
    list->head = list->tail;
    list->tail = temp;
}