@ Start code for programs compiled from C and for use with
@ Angel SWI instructions.
@
@ This file needs to be assembled with the command
@     arm-none-eabi-as start.s -o start.o
@ to generate an ARM object file.
@
@ The source code is provided in case a user wishes to pass an argv
@ array other than { "a.out" } to the main function.
@
@ Author:  Nigel Horspool, Dec 2010

	.global	_start
	.global __cs3_heap_limit
	.global	__errno
	.global __aeabi_idiv0
	.global __aeabi_ldiv0
	.text

_start:
        mov	r0, #0x16	@ heapInfo code
	ldr	r1,=heapInfo
	swi	0x123456	@ we need heap limit value

        @ call the C program
        mov     r0, #1          @ argc parameter
        ldr     r1, =argv       @ fake argv array
        bl	main

        @ exit
	mov	r0, #0x18
	mov	r1, #0x20000
	orr	r1, r1, #0x26	@ code 0x20026 = Stopped_ApplicationExit
	swi	0x123456	@ exit

        @ __errno function returns address of errno location
__errno:
        mov     r0, #0x13
        swi     0x123456        @ retrieve errno value
        ldr     r1, =errno
        str     r0, [r1]
        mov     r0, r1
        mov     pc, lr

        @ two dummy handlers for division by zero errors
__aeabi_idiv0:
__aeabi_ldiv0:
        mov     r0, #0
        mov     pc, lr

	.data
errno:  .word   0
argv:   .word   pgmname
        .word   0
pgmname:
        .asciz  "a.out"

        .align
heapInfo:
        .word   0	@ to receive value of heap_start
__cs3_heap_limit:
        .word   0
stack_base:
	.word	0
stack_limit:
	.word	0
	.end

