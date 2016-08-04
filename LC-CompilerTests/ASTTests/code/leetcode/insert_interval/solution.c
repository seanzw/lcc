int printf(const char* format, ...);

typedef unsigned int size_t;

void* malloc(size_t);
void free(void*);
void* memcpy(void*, const void*, size_t);

struct Interval {
    int start;
    int end;
};


typedef struct node_t {
    struct node_t* next;
    struct Interval* i;
} Node;

void print_list(Node* nil) {
    Node* t;
    t = nil->next;
    while (t) {
        printf("[%d, %d]\n", t->i->start, t->i->end);
        t = t->next;
    }
}

Node* arr2list(struct Interval* intervals, int intervalsSize) {
    Node* nil, * tail;
    int i;
    nil = malloc(sizeof(Node));
    tail = nil;
    nil->next = 0;
    for (i = 0; i < intervalsSize; ++i) {
        Node* n;
        n = malloc(sizeof(Node));
        n->i = intervals + i;
        n->next = 0;
        tail->next = n;
        tail = n;
    }
    return nil;
}

void list_free(Node* nil) {
    Node* t;
    while (nil != 0) {
        t = nil->next;
        free(nil);
        nil = t;
    }
}

struct Interval* list2arr(Node* nil, int* size) {
    Node* t;
    t = nil->next;
    int i;
    i = 0;
    while (t != 0) {
        i++;
        t = t->next;
    }
    struct Interval* arr;
    arr = malloc(sizeof(struct Interval) * i);
    i = 0;
    t = nil->next;
    while (t != 0) {
        arr[i++] = *(t->i);
        t = t->next;
    }
    *size = i;
    return arr;
}

/**
* Definition for an interval.
* struct Interval {
*     int start;
*     int end;
* };
*/
/**
* Return an array of size *returnSize.
* Note: The returned array must be malloced, assume caller calls free().
*/
struct Interval* insert(struct Interval* intervals, int intervalsSize, struct Interval newInterval, int* returnSize) {
    Node* nil;
    Node* s;
    Node* t;
    Node* n;
    nil = arr2list(intervals, intervalsSize);

    n = malloc(sizeof(Node));
    n->i = &newInterval;
    n->next = 0;

    s = nil;
    t = nil->next;

    while (t != 0) {
        if (n->i->end < t->i->start) {
            // Won't overlab.
            break;
        }
        else if (n->i->start > t->i->end) {
            // Do not overlap.
            s = t;
            t = t->next;
        }
        else {
            // Overlap.
            n->i->start = n->i->start > t->i->start ? t->i->start : n->i->start;
            n->i->end = n->i->end > t->i->end ? n->i->end : t->i->end;
            // remove t;
            t = t->next;
            free(s->next);
            s->next = t;
        }
        //print_list(nil);
    }
    // insert n after s.
    n->next = s->next;
    s->next = n;

    //print_list(nil);

    // convert back to array.
    struct Interval* ret;
    //printf("x");

    ret = list2arr(nil, returnSize);

    //printf("x");
    list_free(nil);
    return ret;
}