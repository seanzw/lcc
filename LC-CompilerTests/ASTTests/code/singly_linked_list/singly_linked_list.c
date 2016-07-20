typedef unsigned int size_t;
void* malloc(size_t size);
void free(void* ptr);

typedef short KeyT;

typedef struct LinkedListNode {
    KeyT key;
    struct LinkedListNode* next;
} LinkedListNode;

typedef struct LinkedList {
    LinkedListNode* nil;
} LinkedList;

LinkedList* linked_list_init() {
    LinkedList* list;
    list = malloc(sizeof(LinkedList));
    if (list != 0) {
        list->nil = malloc(sizeof(LinkedListNode));
        if (list->nil != 0) {
            list->nil->next = list->nil;
            return list;
        }
        else {
            free(list);
        }
    }
    return 0;
}

void linked_list_free(LinkedList* list) {
    LinkedListNode* iter;
    iter = list->nil->next;
    while (iter != list->nil) {
        LinkedListNode* next;
        next = iter->next;
        free(iter);
        iter = next;
    }
    free(list->nil);
    free(list);
}

int linked_list_is_empty(LinkedList* list) {
    return list->nil->next == list->nil;
}

/// Insert at head.
int linked_list_insert(LinkedList* list, KeyT key) {
    LinkedListNode* i;
    i = malloc(sizeof(LinkedListNode));
    if (i != 0) {
        i->key = key;
        i->next = list->nil->next;
        list->nil->next = i;
        return 0;
    }
    return -1;
}

/// Search a key, return list->nil if not found.
LinkedListNode* linked_list_search(LinkedList* list, KeyT key) {
    LinkedListNode* iter;
    iter = list->nil->next;
    list->nil->key = key;
    while (iter->key != key) {
        iter = iter->next;
    }
    return iter;
}

/// Delete a node, return -1 if not found.
int linked_list_delete(LinkedList* list, LinkedListNode* x) {
    LinkedListNode* prev;
    prev = list->nil;
    while (prev->next != list->nil) {
        if (prev->next == x) {
            prev->next = x->next;
            free(x);
            return 0;
        }
        else {
            prev = prev->next;
        }
    }
    return -1;
}

/// Reverse a linked list (nonrecursive).
void linked_list_reverse(LinkedList* list) {
    LinkedListNode* prev, * curr, * next;
    prev = list->nil;
    curr = list->nil->next;
    while (curr != list->nil) {
        next = curr->next;
        curr->next = prev;
        prev = curr;
        curr = next;
    }
    list->nil->next = prev;
}