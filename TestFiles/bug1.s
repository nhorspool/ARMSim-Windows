	movs 	r1, #65280 			@ 0xff00
	movs 	r2, r1, LSL #22			@ generate Carry flag
	mov 	r2, r1, RRX 			@ RRX shift mode
	@ r2 should be 0x80007f80, it's 0xff00 instead.
	mov	pc,lr

