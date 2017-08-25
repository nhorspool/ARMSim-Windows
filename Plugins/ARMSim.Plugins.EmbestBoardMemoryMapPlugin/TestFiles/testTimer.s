	.equ	LEFT_LED,		0x0200
	.equ	RIGHT_LED,		0x0400

	@IO Ports
	.equ	rPCONA,		0x01d20000
	.equ	rPDATA,		0x01d20004
	.equ	rPCONB,		0x01d20008
	.equ	rPDATB,		0x01d2000c
	.equ	rPCONC,		0x01d20010
	.equ	rPDATC,		0x01d20014
	.equ	rPUPC,		0x01d20018
	.equ	rPCOND,		0x01d2001c
	.equ	rPDATD,		0x01d20020
	.equ	rPUPD,		0x01d20024
	.equ	rPCONE,		0x01d20028
	.equ	rPDATE,		0x01d2002c
	.equ	rPUPE,		0x01d20030
	.equ	rPCONF,		0x01d20034
	.equ	rPDATF,		0x01d20038
	.equ	rPUPF,		0x01d2003c
	.equ	rPCONG,		0x01d20040
	.equ	rPDATG,		0x01d20044
	.equ	rPUPG,		0x01d20048
	.equ	rSPUCR,		0x01d2004c
	.equ	rEXTINT,	0x01d20050
	.equ	rEXTINTPND,	0x01d20054
	
	@Eight segment display constants
	@Each entry is the bit pattern for a specific segment
	.equ	SEG_A,			0x80
	.equ	SEG_B,			0x40
	.equ	SEG_C,			0x20
	.equ	SEG_D,			0x08
	.equ	SEG_E,			0x04
	.equ	SEG_F,			0x02
	.equ	SEG_G,			0x01

	@PWM Timer registers
	.equ	rTCFG0,			0x01d50000
	.equ	rTCFG1,			0x01d50004
	.equ	rTCON,			0x01d50008
	.equ	rTCNTB0,		0x01d5000c
	.equ	rTCMPB0,		0x01d50010
	.equ	rTCNTB1,		0x01d50018
	.equ	rTCMPB1,		0x01d5001c
	.equ	rTCNTO1,		0x01d50020
	.equ	rTCNTB2,		0x01d50024
	.equ	rTCMPB2,		0x01d50028
	.equ	rTCNTO2,		0x01d5002c
	.equ	rTCNTB3,		0x01d50030
	.equ	rTCMPB3,		0x01d50034
	.equ	rTCNTO3,		0x01d50038
	.equ	rTCNTB4,		0x01d5003c
	.equ	rTCMPB4,		0x01d50040
	.equ	rTCNTO4,		0x01d50044
	.equ	rTCNTB5,		0x01d50048
	.equ	rTCNTO5,		0x01d5004c


	@Memory address of eight segment display
	.equ	ADDR_EightSegment,	0x02140000

	.equ 	NOINT,			0xc0
	.equ    IRQ_MODE,		0x40		@disable Interrupt Mode (IRQ)
	.equ    FIQ_MODE,		0x80		@disable Fast Interrupt Mode (FIQ)

	@Pre-defined constants
	.equ 	USERMODE,		0x10
	.equ 	FIQMODE,		0x11
	.equ 	IRQMODE,		0x12
	.equ 	SVCMODE,		0x13
	.equ 	ABORTMODE,		0x17
	.equ 	UNDEFMODE,		0x1b
	.equ	SYSTEMMODE,		0x1f
	.equ 	MODEMASK,		0x1f

	@Interrupt controller bits
	.equ	BIT_TIMER5,		(0x1<<8)
	.equ	BIT_TIMER4,		(0x1<<9)
	.equ	BIT_TIMER3,		(0x1<<10)
	.equ	BIT_TIMER2,		(0x1<<11)
	.equ	BIT_TIMER1,		(0x1<<12)
	.equ	BIT_TIMER0,		(0x1<<13)
	.equ	BIT_EINT4567,		(0x1<<21)
	.equ	BIT_EINT1,		(0x1<<24)
	.equ	BIT_GLOBAL,		(0x1<<26)

	@Interrupt controller registers
	.equ	rINTCON,		0x1e00000
	.equ	rINTPND,		0x1e00004
	.equ	rINTMOD,		0x1e00008
	.equ	rINTMSK,		0x1e0000c
	.equ	rI_PSLV,		0x1e00010
	.equ	rI_PMST,		0x1e00014
	.equ	rI_CSLV,		0x1e00018
	.equ	rI_CMST,		0x1e0001c
	.equ	rI_ISPR,		0x1e00020
	.equ	rI_ISPC,		0x1e00024
	.equ	rF_ISPC,		0x1e0003c

	.text
	.global _start
	
	@ need to setup interupt vectors
	ldr		pc, =Reset_Handler
	ldr		pc, =UndefinedHandler
	ldr		pc, =SWIHandler
	ldr		pc, =PrefetchAbortHandler
	ldr		pc, =DataAbortHandler
	.word	0	@Reserved vector
	ldr		pc, =IRQHandler
	ldr		pc, =FIQHandler

