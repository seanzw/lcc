/**
* Definition for singly-linked list.
* struct ListNode {
*     int val;
*     struct ListNode *next;
* };
*/
int printf(const char* format, ...);

typedef unsigned int size_t;
size_t strlen(const char*);

void* malloc(size_t);
void free(void*);

static void print_mat(char**mat, size_t s_len, char** words, int wordsSize) {
    int i, j;
    for (i = 0; i < wordsSize; ++i) {
        printf("%s: ", words[i]);
        for (j = 0; j < s_len; ++j) {
            printf("%d ", mat[i][j]);
        }
        printf("\n");
    }
    printf("\n");
}

static int aux(size_t s_len, int wordsSize, size_t pos, int n, char** mat, size_t* words_len, char* used) {

    if (n == 0) return 1;
    if (pos == s_len) return 0;
    int i;
    int total;
    total = 0;
    for (i = 0; i < wordsSize; ++i) {
        if (!used[i] && mat[i][pos]) {
            //printf("n %d, use %d, pos %d, new pos %d\n", n, i, pos, pos + words_len[i]);
            used[i] = 1;
            total += aux(s_len, wordsSize, pos + words_len[i], n - 1, mat, words_len, used);
            used[i] = 0;
        }
    }
    return total;
}

int* findSubstring(char* s, char** words, int wordsSize, int* returnSize) {

    int i, j, k;

    size_t s_len;
    s_len = strlen(s);
    size_t words_total_len;
    words_total_len = 0;

    char** mat;
    size_t* words_len;
    char* used;
    mat = malloc(wordsSize * sizeof(char*));
    words_len = malloc(wordsSize * sizeof(size_t));
    used = malloc(wordsSize * sizeof(char));
    for (i = 0; i < wordsSize; ++i) {
        words_len[i] = strlen(words[i]);
        words_total_len += words_len[i];
        used[i] = 0;
        mat[i] = malloc(s_len * sizeof(char));
        for (j = 0; j < s_len; ++j) {
            mat[i][j] = 1;
            for (k = 0; k < words_len[i]; ++k) {
                if (k + j >= s_len || words[i][k] != s[j + k]) {
                    mat[i][j] = 0;
                    break;
                }
            }
        }
    }

    //print_mat(mat, s_len, words, wordsSize);

    int* ret;
    ret = malloc(s_len * sizeof(int));
    int cnt;
    cnt = 0;

    for (i = 0; i < s_len - words_total_len + 1; ++i) {
        for (j = 0; j < wordsSize; ++j) {
            used[j] = 0;
        }

        size_t pos;
        pos = i;
        int ans;
        ans = 1;
        for (j = 0; j < wordsSize; ++j) {
            int found;
            found = 0;
            if (pos >= s_len) {
                ans = 0; break;
            }
            for (k = 0; k < wordsSize; ++k) {
                //printf("!used[k] = %d\n", !used[k]);
                if (!used[k] && mat[k][pos]) {
                    //printf("found!\n");
                    pos += words_len[k];
                    used[k] = 1;
                    found = 1;
                    break;
                }
            }
            if (found) continue;
            else {
                ans = 0;
                break;
            }
        }
        if (ans) ret[cnt++] = i;
        /*if (aux(s_len, wordsSize, i, wordsSize, mat, words_len, used)) {
            ret[cnt++] = i;
        }*/
    }
    for (i = 0; i < wordsSize; ++i) {
        free(mat[i]);
    }
    free(mat);
    free(words_len);
    free(used);

    //printf("cnt = %d\n", cnt);
    *returnSize = cnt;
    return ret;
}