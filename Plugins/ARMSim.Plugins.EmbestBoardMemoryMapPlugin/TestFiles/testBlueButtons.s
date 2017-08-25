

.equ 	NOINT,		0xc0
.equ    IRQ_MODE,	0x40       /* disable Interrupt Mode (IRQ) */
.equ    FIQ_MODE,	0x80       /* disable Fast Interrupt Mode (FIQ) */

@Pre-defined constants
.equ 	USERMODE,	0x10
.equ 	FIQMODE,	0x11
.equ 	IRQMODE,	0x12
.equ 	SVCMODE,	0x13
.equ 	ABORTMODE,	0x17
.equ 	UNDEFMODE,	0x1b
.equ	SYSTEMMODE,	0x1f
@.equ 	MODEMASK,	0x1f

	.equ	keyboard_base,	0x06000000

	.equ	rEXTINT,		0x1d20050
	.equ	rEXTINTPND,		0x1d20054
	.equ	rPCONG,			0x1d20040
	.equ	rPDATG,			0x1d20044
	.equ	rPUPG,			0x1d20048

	.equ	BIT_EINT4567,		(0x1<<21)
	.equ	BIT_EINT1,		(0x1<<24)

	@INTERRUPT
	.equ	rINTCON,	0x1e00000
	.equ	rINTPND,	0x1e00004
	.equ	rINTMOD,	0x1e00008
	.equ	rINTMSK,	0x1e0000c
	.equ	rINTISPR,	0x01e00020
	
	.equ	rI_PSLV,	0x1e00010
	.equ	rI_PMST,	0x1e00014
	.equ	rI_CSLV,	0x1e00018
	.equ	rI_CMST,	0x1e0001c
	.equ	rI_ISPR,	0x1e00020
	.equ	rI_ISPC,	0x1e00024

	.equ	BIT_GLOBAL,		(0x1<<26)

	.text
	.global _start
	
	@ need to setup interupt vectors
        LDR     PC, =Reset_Handler
        LDR     PC, =UndefinedHandler
        LDR     PC, =SWIHandler
        LDR     PC, =PrefetchAbortHandler
        LDR     PC, =DataAbortHandler
	.word	0			/* Reserved vector */
        LDR     PC, =IRQHandler
        LDR     PC, =FIQHandler

@these handlers do nothing for now
Reset_Handler:
UndefinedHandler:
PrefetchAbortHandler:
DataAbortHandler:
@handles the SWI
SWIHandler:
FIQHandler:
		b FIQHandler

InteruptHandlerTable:
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
	.word	HandleDummy
HandleDummy:
	movs	pc,lr

@ handles the IRQ
IRQHandler:
	@must determine which interupt caused the IRQ
	sub	sp,sp,#4
	stmfd	sp!,{r0-r1}

	ldr	r0,=rINTISPR
	ldr	r0,[r0]
	mov	r1,#0
irqh05:
	movs	r0,r0,lsr#1
	bcs	irqh10
	add	r1,r1,#4
	b	irqh05
