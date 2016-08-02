
typedef unsigned int size_t;

int printf(const char*, ...);
void* malloc(unsigned int size);
void free(void* ptr);
size_t strlen(const char*);

static void print_stack(int* stack, int n) {
    int i;
    for (i = 0; i < n; ++i) {
        printf("s[%d] = %d, ", i, stack[i]);
    }
    printf("\n");
}

int longestValidParentheses(char* s) {
    int* stack;
    int stack_cnt;  // number of element in the stack.
    int idx, ret, run;
    int s_len;
    
    s_len = strlen(s);
    if (s_len == 0) return 0;

    stack = malloc(sizeof(int) * (s_len + 1));
    stack[0] = -1;
    stack_cnt = 1;
    for (idx = 0, ret = 0; idx < s_len; ++idx) {
        //print_stack(stack, stack_cnt);
        if (s[idx] == '(') {
            stack[stack_cnt++] = idx;
        }
        else {
            //printf("idx = %d\n", idx);
            if (stack_cnt > 1) {
                /// The stack is not empty.
                run = idx - stack[(stack_cnt--) - 2];
                if (run > ret) ret = run;
            }
            else {
                /// The stack is empty.
                /// This is invalid situation.
                /// Update stack[0].
                /// Therefore the run is [stack[0] + 1, idx - 1].
                //printf("idx = %d, run = %d, ret = %d\n", idx, run, ret);
                stack[0] = idx;
            }
        }
    }
    free(stack);
    return ret;
}