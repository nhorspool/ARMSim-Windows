@ In order for this code to work, ARMSim must be configured
@ for RAM to start at address 0

@ *************** Code Segment ***************
@test segment - Summer 2005 - printing integers to console

	.equ	SWI_WriteC, 0x00
	.equ	SWI_Write0, 0x02
	.equ	SWI_Exit,   0x11


	.text

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
	movs	pc,r14

@handles the SWI
SWIHandler:

	@both these returns are valid
	movs	pc,r14
	subs	pc,r14,#0


_start:
	@make an SWI call that we dont handle
	SWI	0x5656
	SWI	0x0

	@some junk code
	and	r0,r1,r2
	and	r0,r1,r2
	and	r0,r1,r2
	b	_start

	.end
	
