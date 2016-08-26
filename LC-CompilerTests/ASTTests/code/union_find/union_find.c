typedef unsigned int size_t;

void* malloc(size_t size);
void free(void* ptr);

int printf(const char* format, ...);

typedef struct {
    int* p;
    int* r;
    int n;
} UF;

UF* uf_make(int n) {
    UF* uf = malloc(sizeof(UF));
    if (uf == 0) return 0;
    uf->n = n;
    uf->p = malloc(sizeof(int) * n);
    if (uf->p == 0) return 0;
    uf->r = malloc(sizeof(int) * n);
    if (uf->r == 0) return 0;
    for (int i = 0; i < n; ++i) {
        uf->p[i] = i;
        uf->r[i] = 0;
    }
    return uf;
}

void uf_free(UF* uf) {
    if (uf) {
        free(uf->p);
        free(uf->r);
        free(uf);
    }
}

int uf_find_set(UF* uf, int a) {
    if (uf->p[a] == a) return a;
    else return uf->p[a] = uf_find_set(uf, uf->p[a]);
}

void uf_link(UF* uf, int a, int b) {
    if (uf->r[a] < uf->r[b]) {
        uf->p[a] = b;
    }
    else if (uf->r[a] > uf->r[b]) {
        uf->p[b] = a;
    }
    else {
        uf->p[b] = a;
        uf->r[b]++;
    }
}

void uf_union(UF* uf, int a, int b) {
    uf_link(uf, uf_find_set(uf, a), uf_find_set(uf, b));
}



