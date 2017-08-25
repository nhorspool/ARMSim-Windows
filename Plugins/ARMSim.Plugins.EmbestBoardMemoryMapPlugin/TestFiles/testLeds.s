
	@Memory address of port controlling Leds
	.equ	PORTB_DATA, 0x01D2000C

	.text
	.global _start
	
_start:

	ldr	r0,=PORTB_DATA

	@both leds off
	mov	r1,#0x000
	str	r1,[r0]

	@left led on
	mov	r1,#0x200
	str	r1,[r0]

	@right led on
	mov	r1,#0x400
	str	r1,[r0]

	@both leds on
	mov	r1,#0x600
	str	r1,[r0]



	swi	0x11


		
		.end
