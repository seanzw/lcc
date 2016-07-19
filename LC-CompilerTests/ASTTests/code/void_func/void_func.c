struct s {
    int a;
    char b;
};

int foo(int a) {
    a += 1;
    ++a;
    return a;
}

int test_if(int x) {
    if (x <= 0) {
        if (x <= 2) {
            return x + 1;
        }
        return x;
    }
    else {
        return x - 4;
    }
}

int test_for(int x) {
    int i, sum;
    sum = 0;
    for (i = 0; i < x; i++) {
        sum = sum + x;
    }
    return sum;
}

int sum(int x) {
    if (x <= 0) return 0;
    else return x + sum(x - 1);
}

int func1(int argc, char* argv[]) {
    int i, j;
    i = argc - 4;
    j = i++;
    j = i <= 5;
    return j;
}

int test_mul(int x) {
    int j;
    j = x * 5;
    return j;
}

int test_div(int x) {
    int j;
    j = x / 2;
    return j;
}

int test_log_and(int x) {
    if (x > 0 && x < 5) {
        return 1;
    }
    else {
        return 0;
    }
}