@these handlers do nothing for now
Reset_Handler:			b Reset_Handler
UndefinedHandler:		b UndefinedHandler
PrefetchAbortHandler:	b PrefetchAbortHandler
DataAbortHandler:		b DataAbortHandler
SWIHandler:				b SWIHandler
FIQHandler:				b FIQHandler

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
	movs		pc,lr

_start:
	bl		SetupStacks

	@PORT G
	ldr		r0,=rPCONG
	ldr		r1,=0xffff
	str		r1,[r0]

	ldr		r0,=rPUPG
	ldr		r1,=0x00			@pull up enable
	str		r1,[r0]

	ldr		r0,=rEXTINT
	ldr		r1,[r0]
	orr		r1,r1,#0x20			@EINT1 falling edge mode
	str		r1,[r0]

	@clear the 8 segment display
	mov		r0,#10
	mov		r1,#0
	bl		Segdisplay

	@turn off both Leds
	mov		r0,#0
	bl		SetLed

	@all interrupts are set to generate an IRQ	
	ldr		r0,=rINTMOD
	ldr		r1,=0x00
	str		r1,[r0]

	@non-vector mode, IRQ enabled, FIQ disabled
	ldr		r0,=rINTCON
	ldr		r1,=0x05
	str		r1,[r0]

	@clear all pending bits
	bl		interrupt_clearAll
	
	@clear EXTINTPND reg
	ldr		r0,=rEXTINTPND
	ldr		r1,=0xf
	str		r1,[r0]

	@set timer0 interrupt handler
	ldr		r0,=BIT_TIMER0
	ldr		r1,=timer0_handler
	bl		interrupt_setvector

	@set timer1 interrupt handler
	ldr		r0,=BIT_TIMER1
	ldr		r1,=timer1_handler
	bl		interrupt_setvector

	@sets all timers to prescale value of 255
	ldr		r1,=rTCFG0
	mov		r2,#0x00ffffff
	str		r2,[r1]

	@sets all timers to use MUX of 1/2
	ldr		r1,=rTCFG1
	mov		r2,#0
	str		r2,[r1]

	@load timer 0 count register to input cycle count / 2
	ldr		r1,=rTCNTB0
	mov		r0,#0x00000200
	str		r0,[r1]

	@load timer 1 count register to input cycle count / 2
	ldr		r1,=rTCNTB1
	mov		r0,#0x00000400
	str		r0,[r1]

	@timer0:manual load, stop, timer1:manual load, stop
	ldr		r1,=rTCON
	ldr		r0,=0x00000606
	str		r0,[r1]

	@timer0:autoreload, start, timer1:autoreload, start
	ldr		r1,=rTCON
	ldr		r0,=0x00000909
	str		r0,[r1]

	@enable timer0,timer1 and global interrupts in controller
	ldr		r0,=(BIT_TIMER0 | BIT_TIMER1 | BIT_GLOBAL)
	bl		interrupt_enable

	@enable irq, fiq interupts
	mrs		r0, CPSR
	bic		r0, r0, #NOINT		@enable interrupt
	msr		CPSR_cxsf, r0

loop:
	bal		loop


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

	ldmfd		sp!,{r0-r2,pc}


@Clears a specific set of pending interrupts
@R0:bits set will clear corresponding interrupts
interrupt_clear_pending:
	stmfd		sp!,{r1,lr}
	ldr		r1,=rI_ISPC
	str		r0,[r1]
	ldmfd		sp!,{r1,pc}

@Enable a set of interrupts
@R0:bits set will enable corresponding interrupts
interrupt_enable:
	stmfd		sp!,{r0-r2,lr}
	ldr		r1,=rINTMSK
	ldr		r2,[r1]
	mvn		r0,r0
	and		r2,r2,r0
	str		r2,[r1]
	ldmfd		sp!,{r0-r2,pc}

@Clears all pending interrupts
interrupt_clearAll:
	stmfd	sp!,{r0,lr}
	ldr		r0,=0xffffffff
	bl		interrupt_clear_pending
	ldmfd	sp!,{r0,pc}

