typedef unsigned int size_t;
void* malloc(size_t);
void free(void*);
void* memcpy(void*, const void*, size_t);
int printf(const char*, ...);
/**
* Definition for binary tree with next pointer.
* struct TreeLinkNode {
*  int val;
*  struct TreeLinkNode *left, *right, *next;
* };
*
*/

struct TreeLinkNode {
    int val;
    struct TreeLinkNode *left, *right, *next;
};

int set_parent(struct TreeLinkNode* root, int level) {
    if (!root) return level;
    if (root->left) root->left->next = root;
    if (root->right) root->right->next = root;
    int l = set_parent(root->left, level + 1);
    int r = set_parent(root->right, level + 1);
    return l > r ? (l > level ? l : level) : (r > level ? r : level);
}

struct TreeLinkNode* start(struct TreeLinkNode* node, int target, int current) {
    if (node == 0) return 0;
    else {
        if (current == target) return node;
        else {
            struct TreeLinkNode* x;
            if ((x = start(node->left, target, current + 1))) return x;
            else return start(node->right, target, current + 1);
        }
    }
}

struct TreeLinkNode* successor(struct TreeLinkNode* node, int target) {
    int current = target - 1;
    struct TreeLinkNode* curr = node->next;
    struct TreeLinkNode* prev = node;
    while (curr) {
        if (prev == curr->left) {
            /// prev is the left child of curr.
            struct TreeLinkNode* s = start(curr->right, target, current + 1);
            if (s) return s;
            else {
                prev = prev->next;
                curr = curr->next;
                current--;
            }
        }
        else {
            /// prev is the right child of curr.
            prev = prev->next;
            curr = curr->next;
            current--;
        }
    }
    return 0;
}

void connect(struct TreeLinkNode *root) {

    if (!root) return;

    root->next = 0;
    int level = set_parent(root, 0);
    for (int i = level; i > 0; --i) {
        struct TreeLinkNode* node = start(root, i, 1);
        struct TreeLinkNode* next = successor(node, i);
        while (next) {
            node->next = next;
            node = next;
            next = successor(node, i);
        }
        node->next = 0;
    }

}