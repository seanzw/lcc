#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
} 


typedef unsigned int size_t;

enum Color {
    RED,
    BLACK
};

typedef struct rbt_node_t {
    enum Color color;
    struct rbt_node_t* lhs;
    struct rbt_node_t* rhs;
    struct rbt_node_t* parent;
    char data[];
} rbt_node_t;

typedef struct {
    rbt_node_t* nil;
    rbt_node_t* root;
    int(*comparer)(const void*, const void*);
    size_t element_size;
} rbt_t;

rbt_t* rbt_init(size_t element_size, int(*comparer)(const void*, const void*));

void rbt_free(rbt_t* tree);

rbt_node_t* rbt_search(rbt_t* tree, const void* data);

rbt_node_t* rbt_minimum(rbt_t* tree);

rbt_node_t* rbt_maximum(rbt_t* tree);

rbt_node_t* rbt_successor(rbt_t* tree, rbt_node_t* curr);

rbt_node_t* rbt_precessor(rbt_t* tree, rbt_node_t* curr);

rbt_node_t* rbt_insert(rbt_t* tree, const void* data);

void rbt_delete(rbt_t* tree, rbt_node_t* node);

void rbt_print(rbt_t* tree, void(*print)(const rbt_node_t*));

int rbt_assert(rbt_t* tree);

void quick_sort(int* arr, int lhs, int rhs);

typedef int data_t;

int comparer(const data_t* lhs, const data_t* rhs) {
    if (*lhs < *rhs) return -1;
    else if (*lhs == *rhs) return 0;
    else return 1;
}

void print(const rbt_node_t* node) {
    if (node->color == RED) {
        printf("RED   %d\n", *(data_t*)(node->data));
    }
    else {
        printf("BLACK %d\n", *(data_t*)(node->data));
    }
}

int main(int argc, char* argv[]) {
#define SIZE 10
    data_t data[SIZE] = { 5, 6, 8, 4, 5, 6, 9, 8, 0, -1 };

    rbt_t* tree;
    assert(tree = rbt_init(sizeof(data_t), (int(*)(const void*, const void*))comparer), "init");
    assert(tree->root == tree->nil, "empty");
    assert(rbt_minimum(tree) == tree->nil, "no minimum");

    for (int i = 0; i < SIZE; ++i) {
        assert(rbt_insert(tree, data + i), "insert");
        assert(rbt_assert(tree) > 0, "rb_property");
        //rbt_print(tree, print);
        //printf("\n");
    }

    rbt_print(tree, print);

    // sort the data.
    quick_sort(data, 0, SIZE - 1);
    for (int i = 0; i < SIZE; ++i) {
        int j = i;
        for (rbt_node_t* iter = rbt_minimum(tree); iter != tree->nil; iter = rbt_successor(tree, iter)) {
            //printf("data[%d] = %d, rbt[%d] = %d\n", j, data[j], j, *(data_t*)iter->data);
            assert(comparer(data + j++, (data_t*)iter->data) == 0, "check the order");
        }
        // delete the data.
        rbt_delete(tree, rbt_minimum(tree));
        assert(rbt_assert(tree) > 0, "rb_property after deleting");
    }

    assert(tree->root == tree->nil, "should be empty after deleting all elements");

    rbt_free(tree);

    // Another test.
#define NUM 1000
    rbt_node_t* nodes[NUM];
    assert(tree = rbt_init(sizeof(data_t), (int(*)(const void*, const void*))comparer), "init2");
    for (int i = 0; i < NUM; ++i) {
        int r = rand();
        //rbt_print(tree, print);
        assert((nodes[i] = rbt_insert(tree, &r)) != tree->nil, "insert2");
        assert(rbt_assert(tree) > 0, "assert when insert 2");
    }

    for (int i = 0; i < NUM; ++i) {
        rbt_delete(tree, nodes[i]);
        assert(rbt_assert(tree) > 0, "assert when delete 2");
    }

    rbt_free(tree);

    printf("everything is fine!\n");
    return 0;
}