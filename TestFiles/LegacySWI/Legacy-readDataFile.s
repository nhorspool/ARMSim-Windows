@ *************** Code Segment ***************
@test file - Sept 2013 - read and check the data file that is
@                        created by writeDataFile.s

	.equ	SWI_Write0,	0x69
	.equ	SWI_OpenFile,	0x66
	.equ	SWI_CloseFile,	0x68
	.equ	SWI_Exit,	0x11
	.equ	SWI_WriteChar,	0x60
	.equ	SWI_ReadChar,	0x61
	.equ	SWI_ReadBytes,	0x62
	.equ	SWI_WriteBytes,	0x63
	.equ	SWI_WriteInt,	0x6b

	.text
	.global	_start

_start:
	mov	r4, #0		@ number of reads is counted in r4

	ldr	r0, =filename
	mov	r1, #0		@ open file for input
	swi	SWI_OpenFile
	bcs	err05
	mov	r3, r0		@ remember filehandle in r3

loop:
	ldr	r1, =datablock	@ input buffer address in r1
	mov	r2, #8		@ size of buffer in r2
	mov	r0, r3		@ filehandle
	swi	SWI_ReadBytes
	bcs	err06
	cmp	r0, #8		@ check number of bytes successfully read
	blt	eof
	add	r4, r4, #1
	bl	check		@ check buffer contents
	b	loop

eof:
	mov	r0, r3
	swi	SWI_CloseFile	@ close the file
	bcs	err10

done:
	mov	r0, #1
	ldr	r1, =exitmsg
	swi	SWI_Write0
	mov	r0, #1
	mov	r1, r4
	swi	SWI_WriteInt
	mov	r0, #1
	ldr	r1, =crlf
	swi	SWI_Write0
	swi	SWI_Exit	@ exit

check:
	ldr	r7, =datablock
	ldr	r8, =shouldbe
	mov	r0, #8
ck1:
	ldrsb	r1, [r7], #1
	ldrsb	r2, [r8], #1
	cmp	r1, r2
	bne	err99
	subs	r0, r0, #1
	bgt	ck1
	mov	pc, lr
err99:
	mov	r0, #1
	ldr	r1,=mserr
	swi	SWI_Write0
	b	done

err05:
	mov	r0, #1
	ldr	r1,=foerr
	swi	SWI_Write0
	b	done
err06:
	mov	r0, #1
	ldr	r1,=wrerr
	swi	SWI_Write0
	b	done

err10:
	mov	r0, #1
	ldr	r1,=fcerr
	swi	SWI_Write0
	b	done



@ *************** Data Segment ***************
	.data

filename:
	.asciz	"test.dat"

datablock:
	.skip	8

shouldbe:
	.byte	0, 1, 2, 3, 4, 5, 6, 7

foerr:
	.asciz	"cannot open input file\r\n"
wrerr:
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
