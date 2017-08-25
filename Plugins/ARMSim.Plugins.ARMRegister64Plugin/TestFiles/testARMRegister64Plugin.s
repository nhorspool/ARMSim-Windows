	.equ	SWI_EXIT,		0x11
	.equ	SWI_GETTICKS,		0x6d

	.global _start
_start:
	ldr	r3,=0x00000001
	ldr	r4,=0x00001234

	@LOAD64  r3
	@.word	0xeef000f3
	LOAD64	r3

	@STORE64 r7
	@.word	0xeef001f7
	STORE64 r7

	ldr	r9,=0xffffffff
	@ADDS64 r9
	@.word	0xeef002f9
	ADDS64 r9

	ldr	r10,=0x00000001
	@ADD64 r10
	@.word	0xeef003fa
	ADD64 r10


	ldr	r2,=-1
	ldr	r3,=-2
	@MULS64  r2
	@.word	0xeef004f2
	MULS64  r2

	swi	0x11



	.end

