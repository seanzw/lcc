/**
* Definition for singly-linked list.
* struct ListNode {
*     int val;
*     struct ListNode *next;
* };
*/


int printf(const char*, ...);
void* malloc(unsigned int size);
void free(void*);

int canCompleteCircuit(int* gas, int gasSize, int* cost, int costSize) {
    int min;
    min = 0;
    int idx;
    idx = 0;
    int reserve;
    reserve = 0;
    int i;
    for (i = 1; i <= gasSize; ++i) {
        reserve += gas[i - 1] - cost[i - 1];
        if (reserve < min) {
            min = reserve;
            idx = i % gasSize;
        }
    }
    if (reserve < 0) return -1;
    else return idx;
}