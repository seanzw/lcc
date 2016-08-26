#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
} 

typedef struct {
    int* p;
    int* r;
    int n;
} UF;

UF* uf_make(int n);

void uf_free(UF* uf);

int uf_find_set(UF* uf, int a);

void uf_union(UF* uf, int a, int b);

int main(int argc, char* argv[]) {
    UF* uf = uf_make(100);
    for (int i = 0; i < 100 - 1; i += 2) {
        uf_union(uf, i, i + 1);
        assert(uf_find_set(uf, i) == uf_find_set(uf, i + 1), "union i with i + 1");
    }
    for (int i = 0; i < 100 - 2; i += 3) {
        uf_union(uf, i, i + 2);
        assert(uf_find_set(uf, i + 1) == uf_find_set(uf, i + 2), "union i with i + 2");
    }

    printf("everything is fine!\n");

    return 0;
}