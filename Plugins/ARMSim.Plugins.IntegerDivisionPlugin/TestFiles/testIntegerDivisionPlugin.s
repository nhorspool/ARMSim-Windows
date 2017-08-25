	.equ	SWI_EXIT,		0x11
	.global _start
_start:
	ldr	r3,=0x00000012
	ldr	r4,=0x00001234

	UDIV	r5,r4,r3
	SDIV	r6,r4,r3

	ldr	r3,=-3
	ldr	r4,=-9
	UDIV	r5,r4,r3
	SDIV	r6,r4,r3

	swi	SWI_EXIT

	.end