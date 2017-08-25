	.text
	.global SetupStacks

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

	orr	r1,r0,#SYSTEMMODE
	msr	cpsr_c,r1
	ldmfd	sp!,{r0-r1,pc}


	.data

	@stack space for cpu modes

	.skip	256
UserStack:
	.skip	256
FIQStack:
	.skip	256
IRQStack:
	.skip	256
UndefStack:

	.end