@Setup the CPU mode stacks, leave cpu in system mode
SetupStacks:

	@disable irq/fiq
	mrs		r0,cpsr
	bic		r0,r0,#0xff
	orr		r0,r0,#NOINT

	@setup undefined instruction mode stack
	orr		r1,r0,#UNDEFMODE
	msr		cpsr_c,r1
	ldr		sp,=UndefStack

	@setup irq mode stack
	orr		r1,r0,#IRQMODE
	msr		cpsr_c,r1
	ldr		sp,=IRQStack

	@setup fiq mode stack
	orr		r1,r0,#FIQMODE
	msr		cpsr_c,r1
	ldr		sp,=FIQStack

	@setup service mode stack
	orr		r1,r0,#SVCMODE
	msr		cpsr_c,r1
	ldr		sp,=SVCStack

	@setup system mode stack
	orr		r1,r0,#SYSTEMMODE
	msr		cpsr_c,r1
	ldr		sp,=SystemStack
	mov		pc,lr

@R0:led(s) to turn on/off
SetLed:
	stmfd		sp!,{r1,lr}
	ldr			r1,=rPDATB
	str			r0,[r1]
	ldmfd		sp!,{r1,pc}

@Function to display a given integer between 0-9
@ on the 8 segment display.
@R0:digit to display (0-9)
Segdisplay:
	stmfd		sp!,{r0-r1,lr}
	ldr		r1,=digits		@ get byte value equivalent to 
	ldr		r0,[r1,r0,lsl#2]	@ number from digits array	
	ldr		r1,=ADDR_EightSegment
	str		r0,[r1]
	ldmfd		sp!,{r0-r1,pc}

@ handles the IRQ
IRQHandler:
	@must determine which interupt caused the IRQ
	sub		sp,sp,#4
	stmfd		sp!,{r0-r1}

	ldr		r0,=rI_ISPR
	ldr		r0,[r0]
	mov		r1,#0
irqh05:
	movs		r0,r0,lsr#1
	bcs		irqh10
	add		r1,r1,#4
	b		irqh05
irqh10:
	ldr		r0,=InteruptHandlerTable
	add		r0,r0,r1
	ldr		r0,[r0]
	str		r0,[sp,#8]
	ldmfd		sp!,{r0-r1,pc}


timer0_handler:
	stmfd		sp!,{r0-r2,lr}

	ldr		r0,=flag
	ldr		r1,[r0]
	add		r1,r1,#1
	cmp		r1,#10
	movge		r1,#0
	str		r1,[r0]

	mov		r0,r1
	bl		Segdisplay

	ldr		r0,=rI_ISPC
	mov		r1,#BIT_TIMER0
	str		r1,[r0]

	ldmfd		sp!,{r0-r2,lr}
	movs		pc,lr

timer1_handler:
	stmfd		sp!,{r0-r2,lr}

	ldr			r1,=ledState
	ldr			r0,[r1]
	cmp			r0,#LEFT_LED
	moveq		r0,#RIGHT_LED
	movne		r0,#LEFT_LED
	str			r0,[r1]
	bl			SetLed

	ldr		r0,=rI_ISPC
	mov		r1,#BIT_TIMER1
	str		r1,[r0]

	ldmfd		sp!,{r0-r2,lr}
	movs		pc,lr


		.data
		.align
flag:		.word	0
ledState:	.word	LEFT_LED

digits:	
		.word	SEG_A|SEG_B|SEG_C|SEG_D|SEG_E|SEG_G			@0
		.word	SEG_B|SEG_C									@1
		.word	SEG_A|SEG_B|SEG_F|SEG_E|SEG_D				@2
		.word	SEG_A|SEG_B|SEG_F|SEG_C|SEG_D				@3
		.word	SEG_G|SEG_F|SEG_B|SEG_C						@4
		.word	SEG_A|SEG_G|SEG_F|SEG_C|SEG_D				@5
		.word	SEG_A|SEG_G|SEG_F|SEG_E|SEG_D|SEG_C			@6
		.word	SEG_A|SEG_B|SEG_C							@7
		.word	SEG_A|SEG_B|SEG_C|SEG_D|SEG_E|SEG_F|SEG_G	@8
		.word	SEG_A|SEG_B|SEG_F|SEG_G|SEG_C				@9
		.word	0											@Clear
		
		.skip	256
SVCStack:
		.skip	256
UndefStack:
		.skip	256
IRQStack:
		.skip	256
FIQStack:
		.skip	256
SystemStack:

		.end
