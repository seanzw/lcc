#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
} 

typedef unsigned int size_t;
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

Pool* pool_init(size_t elementSize, size_t capacity);

void pool_free(Pool* pool);

/// Allocate one object from the free list.
/// Return 0 if runs out of space.
void* alloc_obj(Pool* pool);

/// Free one object.
void free_obj(Pool* pool, void* data);

int main(int argc, char* argv[]) {

#define SIZE 50
    void* x[SIZE];
    Pool* pool;
    assert(pool = pool_init(sizeof(int), SIZE), "init");
    
    for (int i = 0; i < SIZE; ++i) {
        assert(x[i] = alloc_obj(pool), "alloc");
    }

    assert(alloc_obj(pool) == 0, "alloc when full should fail");

    for (int i = 0; i < SIZE; ++i) {
        free_obj(pool, x[i]);
    }

    for (int i = 0; i < SIZE; ++i) {
        assert(x[i] = alloc_obj(pool), "alloc after free all objs");
    }

    assert(alloc_obj(pool) == 0, "alloc again when full should fail");


    pool_free(pool);
    printf("everything is fine!\n");
    return 0;

}