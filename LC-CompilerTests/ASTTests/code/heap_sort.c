int parent(int i) {
    return i / 2;
}

int left(int i) {
    return 2 * i;
}

int right(int i) {
    return 2 * i + 1;
}

void swap(int* arr, int i, int j) {
    int t;
    t = arr[i];
    arr[i] = arr[j];
    arr[j] = t;
}

/// Assume left(i) and right(i) is a heap.
/// Restore the max heap property at i.
void max_heapify(int* arr, int len, int i) {
    int lhs, rhs, maximum;
    lhs = left(i);
    rhs = right(i);
    /// Get the maximum idx.
    if (lhs < len && arr[lhs] > arr[i]) maximum = lhs;
    else maximum = i;
    if (rhs < len && arr[rhs] > arr[maximum]) maximum = rhs;

    if (maximum != i) {
        swap(arr, i, maximum);
        max_heapify(arr, len, maximum);
    }
}

void build_max_heap(int* arr, int len) {
    int i;
    for (i = len / 2 - 1; i >= 0; i--) {
        max_heapify(arr, len, i);
    }
}

void heap_sort(int* arr, int len) {
    int i;
    build_max_heap(arr, len);
    for (i = len - 1; i >= 1; i--) {
        swap(arr, i, 0);
        max_heapify(arr, i, 0);
    }
}