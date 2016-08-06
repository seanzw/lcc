/**
* Definition for singly-linked list.
* struct ListNode {
*     int val;
*     struct ListNode *next;
* };
*/


int printf(const char*, ...);
typedef struct {
    int height;
    int start;
} Node;

void* malloc(unsigned int size);
void free(void*);

int largestRectangleArea(int* heights, int heightsSize) {

    Node* nodes;
    int rhs;
    nodes = malloc(sizeof(Node) * (heightsSize + 1));
    nodes[0].start = -1;
    nodes[0].height = 0;
    rhs = 1;

    int area;
    area = 0;

    int i, j;
    for (i = 0; i < heightsSize; ++i) {
        //for (j = 0; j < rhs; ++j) {
        //    printf("nodes[%d].height = %d, .start = %d\n", j, nodes[j].height, nodes[j].start);
        //}
        if (nodes[rhs - 1].height < heights[i]) {
            nodes[rhs].height = heights[i];
            nodes[rhs].start = i;
            rhs++;
        }
        else {
            int start;
            start = i;
            for (j = rhs - 1; nodes[j].height > heights[i]; --j) {
                int new_area;
                new_area = (i - nodes[j].start) * nodes[j].height;
                if (new_area > area) area = new_area;
                start = nodes[j].start;
            }
            if (nodes[j].height < heights[i]) {
                nodes[j + 1].height = heights[i];
                nodes[j + 1].start = start;
                rhs = j + 2;
            }
            else {
                rhs = j + 1;
            }
        }
    }

    //for (j = 0; j < rhs; ++j) {
    //    printf("nodes[%d].height = %d, .start = %d\n", j, nodes[j].height, nodes[j].start);
    //}
    for (i = rhs - 1; i > 0; --i) {
        int new_area;
        new_area = (heightsSize - nodes[i].start) * nodes[i].height;
        //printf("new area = %d\n", new_area);
        if (new_area > area) area = new_area;
    }

    //printf("area = %d\n", area);
    free(nodes);
    return area;
}

int maximalRectangle(char** matrix, int matrixRowSize, int matrixColSize) {
    int* heights;
    heights = malloc(sizeof(int) * matrixColSize);
    int i, j, area;
    area = 0;
    for (i = 0; i < matrixColSize; ++i) {
        heights[i] = 0;
    }
    for (i = 0; i < matrixRowSize; ++i) {
        for (j = 0; j < matrixColSize; ++j) {
            if (matrix[i][j] == '0') heights[j] = 0;
            else heights[j]++;
        }
        int new_area;
        new_area = largestRectangleArea(heights, matrixColSize);
        if (new_area > area) area = new_area;
    }
    free(heights);
    return area;
}