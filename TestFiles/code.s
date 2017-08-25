.arm
.text
.data
string:
        .asciz "OUT %10x\n"
	.align	2
@mag:	.long 0xFFFFFFFF
fixedmagic:
	.long 0x070100
msg:
	.byte 0xa0, 0x82, 0x00, 0x01, 0x02, 0x00, 0x01, 0x1e, 0x00
	.align 4

.global main

.func main

main:
	PUSH {R4-R6,LR}

	LDR		R0,=msg
	MOVS            R6, R0

	LDRB            R3, [R0,#6]
	LDRB            R2, [R0,#5]
	@ fix magic
	LDR	R0,=fixedmagic
	LDR	R0,[R0]
	LSLS            R1, R0, #0x10
	LSLS            R5, R0, #0xE
	LSRS            R0, R5, #0x1E
	LSRS            R3, R1, #0x10
	MOVS            R5, #0x10
	SUBS            R4, R5, R0
	MOVS            R2, R3
	ASRS            R2, R4
	LSLS            R3, R0
	MOVS            R1, R2
	ORRS            R1, R3
	LSLS            R4, R1, #0x10
	LDRB            R1, [R6]
	LDR             R3, =0xFFFFA500
	LSLS            R0, R1, #8
	LDRB            R1, [R6,#1]
	MOV             R2, R10
	ORRS            R3, R2
	ORRS            R1, R0
	LDRB            R0, [R6,#2]
	EORS            R1, R3
	LDRB            R3, [R6,#3]
	LSLS            R2, R0, #8
	LDRB            R0, [R6,#4]
	ORRS            R3, R2
	EORS            R1, R3
	MOV             R2, R9
	LSLS            R3, R0, #8
	ORRS            R3, R2
	LSRS            R4, R4, #0x10
	EORS            R1, R3
	MOVS            R0, R4
	EORS            R0, R1
	LSLS            R3, R0, #0x10
	MOVS            R2, #7
	ANDS            R4, R2
	LSRS            R0, R3, #0x10
	SUBS            R5, R5, R4
	MOVS            R1, R0
	ASRS            R1, R5
	LSLS            R0, R4
	MOVS            R5, R1
	ORRS            R5, R0
	LSLS            R2, R5, #0x10
	LSLS            R1, R5, #0x18
	LDRB            R0, [R6,#7]
	LSRS            R3, R1, #0x18
	LSRS            R4, R2, #0x18
	MOV             R8, R3

@	LDR	R0,=string
	@MOV	R1,R4
	@BL	printf

	POP {R4-R6,PC}
        MOV PC, LR

@	CMP             R0, R4
@      STMFD           SP!, {R0,R1,LR}
@      BL              __udivsi3
@      LDMFD           SP!, {R1,R2,LR}
@      MUL             R3, R2, R0
@      SUB             R1, R1, R3
@      BX              LR
.endfunc



