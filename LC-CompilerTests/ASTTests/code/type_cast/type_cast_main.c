#include <stdio.h>

signed char uchar2schar(unsigned char x);
int uint2int(unsigned int x);

#define test(func, x) printf(#func "(%d) = %d\n", x, func(x))

int main(int argc, char* argv[]) {
    unsigned char uc128 = 128u;
    double d100 = 100;
    test(uchar2schar, uc128);
    test(int2double, d100);
    return 0;
}