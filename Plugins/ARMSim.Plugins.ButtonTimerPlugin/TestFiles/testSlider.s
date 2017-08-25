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
	bl		LCD_Init
	bl		LED_Init
	bl		Slider_Init

@enable irq and fiq interrupts on the ARM processor
	mrs		r0,CPSR
	bic		r0,r0,#(IRQ_MODE | FIQ_MODE)
	msr		CPSR_cxsf, r0

	mov		r8,#-1
loop:
	ldr		r0,=Flag
	ldr		r1,[r0]
	cmp		r1,#0
	bne		done

	bl		Slider_Read

	mov		r2,#0x00
	ldr		r3,=Slider64
	cmp		r0,#0x40
	blt		lp05

	mov		r2,#0x01
	ldr		r3,=Slider128
	cmp		r0,#0x80
	blt		lp05

	mov		r2,#0x02
	ldr		r3,=Slider192
	cmp		r0,#0xc0
	blt		lp05

	mov		r2,#0x03
	ldr		r3,=Slider256

lp05:
	cmp		r2,r8
	beq		loop
	mov		r8,r2
		
	mov		r0,r2
	bl		LED_Set

	mov		r0,#1
	bl		LCD_ClearLine	
	
	mov		r0,#0
	mov		r1,#1
	mov		r2,r3
	bl		LCD_PutsXY
	
	bal		loop
	
done:
	swi		SWI_EXIT

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
Flag:	.word	0

Slider64:	.asciz	"Slider:0-63"
Slider128:	.asciz	"Slider:64-127"
Slider192:	.asciz	"Slider:128-191"
Slider256:	.asciz	"Slider:193-255"

	.end