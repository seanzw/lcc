#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

int candy(int* ratings, int ratingsSize);

int main(int argc, char* argv[]) {
    int h1[] = { 9 };
    int h2[] = { 8, 7, 8 };
    int x;
    assert(candy(h1, 1) == 1, "candy 1");
    assert(candy(h2, 2) == 3, "candy 2");
    assert(candy(h2, 3) == 5, "candy 3");
    printf("everything is fine!\n");
    return 0;
}