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
	.file	"TestFloatCall2.c"
	.comm	buffer,64,4
	.global	fmt
	.data
	.align	2
	.type	fmt, %object
	.size	fmt, 8
fmt:
	.ascii	"d = %f\012\000"
	.global	d
	.align	3
	.type	d, %object
	.size	d, 8
d:
	.word	-266631570
	.word	1074340345
	.text
	.align	2
	.global	main
	.type	main, %function
main:
	@ Function supports interworking.
	@ args = 0, pretend = 0, frame = 8
	@ frame_needed = 1, uses_anonymous_args = 0
	stmfd	sp!, {r4, fp, lr}
	add	fp, sp, #8
	sub	sp, sp, #12
	str	r0, [fp, #-16]
	str	r1, [fp, #-20]
	ldr	r3, .L3
	ldmia	r3, {r3-r4}
	ldr	r0, .L3+4
	ldr	r1, .L3+8
	mov	r2, r3
	mov	r3, r4
	bl	sprintf
	mov	r3, #0
	mov	r0, r3
	sub	sp, fp, #8
	@ sp needed
	ldmfd	sp!, {r4, fp, lr}
	bx	lr
.L4:
	.align	2
.L3:
	.word	d
	.word	buffer
	.word	fmt
	.size	main, .-main
	.ident	"GCC: (Sourcery CodeBench Lite 2014.05-28) 4.8.3 20140320 (prerelease)"
