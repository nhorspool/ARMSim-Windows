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
	.file	"sinetest.c"
	.global	sd
	.bss
	.align	3
	.type	sd, %object
	.size	sd, 8
sd:
	.space	8
	.text
	.align	2
	.global	main
	.type	main, %function
main:
	@ Function supports interworking.
	@ args = 0, pretend = 0, frame = 16
	@ frame_needed = 1, uses_anonymous_args = 0
	stmfd	sp!, {r4, fp, lr}
	add	fp, sp, #8
	sub	sp, sp, #20
	str	r0, [fp, #-24]
	str	r1, [fp, #-28]
	mov	r3, #0
	ldr	r4, .L3
	str	r3, [fp, #-20]
	str	r4, [fp, #-16]
	sub	r1, fp, #20
	ldmia	r1, {r0-r1}
	bl	sin
	mov	r3, r0
	mov	r4, r1
	ldr	r2, .L3+4
	stmia	r2, {r3-r4}
	mov	r3, #0
	mov	r0, r3
	sub	sp, fp, #8
	@ sp needed
	ldmfd	sp!, {r4, fp, lr}
	bx	lr
.L4:
	.align	2
.L3:
	.word	1071644672
	.word	sd
	.size	main, .-main
	.ident	"GCC: (Sourcery CodeBench Lite 2014.05-28) 4.8.3 20140320 (prerelease)"
