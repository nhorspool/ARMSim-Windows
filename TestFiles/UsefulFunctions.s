@ This file contains functions that may be useful when using the Angel SWI operations
@
@ Functions:
@    prints  -- print a null-terminated ASCII string to standard output
@    fprints -- print a null-terminated ASCII string to a text file
@    fgets   -- read one line from a text file or standard input
@    strlen  -- compute length of a null-terminated ASCII string
@    atoi    -- convert an ASCII representation of a number to an integer
@    itoa    -- convert an integer to a null-terminated ASCII string
@
@ Author: R.N. Horspool, June 2015

	.global	prints, fprints, fgets
	.global	strlen
	.global	itoa, atoi

@ TEXT SEGMENT

	.text

@ This file contains only functions; execution is not intended to begin here.
@ If control does enter here, the atoi and itoa functions are tested.
	ldr	r0, =errormsg
	bl	prints
	bl	runtests
	mov	r0, #0x18
	mov	r1, #0
	swi	0x123456
errormsg:
	.ascii	"Control is not expected to enter the code in this file at the top!\r\n"
	.ascii	"This file contains functions which should be called from other files.\r\n"
	.ascii	"\r\n"
	.asciz	"Entering test mode for the atoi&itoa functions ...\r\n"
	.align	2

@ prints: Output a null-terminated ASCII string to the standard output stream
@
@ Usage:
@    prints(r0)
@ Input parameter:
@    r0: the address of a null-terminated ASCII string
@ Result:
@    None, but the string is written to standard output (the console)
prints:
	stmfd	sp!, {r0,r1,lr}
	ldr	r1, =operands
	str	r0, [r1,#4]
	bl	strlen
	str	r0, [r1,#8]
	mov	r0, #0x0
	str	r0, [r1]
	mov	r0, #0x05
	swi	0x123456
	ldmfd	sp!, {r0,r1,pc}

@ fprints: Output a null-terminated ASCII string to a selected stream
@          (a text file opened for output or stdout or stderr)
@
@ Usage:
@    fprints(r0, r1)
@ Input parameters:
@    r0: a file handle
@    r1: the address of a null-terminated ASCII string
@ Result:
@    None, but the string is written to the specified stream
fprints:
	stmfd	sp!, {r0-r2,lr}
	ldr	r2, =operands
	str	r0, [r2]
	str	r1, [r2,#4]
	mov	r0, r1
	bl	strlen
	str	r0, [r2,#8]
	mov	r1, r2
	mov	r0, #0x05
	swi	0x123456
	ldmfd	sp!, {r0-r2,pc}

@ fgets: read an ASCII text line from an input stream (a text file
@        opened for input or standard input)
@
@ Usage:
@    r0 = fgets(r0, r1, r2)
@ Input parameters:
@    r0: the address of a buffer to receive the input line
@    r1: the size of the buffer (which needs to accommodate a
@        terminating null byte)
@    r2: the handle for a text file opened for input or 0 if input
@        should be read from stdin
@ Result:
@    r0: the address of the buffer if some characters were read,
@        or 0 if no characters were read due to EOF or error.
@        One text line including a terminating linefeed character
@        is read into the buffer, if the buffer is large enough.
@        Otherwise the buffer holds size-1 characters and a null byte.
@        Note: the line stored in the buffer will have only a linefeed
@        (\n) line ending, even if the input source has a DOS line
@        ending (a \r\n pair).
fgets:	stmfd	sp!, {r1-r4,lr}
	ldr	r3, =operands
	str	r2, [r3]	@ specify input stream
	mov	r2, r0
	mov	r4, r1
	mov	r0, #1
	str	r0, [r3,#8]	@ to read one character
	mov	r1, r3
	mov	r3, r2
1:	sub	r4, r4, #1
	ble	3f		@ jump if buffer has been filled
	str	r3, [r1,#4]
2:	mov	r0, #0x06	@ read operation
	swi	0x123456
	cmp	r0, #0
	bne	4f		@ branch if read failed
	ldrb	r0, [r3]
	cmp	r0, #'\r'	@ ignore \r char (result is a Unix line)
	beq	2b
	add	r3, r3, #1
	cmp	r0, #'\n'
	bne	1b
3:	mov	r0, #0
	strb	r0, [r3]
	mov	r0, r2		@ set success result
	ldmfd	sp!, {r1-r4,pc}
4:	cmp	r4, r2
	bne	3b		@ some chars were read, so return them
	mov	r0, #0		@ set failure code
	strb	r0, [r2]	@ put empty string in the buffer
	ldmfd	sp!, {r1-r4,pc}

@ strlen: compute length of a null-terminated ASCII string
@
@ Usage:
@    r0 = strlen(r0)
@ Input parameter:
@    r0: the address of a null-terminated ASCII string
@ Result:
@    r0: the length of the string (excluding the null byte at the end)
strlen:
	stmfd	sp!, {r1-r3,lr}
	mov	r1, #0
	mov	r3, r0
1:	ldrb	r2, [r3], #1
	cmp	r2, #0
	bne	1b
	sub	r0, r3, r0
	ldmfd	sp!, {r1-r3,pc}

@ itoa: Integer to ASCII string conversion
@
@ Usage:
@    r0 = itoa(r0, r1)
@ Input parameters:
@    r0: a signed integer
@    r1: the address of a buffer sufficiently large to hold the
@        function result, which is a null-terminated ASCII string
@ Result:
@    r0: the address of the buffer
itoa:
	stmfd	sp!, {r1-r7,lr}
	mov	r7, r1		@ remember buffer address
	cmp	r0, #0		@ check if negative and if zero
	movlt	r2, #'-'
	moveq	r2, #'0'
	strleb	r2, [r1],#1	@ store a '-' symbol or a '0' digit
	beq	3f
	mvnlt	r0, r0
	ldr	r3, =4f		@ R3: multiple pointer
	mov	r6, #0		@ R6: write zero digits? (no, for leading zeros)
1:	ldr	r4, [r3],#4	@ R4: current power of ten
	cmp	r4, #1		@ stop when power of ten < 1
	blt	3f
	mov	r5, #0		@ R5: multiples count
2:	subs	r0, r0, r4	@ subtract multiple from value
	addpl	r5, r5, #1	@ increment the multiples count
	bpl	2b
	add	r0, r0, r4	@ correct overshoot
	cmp	r5, #0		@ if digit is '0' and ...
	cmpeq	r6, #0		@    if it's a leading zero
	beq	1b		@ then skip it
	mov	r6, #1
	add	r2, r5, #'0'	@ ASCII code for the digit
	strb	r2, [r1],#1	@ store it
	b	1b
3:	mov	r0, #0
	strb	r0, [r1]
	mov	r0, r7
	ldmfd	sp!, {r1-r7,pc}
4:	.word	1000000000, 100000000, 10000000, 1000000
	.word	100000, 10000, 1000, 100, 10, 1, 0

@ atoi: ASCII string to integer conversion
@
@ Usage:
@    r0 = atoi(r0)
@ Input parameters:
@    r0: the address of a null-terminated ASCII string
@ Result:
@    r0: the value of the converted integer
atoi:
	stmfd	sp!, {r1-r4,lr}
	mov	r2, #0		@ holds result
	mov	r3, #0		@ set to 1 if a negative number
	mov	r4, #10
1:	ldrb	r1, [r0], #1	@ get next char
	cmp	r1, #0
	beq	4f
	cmp	r1, #' '
	beq	1b
	cmp	r1, #'-'
	moveq	r3, #1
	ldreqb	r1, [r0], #1
	b	3f
2:	cmp	r1, #9
	bgt	4f
	mul	r2, r4, r2
	add	r2, r2, r1
	ldrb	r1, [r0], #1
3:	subs	r1, r1, #'0'
	bge	2b
4:	cmp	r3, #0
	moveq	r0, r2
	mvnne	r0, r2
	ldmfd	sp!, {r1-r4,pc}

@ test function, invoked if control enters at top of the file
runtests:
	stmfd	sp!, {r0-r3,lr}
	ldr	r0, =testmsg0
	bl	prints
	ldr	r2, =testnums
1:	ldr	r1, =testbuff
	ldr	r0, [r2], #4	@ load a test value
2:	mov	r3, r0		@ keep a copy in r3
	bl	itoa		@ convert to ASCII
	ldr	r0, =testmsg1
	bl	prints
	ldr	r0, =testbuff	@ print the ASCII version of the number
	bl	prints
	ldr	r0, =testbuff
	bl	atoi		@ convert the ASCII back to a number
	cmp	r0, r3		@ get back the same number?
	ldreq	r0, =testcrlf	@ yes -> an empty message
	ldrne	r0, =testmsg2	@ no  -> an error message
	bl	prints		@ print the message
	cmp	r3, #0
	blt	1b		@ repeat with a new test integer
	mvn	r0, r3
	bgt	2b		@ repeat test with negated number
	ldr	r0, =testmsg3
	bl	prints
	ldmfd	sp!, {r0-r3,pc}
@ test values: the end of the list is indicated by a 0 value
testnums:
	.word	9, 123, 546781, 2147483647, 0
testmsg0:
	.asciz	"Testing itoa/atoi in pairs...\n"
testmsg1:
	.asciz	"- Number being tested: "
testmsg2:
	.asciz	" *** TEST FAILED!\r\n"
testmsg3:
	.asciz	"Testing of itoa/atoi completed\r\n"
testcrlf:
	.asciz	"\r\n"

@ DATA SEGMENT
	.data
operands:
	.word	0, 0, 0
testbuff:
	.space	16