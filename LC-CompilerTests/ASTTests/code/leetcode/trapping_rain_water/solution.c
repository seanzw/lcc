int trap(int* height, int heightSize) {
    if (heightSize < 2) return 0;
    int i, heightest, pos, water;
    heightest = height[0];
    pos = 0;
    for (i = 1; i < heightSize; ++i) {
        if (heightest < height[i]) {
            heightest = height[i];
            pos = i;
        }
    }
    water = 0;
    heightest = height[0];
    for (i = 1; i < pos; ++i) {
        if (height[i] > heightest) heightest = height[i];
        else water += heightest - height[i];
    }
    heightest = height[heightSize - 1];
    for (i = heightSize - 2; i > pos; --i) {
        if (height[i] > heightest) heightest = height[i];
        else water += heightest - height[i];
    }
    return water;
}