@ *************** Code Segment ***************
@ Read and check the data file  created by WriteDataFile.s
@
@ This version of the program uses the Angel SWI operations
	.text
	.global	_start

_start:
				@ initialize the globals
	mov	r4, #0		@ number of reads is counted in r4
	ldr	r1, =operands	@ operands for the SWI instruction

	ldr	r0, =filename
	str	r0, [r1,#0]	@ file name
	bl	strlen
	str	r0, [r1,#4]	@ length of name
	mov	r0, #0x1
	str	r0, [r1,#8]	@ byte input mode
	mov	r0, #0x01	@ open file for input
	swi	0x123456
	cmp	r0, #0
	blt	err05
	mov	r3, r0		@ remember file handle in r3

loop:
	ldr	r1, =operands
	str	r3, [r1]	@ file handle
	ldr	r0, =datablock
	str	r0, [r1,#4]	@ input buffer address
	mov	r0, #8
	str	r0, [r1,#8]	@ number of bytes to read
	mov	r0, #0x06		
	swi	0x123456	@ read bytes
	cmp	r0, #0
	bne	eof
	add	r4, r4, #1
	bl	check		@ check buffer contents
	b	loop

eof:
	str	r3, [r1]	@ file handle
	mov	r0, #0x02	@ close the file
	swi	0x123456
	cmp	r0, #0
	bne	err10

done:
	ldr	r0, =exitmsg
	bl	prints
	mov	r0, r4
	ldr	r1, =buffer
	bl	itoa
	bl	prints
	ldr	r0, =crlf
	bl	prints
	mov	r1, #0
	mov	r0, #0x18
	swi	0x123456	@ exit

check:
	ldr	r7, =datablock
	ldr	r8, =shouldbe
	mov	r0, #8
ck1:
	ldrsb	r9, [r7], #1
	ldrsb	r2, [r8], #1
	cmp	r9, r2
	bne	err99
	subs	r0, r0, #1
	bgt	ck1
	mov	pc, lr

err99:
	ldr	r0, =mserr
	bl	prints
	b	done
err05:
	ldr	r0, =foerr
	bl	prints
	b	done
err06:
	ldr	r0, =rderr
	bl	prints
	b	done
err10:
	ldr	r0,=fcerr
	bl	prints
	b	done

@ prints: Output a null-terminated ASCII string to standard output
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

@ strlen: compute length of a null-terminated ASCII string
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
3:	mov	r0, r7
	ldmfd	sp!, {r1-r7,pc}
4:	.word	1000000000, 100000000, 10000000, 1000000
	.word	100000, 10000, 1000, 100, 10, 1, 0
	

@ *************** Data Segment ***************
	.data
operands:
	.word	0, 0, 0
buffer:
	.skip	16
filename:
	.ascii	"test.dat"
datablock:
	.skip	8
shouldbe:
	.byte	0, 1, 2, 3, 4, 5, 6, 7
foerr:
	.asciz	"cannot open input file\r\n"
rderr:
	.asciz	"error reading from file\r\n"
mserr:
	.asciz	"unexpected values for bytes read from file\r\n"
fcerr:
	.asciz	"cannot close file\r\n"
exitmsg:
	.asciz	"number of 8 byte blocks read = "
crlf:
	.asciz	"\r\n"
	.end
