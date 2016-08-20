#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
} 

struct TreeLinkNode {
    int val;
    struct TreeLinkNode *left, *right, *next;
};

void connect(struct TreeLinkNode *root);

int main(int argc, char* argv[]) {
    connect(0);
    struct TreeLinkNode n1, n2;
    n1.left = &n2;
    n1.right = 0;
    n2.left = n2.right = 0;
    connect(&n1);
    assert(n1.next == 0, "root next is 0");
    assert(n2.next == 0, "n2 next is 0");
    printf("everything is fine!\n");
    return 0;
}