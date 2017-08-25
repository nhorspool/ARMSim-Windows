	.equ	SWI_EXIT,		0x11

	.equ	LCD_COMMAND,	0x02f00000		@Register for LCD commands
	.equ	LCD_DATA,		0x02f00004		@Register for LCD data

@http://www.electronics123.net/amazon/datasheet/MTC-C162DPRN-2N_V10_.pdf
@http://www.electronics123.com/s.nl/it.A/id.53/.f

	.global LCD_Init
	.global LCD_Clear
	.global LCD_ClearLine
	.global LCD_Home
	.global LCD_ShiftCursorR
	.global LCD_ShiftCursorL
	.global LCD_Goto
	.global LCD_Putc
	.global LCD_PutcXY
	.global LCD_Puts
	.global LCD_PutsXY

@Clear the lcd display line
@R0:line to clear (0-1)
LCD_ClearLine:
	stmfd	sp!,{r0-r3,lr}
	mov		r1,r0
	mov		r0,#0
	bl		LCD_Goto
	
	mov		r3,#16
	mov		r2,#' '
lcrl05:
	bl		LCD_PutcXY
	add		r0,r0,#1
	subs	r3,r3,#1
	bgt		lcrl05
	ldmfd	sp!,{r0-r3,pc}

@Shift the LCD cursor to the right 1 position
LCD_ShiftCursorR:
	stmfd	sp!,{r0-r1,lr}
	ldr		r0,=LCD_COMMAND
	mov		r1,#0x14
	str		r1,[r0]
	ldmfd	sp!,{r0-r1,pc}

@Shift the LCD cursor to the left 1 position
LCD_ShiftCursorL:
	stmfd	sp!,{r0-r1,lr}
	ldr		r0,=LCD_COMMAND
	mov		r1,#0x10
	str		r1,[r0]
	ldmfd	sp!,{r0-r1,pc}

@Clear the LCD display
LCD_Clear:
	stmfd	sp!,{r0-r1,lr}
	ldr		r0,=LCD_COMMAND
	mov		r1,#0x01
	str		r1,[r0]
	ldmfd	sp!,{r0-r1,pc}

@Move the LCD cursor to line 0, position 0
LCD_Home:
	stmfd	sp!,{r0-r1,lr}
	ldr		r0,=LCD_COMMAND
	mov		r1,#0x02
	str		r1,[r0]
	ldmfd	sp!,{r0-r1,pc}

@Move the cursor to the given position
@R0:xpos(0-15)
@R1:ypos(0-1)
LCD_Goto:
	stmfd	sp!,{r2-r3,lr}

@compute the lcd address of the cursor position.
@Characters  0   1   2   3   4   5   6   7   8   9   10  11  12  13  14  15
@First line  00H 01H 02H 03H 04H 05H 06H 07H 08H 09H 0AH 0BH 0CH 0DH 0EH 0FH
@Second line 40H 41H 42H 43H 44H 45H 46H 47H 48H 49H 4AH 4BH 4CH 4DH 4EH 4FH
	mov		r2,#0
	cmp		r1,#0
	movne	r2,#0x40
	and		r3,r0,#0x0f
	orr		r2,r2,r3
	orr		r2,r2,#0x80
	
	ldr		r3,=LCD_COMMAND
	str		r2,[r3]

	ldmfd	sp!,{r2-r3,pc}

@Put a character at the current cursor location
@R0:char to put
LCD_Putc:
	stmfd	sp!,{r1,lr}
	ldr		r1,=LCD_DATA
	str		r0,[r1]
	ldmfd	sp!,{r1,pc}

@Put a character at the given position.
@Note:the cursor is moved
@R0:xpos(0-15)
@R1:ypos(0-1)
@R2:char(0-0x7f)
LCD_PutcXY:
	stmfd	sp!,{r0,lr}
	bl	LCD_Goto
	mov	r0,r2
	bl	LCD_Putc
	ldmfd	sp!,{r0,pc}

@Put a string at the cursor location
@R0:ptr to string(null terminated)
LCD_Puts:
	stmfd	sp!,{r0-r1,lr}
	mov		r1,r0
lcdp05:
	ldrb	r0,[r1],#1
	cmp		r0,#0
	beq		lcdp10
	bl		LCD_Putc
	bal		lcdp05
lcdp10:
	ldmfd	sp!,{r0-r1,pc}

@Put a string at the given position.
@R0:xpos(0-15)
@R1:ypos(0-1)
@R2:ptr to string(null terminated)
LCD_PutsXY:
	stmfd	sp!,{r0,lr}
	bl		LCD_Goto
	mov		r0,r2
	bl		LCD_Puts
	ldmfd	sp!,{r0,pc}

@Initialize the LCD display.
LCD_Init:
	stmfd	sp!,{r0-r1,lr}
	ldr		r0,=LCD_COMMAND
	mov		r1,#0x06			@Entry Mode:Cursor shift right, Display not shifted
	@mov	r1,#0x04			@Entry Mode:Cursor shift left, Display not shifted
	str		r1,[r0]
	
	mov		r1,#0x0f			@Display:Display On, Cursor On, Cursor Blink
	@mov	r1,#0x0c			@Display:Display On, Cursor Off, Cursor No-Blink
	str		r1,[r0]
	bl		LCD_Home
	ldmfd	sp!,{r0-r1,pc}
	.end