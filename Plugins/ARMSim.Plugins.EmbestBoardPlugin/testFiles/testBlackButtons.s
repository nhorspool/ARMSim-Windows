	.global _start

	.equ		SWI_PUT_LEDS,            0x201
	.equ		SWI_CHECK_BLACK_BUTTONS, 0x202
	.equ		SWI_PRINT_LCD_LINE,      0x204
	.equ		SWI_CLEAR_LCDSCREEN,     0x206
	.equ		SWI_CLEAR_LCDLINE,	     0x208
		
_start:

	mov		r0,#0
	mov		r1,#0
	ldr		r2,=init1_msg
	bl		PrintLCDString

	mov		r1,#1
	ldr		r2,=init2_msg
	bl		PrintLCDString

	mov		r1,#2
	ldr		r2,=init3_msg
	bl		PrintLCDString

	mov		r1,#3
	ldr		r2,=init4_msg
	bl		PrintLCDString
	
loop:
	bl		CheckBlackButtons		@ toss out any black buttons
	cmp		r0,#0
	beq		loop

	mov		r1,#0
	tst		r0,#0x02				@left button?
	ldrne	r2,=btnleft_msg
	ldrne	r3,=ledleft_msg
	orrne	r1,r1,#0x02

	tst		r0,#0x01				@right button?
	ldrne	r2,=btnright_msg
	ldrne	r3,=ledright_msg
	orrne	r1,r1,#0x01

	mov		r0,r1
	bl		PutLeds
	
	mov		r0,#6
	bl		ClearLCDLine
	mov		r0,#7
	bl		ClearLCDLine
	
	mov		r0,#0
	mov		r1,#6
	bl		PrintLCDString

	mov		r0,#0
	mov		r1,#7
	mov		r2,r3
	bl		PrintLCDString
	
	bal		loop	

PutLeds:
	swi		SWI_PUT_LEDS
	mov		pc,lr

CheckBlackButtons:
	swi		SWI_CHECK_BLACK_BUTTONS
	mov		pc,lr
	
PrintLCDString:
	swi		SWI_PRINT_LCD_LINE
	mov		pc,lr

ClearLCDLine:
	swi		SWI_CLEAR_LCDLINE
	mov		pc,lr
	
	.data
	.align

init1_msg:	.asciz		"Press Left Black Button"
init2_msg:	.asciz		"   TO turn on Left LED."
init3_msg:	.asciz		"Press Right Black Button"
init4_msg:	.asciz		"   To turn on Right LED."

btnleft_msg:	.asciz		"Left Button has been pressed."
btnright_msg:	.asciz		"Right Button has been pressed."

ledleft_msg:	.asciz		"Left LED should be ON."
ledright_msg:	.asciz		"Right LED should be ON."

	.end
