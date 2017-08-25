	.equ	SWI_EXIT,		0x11
	.equ	SWI_GETTICKS,		0x6d

	.global _start
_start:

	ldr		r3,=1234

	LOG2I	r2,r3
	ALOG2I	r4,r2

	swi	0x11

	.end

