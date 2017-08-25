	.global _start

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
	
	.equ		SWI_PUT_EIGHT_SEGMENT,   0x200
	.equ		SWI_PUT_LEDS,            0x201
	.equ		SWI_CHECK_BLACK_BUTTONS, 0x202
	.equ		SWI_CHECK_BLUE_BUTTONS,  0x203
	
	.equ		SWI_PRINT_LCD_LINE,      0x204
	.equ		SWI_PRINT_LCD_INT,       0x205
	.equ		SWI_CLEAR_LCDSCREEN,     0x206
	.equ		SWI_GET_TIMER15,         0x20a
		
_start:

	bl		ClearLCDScreen

	mov		r0,#0					@ both leds off
	bl		PutLeds

	mov		r0,#10					@ blank code
	mov		r1,#0					@ no dot
	bl		PutEightSegmentDisplay

	mov		r0,#0
	mov		r1,#0
	ldr		r2,=init_msg
	bl		PrintLCDString

	mov		r1,#1
	ldr		r2,=start_msg	
	bl		PrintLCDString
mn05:	
	bl		CheckBlackButtons		@ toss out any black buttons
	cmp		r0,#0
	beq		mn05
	
	mov		r0,#0
	mov		r1,#2
	ldr		r2,=run_msg1
	bl		PrintLCDString

	mov		r1,#3
	ldr		r2,=run_msg2
	bl		PrintLCDString

	mov		r9,#0
mn10:
	mov		r0,#500
	bl		Wait
	
	mov		r1,#0					@ no dot
	cmp		r9,#9
	movgt	r1,#1
	mov		r0,r9
	subgt	r0,r0,#10
	bl		PutEightSegmentDisplay

	add		r9,r9,#1
	cmp		r9,#21
	blt		mn10
	
	mov		r0,#10					@ blank code
	mov		r1,#0					@ no dot
	bl		PutEightSegmentDisplay

	mov		r0,#0
	mov		r1,#4
	ldr		r2,=init_msg
	bl		PrintLCDString

	swi		0x11

@Wait for R0 ms
Wait:
	stmfd		sp!,{r0-r3,lr}
	ldr			r3,=0x7fff
	mov			r2,r0
	swi			SWI_GET_TIMER15
	mov			r1,r0,LSL#17
	mov			r1,r1,ASR#17
wt05:
	swi			SWI_GET_TIMER15
	mov			r0,r0,LSL#17
	mov			r0,r0,ASR#17
	sub			r0,r0,r1
	and			r0,r0,r3
	cmp			r0,r2
	blt			wt05	
	ldmfd		sp!,{r0-r3,pc}

ClearLCDScreen:
	swi		SWI_CLEAR_LCDSCREEN
	mov		pc,lr

@ *** void Segdisplay (number:R0) ***
@ procedure to display a given integer between 0-9
@ on a 8 segment display.
@R0:digit to display 0-9. 10 will result in blank display
@R1:0 - no dot, 1 - dot
PutEightSegmentDisplay:
	stmfd		sp!,{r0,r2,lr}
	ldr			r2,=digits		@ get byte value equivalent to 
	ldr			r0,[r2,r0,lsl#2]	@ number from digits array	
	cmp			r1,#0
	orrne		r0,r0,#SEG_P
	swi			SWI_PUT_EIGHT_SEGMENT
	ldmfd		sp!,{r0,r2,pc}

PutLeds:
	swi		SWI_PUT_LEDS
	mov		pc,lr

CheckBlackButtons:
	swi		SWI_CHECK_BLACK_BUTTONS
	mov		pc,lr
	
PrintLCDString:
	swi		SWI_PRINT_LCD_LINE
	mov		pc,lr

	.data
	.align

digits:	
		.word	SEG_A|SEG_B|SEG_C|SEG_D|SEG_E|SEG_G		@0
		.word	SEG_B|SEG_C					@1
		.word	SEG_A|SEG_B|SEG_F|SEG_E|SEG_D			@2
		.word	SEG_A|SEG_B|SEG_F|SEG_C|SEG_D			@3
		.word	SEG_G|SEG_F|SEG_B|SEG_C				@4
		.word	SEG_A|SEG_G|SEG_F|SEG_C|SEG_D			@5
		.word	SEG_A|SEG_G|SEG_F|SEG_E|SEG_D|SEG_C		@6
		.word	SEG_A|SEG_B|SEG_C				@7
		.word	SEG_A|SEG_B|SEG_C|SEG_D|SEG_E|SEG_F|SEG_G	@8
		.word	SEG_A|SEG_B|SEG_F|SEG_G|SEG_C			@9
		.word	0						@Blank

init_msg:	.asciz		"8segment display should be blank."
start_msg:	.asciz		"Press a black button to start test."
run_msg1:	.asciz		"8segment display should be counting up."
run_msg2:	.asciz		"The dot should be off 0-9 and on 10-19."

	.end
