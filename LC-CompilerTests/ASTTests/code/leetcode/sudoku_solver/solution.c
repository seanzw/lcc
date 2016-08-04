int printf(const char* format, ...);

static void print_board(char** board) {
    int i;
    for (i = 0; i < 9; ++i) {
        printf("%s\n", board[i]);
    }
    printf("\n");
}


static int aux(char** board, char(*rows)[9], char(*cols)[9], char(*blks)[9], int nUnfilled, int row, int col) {
    //printf("nunfilled = %d\n", nUnfilled);
    if (nUnfilled == 0) return 1;
    if (col == 9) {
        row++;
        col = 0;
    }
    if (board[row][col] != '.') {
        return aux(board, rows, cols, blks, nUnfilled, row, col + 1);
    }
    int blk;
    int i;
    for (i = 0; i < 9; ++i) {
        blk = (row / 3) * 3 + col / 3;
        if (rows[row][i] == 0 && cols[col][i] == 0 && blks[blk][i] == 0) {
            /// Found a pos.
            //printf("put %d at (%d, %d)\n", i, row, col);
            board[row][col] = i + '1';
            rows[row][i] = cols[col][i] = blks[blk][i] = 1;
            if (aux(board, rows, cols, blks, nUnfilled - 1, row, col + 1)) return 1;
            /// This is not a valid position.
            board[row][col] = '.';
            rows[row][i] = cols[col][i] = blks[blk][i] = 0;
        }
    }
    return 0;
}

void solveSudoku(char** board, int boardRowSize, int boardColSize) {
    char rows[9][9];
    char cols[9][9];
    char blks[9][9];
    int row, col, blk, nUnfilled;
    nUnfilled = 0;
    for (row = 0; row < 9; ++row) {
        for (col = 0; col < 9; ++col) {
            rows[row][col] = cols[row][col] = blks[row][col] = 0;
        }
    }

    for (row = 0; row < 9; ++row) {
        for (col = 0; col < 9; ++col) {
            blk = (row / 3) * 3 + col / 3;
            if (board[row][col] == '.') {
                nUnfilled++;
            }
            else {
                int i;
                i = board[row][col] - '1';
                rows[row][i] = cols[col][i] = blks[blk][i] = 1;
            }
        }
    }

    int ret;
    ret = aux(board, rows, cols, blks, nUnfilled, 0, 0);
    //printf("ret = %d\n", ret);
    //print_board(board);
}