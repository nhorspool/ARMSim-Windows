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
	.file	"callprintf.c"
	.section	.rodata
	.align	2
.LC0:
	.ascii	"hello there\000"
	.align	2
.LC1:
	.ascii	"x = %d, y = %X, z = %s, pi = %g\012\000"
pi:	.double	3.14159265358979
	.text
	.align	2
	.global	main
	.type	main, %function
main:
	@ Function supports interworking.
	@ args = 0, pretend = 0, frame = 32
	@ frame_needed = 1, uses_anonymous_args = 0
	stmfd	sp!, {r4, fp, lr}
	add	fp, sp, #8
	sub	sp, sp, #44
	str	r0, [fp, #-40]
	str	r1, [fp, #-44]
	mov	r3, #99
	str	r3, [fp, #-16]
	mvn	r3, #98
	str	r3, [fp, #-20]
	ldr	r3, .L3+8
	str	r3, [fp, #-24]
	adr	r4, .L3
	ldmia	r4, {r3-r4}
	str	r3, [fp, #-36]
	str	r4, [fp, #-32]
	sub	r4, fp, #36
	ldmia	r4, {r3-r4}
	stmia	sp, {r3-r4}
	ldr	r0, .L3+12
	ldr	r1, [fp, #-16]
	ldr	r2, [fp, #-20]
	ldr	r3, [fp, #-24]
	bl	printf
	mov	r3, #0
	mov	r0, r3
	sub	sp, fp, #8
	@ sp needed
	ldmfd	sp!, {r4, fp, lr}
	bx	lr
.L4:
	.align	3
.L3:
	.word	1413754129
	.word	1074340347
	.word	.LC0
	.word	.LC1
	.size	main, .-main
	.ident	"GCC: (Sourcery CodeBench Lite 2014.05-28) 4.8.3 20140320 (prerelease)"
