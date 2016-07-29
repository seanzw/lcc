typedef unsigned int size_t;
void* malloc(size_t size);
void free(void* ptr);
void* memcpy(void* dst, const void* src, size_t size);

typedef enum rbt_color_t {
    RED,
    BLACK
} rbt_color_t;

typedef struct rbt_node_t {
    enum rbt_color_t color;
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


int printf(const char* format, ...);

static void rbt_print_aux(rbt_t* tree, void(*print)(const rbt_node_t*), rbt_node_t* node, int level) {
    if (node != tree->nil) {
        int i;
        i = 0;
        while (i < level) {
            printf("  ");
            i++;
        }
        print(node);
        printf("\n");
        rbt_print_aux(tree, print, node->lhs, level + 1);
        rbt_print_aux(tree, print, node->rhs, level + 1);
    }
}

void rbt_print(rbt_t* tree, void(*print)(const rbt_node_t*)) {
    rbt_print_aux(tree, print, tree->root, 0);
}

rbt_t* rbt_init(size_t element_size, int(*comparer)(const void*, const void*)) {
    rbt_t* tree;
    tree = malloc(sizeof(rbt_t));
    if (tree) {
        tree->element_size = element_size;
        tree->comparer = comparer;
        tree->nil = malloc(sizeof(rbt_node_t) + element_size);
        if (tree->nil) {
            tree->nil->color = BLACK;
            tree->nil->parent = 0;
            tree->nil->lhs = tree->nil->rhs = 0;
            tree->root = tree->nil;
        }
        else {
            free(tree);
            tree = 0;
        }
    }
    return tree;
}

static void rbt_free_aux(rbt_t* tree, rbt_node_t* node) {
    if (node != tree->nil) {
        rbt_free_aux(tree, node->lhs);
        rbt_free_aux(tree, node->rhs);
        free(node);
    }
}

void rbt_free(rbt_t* tree) {
    rbt_free_aux(tree, tree->root);
    free(tree->nil);
    free(tree);
}

/// returns nil if nof found.
rbt_node_t* rbt_search(rbt_t* tree, const void* data) {
    rbt_node_t* iter;
    iter = tree->root;
    while (iter != tree->nil) {
        int ret;
        ret = tree->comparer(data, iter->data);
        if (ret == 0) return iter;
        else if (ret < 0) iter = iter->lhs;
        else iter = iter->rhs;
    }
    return tree->nil;
}

rbt_node_t* rbt_minimum(rbt_t* tree) {
    rbt_node_t* iter;
    iter = tree->root;
    while (iter != tree->nil && iter->lhs != tree->nil) iter = iter->lhs;
    return iter;
}

rbt_node_t* rbt_maximum(rbt_t* tree) {
    rbt_node_t* iter;
    iter = tree->root;
    while (iter != tree->nil && iter->rhs != tree->nil) iter = iter->rhs;
    return iter;
}

rbt_node_t* rbt_successor(rbt_t* tree, rbt_node_t* curr) {
    if (curr->rhs != tree->nil) {
        curr = curr->rhs;
        while (curr->lhs != tree->nil) curr = curr->lhs;
    }
    else {
        while (curr->parent != tree->nil && curr->parent->rhs == curr) curr = curr->parent;
        curr = curr->parent;
    }
    return curr;
}

rbt_node_t* rbt_precessor(rbt_t* tree, rbt_node_t* curr) {
    if (curr->lhs != tree->nil) {
        curr = curr->lhs;
        while (curr->rhs != tree->nil) curr = curr->rhs;
    }
    else {
        while (curr->parent != tree->nil && curr->parent->lhs == curr) curr = curr->parent;
        curr = curr->parent;
    }
    return curr;
}

/// node->rhs should not be nil.
static void rbt_rotate_left(rbt_t* tree, rbt_node_t* node) {
    rbt_node_t* rhs;
    rhs = node->rhs;
    node->rhs = rhs->lhs;
    if (rhs->lhs != tree->nil) {
        rhs->lhs->parent = node;
    }
    rhs->parent = node->parent;
    if (node->parent == tree->nil) {
        tree->root = rhs;
    }
    else if (node == node->parent->lhs) {
        node->parent->lhs = rhs;
    } 
    else {
        node->parent->rhs = rhs;
    }
    node->parent = rhs;
    rhs->lhs = node;
}

static void rbt_rotate_right(rbt_t* tree, rbt_node_t* node) {
    rbt_node_t* lhs;
    lhs = node->lhs;
    node->lhs = lhs->rhs;
    if (lhs->rhs != tree->nil) lhs->rhs->parent = node;
    lhs->rhs = node;
    lhs->parent = node->parent;
    if (node->parent != tree->nil) {
        if (node == node->parent->lhs) node->parent->lhs = lhs;
        else node->parent->rhs = lhs;
    }
    node->parent = lhs;
    if (node == tree->root) {
        tree->root = lhs;
    }
}

static void rbt_insert_fix(rbt_t* tree, rbt_node_t* node) {
    rbt_node_t* grand_parent;
    rbt_node_t* uncle;
    while (node->parent->color == RED) {
        grand_parent = node->parent->parent;
        if (node->parent == grand_parent->lhs) {
            uncle = grand_parent->rhs;
            if (uncle->color == RED) {
                node->parent->color = uncle->color = BLACK;
                grand_parent->color = RED;
                node = grand_parent;
            }
            else {
                if (node == node->parent->rhs) {
                    rbt_rotate_left(tree, node->parent);
                    node = node->lhs;
                }
                node->parent->color = BLACK;
                grand_parent->color = RED;
                rbt_rotate_right(tree, grand_parent);
            }
        }
        else {
            uncle = grand_parent->lhs;
            if (uncle->color == RED) {
                node->parent->color = uncle->color = BLACK;
                grand_parent->color = RED;
                node = grand_parent;
            }
            else {
                if (node == node->parent->lhs) {
                    rbt_rotate_right(tree, node->parent);
                    node = node->rhs;
                }
                node->parent->color = BLACK;
                grand_parent->color = RED;
                rbt_rotate_left(tree, grand_parent);
            }
        }
    }
    tree->root->color = BLACK;
}

rbt_node_t* rbt_insert(rbt_t* tree, const void* data) {
    rbt_node_t* node;
    node = malloc(sizeof(rbt_node_t) + tree->element_size);
    if (node) {
        memcpy(node->data, data, tree->element_size);
        node->parent = tree->nil;
        node->lhs = node->rhs = tree->nil;
        node->color = RED;
        rbt_node_t* curr;
        rbt_node_t* prev;
        curr = tree->root;
        prev = tree->nil;
        int ret;
        while (curr != tree->nil) {
            prev = curr;
            ret = tree->comparer(node->data, curr->data);
            if (ret < 0) curr = curr->lhs;
            else curr = curr->rhs;
        }
        node->parent = prev;
        if (tree->root == tree->nil) {
            tree->root = node;
        }
        else {
            if (ret < 0) prev->lhs = node;
            else prev->rhs = node;
        }

        // fix.
        //void print(const rbt_node_t*);
        //rbt_print(tree, print);
        rbt_insert_fix(tree, node);
    }
    return node;
}

static void rbt_transplant(rbt_t* tree, rbt_node_t* x, rbt_node_t* y) {
    y->parent = x->parent;
    if (x->parent != tree->nil) {
        if (x == x->parent->lhs) x->parent->lhs = y;
        else x->parent->rhs = y;
    }
    else {
        tree->root = y;
    }
}

static void rbt_delete_fix(rbt_t* tree, rbt_node_t* x) {
    while (x != tree->root && x->color == BLACK) {
        rbt_node_t* brother;
        if (x == x->parent->lhs) {
            brother = x->parent->rhs;
            if (brother->color == RED) {
                brother->color = BLACK;
                x->parent->color = RED;
                rbt_rotate_left(tree, x->parent);
                brother = x->parent->rhs;
            }
            if (brother->rhs->color == BLACK && brother->lhs->color == BLACK) {
                brother->color = RED;
                x = x->parent;
            }
            else {
                if (brother->rhs->color == BLACK) {
                    brother->lhs->color = BLACK;
                    brother->color = RED;
                    rbt_rotate_right(tree, brother);
                    brother = brother->parent;
                }
                brother->color = x->parent->color;
                brother->rhs->color = BLACK;
                x->parent->color = BLACK;
                rbt_rotate_left(tree, x->parent);
                x = tree->root;
            }
        }
        else {
            brother = x->parent->lhs;
            if (brother->color == RED) {
                brother->color = BLACK;
                x->parent->color = RED;
                rbt_rotate_right(tree, x->parent);
                brother = x->parent->lhs;
            }
            if (brother->lhs->color == BLACK && brother->rhs->color == BLACK) {
                brother->color = RED;
                x = x->parent;
            }
            else {
                if (brother->lhs->color == BLACK) {
                    brother->rhs->color = BLACK;
                    brother->color = RED;
                    rbt_rotate_left(tree, brother);
                    brother = brother->parent;
                }
                brother->color = x->parent->color;
                brother->lhs->color = BLACK;
                x->parent->color = BLACK;
                rbt_rotate_right(tree, x->parent);
                x = tree->root;
            }
        }
    }
    x->color = BLACK;
}

void rbt_delete(rbt_t* tree, rbt_node_t* node) {
    rbt_node_t* y;
    rbt_node_t* z;
    rbt_color_t y_original_color;
    y = node;
    y_original_color = y->color;
    if (node->lhs == tree->nil) {
        z = node->rhs;
        rbt_transplant(tree, node, z);
    }
    else if (node->rhs == tree->nil) {
        z = node->lhs;
        rbt_transplant(tree, node, z);
    }
    else {
        y = rbt_successor(tree, node);
        y_original_color = y->color;
        z = y->rhs;
        if (y->parent == node) {
            z->parent = y;
        }
        else {
            rbt_transplant(tree, y, z);
            y->rhs = node->rhs;
            node->rhs->parent = y;
        }
        rbt_transplant(tree, node, y);
        y->lhs = node->lhs;
        y->lhs->parent = y;
        y->color = node->color;
    }
    if (y_original_color == BLACK) {
        rbt_delete_fix(tree, z);
    }
    free(node);

}


static int rbt_assert_aux(rbt_t* tree, rbt_node_t* node) {
    void print(const rbt_node_t*);
    if (node != tree->nil) {
        if (node->color == RED) {
            if (node->lhs->color == RED || node->rhs->color == RED) {
                //printf("-4\n");
                //rbt_print(tree, print);
                return -4;
            }
        }
        int bh_lhs, bh_rhs;
        bh_lhs = rbt_assert_aux(tree, node->lhs);
        bh_rhs = rbt_assert_aux(tree, node->rhs);
        if (bh_lhs < 0 || bh_rhs < 0) return bh_lhs;
        if (bh_lhs != bh_rhs) return -5;
        if (node->color == RED) return bh_lhs;
        else return bh_lhs + 1;
    }
    else {
        if (node->color != BLACK) return -3;
        else return 1;
    }
}

int rbt_assert(rbt_t* tree) {
    if (tree->root->color != BLACK) return -2;
    return rbt_assert_aux(tree, tree->root);
}