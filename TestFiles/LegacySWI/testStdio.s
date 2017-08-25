		.global _start
		.text
		
	_start:
		
		mov	r0,#1
		ldr	r1,=msg1
		swi	0x69
		
		mov	r0,#0
		ldr	r1,=buffer
		mov	r2,#256
		swi	0x6a

		mov	r0,#1
		ldr	r1,=msg4
		swi	0x69

		mov	r0,#1
		ldr	r1,=msg2
		swi	0x69

		mov	r0,#1
		ldr	r1,=buffer
		swi	0x69

		mov	r0,#1
		ldr	r1,=msg4
		swi	0x69

		mov	r0,#1
		ldr	r1,=msg3
		swi	0x69

		ldr	r1,=buffer
		mov	r2,r1
loop:
		ldrb	r3,[r2],#1
		cmp	r3,#0
		bne	loop
		sub	r2,r2,#2
				
loop2:
		cmp	r2,r1
		ble	done
		
		ldrb	r3,[r2]
		ldrb	r4,[r1]
		strb	r4,[r2]
		strb	r3,[r1]
		
		add	r1,r1,#1
		sub	r2,r2,#1
		bal	loop2
		
done:
		mov	r0,#1
		ldr	r1,=buffer
		swi	0x69

		mov	r0,#1
		ldr	r1,=msg4
		swi	0x69
		
		swi	0x11
	
	
		.data
msg1:	.asciz	"Enter a string then press Enter:"
msg2:	.asciz	"You entered:"
msg3:	.asciz	"Backwards:"		
msg4:	.asciz	"\r\n"
		
buffer:	.skip	256

		.end