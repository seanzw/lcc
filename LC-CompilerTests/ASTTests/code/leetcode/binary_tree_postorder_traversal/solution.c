/**
* Definition for a binary tree node.
* struct TreeNode {
*     int val;
*     struct TreeNode *left;
*     struct TreeNode *right;
* };
*/
/**
* Return an array of size *returnSize.
* Note: The returned array must be malloced, assume caller calls free().
*/

typedef unsigned int size_t;
void* malloc(size_t);
void free(void*);
void* memcpy(void*, const void*, size_t);

struct TreeNode {
    int val;
    struct TreeNode *left;
    struct TreeNode *right;
};

typedef struct {
    enum {
        NODE,
        VAL
    } type;
    union {
        struct TreeNode* node;
        int val;
    } data;
} E;

void push(void** arr, size_t* capacity, size_t* n, const void* data, const size_t piece) {
    if (*n + piece > *capacity) {
        void* new_arr = malloc((*capacity) * 2);
        memcpy(new_arr, *arr, *n);
        free(*arr);
        *arr = new_arr;
        *capacity *= 2;
    }
    memcpy(*arr + *n, data, piece);
    *n += piece;
}

void pop(void* arr, size_t* n, void* data, const size_t piece) {
    memcpy(data, arr + *n - piece, piece);
    *n -= piece;
}

int* postorderTraversal(struct TreeNode* root, int* returnSize) {
    void* stack = malloc(sizeof(E) * 2);
    size_t stack_capacity = 2 * sizeof(E);
    size_t stack_n = 0;

    void* data = malloc(sizeof(int) * 2);
    size_t data_capacity = 2 * sizeof(int);
    size_t data_n = 0;

    /// Initialize the root.
    E e;
    if (root) {
        e.type = NODE;
        e.data.node = root;
        push(&stack, &stack_capacity, &stack_n, &e, sizeof(E));
    }

    while (stack_n > 0) {
        pop(stack, &stack_n, &e, sizeof(E));
        switch (e.type) {
        case NODE: {
            struct TreeNode* node = e.data.node;
            e.type = VAL;
            e.data.val = node->val;
            push(&stack, &stack_capacity, &stack_n, &e, sizeof(E));

            e.type = NODE;
            if (node->right) {
                e.data.node = node->right;
                push(&stack, &stack_capacity, &stack_n, &e, sizeof(E));
            }
            if (node->left) {
                e.data.node = node->left;
                push(&stack, &stack_capacity, &stack_n, &e, sizeof(E));
            }
            
            break;
        }
        case VAL: {
            push(&data, &data_capacity, &data_n, &(e.data.val), sizeof(int));
            break;
        }
        default:
            break;
        }
    }

    *returnSize = data_n / sizeof(int);
    free(stack);
    return data;
}