irqh10:
	ldr	r0,=InteruptHandlerTable
	add	r0,r0,r1
	ldr	r0,[r0]
	str	r0,[sp,#8]
	ldmfd	sp!,{r0-r1,pc}


_start:
	bl	SetupStacks

	@PORT G
	ldr	r0,=rPCONG
	ldr	r1,=0xffff
	str	r1,[r0]

	ldr	r0,=rPUPG
	ldr	r1,=0x00			@pull up enable
	str	r1,[r0]

	ldr	r0,=rEXTINT
	ldr	r1,[r0]
	orr	r1,r1,#0x20			@EINT1 falling edge mode
	str	r1,[r0]

	ldr	r0,=blue_key
	mov	r1,#0
	str	r1,[r0]

	@all interrupts are set to generate an IRQ	
	ldr		r0,=rINTMOD
	ldr		r1,=0x00
	str		r1,[r0]

	@non-vector mode, IRQ enabled, FIQ disabled
	ldr		r0,=rINTCON
	ldr		r1,=0x05
	str		r1,[r0]

	@clear pending bit
	ldr	r0,=(BIT_EINT1|BIT_EINT4567)
	bl	interrupt_clear_pending
	
	@clear EXTINTPND reg
	ldr	r0,=rEXTINTPND
	ldr	r1,=0xf
	str	r1,[r0]

	@set black keys interrupt handler
	ldr	r0,=BIT_EINT1
	ldr	r1,=keyboard_blue_int
	bl	interrupt_setvector

	@enable interrupts in controller
	ldr	r0,=(BIT_EINT1|BIT_EINT4567)
	bl	interrupt_enable

	@enable global interrupts
	ldr		r0,=BIT_GLOBAL
	bl		interrupt_enable

	@enable irq, fiq interupts
	MRS		r0, CPSR
	BIC		r0, r0, #NOINT /* enable interrupt */
	MSR		CPSR_cxsf, r0

	ldr	sp,=SVCStack

loop:
	ldr	r0,=blue_key
	ldr	r1,[r0]
	cmp		r1,#0
	beq	loop

	swi	0x11


@R0:interrupt index to set
@R1:vector to set	
interrupt_setvector:
	stmfd		sp!,{r0-r2,lr}

	mov		r2,#0
isv05:
	movs		r0,r0,ror#1
	addcc		r2,r2,#1
	bcc		isv05
	
	ldr		r0,=InteruptHandlerTable
	str		r1,[r0,r2,lsl#2]

	ldmfd	sp!,{r0-r2,pc}


@Clears a specific set of pending interrupts
@R0:bits set will clear corresponding interrupts
interrupt_clear_pending:
	stmfd		sp!,{r1,lr}
	ldr		r1,=rI_ISPC
	str		r0,[r1]
	ldmfd		sp!,{r1,pc}


keyboard_blue_int:
	stmfd	sp!,{r0-r4,lr}

	@clear pending bit
	ldr		r0,=BIT_EINT1
	bl		interrupt_clear_pending
	
	mov		r2,#1
	mov		r1,#2
	ldr		r0,=(keyboard_base+0xfd)
kbi05:
	ldrb	r3,[r0]
	and		r3,r3,#0x0f
	cmp		r3,#0x0f
	beq		kbi10
		
	mov		r3,r3,lsl#28
	mov		r4,#4
kbi15:
	movs	r3,r3,lsl#1
	bcc		kbi99
	mov		r2,r2,lsl#1
	subs	r4,r4,#1
	bne		kbi15	
kbi10:
	mov		r2,r2,lsl#4
	sub		r0,r0,r1
	mov		r1,r1,lsl#1
	cmp		r1,#0x10
	ble		kbi05
	b		kbi98
	;;mov		r2,#0
kbi99:
	ldr		r0,=blue_key
	str		r2,[r0]
	
kbi98:
	ldmfd	sp!,{r0-r4,lr}
	subs	pc,lr,#4


@Enable a set of interrupts
@R0:bits set will enable corresponding interrupts
interrupt_enable:
	stmfd	sp!,{r0-r2,lr}
	ldr		r1,=rINTMSK
	ldr		r2,[r1]
	mvn		r0,r0
	and		r2,r2,r0
	str		r2,[r1]
	ldmfd	sp!,{r0-r2,pc}

SetupStacks:
	stmfd	sp!,{r0-r1,lr}
	mrs	r0,cpsr
	bic	r0,r0,#0xff
	orr	r0,r0,#NOINT

	orr	r1,r0,#UNDEFMODE
	msr	cpsr_c,r1
	ldr	sp,=UndefStack

	orr	r1,r0,#IRQMODE
	msr	cpsr_c,r1
	ldr	sp,=IRQStack

	orr	r1,r0,#FIQMODE
	msr	cpsr_c,r1
	ldr	sp,=FIQStack

	orr	r1,r0,#SVCMODE
	msr	cpsr_c,r1
	ldr	sp,=SVCStack

	orr	r1,r0,#SYSTEMMODE
	msr	cpsr_c,r1
	ldmfd	sp!,{r0-r1,pc}

		.data
		.align
blue_key:	.word	0

		.data

		.skip	256
SVCStack:
		.skip	256
UndefStack:
		.skip	256
IRQStack:
		.skip	256
FIQStack:

		.end
