	.equ	SLIDER_VALUE,		0x01a00000

	.global Slider_Init
	.global Slider_Read

@Initialize the slider control
Slider_Init:
	mov	pc,lr

@Read the current position of the slider.
@Returns - R0:slider position (0-255)
Slider_Read:
	stmfd	sp!,{r1,lr}
	ldr		r1,=SLIDER_VALUE
	ldr		r0,[r1]
	ldmfd	sp!,{r1,pc}

	.end
