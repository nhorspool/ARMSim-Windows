extern int sprintf(char *, ...);

char buffer[64];
char fmt[] = "d = %f\n";
double d = 3.14159;

int main( int argc, char* args[] ) {
    sprintf(buffer, fmt, d);
    return 0;
}
