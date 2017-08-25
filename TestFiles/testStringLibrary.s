@ test some string library functions
@   -- must be linked with libc.a and other files
@   -- read the instructions!

        .globl main
        .extern printf
        .extern strcpy
        .extern strcat

        .text
main:
	mov	r11, lr
        ldr     r0,=target
        ldr     r1,=str1
        ldr     r10,=strcpy
        blx     r10
        ldr     r0,=fmt
        ldr     r1,=target
        ldr     r10,=printf
        stmfd   sp!,{r1}
        blx     r10
        add     sp,sp,#4
        ldr     r0,=target
        ldr     r1,=str2
        ldr     r10,=strcat
        blx     r10
        ldr     r0,=fmt
        ldr     r1,=target
        ldr     r10,=printf
        stmfd   sp!,{r1}
        blx     r10
        add     sp,sp,#4
	mov	r0, #0x18
	mov	r1, #0
	bx	r11	
        .data
target: .space  64
str1:   .asciz  "hello"
str2:   .asciz  " there"
fmt:    .asciz  "target = \"%s\"\n"
        .end