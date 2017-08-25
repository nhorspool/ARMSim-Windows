	.equ	SWI_EXIT,		0x11

@these are the bits of the CPSR that represent the 2 interrupts
	.equ    IRQ_MODE,		0x40		@disable Interrupt Mode (IRQ)
	.equ    FIQ_MODE,		0x80		@disable Fast Interrupt Mode (FIQ)
	.equ 	NOINT,			(IRQ_MODE|FIQ_MODE)

@Pre-defined CPU mode constants
	.equ 	USERMODE,		0x10
	.equ 	FIQMODE,		0x11
	.equ 	IRQMODE,		0x12
	.equ 	SVCMODE,		0x13
	.equ 	ABORTMODE,		0x17
	.equ 	UNDEFMODE,		0x1b
	.equ	SYSTEMMODE,		0x1f
	.equ 	MODEMASK,		0x1f

@the direction the 8 segment display counts
	.equ	DIRECTION_UP,	+1
	.equ	DIRECTION_DOWN,	-1
				
	.global _start
	
@This must be memory location 0. If not, set main memory to start at 0
@in the preferences tab of ARMSim
@Setup interupt vectors
	ldr		pc, =Reset_Handler
	ldr		pc, =UndefinedHandler
	ldr		pc, =SWIHandler
	ldr		pc, =PrefetchAbortHandler
	ldr		pc, =DataAbortHandler
	.word	0							@Reserved vector
	ldr		pc, =IRQHandler
	ldr		pc, =FIQHandler

@skip enough space to set _start label to memory location 0x400
@Note , this is not required, just like to have code located at a nice even number
	.skip 1024-32

@entry point into program.
@make sure we have some stack space for irq and fiq modes.
@Note , this is only required if irq or fiq interrupt handlers use the stack.
@If they do not, no stack is required. We will set them up anyways just in case.
_start:
	bl		SetupStacks
	bl		LCD_Init
	bl		LED_Init
	bl		Slider_Init
	bl		BlackButton_Init
	bl		Timer_Init

@Display some strings on the lcd display
	mov		r0,#0
	mov		r1,#0
	ldr		r2,=LCD_InitString1
	bl		LCD_PutsXY
	
	mov		r0,#0
	mov		r1,#1
	ldr		r2,=LCD_InitString2
	bl		LCD_PutsXY

@left led off, right led on
	mov		r0,#RIGHT_LED
	bl		LED_Set
	
@Eight segment display a zero
	mov		r0,#0
	bl		Seg8_Digit
	
@setup timer0 for a period of approx 1 second
	mov		r0,#0
	mov		r1,#0xff00
	mov		r2,#0x0c00
	bl		Timer_Set
	
@setup timer1 for a period of approx 1/2 second
	mov		r0,#1
	ldr		r1,=(0xff00/2)
	mov		r2,#0x0c00
	bl		Timer_Set

@enable irq and fiq interrupts on the ARM processor
	mrs		r0,CPSR
	bic		r0,r0,#(IRQ_MODE | FIQ_MODE)
	msr		CPSR_cxsf, r0

@loop forever
loop:

@Check the last digit sent to the 8 segment display. If the current
@digit has changed then we must update the display.
	ldr		r3,=LastDigit
	ldr		r4,[r3]
	ldr		r5,=CurrDigit
	ldr		r6,[r5]
	cmp		r4,r6
	beq		main05

@Digit has changed, output new one.	
	mov		r0,r6
	bl		Seg8_Digit
	str		r6,[r3]

@Update the LCD display by shifting the cursor one position based on the direction.
	ldr		r0,=Direction
	ldr		r10,[r0]
	cmp		r10,#1
	bleq	LCD_ShiftCursorR		@shift cursor right if counting up
	blne	LCD_ShiftCursorL		@shift cursor left if counting down
	
main05:
@Check if the current LED pattern is different than the last one we output
@If changed, output new pattern.
	bl		LED_Get
	ldr		r5,=CurrLeds
	ldr		r6,[r5]
	cmp		r0,r6
	beq		main10
	
