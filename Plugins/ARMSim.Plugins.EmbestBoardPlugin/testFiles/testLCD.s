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

	mov		r0,#0
	mov		r1,#14
	ldr		r2,=init1_msg
	bl		PrintLCDString

	mov		r0,#0
	mov		r1,#4
	ldr		r2,=init2_msg
	bl		PrintLCDString

	mov		r0,#0
	mov		r1,#8
	ldr		r2,=init3_msg
	bl		PrintLCDString

	swi		0x11
	
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

init1_msg:	.asciz		"This string is exactly 40 chars in width"

init2_msg:	.asciz		"The top line should be on line#0"
init3_msg:	.asciz		"The bottom line should be on line#14"

	.end
