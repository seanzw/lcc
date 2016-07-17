#include <stdio.h>

void quick_sort(int* arr, int lhs, int rhs);

int main(int argc, char* argv[]) {
    int a1[] = { 3, 2, 1 };
    int a2[] = { 3 };
    int a3[] = { 1, 2, 3 };

#define BIGSIZE 100
    int a4[BIGSIZE];
    for (int i = 0; i < BIGSIZE; ++i) {
        a4[i] = BIGSIZE - i;
    }
    quick_sort(a4, 0, BIGSIZE - 1);
    for (int i = 0; i < BIGSIZE; ++i) {
        if (a4[i] != i + 1) {
            printf("wrong answer for a4[%d] = %d!\n", i, a4[i]);
        }
    }
    printf("everyting is fine!\n");
    return 0;
}