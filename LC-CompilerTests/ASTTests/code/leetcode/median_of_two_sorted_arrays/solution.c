int printf(const char* format, ...);

static int kth(int* a, int sa, int* b, int sb, int k) {
    int xa, xb;
    xa = 0;
    xb = 0;
    while (k > 1) {
        if (xa < sa && xb < sb) {
            int ia, ib;
            ia = xa + k / 2 - 1;
            ib = xb + k / 2 - 1;
            ia = ia >= sa ? sa - 1 : ia;
            ib = ib >= sb ? sb - 1 : ib;
            if (a[ia] > b[ib]) {
                k -= ib - xb + 1;
                xb = ib + 1;
            }
            else {
                k -= ia - xa + 1;
                xa = ia + 1;
            }
        }
        else if (xa < sa) {
            return a[xa + k - 1];
        }
        else {
            return b[xb + k - 1];
        }
    }
    /// k = 1.
    //printf("xa = %d, sa = %d, xb = %d, sb = %d\n", xa, sa, xb, sb);
    //printf("xa < sa && xb < sb = %d\n", xa < sa && xb < sb);
    if (xa < sa && xb < sb) return a[xa] > b[xb] ? b[xb] : a[xa];
    else if (xa < sa) return a[xa];
    else return b[xb];
}

double findMedianSortedArrays(int* nums1, int nums1Size, int* nums2, int nums2Size) {
    //printf("(nums1Size + nums2Size) %% 2 = %d", (nums1Size + nums2Size) % 2);
    //printf("(nums1Size + nums2Size) %% 2 == 0 = %d", (nums1Size + nums2Size) % 2 == 0);
    if ((nums1Size + nums2Size) % 2 == 0) {
        int m1, m2;
        m1 = kth(nums1, nums1Size, nums2, nums2Size, (nums1Size + nums2Size) / 2);
        m2 = kth(nums1, nums1Size, nums2, nums2Size, (nums1Size + nums2Size) / 2 + 1);
        return (m1 + m2) / 2.0;
        //return (double)(kth(nums1, nums1Size, nums2, nums2Size, (nums1Size + nums2Size) / 2) +
            //kth(nums1, nums1Size, nums2, nums2Size, (nums1Size + nums2Size) / 2 + 1)) / 2.0;
    }
    else {
        return kth(nums1, nums1Size, nums2, nums2Size, (nums1Size + nums2Size) / 2 + 1);
    }
}