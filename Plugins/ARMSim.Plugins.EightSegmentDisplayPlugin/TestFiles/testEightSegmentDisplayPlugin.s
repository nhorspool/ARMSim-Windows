	@ Test ARM file to drive the Sample Eight Segment Display Plugin
	@
    @ Memory Mapped Method
    @     In this case we have an eight segment display mapped to the physical memory address 0x02140000.
    @     The word (32 bits) at this address represents the pattern to display in the control.
    @     Only the bottom 8 bits are significant the upper 24 bits are ignored.
    @     Note that non word reads/writes to this address are ignored.
    @     
    @ SWI Instruction Method
    @     We will extend the SWI instruction   "swi 0x100" to perform a write operation to the display.
    @     R0 will contain a digit(0-9) to display and R1 will contain a flag to indicate if the Point segment should be shown.
    @ 
    @ New Instruction Method
    @     We will create 2 new ARM instructions, SEGDIGIT and SEGPATTERN that will update the display based on a register value.
    @     These instructions work like this:
    @     SEGDIGIT reg, where reg is a register number  ie      SEGDIGIT r5
    @         This instruction will take the digit (0-9) in register 5 and write it to the display
    @         (values outside 0-9 are ignored)
    @     SEGPATTERN reg, where reg is a register number   ie    SEGPATTERN r7
    @         This instruction will take the 8 bit pattern in the least significant bits of r7 and write to the display
    @     
    @     This example will also show how to insert the new instruction mnemonic into the parsing tables so that
    @     code can be written using the new symbols.

	.text
	.global	_start

	@ the swi instruction to write to the display
	.equ	SWI_INSTRUCTION,	0x100

	@ the memory address of the display
	.equ	ControlAddress, 	0x02140000

@entry point to program
_start:

	@ display the digits 0 to 9 on the display
	mov	r0,#0
loop:
	mov	r1,#0
	swi	SWI_INSTRUCTION

	add	r0,r0,#1
	cmp	r0,#10
	blt	loop

	@ use a write to the control address to write a pattern to the display
	ldr	r0,=ControlAddress
	mov	r1,#0x33
	str	r1,[r0]

	@ use the new instruction SEGDIGIT to write the digit 3 to the display
	mov		r7,#3
	SEGDIGIT	r7

	@ use the new instruction SEGPATTERN to display a pattern on the display
	mov		r8,#0x19
	SEGPATTERN	r8

	@ stops the simulation
	swi	0x11

	.end

