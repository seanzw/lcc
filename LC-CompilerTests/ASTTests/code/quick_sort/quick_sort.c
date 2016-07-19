int partition(int* arr, int lhs, int rhs) {
    int x, i, j, t;
    x = arr[rhs];
    i = lhs - 1;
    for (j = lhs; j <= rhs - 1; j++) {
        if (arr[j] <= x) {
            i++;
            t = arr[i];
            arr[i] = arr[j];
            arr[j] = t;
        }
    }
    t = arr[i + 1];
    arr[i + 1] = arr[rhs];
    arr[rhs] = t;
    return i + 1;
}

void quick_sort(int* arr, int lhs, int rhs) {
    if (lhs < rhs) {
        int mid;
        mid = partition(arr, lhs, rhs);
        quick_sort(arr, lhs, mid - 1);
        quick_sort(arr, mid + 1, rhs);
    }
}