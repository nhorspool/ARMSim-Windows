	.global _start
_start:
	
	mov	r0,#5
	MIDENT	r0
	
	mov	r0,#4
	mov	r1,#5
	ldr	r2,=mat1
	MLOADD	r0, r1, r2

	MMUL
		
	ldr	r2,=mat5
	MLOADI	r0, r1, r2

	MADD

	ldr		r9,=resultM
	MPOP	r7, r8, r9

	mov	r0,r7
	mov	r1,r8
	mov	r2,r9
	MLOADD	r0, r1, r2

	mov	r0,#3
	mov	r1,#3
	ldr	r2,=mat2
	MLOADF	r0, r1, r2

	mov	r0,#8
	mov	r1,#3
	ldr	r2,=mat3
	MLOADI	r0, r1, r2

	mov	r0,#10
	mov	r1,#8
	ldr	r2,=mat4
	MLOADI	r0, r1, r2

	swi	0x11

	.data
mat1:
	.double	0e1.1e+0, 0e12.2e+0, 0e123.3e+0, 0e1234.4e+0, 0e12345.5e+0
	.double	0e2.1e+0, 0e23.2e+0, 0e234.3e+0, 0e2345.4e+0, 0e23456.5e+0
	.double	0e3.1e+0, 0e34.2e+0, 0e345.3e+0, 0e3456.4e+0, 0e34567.5e+0
	.double	0e4.1e+0, 0e45.2e+0, 0e456.3e+0, 0e4567.4e+0, 0e45678.5e+0

mat5:
	.word	1,1,1,1,1
	.word	1,1,1,1,1
	.word	1,1,1,1,1
	.word	1,1,1,1,1

mat2:
	.float	0e6.1e+0, 0e6.2e+0, 0e6.3e+0
	.float	0e7.1e+0, 0e7.2e+0, 0e7.3e+0
	.float	0e8.1e+0, 0e8.2e+0, 0e8.3e+0

mat3:
	.word	9,8,7
	.word	5,6,5
	.word	3,1,0
	.word	3,1,0
	.word	3,1,0
	.word	3,1,0
	.word	3,1,0
	.word	3,1,0
mat4:
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2

resultM:
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2
	.word	9,8,7,6,5,4,3,2


	.end

