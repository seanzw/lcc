int climbStairs(int n) {
    int i;
    int fib_i, fib_i_1;
    fib_i = 1;
    fib_i_1 = 0;
    for (i = 0; i < n; ++i) {
        int t;
        t = fib_i;
        fib_i += fib_i_1;
        fib_i_1 = t;
    }
    return fib_i;
}