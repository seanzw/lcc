typedef unsigned int size_t;

void* malloc(size_t size);
void* memcpy(void*, const void*, size_t);
void free(void* ptr);

int printf(const char* format, ...);

typedef enum {
    WHITE,
    GRAY,
    BLACK
} Color;

typedef struct {
    int x;
    Color color;
} Vertex;

typedef struct AdjNode {
    int v;
    struct AdjNode* next;
} AdjNode;

typedef struct {
    int nv;
    Vertex* vs;
    AdjNode** adjs;
} Graph;

/// Create a graph with 0 edges.
Graph* g_init(const Vertex* vs, int nv) {
    Graph* g = malloc(sizeof(Graph));
    if (g == 0) return 0;
    g->nv = nv;
    g->vs = malloc(sizeof(Vertex) * nv);
    if (g->vs == 0) {
        free(g);
        return 0;
    }
    memcpy(g->vs, vs, nv * sizeof(Vertex));
    g->adjs = malloc(sizeof(AdjNode*) * nv);
    if (g->adjs == 0) {
        free(g->vs);
        free(g);
        return 0;
    }
    for (int i = 0; i < nv; ++i) {
        g->adjs[i] = 0;
    }
    return g;
}

void g_free(Graph* g) {
    if (g) {
        for (int i = 0; i < g->nv; ++i) {
            AdjNode* edge = g->adjs[i];
            while (edge) {
                AdjNode* next = edge->next;
                free(edge);
                edge = next;
            }
        }
        free(g->adjs);
        free(g->vs);
        free(g);
    }
}

/// Add an edge, we do not check if there is already an edge from i to j
void g_add_edge(Graph* g, int i, int j) {
    AdjNode* edge = malloc(sizeof(AdjNode));
    edge->v = j;
    edge->next = g->adjs[i];
    g->adjs[i] = edge;
}

Graph* g_transpose(Graph* g) {
    Graph* gt = g_init(g->vs, g->nv);
    for (int i = 0; i < g->nv; ++i) {
        AdjNode* edge = g->adjs[i];
        while (edge) {
            g_add_edge(gt, edge->v, i);
            edge = edge->next;
        }
    }
    return gt;
}

typedef struct TPSortRet {
    int v;
    struct TPSortRet* next;
} TPSortRet;

void stack_push(int** stack, int* capacity, int* stack_top, int data) {
    if (*stack_top == *capacity) {
        int* new_stack = malloc(sizeof(int) * 2 * *capacity);
        memcpy(new_stack, *stack, sizeof(int) * *capacity);
        free(*stack);
        *stack = new_stack;
        *capacity *= 2;
    }
    (*stack)[*stack_top] = data;
    (*stack_top)++;
}

TPSortRet* g_topological_sort(Graph* g) {

    TPSortRet* ret = 0;

    int* stack = malloc(sizeof(int) * 2);
    int stack_top = 0;
    int capacity = 2;

    /// Set all the color to be white.
    for (int i = 0; i < g->nv; ++i) {
        g->vs[i].color = WHITE;
    }

    for (int i = 0; i < g->nv; ++i) {
        if (g->vs[i].color == WHITE) {
            // Find a new start point.
            stack_push(&stack, &capacity, &stack_top, i);

            // Start DFS.
            while (stack_top != 0) {
                int v = stack[stack_top - 1];

                if (g->vs[v].color == BLACK) {
                    // v has finished, simply pop out.
                    stack_top--;
                } else if (g->vs[v].color == GRAY) {
                    // v has finished, push to ret list.
                    g->vs[v].color = BLACK;
                    stack_top--;
                    TPSortRet* node = malloc(sizeof(TPSortRet));
                    node->v = v;
                    node->next = ret;
                    ret = node;
                } else {
                    g->vs[v].color = GRAY;
                    // DFS.
                    AdjNode* edge = g->adjs[v];
                    while (edge) {
                        if (g->vs[edge->v].color == WHITE) {
                            stack_push(&stack, &capacity, &stack_top, edge->v);
                        }
                        edge = edge->next;
                    }
                }
            }
        }
    }

    free(stack);
    return ret;
}

typedef struct SCComponentRet {
    TPSortRet* component;
    struct SCComponentRet* next;
} SCComponentRet;

