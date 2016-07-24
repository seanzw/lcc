typedef unsigned int size_t;

void* malloc(size_t size);
void free(void* ptr);
void* memcpy(void* dst, const void* src, size_t size);

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

bst_t* bst_init(size_t element_size, int(*comparer)(const void* lhs, const void* rhs)) {
    bst_t* tree;
    tree = malloc(sizeof(bst_t));
    if (tree) {
        tree->element_size = element_size;
        tree->comparer = comparer;
        tree->root = 0;
    }
    return tree;
}

void bst_free_aux(bst_node_t* node) {
    if (node) {
        bst_free_aux(node->lhs);
        bst_free_aux(node->rhs);
        free(node);
    }
}

void bst_free(bst_t* tree) {
    bst_free_aux(tree->root);
    free(tree);
}

// returns 0 if not found.
bst_node_t* bst_search(bst_t* tree, const void* data) {
    bst_node_t* curr;
    curr = tree->root;
    while (curr) {
        int ret;
        ret = tree->comparer(curr->data, data);
        if (ret == 0) {
            break;
        }
        else if (ret < 0) {
            curr = curr->rhs;
        }
        else {
            curr = curr->lhs;
        }
    }
    return curr;
}

/// Find the minimum element in the tree.
/// Return 0 if the tree is empty.
bst_node_t* bst_minimum(bst_t* tree) {
    if (tree->root) {
        bst_node_t* curr;
        curr = tree->root;
        while (curr->lhs) {
            curr = curr->lhs;
        }
        return curr;
    }
    else {
        return 0;
    }
}

/// Find the maximum element in the tree.
/// Return 0 if the tree is empty.
bst_node_t* bst_maximum(bst_t* tree) {
    if (tree->root) {
        bst_node_t* curr;
        curr = tree->root;
        while (curr->rhs) curr = curr->rhs;
        return curr;
    }
    else {
        return 0;
    }
}

/// Find the successor, return 0 if none.
bst_node_t* bst_successor(bst_node_t* curr) {
    if (curr->rhs) {
        curr = curr->rhs;
        while (curr->lhs) curr = curr->lhs;
        return curr;
    }
    else {
        while (curr->parent && curr->parent->rhs == curr) {
            curr = curr->parent;
        }
        return curr->parent;
    }
}

/// Find the precessor, return 0 if none.
bst_node_t* bst_precessor(bst_node_t* curr) {
    if (curr->lhs) {
        curr = curr->lhs;
        while (curr->rhs) curr = curr->rhs;
        return curr;
    }
    else {
        while (curr->parent && curr->parent->lhs == curr) {
            curr = curr->parent;
        }
        return curr->parent;
    }
}

/// Insert the data into the tree, return 0 if failed.
bst_node_t* bst_insert(bst_t* tree, const void* data) {
    bst_node_t* node;
    node = malloc(sizeof(bst_node_t) + tree->element_size);
    if (node) {
        memcpy(node->data, data, tree->element_size);
        node->parent = node->lhs = node->rhs = 0;
        if (tree->root) {
            bst_node_t* curr;
            curr = tree->root;
            while (1) {
                int ret;
                ret = tree->comparer(curr->data, node->data);
                if (ret > 0) {
                    if (curr->lhs) curr = curr->lhs;
                    else {
                        curr->lhs = node;
                        node->parent = curr;
                        break;
                    }
                }
                else {
                    if (curr->rhs) curr = curr->rhs;
                    else {
                        curr->rhs = node;
                        node->parent = curr;
                        break;
                    }
                }
            }
        }
        else {
            tree->root = node;
        }
    }
    return node;
}

void bst_transplant(bst_t* tree, bst_node_t* u, bst_node_t* v) {
    if (u->parent == 0) tree->root = v;
    else if (u == u->parent->lhs) u->parent->lhs = v;
    else u->parent->rhs = v;
    if (v) v->parent = u->parent;
}

void bst_delete(bst_t* tree, bst_node_t* node) {
    if (node->lhs == 0) {
        bst_transplant(tree, node, node->rhs);
    }
    else if (node->rhs == 0) {
        bst_transplant(tree, node, node->lhs);
    } else {
        bst_node_t* next;
        next = bst_successor(node);
        if (next != node->rhs) {
            bst_transplant(tree, next, next->rhs);
            next->rhs = node->rhs;
            node->rhs->parent = next;
        }
        bst_transplant(tree, node, next);
        next->lhs = node->lhs;
        node->lhs->parent = next;
    }
    free(node);
}