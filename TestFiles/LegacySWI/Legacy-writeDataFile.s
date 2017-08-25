@ *************** Code Segment ***************
@test file - Sept 2013 - create a data file

	.equ	SWI_Write0,	0x69
	.equ	SWI_OpenFile,	0x66
	.equ	SWI_CloseFile,	0x68
	.equ	SWI_Exit,	0x11
	.equ	SWI_WriteChar,	0x60
	.equ	SWI_ReadChar,	0x61
	.equ	SWI_ReadBytes,	0x62
	.equ	SWI_WriteBytes,	0x63

	.text
	.global	_start

_start:
	ldr	r0,=filename
	mov	r1,#1		@ open file for output
	swi	SWI_OpenFile
	bcs	err05
	mov	r3, r0		@ remember filehandle in r3

	mov	r4, #10		@ number of repetitions in r4
loop:
	ldr	r1, =datablock	@ output buffer address in r1
	mov	r2, #8		@ size of buffer in r2
	mov	r0, r3		@ filehandle
	swi	SWI_WriteBytes
	bcs	err06
	subs	r4, r4, #1
	bgt	loop

	mov	r0, r3
	swi	SWI_CloseFile	@ close the file
	bcs	err10

done:
	swi	SWI_Exit	@ exit

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
	.byte	0, 1, 2, 3, 4, 5, 6, 7

foerr:
	.asciz	"cannot open output file\r\n"
wrerr:
	.asciz	"error writing to file\r\n"
fcerr:
	.asciz	"cannot close file\r\n"

	.end
