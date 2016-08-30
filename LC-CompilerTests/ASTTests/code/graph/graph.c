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
