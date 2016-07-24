#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
} 

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

typedef list_node_t hash_entry_t;

typedef struct {
    size_t m;                       // Number of slots.
    size_t(*h)(const void*);       // Hash function.
    linked_list_t** lists;           // Chains.
} hash_table_t;

hash_table_t* hash_init(
    size_t element_size,
    size_t m,
    size_t(*h)(const void*),
    int(*comparer)(const void*, const void*)
    );

void hash_free(hash_table_t* hash);

int hash_insert(hash_table_t* hash, const void* data);

/// Search the hash table, return 0 if not found.
hash_entry_t* hash_search(hash_table_t* hash, const void* data);

/// Delete an entry.
int hash_delete(hash_table_t* hash, hash_entry_t* entry);

typedef long data_t;
size_t m = 701;
size_t h_mod(const data_t* data) {
    return *data % m;
}
int comparer(const data_t* d1, const data_t* d2) {
    return *d1 != *d2;
}

int main(int argc, char* argv[]) {

    data_t data[] = { 1, 702, 4, 6, 7, 3, 5, };
    data_t not_inside_data = 0;
    hash_table_t* hash = hash_init(
        sizeof(data_t), 
        m, 
        (size_t (*)(const void*))h_mod, 
        (int (*)(const void*, const void*))comparer
        );
    assert(hash, "init");

    for (int i = 0; i < sizeof(data) / sizeof(data_t); ++i) {
        assert(hash_insert(hash, data + i) == 0, "insert");
    }

    assert(hash_search(hash, &not_inside_data) == 0, "search 0 should not be found");
    hash_entry_t* e1 = hash_search(hash, data);
    assert(e1 != 0, "search 1 should found");
    hash_entry_t* e2 = hash_search(hash, data + 1);
    assert(e2 != 0, "search 702 should found");
    assert(e2->next == e1, "collision in chain");

    assert(hash_delete(hash, e1) == 0, "delete 1");
    assert(hash_search(hash, data) == 0, "cannot found 1 after deleting");

    hash_free(hash);
    printf("everything is fine!\n");
    return 0;
}