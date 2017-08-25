	@Eight segment display constants
	@Each entry is the bit pattern for a specific segment
	.equ	SEG_A,		0x80
	.equ	SEG_B,		0x40
	.equ	SEG_C,		0x20
	.equ	SEG_D,		0x08
	.equ	SEG_E,		0x04
	.equ	SEG_F,		0x02
	.equ	SEG_G,		0x01

	@Memory address of eight segment display
	.equ	ADDR_EightSegment,	0x02140000

	.text
	.global _start
	
_start:

	mov	r0,#5
	bl	Segdisplay

	swi	0x11




@ *** void Segdisplay (number:R0) ***
@ procedure to display a given integer between 0-9
@ on a 8 segment display either in ARMSim or 
@ or on the board
Segdisplay:
	stmfd	sp!,{r0-r1,lr}
	ldr	r1,=digits		@ get byte value equivalent to 
	ldr	r0,[r1,r0,lsl#2]	@ number from digits array	
	ldr	r1,=ADDR_EightSegment
	str	r0,[r1]
	ldmfd	sp!,{r0-r1,pc}


		.data
		.align
digits:	
		.word	SEG_A|SEG_B|SEG_C|SEG_D|SEG_E|SEG_G		@0
		.word	SEG_B|SEG_C										@1
		.word	SEG_A|SEG_B|SEG_F|SEG_E|SEG_D				@2
		.word	SEG_A|SEG_B|SEG_F|SEG_C|SEG_D				@3
		.word	SEG_G|SEG_F|SEG_B|SEG_C						@4
		.word	SEG_A|SEG_G|SEG_F|SEG_C|SEG_D				@5
		.word	SEG_A|SEG_G|SEG_F|SEG_E|SEG_D|SEG_C		@6
		.word	SEG_A|SEG_B|SEG_C								@7
		.word	SEG_A|SEG_B|SEG_C|SEG_D|SEG_E|SEG_F|SEG_G	@8
		.word	SEG_A|SEG_B|SEG_F|SEG_G|SEG_C				@9
		
		.end
