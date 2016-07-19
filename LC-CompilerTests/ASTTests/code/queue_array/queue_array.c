typedef unsigned int size_t;

void* malloc(size_t size);
void free(void* ptr);

typedef int QueueElementT;

typedef struct Queue {
    QueueElementT* arr;
    unsigned int head;
    unsigned int tail;
    unsigned int size;
} Queue;

Queue* queue_init(unsigned int max) {
    Queue* queue;
    queue = malloc(sizeof(Queue));
    if (queue != 0) {
        queue->arr = malloc((max + 1) * sizeof(QueueElementT));
        if (queue->arr != 0) {
            queue->size = max + 1;
            queue->head = queue->tail = 0;
        }
        else {
            free(queue);
        }
    }
    return queue;
}

void queue_free(Queue* queue) {
    if (queue != 0) free(queue);
}

int queue_is_empty(Queue* queue) {
    return queue->head == queue->tail;
}

int queue_enqueue(Queue* queue, QueueElementT element) {
    unsigned int next;
    next = (queue->tail + 1) % queue->size;
    if (next == queue->head) return -1;
    queue->arr[queue->tail] = element;
    queue->tail = next;
    return 0;
}

int queue_dequeue(Queue* queue, QueueElementT* dst) {
    if (queue_is_empty(queue)) return -1;
    *dst = queue->arr[queue->head];
    queue->head = (queue->head + 1) % queue->size;
    return 0;
}