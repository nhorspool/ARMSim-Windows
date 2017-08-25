
@disables IRQ,FIQ
.equ 	NOINT,		0xc0

@Pre-defined constants
.equ 	USERMODE,	0x10
.equ 	FIQMODE,	0x11
.equ 	IRQMODE,	0x12
.equ 	SVCMODE,	0x13
.equ 	ABORTMODE,	0x17
.equ 	UNDEFMODE,	0x1b
.equ	SYSTEMMODE,	0x1f
@.equ 	MODEMASK,	0x1f

	.text
	.global	_start

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
FIQHandler:
		b FIQHandler

@ handles the IRQ
IRQHandler:
		b IRQHandler


@handles the SWI
SWIHandler:
	stmfd	sp!,{r0-r6,lr}

	@both these returns are valid
	@movs	pc,r14
	@subs	pc,r14,#0

@	ldr	r0,=dale
@	ldr	r1,[r0]
@	orr	r1,r1,#0x00400000
@	str	r1,[r0]
@dale:
	ldmfd	sp!,{r0-r6,pc}^


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


_start:
	bl	SetupStacks
	swi	0x100
	ldr	r0,=thumbCode+1
	blx	r0

	b	.


	.thumb
thumbCode:
	swi	100
	bx	lr

	.arm


@stack space for cpu modes
@	.skip	256
@UserStack:
	.skip	256
SVCStack:
	.skip	256
FIQStack:
	.skip	256
IRQStack:
	.skip	256
UndefStack:

	.align
	.end
