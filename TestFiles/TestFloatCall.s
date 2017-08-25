	.cpu arm7tdmi
	.fpu softvfp
	.eabi_attribute 20, 1
	.eabi_attribute 21, 1
	.eabi_attribute 23, 3
	.eabi_attribute 24, 1
	.eabi_attribute 25, 1
	.eabi_attribute 26, 1
	.eabi_attribute 30, 6
	.eabi_attribute 34, 0
	.eabi_attribute 18, 4
	.file	"TestFloatCall.c"
	.global	__aeabi_f2d
	.section	.rodata
	.align	2
.LC0:
	.ascii	"f = %f, d = %g\012\000"
	.align	2
.LC1:
	.ascii	"<<standard error>>\012\000"
	.text
	.align	2
	.global	main
	.type	main, %function
main:
	@ Function supports interworking.
	@ args = 0, pretend = 0, frame = 24
	@ frame_needed = 1, uses_anonymous_args = 0
	stmfd	sp!, {r4, fp, lr}
	add	fp, sp, #8
	sub	sp, sp, #36
	str	r0, [fp, #-32]
	str	r1, [fp, #-36]
	ldr	r3, .L3+8
	str	r3, [fp, #-16]	@ float
	adr	r4, .L3
	ldmia	r4, {r3-r4}
	str	r3, [fp, #-28]
	str	r4, [fp, #-24]
	ldr	r0, [fp, #-16]	@ float
	bl	__aeabi_f2d
	mov	r3, r0
	mov	r4, r1
	sub	r2, fp, #28
	ldmia	r2, {r1-r2}
	stmia	sp, {r1-r2}
	ldr	r0, .L3+12
	mov	r2, r3
	mov	r3, r4
	bl	printf
	ldr	r3, .L3+16
	ldr	r3, [r3]
	ldr	r3, [r3, #12]
	ldr	r0, .L3+20
	mov	r1, #1
	mov	r2, #19
	bl	fwrite
	mov	r3, #0
	mov	r0, r3
	sub	sp, fp, #8
	@ sp needed
	ldmfd	sp!, {r4, fp, lr}
	bx	lr
.L4:
	.align	3
.L3:
	.word	-266631570
	.word	1074340345
	.word	1067316150
	.word	.LC0
	.word	_impure_ptr
	.word	.LC1
	.size	main, .-main
	.ident	"GCC: (Sourcery CodeBench Lite 2014.05-28) 4.8.3 20140320 (prerelease)"
