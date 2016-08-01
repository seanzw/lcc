#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
}

int close(double x, double y) {
    if (x > y) return x - y < 0.000001;
    else return y - x < 0.000001;
}

double findMedianSortedArrays(int* nums1, int nums1Size, int* nums2, int nums2Size);

int main(int argc, char* argv[]) {
    int n1[] = { 1, 3 };
    int n2[] = { 2 };
    int n3[] = { 1, 2 };
    int n4[] = { 3, 4 };
    printf("%lf\n", findMedianSortedArrays(n1, 2, n2, 1));
    assert(close(findMedianSortedArrays(n1, 2, n2, 1), 2.0), "test1");
    printf("%lf\n", findMedianSortedArrays(n3, 2, n4, 2));
    assert(close(findMedianSortedArrays(n3, 2, n4, 2), 2.5), "test2");
    printf("everything is fine!\n");
    return 0;
}