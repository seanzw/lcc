#include <stdio.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    return -1;                              \
}                                           

typedef int QueueElementT;

typedef struct Queue {
    QueueElementT* arr;
    unsigned int head;
    unsigned int tail;
    unsigned int size;
} Queue;

Queue* queue_init(unsigned int max);

void queue_free(Queue* queue);

int queue_is_empty(Queue* queue);

int queue_enqueue(Queue* queue, QueueElementT element);

int queue_dequeue(Queue* queue, QueueElementT* dst);


int main(int argc, char* argv[]) {
    int a[] = { 5, 4, 5, 3 };
    Queue* queue = queue_init(4);
    assert(queue != 0, "init");
    assert(queue_is_empty(queue), "empty");
    assert(queue->size == 5, "size");
    for (int i = 0; i < 4; ++i) {
        assert(queue_enqueue(queue, a[i]) == 0, "enqueue");
    }

    assert(queue->tail == queue->size - 1, "full");
    assert(queue_enqueue(queue, 1) == -1, "enqueue after full");
    for (int i = 0; i < 4; ++i) {
        int x;
        assert(queue_dequeue(queue, &x) == 0, "deque");
        assert(x == a[i], "pop_value");
    }

    assert(queue_is_empty(queue), "empty2");

    for (int i = 0; i < 4; ++i) {
        assert(queue_enqueue(queue, a[i]) == 0, "enqueue2");
    }

    assert(queue_enqueue(queue, 1) == -1, "enqueue after full");
    for (int i = 0; i < 4; ++i) {
        int x;
        assert(queue_dequeue(queue, &x) == 0, "deque");
        assert(x == a[i], "pop_value");
    }

    assert(queue_is_empty(queue), "empty3");

    queue_free(queue);

    printf("everything is fine!\n");

    return 0;
}