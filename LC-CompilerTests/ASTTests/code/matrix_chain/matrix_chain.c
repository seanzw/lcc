typedef unsigned int size_t;

void* malloc(size_t size);
void free(void* ptr);

int printf(const char* format, ...);

static void print_solution(int** s, int i, int j) {
    if (s[i][j] == -1) printf("%c", 'A' + i);
    else {
        printf("(");
        print_solution(s, i, s[i][j] - 1);
        printf("x");
        print_solution(s, s[i][j], j);
        printf(")");
    }
}

static void print_s(int **s, int n) {
    int i;
    for (i = 0; i < n; ++i) {
        int j;
        for (j = i; j < n; ++j) {
            printf("%d ", s[i][j]);
        }
        printf("\n");
    }
    printf("\n");
}

/// Solve and print the matrix chain association problem.
/// p: the dimention of each matrix
/// n: the number of matrix.
/// the ith (i is 0 based) matrix is p[i] x p[i + 1].
/// return the cost > 0.
int matrix_chain(const int* p, int n) {
    int** c;   // cost.
    int** s;   // seperation point to reconstruct the solution.

    /// Allocate the memory.
    c = malloc(sizeof(int*) * n);
    if (!c) return -1;
    int i, j;
    for (i = 0; i < n; ++i) {
        c[i] = malloc(sizeof(int) * n);
        if (!c[i]) {
            for (j = 0; j < i; ++j) {
                free(c[j]);
            }
            free(c);
            return -1;
        }
        c[i][i] = 0;
    }
    s = malloc(sizeof(int*) * n);
    if (!s) {
        for (i = 0; j < n; ++i) {
            free(c[i]);
        }
        free(c);
        return -1;
    }
    for (i = 0; i < n; ++i) {
        s[i] = malloc(sizeof(int) * n);
        if (!s[i]) {
            for (j = 0; j < n; ++j) {
                free(c[j]);
            }
            free(c);
            for (j = 0; j < i; ++j) {
                free(s[j]);
            }
            free(s);
            return -1;
        }
        s[i][i] = -1;
    }

    /// Solve the problem from bottom up.
    int len;
    for (len = 2; len <= n; ++len) {
        int start;
        for (start = 0; start <= n - len; ++start) {
            int end;
            int sep;
            end = start + len;
            c[start][end - 1] = c[start + 1][end - 1] + p[start] * p[start + 1] * p[end];
            s[start][end - 1] = start + 1;
            for (sep = start + 2; sep < end; ++sep) {
                int temp;
                temp = c[start][sep - 1] + c[sep][end - 1] + p[start] * p[sep] * p[end];
                if (temp < c[start][end - 1]) {
                    c[start][end - 1] = temp;
                    s[start][end - 1] = sep;
                }
            }
        }
    }

    print_s(s, n);
    print_s(c, n);

    // Find the solution.
    print_solution(s, 0, n - 1);

    int cost;
    cost = c[0][n - 1];

    /// Free the memory.
    for (i = 0; i < n; ++i) {
        free(c[i]);
        free(s[i]);
    }
    free(s);
    free(c);
    return cost;
}