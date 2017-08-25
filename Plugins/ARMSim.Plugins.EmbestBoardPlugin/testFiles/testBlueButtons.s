	.global _start

	.equ		SWI_PUT_LEDS,            0x201
	.equ		SWI_CHECK_BLUE_BUTTONS,  0x203
	.equ		SWI_PRINT_LCD_LINE,      0x204
	.equ		SWI_PRINT_LCD_INT,       0x205
	.equ		SWI_CLEAR_LCDSCREEN,     0x206
	.equ		SWI_CLEAR_LCDCHAR,       0x207
	.equ		SWI_CLEAR_LCDLINE,	     0x208
		
_start:

	mov		r0,#0
	mov		r1,#0
	ldr		r2,=init_msg
	bl		PrintLCDString

loop:
	bl		CheckBlueButtons		@ toss out any black buttons
	cmp		r0,#0
	beq		loop

	mov		r5,r0
	mov		r0,#3
	bl		ClearLCDLine

	clz		r5,r5
	mov		r6,#0x1f
	sub		r5,r6,r5
	
	mov		r8,r5,LSR#2
	and		r9,r5,#0x03
	
	mov		r0,#0
	mov		r1,#3
	ldr		r2,=btn_msg1
	bl		PrintLCDString
	
	mov		r0,#12
	mov		r1,#3
	add		r2,r8,#'0'
	bl		PrintLCDChar

	add		r0,r0,#1
	mov		r2,#','
	bl		PrintLCDChar

	add		r0,r0,#1
	add		r2,r9,#'0'
	bl		PrintLCDChar
	
	add		r0,r0,#1
	ldr		r2,=btn_msg2
	bl		PrintLCDString
	
	bal		loop	

PutLeds:
	swi		SWI_PUT_LEDS
	mov		pc,lr

CheckBlueButtons:
	swi		SWI_CHECK_BLUE_BUTTONS
	mov		pc,lr
	
PrintLCDString:
	swi		SWI_PRINT_LCD_LINE
	mov		pc,lr

ClearLCDLine:
	swi		SWI_CLEAR_LCDLINE
	mov		pc,lr

PrintLCDInt:
	swi		SWI_PRINT_LCD_INT
	mov		pc,lr

PrintLCDChar:
	swi		SWI_CLEAR_LCDCHAR
	mov		pc,lr
	
	.data
	.align

init_msg:	.asciz		"Press a blue button."

btn_msg1:	.asciz		"Blue Button "
btn_msg2:	.asciz		" has been pressed."

	.end
