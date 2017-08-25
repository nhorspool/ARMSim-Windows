	@Eight segment display constants
	@Each entry is the bit pattern for a specific segment
	.equ	SEG_A,		0x80
	.equ	SEG_B,		0x40
	.equ	SEG_C,		0x20
	.equ	SEG_P,		0x10
	.equ	SEG_D,		0x08
	.equ	SEG_E,		0x04
	.equ	SEG_F,		0x02
	.equ	SEG_G,		0x01

	@The memory location for the eight segment display segments
	.equ	EIGHTSEGMENT,	0x01d10000

	.global Seg8_Init
	.global Seg8_Set
	.global Seg8_Digit
	.global Seg8_Point

@Init the display. Set all segments off (blank)
Seg8_Init:
	stmfd	sp!,{r0-r1,lr}
	mov	r0,#0
	bl	Seg8_Set
	ldmfd	sp!,{r0-r1,pc}

@Turn the point segment on or off.
@R0:0:point off, non-0:point on
Seg8_Point:
	stmfd	sp!,{r0,r1,lr}
	mov		r1,#0
	cmp		r0,#0
	movne	r1,#SEG_P
	bl		Seg8_Get
	orr		r0,r0,r1
	bl		Seg8_Set	
	ldmfd	sp!,{r0,r1,pc}

@Set the digit(0-9 or blank) to the display.
@The pattern to form the digit is located in the data section.
@R0:digit to display(0-9), use a 10 to display a blank
Seg8_Digit:
	stmfd	sp!,{r0,r1,lr}
	ldr		r1,=digits			@ get byte value equivalent to 
	ldr		r0,[r1,r0,lsl#2]	@ number from digits array
	bl		Seg8_Set	
	ldmfd	sp!,{r0,r1,pc}

@Get the current pattern being displayed
@Returns - @R0:pattern
Seg8_Get:
	ldr	r0,=EIGHTSEGMENT
	ldr	r0,[r0]
	mov	pc,lr

@Set the pattern to display
@R0:pattern to display (lower 8 bits)
Seg8_Set:
	stmfd	sp!,{r1,lr}
	ldr	r1,=EIGHTSEGMENT
	str	r0,[r1]
	ldmfd	sp!,{r1,pc}

	.data
@this table holds the segments required to form the digits 0 to 9
@the 10th entry is to display a blank
digits:	
	.word	SEG_A|SEG_B|SEG_C|SEG_D|SEG_E|SEG_G			@0
	.word	SEG_B|SEG_C									@1
	.word	SEG_A|SEG_B|SEG_F|SEG_E|SEG_D				@2
	.word	SEG_A|SEG_B|SEG_F|SEG_C|SEG_D				@3
	.word	SEG_G|SEG_F|SEG_B|SEG_C						@4
	.word	SEG_A|SEG_G|SEG_F|SEG_C|SEG_D				@5
	.word	SEG_A|SEG_G|SEG_F|SEG_E|SEG_D|SEG_C			@6
	.word	SEG_A|SEG_B|SEG_C							@7
	.word	SEG_A|SEG_B|SEG_C|SEG_D|SEG_E|SEG_F|SEG_G	@8
	.word	SEG_A|SEG_B|SEG_F|SEG_G|SEG_C				@9
	.word	0											@blank
	
	.end
