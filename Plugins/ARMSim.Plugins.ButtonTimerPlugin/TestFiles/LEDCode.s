	@the led bit values and the memory location for the led bits
	.equ	RIGHT_LED,		0x01
	.equ	LEFT_LED,		0x02
	.equ	LEDS,			0x01d20000

	.global	RIGHT_LED
	.global	LEFT_LED
	
	.global LED_Init
	.global LED_Get
	.global LED_Set
	.global LED_Left
	.global LED_Right

@Initialize the LEDs (turn both off)
LED_Init:
	stmfd	sp!,{r0,lr}
	mov		r0,#0
	bl		LED_Set
	ldmfd	sp!,{r0,pc}

@Set the left LED either on or off
@Does not affect right LED
@R0: 0:off, non-0:on
LED_Left:
	stmfd	sp!,{r0-r1,lr}
	mov		r1,r0
	bl		LED_Get
	bic		r0,r0,#LEFT_LED
	cmp		r1,#0
	orrne	r0,r0,#LEFT_LED
	bl		LED_Set
	ldmfd	sp!,{r0-r1,pc}

@Set the right LED either on or off
@Does not affect left LED
@R0: 0:off, non-0:on
LED_Right:
	stmfd	sp!,{r0-r1,lr}
	mov		r1,r0
	bl		LED_Get
	bic		r0,r0,#RIGHT_LED
	cmp		r1,#0
	orrne	r0,r0,#RIGHT_LED
	bl		LED_Set
	ldmfd	sp!,{r0-r1,pc}

@Get the state of the 2 LEDs
@Returns - R0:state of LEDs(bottom 2 bits)
LED_Get:
	ldr		r0,=LEDS
	ldr		r0,[r0]
	mov		pc,lr

@Set the state of the 2 LEDs
@R0:pattern
LED_Set:
	stmfd	sp!,{r1,lr}
	ldr		r1,=LEDS
	str		r0,[r1]
	ldmfd	sp!,{r1,pc}
	
	.end