#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
} 

typedef unsigned int size_t;

typedef struct bst_node_t {
    struct bst_node_t* parent;
    struct bst_node_t* lhs;
    struct bst_node_t* rhs;
    char data[];
} bst_node_t;

typedef struct bst_t {
    size_t element_size;
    int(*comparer)(const void* lhs, const void* rhs);   // Returns -1 if lhs < rhs, 0 if lhs == rhs, 1 if lhs > rhs.
    bst_node_t* root;
} bst_t;

bst_t* bst_init(size_t element_size, int(*comparer)(const void* lhs, const void* rhs));

void bst_free(bst_t* tree);

// returns 0 if not found.
bst_node_t* bst_search(bst_t* tree, const void* data);

/// Find the minimum element in the tree.
/// Return 0 if the tree is empty.
bst_node_t* bst_minimum(bst_t* tree);

/// Find the maximum element in the tree.
/// Return 0 if the tree is empty.
bst_node_t* bst_maximum(bst_t* tree);

/// Find the successor, return 0 if none.
bst_node_t* bst_successor(bst_node_t* curr);

/// Find the precessor, return 0 if none.
bst_node_t* bst_precessor(bst_node_t* curr);

/// Insert the data into the tree, return 0 if failed.
bst_node_t* bst_insert(bst_t* tree, const void* data);

void bst_delete(bst_t* tree, bst_node_t* node);

void quick_sort(int* arr, int lhs, int rhs);

typedef int data_t;

int comparer(const data_t* lhs, const data_t* rhs) {
    if (*lhs < *rhs) return -1;
    else if (*lhs == *rhs) return 0;
    else return 1;
}

int main(int argc, char* argv[]) {
#define SIZE 10
    data_t data[SIZE] = { 5, 6, 8, 4, 5, 6, 9, 8, 0, -1 };
    
    bst_t* tree;
    assert(tree = bst_init(sizeof(data_t), (int(*)(const void*, const void*))comparer), "init");
    assert(tree->root == 0, "empty");
    assert(bst_minimum(tree) == 0, "no minimum");

    for (int i = 0; i < SIZE; ++i) {
        assert(bst_insert(tree, data + i), "insert");
    }

    // sort the data.
    quick_sort(data, 0, SIZE - 1);
    for (int i = 0; i < SIZE; ++i) {
        int j = i;
        for (bst_node_t* iter = bst_minimum(tree); iter; iter = bst_successor(iter)) {
            //printf("data[%d] = %d, bst[%d] = %d\n", j, data[j], j, *(data_t*)iter->data);
            assert(comparer(data + j++, (data_t*)iter->data) == 0, "check the order");
        }
        // delete the data.
        bst_delete(tree, bst_minimum(tree));
    }

    assert(tree->root == 0, "should be empty after deleting all elements");

    bst_free(tree);

    printf("everything is fine!\n");
    return 0;
}