#include <stdio.h>

void heap_sort(int* arr, int len);

int main(int argc, char* argv[]) {

    int a0[] = { 1 };
    heap_sort(a0, 0);
    if (a0[0] != 1) printf("wrong answer!\n");
    heap_sort(a0, 1);
    if (a0[0] != 1) printf("wrong answer!\n");
    
#define BIGSIZE 100
    int a[BIGSIZE];
    for (int i = 0; i < BIGSIZE; ++i) {
        a[i] = BIGSIZE - i;
    }
    heap_sort(a, BIGSIZE);
    for (int i = 0; i < BIGSIZE; ++i) {
        if (a[i] != i + 1) {
            printf("wrong answer for a[%d] = %d!\n", i, a[i]);
        }
    }
    printf("everyting is fine!\n");
    return 0;
}