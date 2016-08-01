int printf(const char* format, ...);

typedef unsigned int size_t;

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

typedef struct {
    int self;
    int min;
    int max;
} data_t;

int compare(const data_t* x, const data_t* y) {
    if (x->self < y->self) return -1;
    else if (x->self == y->self) return 0;
    else return 1;
}

size_t h(const data_t* x) {
    return (size_t)x->self % 697;
}

int longestConsecutive(int* nums, int numsSize) {
    hash_table_t* hash;
    hash = hash_init(sizeof(data_t), 697, (size_t(*)(const void*))h, (int(*)(const void*, const void*))compare);

    int len;
    int i;
    data_t data;
    len = 0;
    for (i = 0; i < numsSize; ++i) {
        data.self = data.min = data.max = nums[i];
        hash_entry_t* curr;
        curr = hash_search(hash, &data);
        if (curr == 0) {
            hash_insert(hash, &data);
            curr = hash_search(hash, &data);
            hash_entry_t* prev;
            hash_entry_t* next;
            data.self = nums[i] - 1;
            prev = hash_search(hash, &data);
            data.self = nums[i] + 1;
            next = hash_search(hash, &data);
            hash_entry_t* min;
            hash_entry_t* max;
            if (prev != 0) {
                data.self = ((data_t*)prev->data)->min;
                min = hash_search(hash, &data);
            }
            else {
                min = curr;
            }
            if (next != 0) {
                data.self = ((data_t*)next->data)->max;
                max = hash_search(hash, &data);
            }
            else {
                max = curr;
            }
            int min_self, max_self;
            min_self = ((data_t*)min->data)->self;
            max_self = ((data_t*)max->data)->self;
            ((data_t*)min->data)->max = max_self;
            ((data_t*)max->data)->min = min_self;
            if (len < max_self - min_self + 1) {
                len = max_self - min_self + 1;
            }
        }
    }

    hash_free(hash);
    return len;
}