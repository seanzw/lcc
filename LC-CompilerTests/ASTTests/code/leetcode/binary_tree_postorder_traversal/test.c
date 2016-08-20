#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

struct TreeNode {
    int val;
    struct TreeNode *left;
    struct TreeNode *right;
};

int* postorderTraversal(struct TreeNode* root, int* returnSize);

int main(int argc, char* argv[]) {
    struct TreeNode root, left, right;
    root.val = 0;
    left.val = 1;
    right.val = 2;

    root.left = &left;
    root.right = &right;
    left.left = left.right = right.left = right.right = 0;

    int returnSize;
    int* data = postorderTraversal(&root, &returnSize);
    assert(returnSize == 3, "return size should be 3");
    assert(data[0] == 1, "data[0] == 1");
    assert(data[1] == 2, "data[1] == 2");
    assert(data[2] == 0, "data[2] == 0");
    printf("everything is fine!\n");
    return 0;
}