@Pattern has changed, output new pattern to LEDs and update current pattern.
	mov		r0,r6
	bl		LED_Set
	str		r6,[r5]

main10:
	bal		loop

@IRQ handler. Called when the IRQ interrupt fires.
@The IRQ line is hooked up to the black buttons. When one
@of the black buttons is pressed the IRQ fires. We need to inspect
@the button id register to determine which button was pressed.
@We also need to clear that bit.
@Note:registers r13 and r14 are banked for IRQ mode.
IRQHandler:
	stmfd	sp!,{r0-r2}
	
@point to button id register, get its value and test the bottom 2 bits
	ldr		r1,=BLACKBUTTONID
	ldr		r0,[r1]
	tst		r0,#RIGHT_BLACK_BUTTON

	ldr		r0,=Direction
	bne		irq05

@left button has been pressed.
@Clear left button bit in id register
@Set direction to DIRECTION_DOWN
	mov		r2,#LEFT_BLACK_BUTTON
	str		r2,[r1]

	mov		r1,#DIRECTION_DOWN
	str		r1,[r0]
	bal		irq99	

@right button has been pressed.
@Clear right button bit in id register
@Set direction to DIRECTION_UP
irq05:
	mov		r2,#RIGHT_BLACK_BUTTON
	str		r2,[r1]
	
	mov		r1,#DIRECTION_UP
	str		r1,[r0]
	bal		irq99	

@all done, return from interrupt
irq99:
	ldmfd	sp!,{r0-r2}
	movs	pc,lr


@FIQ handler. Called when a timer fires.
@The FIQ line is hooked up to the timers. When one
@of the timers fires the FIQ fires. We need to inspect
@the timer id register to determine which button was pressed.
@We also need to clear that bit.
@Note:registers r8 to r14 are banked for IRQ mode.
@Note:it is possible that both timers caused the int.
FIQHandler:
	@stmfd	sp!,{r0,lr}

@inspect the button id register and determine which one caused the int	
	ldr		r8,=TIMER_ID
	ldr		r9,[r8]
	tst		r9,#TIMER1_ID
	bne		fiq05
	
@timer2 has fired.
@Clear the timer2 id bit in the timer id register
	mov		r10,#TIMER2_ID
	str		r10,[r8]

@Invert the state of the 2 leds. We can do this by xor'ing the state by 0x03
@get the current led state value and invert the bits	
	ldr		r8,=CurrLeds
	ldr		r9,[r8]
	eor		r9,r9,#(LEFT_LED | RIGHT_LED)
	str		r9,[r8]
	bal		fiq99	

@timer1 has fired
@Clear the timer1 id bit in the timer id register
fiq05:
	mov		r10,#TIMER1_ID
	str		r10,[r8]

@Increment/Decrement the current digit in the 8 segment display based in the current direction.
@get the current direction value
	ldr		r8,=Direction
	ldr		r10,[r8]

@get current digit	
	ldr		r8,=CurrDigit
	ldr		r9,[r8]
	
@add the direction value and update current digit.
@make sure digit stays 0-9
	add		r9,r9,r10
	cmp		r9,#10
	movge	r9,#0
	cmp		r9,#0
	movlt	r9,#9
	str		r9,[r8]
	bal		fiq99

@return from interrupt
fiq99:	
	@ldmfd	sp!,{r0,lr}
	movs	pc,lr

@If any of these modes are invoked, halt the program.
Reset_Handler:
UndefinedHandler:
SWIHandler:
PrefetchAbortHandler:
DataAbortHandler:
	swi		SWI_EXIT

	
	.data
	.align

LastDigit:		.word		-1					@the last digit being displayed on the 8 segment display
CurrDigit:		.word		0					@the current digit to display on the 8 segment display
CurrLeds:		.word		RIGHT_LED			@the current led pattern to display
Direction:		.word		DIRECTION_UP		@the current direction we are counting

@some strings to display on the lcd
LCD_InitString1:		.asciz		"DaleIsHere?@#$%^"
LCD_InitString2:		.asciz		"Line2Isthisone`,"
	.end