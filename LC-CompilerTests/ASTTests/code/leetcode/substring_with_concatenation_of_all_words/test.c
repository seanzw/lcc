#include <stdio.h>
#include <stdlib.h>

#define assert(cond, msg) if (!(cond)) {    \
    printf("assert fail: %s\n", msg);       \
    exit(-1);                               \
}


int* findSubstring(char* s, char** words, int wordsSize, int* returnSize);

int main(int argc, char* argv[]) {
    char* s = "wordgoodgoodgoodbestword";
    char* words[4];
    words[0] = "word";
    words[1] = "good";
    words[2] = "best";
    words[3] = "good";
    int returnSize;
    int* ret = findSubstring(s, words, 4, &returnSize);
    assert(returnSize == 1, "return size == 1");
    assert(ret[0] == 8, "return 8");
    printf("everything is fine!\n");
    return 0;
}