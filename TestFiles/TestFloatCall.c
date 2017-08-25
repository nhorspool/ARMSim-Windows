#include <stdio.h>

int main( int argc, char* args[] ) {
    float f = 1.234;
    double d = 3.14159;
    printf("f = %f, d = %g\n", f, d);
    fprintf(stderr, "<<standard error>>\n");
    return 0;
}
