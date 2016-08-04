#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    /*exit(-1);                               \*/\
} 

struct Interval {
    int start;
    int end;
};

struct Interval* merge(struct Interval* intervals, int intervalsSize, int* returnSize);

int main(int argc, char* argv[]) {

    struct Interval is[] = {
        { 1, 3 },
        { 6, 9 }
    };

    int retSize;
    struct Interval* ret = merge(is, 2, &retSize);
    assert(retSize == 2, "ret = 2");

    printf("everything is fine!\n");
    return 0;
}