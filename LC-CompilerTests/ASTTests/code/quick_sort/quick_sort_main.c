#include <stdio.h>

void quick_sort(int* arr, int lhs, int rhs);

int main(int argc, char* argv[]) {

#define BIGSIZE 100
    int a[BIGSIZE];
    for (int i = 0; i < BIGSIZE; ++i) {
        a[i] = BIGSIZE - i;
    }
    quick_sort(a, 0, BIGSIZE - 1);
    for (int i = 0; i < BIGSIZE; ++i) {
        if (a[i] != i + 1) {
            printf("wrong answer for a[%d] = %d!\n", i, a[i]);
        }
    }
    printf("everyting is fine!\n");
    return 0;
}