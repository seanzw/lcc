typedef unsigned int size_t;

void* malloc(size_t);
void free(void*);
void* memcpy(void*, const void*, size_t);

typedef struct list_node_t {
    struct list_node_t* prev;
    struct list_node_t* next;
    void* data;
} list_node_t;

typedef struct {
    list_node_t* nil;
    size_t element_size;
    // Returns 0 if compares the same.
    int(*comparer)(const void*, const void*);
} linked_list_t;

linked_list_t* list_init(size_t element_size, int(*comparer)(const void*, const void*)) {
    linked_list_t* list;
    list = malloc(sizeof(linked_list_t));
    if (list != 0) {
        list->element_size = element_size;
        list->comparer = comparer;
        list->nil = malloc(sizeof(list_node_t));
        if (list->nil != 0) {
            list->nil->prev = list->nil;
            list->nil->next = list->nil;
            list->nil->data = 0;
            return list;
        }
        else {
            free(list);
        }
    }
    return 0;
}

void list_free(linked_list_t* list) {
    list_node_t* iter;
    iter = list->nil->next;
    while (iter != list->nil) {
        list_node_t* next;
        next = iter->next;
        free(iter->data);
        free(iter);
        iter = next;
    }
    free(list);
}

/// insert into the list, return 0 if succeed.
int list_insert(linked_list_t* list, const void* data) {
    list_node_t* head;
    head = malloc(sizeof(list_node_t));
    if (head != 0) {
        head->data = malloc(list->element_size);
        if (head->data != 0) {
            memcpy(head->data, data, list->element_size);
            head->next = list->nil->next;
            head->prev = list->nil;
            list->nil->next->prev = head;
            list->nil->next = head;
            return 0;
        }
        else {
            free(head);
        }
    }
    return -1;
}

/// Search the list, return nil if not found.
list_node_t* list_search(linked_list_t* list, const void* data) {
    list_node_t* iter;
    iter = list->nil->next;
    while (iter != list->nil && list->comparer(iter->data, data) != 0) {
        iter = iter->next;
    }
    return iter;
}

int list_delete(list_node_t* node) {
    node->next->prev = node->prev;
    node->prev->next = node->next;
    free(node->data);
    free(node);
    return 0;
}