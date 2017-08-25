@ *************** Code Segment ***************
@test segment - printing integers to console

	.equ	SWI_Write, 0x05
	.equ	SWI_Exit,  0x18

	.text

	.global	print_int		@ Make the label visible to all modules
_start:
        ldr     r10, =block
	bl	output_crlf

	ldr	r0, =255
	bl	print_int
	bl	output_crlf

	ldr	r0, =2000000001
	bl	print_int
	bl	output_crlf

	ldr	r0, =0
	bl	print_int
	bl	output_crlf

	ldr	r0, =-255
	bl	print_int
	bl	output_crlf

        mov     r0, #SWI_Exit
	mov	r1, #0
	swi	0x123456

@Integer to be printed is in r0	
print_int:
	STMFD	sp!,{r0-r6,lr}
	mov		r1,r0		@ R1: value to print
	ldr		r2,=space	@ R2: output space

	cmp		r1,#0	@ check sign of value (+/-)
	blt		NegInt
	ldr		r0,='+		@ prepend with '+'
	b		w_sign
NegInt:		
	ldr		r0,='-			@ prepend with '-'
w_sign:	
	strb	r0,[r2],#1
	ldr		r3,=tens	@ R3: multiple pointer
	ldr		r6,=0		@ R6: write zero digits? (no, for leading zeros)
next_d:		
	ldr		r4,[r3],#4	@ R4: current multiple
	cmp		r4,#1		@ stop when multiple < 1
	blt		print_result
	bne		c0
	ldr		r6,=1		@ write zero digit for last multiple
c0:	ldr		r5,=0		@ R5: multiple count
count:	
	mov		r0,r1		@ R0: tmp R1
	cmp		r1,#0
	blt		cnt_n
cnt_p:	
	subs	r0,r0,r4	@ subtract multiple from pos value
	bmi		check
	b		update
cnt_n:	
	add		r0,r0,r4	@ add multiple to neg value
	cmp		r0,#0
	bgt		check
update:	
	mov		r1,r0		@ update value
	add		r5,r5,#1	@ update count
	b		count
check:	
	cmp		r5,#0		@ if digit is '0' and ...
	bne		w_dig
	cmp		r6,#1		@ if not last multiple
	bne		next_d		@ skip leading '0'
w_dig:	
	ldr		r6,=1		@ write '0' from now on
	ldr		r0,=digits
	ldrb	r0,[r0,r5]	@ write digit character at count offset
	strb	r0,[r2],#1	@ save digit output
	b		next_d

output_crlf:
	STMFD	sp!,{r0,lr}
        mov     r0,#1
        str     r0,[r10]
	ldr	r0, =crlf
        str     r0,[r10,#4]
        mov     r0,#1
        str     r0,[r10,#8]
        mov     r0,#0x05
        mov     r1,r10
	swi	0x123456
	LDMFD	sp!,{r0,pc}	@restore state and return

print_result:
	ldrb	r0,=0x00
	strb	r0,[r2]	@ put a null byte at end of output
	ldr		r0,=space
beforeEnd:
        str     r0,[r10,#4]
        sub     r0,r2,r0
        str     r0,[r10,#8]
        mov     r0,#1
        str     r0,[r10]
        mov     r0,#0x05
        mov     r1,r10
	swi	0x123456
end:	
	LDMFD	sp!,{r0-r6,pc}	@restore state and return

@ *************** Data Segment ***************

	.data

block:  .word   0, 0, 0

digits:
	.ascii	"0123456789ABCDEF"	@ Note: NOT null-terminated

crlf:	.byte	0x0a
	.byte	0x00
	.byte	0

str1:	.ascii	"a test string of some length"
	.align	2
tens:
	.word	1000000000,100000000,10000000,1000000,100000,10000,1000,100,10,1,0
space:
	.skip	11	@ Space for 10 characters and a sign (+/-)
	.byte	0	@ Null byte
	.end
	
