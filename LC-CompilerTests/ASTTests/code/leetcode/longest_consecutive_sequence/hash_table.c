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

linked_list_t* list_init(size_t element_size, int(*comparer)(const void*, const void*));

void list_free(linked_list_t* list);

/// insert into the list, return 0 if succeed.
int list_insert(linked_list_t* list, const void* data);

/// Search the list, return nil if not found.
list_node_t* list_search(linked_list_t* list, const void* data);

int list_delete(list_node_t* node);

typedef list_node_t hash_entry_t;

typedef struct {
    size_t m;                       // Number of slots.
    size_t (*h)(const void*);       // Hash function.
    linked_list_t** lists;           // Chains.
} hash_table_t;

hash_table_t* hash_init(
    size_t element_size,
    size_t m,
    size_t(*h)(const void*),
    int(*comparer)(const void*, const void*)
    ) {
    hash_table_t* hash;
    hash = malloc(sizeof(hash_table_t));
    if (hash != 0) {
        hash->m = m;
        hash->h = h;
        hash->lists = malloc(m * sizeof(linked_list_t*));
        if (hash->lists) {
            size_t i, j;
            for (i = 0; i < m; ++i) {
                hash->lists[i] = list_init(element_size, comparer);
                if (!hash->lists[i]) {
                    for (j = 0; j < i; ++j) {
                        free(hash->lists[j]);
                    }
                    free(hash->lists);
                    free(hash);
                    return 0;
                }
            }
            return hash;
        }
        else {
            free(hash);
        }
    }
    return 0;
}

void hash_free(hash_table_t* hash) {
    size_t i;
    for (i = 0; i < hash->m; ++i) {
        list_free(hash->lists[i]);
    }
    free(hash->lists);
    free(hash);
}

int hash_insert(hash_table_t* hash, const void* data) {
    size_t k;
    k = hash->h(data);
    return list_insert(hash->lists[k], data);
}

/// Search the hash table, return 0 if not found.
hash_entry_t* hash_search(hash_table_t* hash, const void* data) {
    size_t k;
    k = hash->h(data);
    hash_entry_t* entry;
    entry = list_search(hash->lists[k], data);
    if (entry != hash->lists[k]->nil)
        return entry;
    else 
        return 0;
}

/// Delete an entry.
int hash_delete(hash_table_t* hash, hash_entry_t* entry) {
    size_t k;
    k = hash->h(entry->data);
    return list_delete(entry);
}