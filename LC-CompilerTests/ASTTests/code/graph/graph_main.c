#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
}

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

/// Create a graph with 0 edgesa.
Graph* g_init(const Vertex* vs, int nv);

void g_free(Graph* g);

/// Add an edge, we do not check if there is already an edge from i to j
void g_add_edge(Graph* g, int i, int j);

typedef struct TPSortRet {
    int v;
    struct TPSortRet* next;
} TPSortRet;

TPSortRet* g_topological_sort(Graph* g);

int main(int argc, char* argv[]) {
    Vertex vs[14];
    for (int i = 0; i < 14; ++i) {
        vs[i].x = i;
    }
    Graph* g = g_init(vs, 14);
    g_add_edge(g, 0, 4);
    g_add_edge(g, 0, 5);
    g_add_edge(g, 0, 11);
    g_add_edge(g, 1, 2);
    g_add_edge(g, 1, 4);
    g_add_edge(g, 1, 8);
    g_add_edge(g, 2, 5);
    g_add_edge(g, 2, 6);
    g_add_edge(g, 3, 2);
    g_add_edge(g, 3, 6);
    g_add_edge(g, 3, 13);
    g_add_edge(g, 4, 7);
    g_add_edge(g, 5, 8);
    g_add_edge(g, 5, 12);
    g_add_edge(g, 6, 5);
    g_add_edge(g, 8, 7);
    g_add_edge(g, 9, 10);
    g_add_edge(g, 9, 11);
    g_add_edge(g, 10, 13);
    g_add_edge(g, 12, 9);

    TPSortRet* tp_sort = g_topological_sort(g);

    TPSortRet* iter = tp_sort;
    assert(iter->v == 3, "3");
    iter = iter->next;
    assert(iter->v == 1, "1");
    iter = iter->next;
    assert(iter->v == 2, "2");
    iter = iter->next;
    assert(iter->v == 6, "6");
    iter = iter->next;
    assert(iter->v == 0, "0");
    iter = iter->next;
    assert(iter->v == 5, "5");
    iter = iter->next;
    assert(iter->v == 12, "12");
    iter = iter->next;
    assert(iter->v == 9, "9");
    iter = iter->next;
    assert(iter->v == 11, "11");
    iter = iter->next;
    assert(iter->v == 10, "10");
    iter = iter->next;
    assert(iter->v == 13, "13");
    iter = iter->next;
    assert(iter->v == 8, "8");
    iter = iter->next;
    assert(iter->v == 4, "4");
    iter = iter->next;
    assert(iter->v == 7, "7");
    iter = iter->next;
    assert(iter == 0, "end");

    printf("everything is fine!\n");
    return 0;
}
