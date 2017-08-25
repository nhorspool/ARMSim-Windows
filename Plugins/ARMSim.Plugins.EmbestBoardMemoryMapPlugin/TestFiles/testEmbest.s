	.global _start
	.text
_start:

	ldr	r3,=0x2140000
	mov	r2,#0xff
	str	r2,[r3]



	bl	DrawBox
	bl	DrawStrings
	bl	DrawChars

	swi	0x11

@Draw the chars A-Z on the bottom line of the lcd
DrawChars:
	stmfd	sp!,{r0-r5,lr}

	mov	r5,#0
	mov	r4,#'A'
drc05:
	mov	r0,r5
	mov	r1,#14
	mov	r2,r4
	swi	0x207

	add	r4,r4,#1
	add	r5,r5,#1
	cmp	r4,#'h'
	ble	drc05

	ldmfd	sp!,{r0-r5,pc}

@Draw a series of strings in the top right of the lcd
DrawStrings:
	stmfd	sp!,{r0-r5,lr}

	mov	r4,#0
drs05:
	mov	r0,#31
	mov	r1,r4
	ldr	r2,=str
	swi	0x204

	add	r4,r4,#1
	cmp	r4,#8
	blt	drs05

	ldmfd	sp!,{r0-r5,pc}


@Draw a 60x50 box in the top left corner of the lcd
DrawBox:
	stmfd	sp!,{r0-r5,lr}

	mov	r4,#0
	mov	r5,#0
drb05:
	mov	r0,r4
	mov	r1,r5
	mov	r2,#1
	swi	0x209

	add	r4,r4,#1
	cmp	r4,#60
	blt	drb05

	mov	r4,#0
	add	r5,r5,#1
	cmp	r5,#50
	blt	drb05

	ldmfd	sp!,{r0-r5,pc}




	.data
str:	.ascii	"Hi Dale!"


	.end
