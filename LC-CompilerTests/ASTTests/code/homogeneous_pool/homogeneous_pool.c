typedef unsigned int size_t;

void* malloc(size_t size);
void free(void* ptr);

/// Let L be a doubly linked list of length n stored in arrays key, pre,and next of
/// length m. Suppose that these arrays are managed by ALLOCATE - OBJECT and
/// FREE - OBJECT procedures that keep a doubly linked free list F. Suppose further
/// that of the m items, exactly n are on list L and m - n are on the free list. Write
/// a procedure COMPACTIFY - LIST(L, F) that, given the list L and the free list F,
/// moves the items in L so that they occupy array positions 1, 2, ..., n and adjusts the
/// free list F so that it remains correct, occupying array positions n + 1, n + 2, ..., m.
/// The running time of your procedure should be O(n), and it should use only a
/// constant amount of extra space. Argue that your procedure is correct.

typedef struct FreeListNode {
    struct FreeListNode* next;
    char data[];
} FreeListNode;

typedef struct Pool {
    size_t size;
    size_t capacity;
    FreeListNode* free;
    char* data;
} Pool;

FreeListNode* pool_nth(Pool* pool, size_t n) {
    return (FreeListNode*)(pool->data + pool->size * n);
}

Pool* pool_init(size_t elementSize, size_t capacity) {
    Pool* pool;
    pool = malloc(sizeof(Pool));
    if (pool) {
        pool->size = sizeof(FreeListNode) + elementSize;
        pool->capacity = capacity;
        pool->data = malloc(pool->size * capacity);
        if (pool->data) {
            pool->free = (FreeListNode*)pool->data;

            // Initialize the free list.
            unsigned int iter;
            for (iter = 0; iter < capacity - 1; ++iter) {
                pool_nth(pool, iter)->next = pool_nth(pool, iter + 1);
            }

            // Set the last free list node pointing to 0.
            pool_nth(pool, pool->capacity - 1)->next = 0;

            return pool;
        }
        else {
            free(pool);
        }
    }
    return 0;
}

void pool_free(Pool* pool) {
    free(pool->data);
    free(pool);
}

/// Allocate one object from the free list.
/// Return 0 if runs out of space.
void* alloc_obj(Pool* pool) {

    if (pool->free == 0) return 0;
    
    void* obj;
    obj = pool->free->data;
    pool->free = pool->free->next;
    return obj;
}

/// Free one object.
void free_obj(Pool* pool, void* data) {
    size_t offset;
    offset = (size_t)(&(((FreeListNode*)0)->data));
    FreeListNode* node;
    node = data - offset;
    node->next = pool->free;
    pool->free = node;
}