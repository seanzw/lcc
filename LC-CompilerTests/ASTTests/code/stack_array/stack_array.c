typedef unsigned int size_t;

void* malloc(size_t size);
void free(void* ptr);

typedef int StackElementT;

typedef struct {
    StackElementT* arr;
    int top;
    int max;
} StackArray;

StackArray* stack_array_initialize(unsigned int max) {
    StackArray* stack;
    stack = malloc(sizeof(StackArray));
    if (stack != 0) {
        stack->arr = malloc(sizeof(StackElementT) * max);
        if (stack->arr != 0) {
            stack->top = 0;
            stack->max = max;
            return stack;
        }
    }
    return 0;
}

void stack_array_destroy(StackArray* stack) {
    if (stack != 0) {
        free(stack->arr);
        free(stack);
    }
}

int stack_array_empty(StackArray* stack) {
    return stack->top == 0;
}

int stack_array_push(StackArray* stack, StackElementT element) {
    if (stack->top < stack->max) {
        stack->arr[stack->top++] = element;
        return 0;
    }
    return -1;
}

int stack_array_pop(StackArray* stack, StackElementT* dst) {
    if (stack_array_empty(stack)) return -1;
    *dst = stack->arr[--stack->top];
    return 0;
}