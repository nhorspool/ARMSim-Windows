@ Write a binary data file  to be read by ReadDataFile.s
@
@ This version of the program uses the Angel SWI operations

	.text
	.global	_start

_start:
				@ initialize the globals
	mov	r4, #10		@ number of writes is counted down in r4
	ldr	r1, =operands	@ operands for the SWI instruction

	ldr	r0, =filename
	str	r0, [r1,#0]	@ file name
	bl	strlen
	str	r0, [r1,#4]	@ length of name
	mov	r0, #0x5
	str	r0, [r1,#8]	@ byte input mode
	mov	r0, #0x01	@ open file for output
	swi	0x123456
	cmp	r0, #0
	blt	err05
	mov	r3, r0		@ remember file handle in r3
loop:
	str	r3, [r1]	@ file handle
	ldr	r0, =datablock
	str	r0, [r1,#4]	@ output buffer address
	mov	r0, #8
	str	r0, [r1,#8]	@ number of bytes to write
	mov	r0, #0x05		
	swi	0x123456	@ write bytes
	cmp	r0, #0
	bne	err06
	subs	r4, r4, #1
	bgt	loop

	str	r3, [r1]	@ file handle
	mov	r0, #0x02	@ close the file
	swi	0x123456
	cmp	r0, #0
	bne	err10

done:
	mov	r1, #0
	mov	r0, #0x18
	swi	0x123456	@ exit

err05:
	ldr	r0,=foerr
	bl	prints
	b	done
err06:
	ldr	r0, =wrerr
	bl	prints
	b	done

err10:
	ldr	r0, =fcerr
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


@ *************** Data Segment ***************
	.data
operands:
	.word	0, 0, 0
filename:
	.asciz	"test.dat"
datablock:
	.byte	0, 1, 2, 3, 4, 5, 6, 7
foerr:
	.asciz	"cannot open output file\r\n"
wrerr:
	.asciz	"error writing to file\r\n"
fcerr:
	.asciz	"cannot close file\r\n"
	.end
