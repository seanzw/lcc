typedef unsigned int size_t;
void* malloc(size_t);
void free(void*);
void* memcpy(void*, const void*, size_t);
size_t strlen(const char*);
int printf(const char*, ...);

int uniquePathsWithObstacles(int** obstacleGrid, int obstacleGridRowSize, int obstacleGridColSize) {
    int* ret = malloc(sizeof(int) * obstacleGridColSize);
    int obstacle = 0;
    for (int i = 0; i < obstacleGridColSize; ++i) {
        if (!obstacle && obstacleGrid[0][i]) obstacle = 1;
        ret[i] = !obstacle;
    }
    for (int i = 1; i < obstacleGridRowSize; ++i) {
        if (obstacleGrid[i][0]) ret[0] = 0;
        for (int j = 1; j < obstacleGridColSize; ++j) {
            if (obstacleGrid[i][j]) ret[j] = 0;
            else ret[j] += ret[j - 1];
        }
    }
    int num = ret[obstacleGridColSize - 1];
    free(ret);
    return num;
}