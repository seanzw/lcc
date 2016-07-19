#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
}                                           

typedef int StackElementT;

typedef struct {
    StackElementT* arr;
    int top;
    int max;
} StackArray;

StackArray* stack_array_initialize(unsigned int max);

void stack_array_destroy(StackArray* stack);

int stack_array_empty(StackArray* stack);

int stack_array_push(StackArray* stack, StackElementT element);

int stack_array_pop(StackArray* stack, StackElementT* dst);

int main(int argc, char* argv[]) {
    int a[] = { 5, 4, 5, 3 };
    StackArray* stack = stack_array_initialize(4);
    assert(stack != 0, "init");
    assert(stack_array_empty(stack), "empty");
    assert(stack->max == 4, "max");
    for (int i = 0; i < 4; ++i) {
        assert(stack_array_push(stack, a[i]) == 0, "push");
    }

    assert(stack->top == stack->max, "full");
    for (int i = 3; i >= 0; --i) {
        int x;
        assert(stack_array_pop(stack, &x) == 0, "pop");
        assert(x == a[i], "pop_value");
    }

    stack_array_destroy(stack);

    printf("everything is fine!\n");

    return 0;
}