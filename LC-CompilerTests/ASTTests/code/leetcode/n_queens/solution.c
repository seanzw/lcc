int printf(const char* format, ...);

typedef unsigned int size_t;

void* malloc(size_t);
void free(void*);
void* memcpy(void*, const void*, size_t);

void clear_board(char** board, int n) {
    int i, j;
    for (i = 0; i < n; ++i) {
        for (j = 0; j < n; ++j) {
            board[i][j] = '.';
        }
        board[i][n] = '\0';
    }
}

void print_board(char** board, int n) {
    int i, j;
    for (i = 0; i < n; ++i) {
        printf("%s\n", board[i]);
    }
    printf("\n");
}

char** init_board(int n) {
    int i;
    char** solution;
    solution = malloc(sizeof(char*) * n);
    for (i = 0; i < n; ++i) {
        solution[i] = malloc(sizeof(char) * (n + 1));
    }
    return solution;
}

void push_solution(char**** solutions, int* size, int* num, int n, char** board) {
    int i, j, k;
    /// Allocate the new solutions array.
    if (*num == *size) {
        char*** tmp;
        tmp = malloc(sizeof(char**) * 2 * (*size));
        memcpy(tmp, *solutions, sizeof(char**) * (*num));
        free(*solutions);
        *solutions = tmp;
        *size = 2 * (*size);
    }
    /// Allocate the new board.
    char** solution;
    solution = init_board(n);
    for (i = 0; i < n; ++i) {
        memcpy(solution[i], board[i], sizeof(char) * (n + 1));
    }
    /// Add the new solution.
    (*solutions)[*num] = solution;
    *num += 1;
}

void aux(char**** solutions, int* size, int* num, char** board, int n, char* col_mask, char* diag_mask, char* anti_diag_mask, int row) {

    //print_board(board, n);

    if (row == n) {
        // Found an solution.
        push_solution(solutions, size, num, n, board);
        return;
    }

    int i, j;
    for (i = 0; i < n; ++i) {
        int diag_idx, anti_diag_idx;
        diag_idx = row - i + n - 1;
        anti_diag_idx = row + i;
        if (col_mask[i] == 0 && diag_mask[diag_idx] == 0 && anti_diag_mask[anti_diag_idx] == 0) {
            /// This is a position.
            board[row][i] = 'Q';
            col_mask[i] = 1;
            diag_mask[diag_idx] = 1;
            anti_diag_mask[anti_diag_idx] = 1;
            /// Recursion.
            aux(solutions, size, num, board, n, col_mask, diag_mask, anti_diag_mask, row + 1);
            /// Restore everything.
            board[row][i] = '.';
            col_mask[i] = 0;
            diag_mask[diag_idx] = 0;
            anti_diag_mask[anti_diag_idx] = 0;
        }
    }
}

/**
* Return an array of arrays of size *returnSize.
* Note: The returned array must be malloced, assume caller calls free().
*/
char*** solveNQueens(int n, int* returnSize) {
    int size;
    int num;
    char* col_mask;
    char* diag_mask;
    char* anti_diag_mask;
    char** board;
    char*** solutions;

    solutions = malloc(sizeof(char**) * n);
    size = n;
    num = 0;

    col_mask = malloc(sizeof(char) * n);
    diag_mask = malloc(sizeof(char) * (2 * n - 1));
    anti_diag_mask = malloc(sizeof(char) * (2 * n - 1));

    int i;
    for (i = 0; i < n; ++i) {
        col_mask[i] = 0;
    }
    for (i = 0; i < 2 * n - 1; ++i) {
        diag_mask[i] = anti_diag_mask[i] = 0;
    }

    board = init_board(n);
    clear_board(board, n);

    aux(&solutions, &size, &num, board, n, col_mask, diag_mask, anti_diag_mask, 0);

    for (i = 0; i < n; ++i) {
        free(board[i]);
    }
    free(board);
    free(col_mask);
    free(diag_mask);
    free(anti_diag_mask);

    *returnSize = num;
    return solutions;
}