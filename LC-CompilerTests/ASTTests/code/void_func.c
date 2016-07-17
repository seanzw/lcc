struct s {
    int a;
    char b;
};

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
