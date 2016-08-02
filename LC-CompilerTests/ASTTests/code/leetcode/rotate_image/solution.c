void rotate(int** matrix, int matrixRowSize, int matrixColSize) {
    int i, j, tmp, n;
    n = matrixColSize;
    for (i = 0; i < n; ++i) {
        for (j = i + 1; j < n; ++j) {
            tmp = matrix[i][j];
            matrix[i][j] = matrix[j][i];
            matrix[j][i] = tmp;
        }
    }

    for (i = 0; i < n; ++i) {
        for (j = 0; j < n / 2; ++j) {
            tmp = matrix[i][j];
            matrix[i][j] = matrix[i][n - j - 1];
            matrix[i][n - j - 1] = tmp;
        }
    }
}