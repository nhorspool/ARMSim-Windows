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

	.global SetupStacks

@Setup the CPU mode stacks for fiq and irq modes. leave cpu in system mode
@Note , also leaves irq and fiq inrterrupts disabled.
SetupStacks:

@disable irq/fiq
	mrs		r0,cpsr
	bic		r0,r0,#0xff
	orr		r0,r0,#NOINT

@setup irq mode stack
	orr		r1,r0,#IRQMODE
	msr		cpsr_c,r1
	ldr		sp,=IRQStack

@setup fiq mode stack
	orr		r1,r0,#FIQMODE
	msr		cpsr_c,r1
	ldr		sp,=FIQStack

@setup system mode stack
	orr		r1,r0,#SYSTEMMODE
	msr		cpsr_c,r1
	mov		pc,lr
	
	.data	
@reserve space for the IRQ interrupt mode
	.skip	256
IRQStack:

@reserve space for the FIQ interrupt mode
	.skip	256
FIQStack:
	.end