	.equ	SWI_EXIT,		0x11

	@these are the bits of the CPSR that represent the 2 interrupts
	.equ    IRQ_MODE,		0x40		@disable Interrupt Mode (IRQ)
	.equ    FIQ_MODE,		0x80		@disable Fast Interrupt Mode (FIQ)
	.equ 	NOINT,			(IRQ_MODE|FIQ_MODE)

	@Pre-defined CPU mode constants
	@.equ 	USERMODE,		0x10
	@.equ 	FIQMODE,		0x11
	@.equ 	IRQMODE,		0x12
	@.equ 	SVCMODE,		0x13
	@.equ 	ABORTMODE,		0x17
	@.equ 	UNDEFMODE,		0x1b
	@.equ	SYSTEMMODE,		0x1f
	@.equ 	MODEMASK,		0x1f

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
	bl		LCD_Init

	@enable irq and fiq interrupts on the ARM processor
	mrs		r0,CPSR
	bic		r0,r0,#(IRQ_MODE | FIQ_MODE)
	msr		CPSR_cxsf, r0
	
	bl		Test1
	bl		Wait
	bl		Test2
	swi		SWI_EXIT

Test1:
	stmfd	sp!,{r0-r1,lr}
	bl		LCD_Clear
	mov		r0,#0
	mov		r1,#0
	ldr		r2,=LCD_InitString1
	bl		LCD_PutsXY

	mov		r0,#0
	mov		r1,#1
	ldr		r2,=LCD_InitString2
	bl		LCD_PutsXY
	ldmfd	sp!,{r0-r1,pc}

Test2:
	stmfd	sp!,{r0-r1,lr}
	bl		LCD_Clear
	mov		r0,#'a'
	bl		LCD_Putc
	bl		LCD_ShiftCursorR
	mov		r0,#'b'
	bl		LCD_Putc
	bl		LCD_ShiftCursorR
	mov		r0,#'c'
	bl		LCD_Putc
	bl		LCD_ShiftCursorL
	bl		LCD_ShiftCursorL
	mov		r0,#'d'
	bl		LCD_Putc
	ldmfd	sp!,{r0-r1,pc}

Wait:
	stmfd	sp!,{r0-r1,lr}
	ldr		r0,=Flag
	mov		r1,#0
	str		r1,[r0]
wat05:
	ldr		r1,[r0]
	cmp		r1,#0
	beq		wat05
	ldmfd	sp!,{r0-r1,pc}

@IRQ handler. Called when the IRQ interrupt fires.
@The IRQ line is hooked up to the black buttons. When one
@of the black buttons is pressed the IRQ fires. We need to inspect
@the button id register to determine which button was pressed.
@We also need to clear that bit.
@Note:registers r13 and r14 are banked for IRQ mode.
IRQHandler:
	stmfd	sp!,{r0-r1}
	ldr		r0,=Flag
	mov		r1,#1
	str		r1,[r0]
	ldmfd	sp!,{r0-r1}
	movs	pc,lr
FIQHandler:
	movs	pc,lr
	
	.data	
Flag:	.word		0
LCD_InitString1:		.asciz		"DaleIsHere?@#$%^"
LCD_InitString2:		.asciz		"Line2Isthisone`,"
	.end
