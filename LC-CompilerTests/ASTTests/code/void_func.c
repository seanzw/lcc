void func1(int argc, char* argv[]) {
    int a;
    {
        int c[10];
        c;
    }
    {
        int d[10];
    }
    argc;
    a;
}

void func2(int argc, char* argv[]) {
    char a;
}
