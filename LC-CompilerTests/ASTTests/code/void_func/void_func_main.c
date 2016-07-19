#include <stdio.h>

int foo(int x);
int func1(int argc, char* argv[]);
int test_if(int x);
int test_for(int x);
int test_mul(int x);
int test_div(int x);
int test_log_and(int x);
int sum(int x);

int main(int argc, char* argv[]) {
    for (int i = 0; i < 10; ++i) {
        printf("func1(%d) = %d\n", i, func1(i, 0));
    }
    for (int i = -4; i < 5; ++i) {
        printf("test_if(%d) = %d\n", i, test_if(i));
    }
    printf("test_for(%d) = %d\n", 10, test_for(10));
    printf("sum(%d) = %d\n", 10, sum(10));
    printf("test_mul(%d) = %d\n", 10, test_mul(10));
    printf("test_div(%d) = %d\n", 1, test_div(1));
    printf("test_log_and(%d) = %d\n", 1, test_log_and(1));
    printf("foo(%d) = %d\n", 2, foo(2));
    return 0;
}
