	.equ	SWI_EXIT,		0x11

	@these are the bits of the CPSR that represent the 2 interrupts
	.equ    IRQ_MODE,		0x40		@disable Interrupt Mode (IRQ)
	.equ    FIQ_MODE,		0x80		@disable Fast Interrupt Mode (FIQ)
	.equ 	NOINT,			(IRQ_MODE|FIQ_MODE)

	.global _start
	@This must be memory location 0. If not, set main memory to start at 0
	@in the preferences tab of ARMSim

	@Setup interupt vectors
	.word	0
	.word	0
	.word	0
	.word	0
	.word	0
	.word	0
	ldr		pc, =IRQHandler
	ldr		pc, =FIQHandler

	@skip enough space to set _start label to memory location 0x400
	@Note , this is not required, just like to have code located at a nice even number
	.skip 1024-32

@entry point into program.
_start:
	bl		SetupStacks
	bl		LED_Init
	bl		BlackButton_Init

	@enable irq and fiq interrupts on the ARM processor
	mrs		r0,CPSR
	bic		r0,r0,#(IRQ_MODE | FIQ_MODE)
	msr		CPSR_cxsf, r0
	
loop:
	bal		loop	
	swi		SWI_EXIT

@IRQ handler. Called when the IRQ interrupt fires.
@The IRQ line is hooked up to the black buttons. When one
@of the black buttons is pressed the IRQ fires. We need to inspect
@the button id register to determine which button was pressed.
@We also need to clear that bit.
@Note:registers r13 and r14 are banked for IRQ mode.
IRQHandler:
	stmfd	sp!,{r0-r1,lr}
	
@point to button id register, get its value and test the bottom 2 bits
	ldr		r1,=BLACKBUTTONID
	ldr		r0,[r1]
	tst		r0,#RIGHT_BLACK_BUTTON
	bne		irq05

@left button has been pressed.
@Clear left button bit in id register
@Turn on left LED, turn off right LED
	mov		r0,#LEFT_BLACK_BUTTON
	str		r0,[r1]
	mov		r0,#LEFT_LED
	bal		irq99	

@right button has been pressed.
@Clear right button bit in id register
@Turn on right LED, turn off left LED
irq05:
	mov		r0,#RIGHT_BLACK_BUTTON
	str		r0,[r1]
	mov		r0,#RIGHT_LED

@all done, return from interrupt
irq99:
	bl		LED_Set

	ldmfd	sp!,{r0-r1,lr}
	movs	pc,lr

FIQHandler:
	movs	pc,lr

	.data
Flag:	.word	0

	.end