@ Test of a call to the printf function in the C library
@,
@ Follow the instructions to run a multifile program.
@
@ The integer 99 is printed in both decimal and hexadecimal,
@ a string is printed, and a double-length float is printed.

        .global main
        .extern printf
        .text
main:
        ldr     r0,=fmt		@ first 4 args in registers r0-r3
        mov     r1,#99
        mov     r2,#-99
        ldr     r3,=string
	ldr	r4,=pi
	ldr	r5,[r4],#4
	ldr	r6,[r4]
        stmfd   sp!,{r5-r6}	@ push additional arguments on the stack
        ldr     r10,=printf
        blx     r10
        add     sp,sp,#8        @ clean up the stack
	mov	r0, #0x18
	mov	r1, #0
	swi	0x123456	@ exit gracefully

        .data
fmt:    .asciz "x = %d, y = %X, z = %s, pi = %g\n"
string: .asciz "hello there"
	.align	2
pi:     .double 3.14159265358979e0
        .end
