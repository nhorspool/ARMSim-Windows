#include <stdio.h>

int main(int argc, char* argv[]) {

    int x = 99, y = -99;
    char *s = "hello there";
    double d = 3.14159265358979;

    printf("x = %d, y = %X, z = %s, pi = %g\n",
        x, y, s, d);
    return 0;
}