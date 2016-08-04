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

struct Interval* insert(struct Interval* intervals, int intervalsSize, struct Interval newInterval, int* returnSize);

int main(int argc, char* argv[]) {

    struct Interval is[] = {
        { 1, 3 },
        { 6, 9 }
    };

    struct Interval x = { 0, 0 };

    int retSize;
    struct Interval* ret = insert(is, 2, x, &retSize);
    assert(retSize == 3, "ret = 3");

    printf("everything is fine!\n");
    return 0;
}