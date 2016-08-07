#include <stdio.h>

int siint(int x);

enum a {
    I
};
enum b {
    J
};

enum a sienum(enum a x);

#define test(func, x) printf(#func "(%d) = %d\n", x, func(x))

int main(int argc, char* argv[]) {
    int i0 = 0;
    enum a enuma = I;
    test(siint, i0);
    test(sienum, enuma);
    return 0;
}