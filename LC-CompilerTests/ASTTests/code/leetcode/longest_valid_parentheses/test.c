#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

int longestValidParentheses(char* s);

int main(int argc, char* argv[]) {
    assert(longestValidParentheses("") == 0, "boundary 0");
    assert(longestValidParentheses("(") == 0, "boundary 1 (");
    assert(longestValidParentheses(")") == 0, ")");
    assert(longestValidParentheses("()") == 2, "()");
    assert(longestValidParentheses("))))") == 0, "))))");
    assert(longestValidParentheses("()()") == 4, "()()");
    assert(longestValidParentheses("())(") == 2, "())(");
    assert(longestValidParentheses("((())") == 4, "((())");
    assert(longestValidParentheses("()(((())))") == 10, "()(((())))");
    assert(longestValidParentheses(")()(((())))") == 10, ")()(((())))");
    assert(longestValidParentheses("()(((())))(") == 10, "()(((())))(");
    assert(longestValidParentheses(")()(((())))(") == 10, ")()(((())))(");
    assert(longestValidParentheses("(())()(()((") == 6, "(())()(()((");
    printf("everything is fine!\n");
    return 0;
}