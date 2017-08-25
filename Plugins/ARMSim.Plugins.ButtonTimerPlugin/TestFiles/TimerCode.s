@The timer memory locations. Note 2 timers here
	.equ	TIMER1_CONTROL,	0x01d30000
	.equ	TIMER1_DATA,	0x01d30004
	.equ	TIMER1_COUNT,	0x01d30008
	.equ	TIMER2_CONTROL,	0x01d30010
	.equ	TIMER2_DATA,	0x01d30014
	.equ	TIMER2_COUNT,	0x01d30018

@The timer control register bits	
	.equ	TIMER_ENABLE,	0x20
	.equ	INT_ENABLE,		0x04

@the id values of the 2 timers and the timer id register memory location
	.equ	TIMER1_ID,		0x01
	.equ	TIMER2_ID,		0x02
	.equ	TIMER_ID,		0x01d3001c

	.global		Timer_Init
	.global		Timer_Set
	.global		TIMER_ID
	.global		TIMER1_ID
	.global		TIMER2_ID

@Initialize timers. Do nothing for now	
Timer_Init:
	mov		pc,lr
		
@R0:0:timer0, non-0:timer1
@R1:data register
@R2:control register
Timer_Set:
	stmfd	sp!,{r0-r3,lr}
	
	cmp		r0,#0
	ldreq	r3,=TIMER1_DATA
	ldrne	r3,=TIMER2_DATA
	str		r1,[r3]

	cmp		r0,#0
	ldreq	r3,=TIMER1_CONTROL
	ldrne	r3,=TIMER2_CONTROL
	
	orr		r2,r2,#(TIMER_ENABLE|INT_ENABLE)
	str		r2,[r3]
	ldmfd	sp!,{r0-r3,pc}

	.end