static int partition(void* arr, size_t size, int lhs, int rhs, int (*f)(void*, void*)) {
    int i, j;
    void* x = arr + rhs * size;
    void* t = malloc(sizeof(size));
    i = lhs - 1;
    for (j = lhs; j <= rhs - 1; j++) {
        if (f(arr + j * size, x) <= 0) {
            i++;
            memcpy(t, arr + i * size, size);
            memcpy(arr + i * size, arr + j * size, size);
            memcpy(arr + j * size, t, size);
        }
    }
    memcpy(t, arr + (i + 1) * size, size);
    memcpy(arr + (i + 1) * size, arr + rhs * size, size);
    memcpy(arr + rhs * size, t, size);
    free(t);
    return i + 1;
}

/// SORT ARR[LHS, RHS].
static void quick_sort(void* arr, size_t size, int lhs, int rhs, int (*f)(void*, void*)) {
    if (lhs < rhs) {
        int mid;
        mid = partition(arr, size, lhs, rhs, f);
        quick_sort(arr, size, lhs, mid - 1, f);
        quick_sort(arr, size, mid + 1, rhs, f);
    }
}

typedef struct {
    int v;
    int finishTime;
} F;

static int FCompare(void* a, void* b) {
    return ((F*)b)->finishTime - ((F*)a)->finishTime;
}

SCComponentRet* g_strongly_connected_components(Graph* g) {

    SCComponentRet* ret = 0;

    /// Initial the time.
    int time = 0;

    /// Array to record the finish time;

    F* fs = malloc(sizeof(F) * g->nv);
    for (int i = 0; i < g->nv; ++i) {
        fs[i].v = i;
    }

    /// First time DFS.
    int* stack = malloc(sizeof(int) * 2);
    int stack_top = 0;
    int stack_capacity = 2;

    for (int i = 0; i < g->nv; ++i) {
        g->vs[i].color = WHITE;
    }

    for (int i = 0; i < g->nv; ++i) {
        if (g->vs[i].color == WHITE) {
            /// Found a new start point.
            stack_push(&stack, &stack_capacity, &stack_top, i);

            while (stack_top != 0) {
                time++;
                int v = stack[stack_top - 1];
                if (g->vs[v].color == BLACK) {
                    /// v has finished, pop out.
                    stack_top--;
                }
                else if (g->vs[v].color == GRAY) {
                    /// v has really finished, pop out.
                    g->vs[v].color = BLACK;
                    stack_top--;
                    /// Add to the finish time.
                    fs[v].finishTime = time;
                }
                else {
                    /// v has not been visited.
                    g->vs[v].color = GRAY;
                    AdjNode* edge = g->adjs[v];
                    while (edge) {
                        if (g->vs[edge->v].color == WHITE) {
                            stack_push(&stack, &stack_capacity, &stack_top, edge->v);
                        }
                        edge = edge->next;
                    }
                }
            }
        }
    }

    // Finish the first DFS.
    // Sort fs.
    quick_sort(fs, sizeof(F), 0, g->nv - 1, FCompare);

    // Create g_transpose.
    Graph* transpose = g_transpose(g);

    for (int i = 0; i < g->nv; ++i) {
        transpose->vs[i].color = WHITE;
    }

    // Second DFS.
    for (int i = 0; i < g->nv; ++i) {
        int v = fs[i].v;
        if (transpose->vs[v].color == WHITE) {
            /// Found a new connected component;
            TPSortRet* cc = 0;
            stack_push(&stack, &stack_capacity, &stack_top, v);

            while (stack_top != 0) {
                int u = stack[stack_top - 1];

                if (transpose->vs[u].color == BLACK) {
                    stack_top--;
                }
                else if (transpose->vs[u].color == GRAY) {
                    // Finish.
                    TPSortRet* x = malloc(sizeof(TPSortRet));
                    x->v = u;
                    x->next = cc;
                    cc = x;
                    transpose->vs[u].color = BLACK;
                    stack_top--;
                }
                else {
                    transpose->vs[u].color = GRAY;
                    AdjNode* edge = transpose->adjs[u];
                    while (edge) {
                        if (transpose->vs[edge->v].color == WHITE) {
                            stack_push(&stack, &stack_capacity, &stack_top, edge->v);
                        }
                        edge = edge->next;
                    }
                }
            }

            SCComponentRet* scc = malloc(sizeof(SCComponentRet));
            scc->component = cc;
            scc->next = ret;
            ret = scc;
        }
    }

    // Clear all the malloc data.
    free(fs);
    free(stack);
    g_free(transpose);

    return ret;
}
