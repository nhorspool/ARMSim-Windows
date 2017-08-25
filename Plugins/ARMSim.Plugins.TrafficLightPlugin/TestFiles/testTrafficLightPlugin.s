
	.equ	SWI_Exit,				0x11
	.equ	SWI_GetTicks,			0x6d

	.equ	SWI_SetStreetLight,		0x100
	.equ	SWI_CheckXWalkButton,	0x101
	.equ	SWI_SetXWalk,			0x102

	.equ	LIGHT_BLACK,			0x00
	.equ	LIGHT_RED,				0x01
	.equ	LIGHT_YELLOW,			0x02
	.equ	LIGHT_GREEN,			0x03

	.equ	MAIN_STREET,			0x00
	.equ	SIDE_STREET,			0x01

	.equ	DONT_WALK,				0x00
	.equ	WALK,					0x01
	.equ	HURRY,					0x02

	.text
	.global	_start
_start:
	bl		Init
	bl		ResetTimer
mn05:
	bl		ElapsedTime
	cmp		r0,#12
	bgt		mn10
	
	bl		CheckSideStreetButton
	cmp		r0,#0
	beq		mn05
mn10:
	mov		r0,#MAIN_STREET
	mov		r1,#HURRY
	bl		SetWalk
	mov		r0,#4
	bl		wait

	mov		r0,#MAIN_STREET
	mov		r1,#DONT_WALK
	bl		SetWalk

	mov		r0,#MAIN_STREET
	mov		r1,#LIGHT_YELLOW
	bl		SetLight

	mov		r0,#4
	bl		wait
	
	mov		r0,#MAIN_STREET
	mov		r1,#LIGHT_RED
	bl		SetLight

	mov		r0,#SIDE_STREET
	mov		r1,#LIGHT_GREEN
	bl		SetLight
	
	mov		r0,#SIDE_STREET
	mov		r1,#WALK
	bl		SetWalk

	bl		ResetTimer

mn20:	
	bl		ElapsedTime
	cmp		r0,#8
	bgt		mn25

	bl		CheckMainStreetButton
	cmp		r0,#0
	beq		mn20
mn25:
	mov		r0,#SIDE_STREET
	mov		r1,#HURRY
	bl		SetWalk

	mov		r0,#4
	bl		wait

	mov		r0,#SIDE_STREET
	mov		r1,#LIGHT_YELLOW
	bl		SetLight

	mov		r0,#SIDE_STREET
	mov		r1,#DONT_WALK
	bl		SetWalk
	
	mov		r0,#4
	bl		wait

	mov		r0,#SIDE_STREET
	mov		r1,#LIGHT_RED
	bl		SetLight

	mov		r0,#MAIN_STREET
	mov		r1,#LIGHT_GREEN
	bl		SetLight

	mov		r0,#MAIN_STREET
	mov		r1,#WALK
	bl		SetWalk
	bl		ResetTimer
	bal		mn05
	


@ wait for r0 seconds
wait:
	stmfd	sp!,{r0-r2,lr}
	mov	r2,r0
	swi	SWI_GetTicks
	mov	r1,r0
wait05:
	swi	SWI_GetTicks
	subs	r0,r0,r1
	rsblt	r0,r0,#0
	mov		r0,r0,lsr#10
	cmp	r0,r2
	blt	wait05
	ldmfd	sp!,{r0-r2,pc}
	
@R0:main or side
@R1:state
SetWalk:
	swi	SWI_SetXWalk
	mov		pc,lr

@R0:main or side
@R1:state
SetLight:
	swi		SWI_SetStreetLight
	mov		pc,lr

Init:
	stmfd	sp!,{r0-r1,lr}
	mov		r0,#MAIN_STREET
	mov		r1,#WALK
	bl		SetWalk

	mov		r0,#SIDE_STREET
	mov		r1,#DONT_WALK
	bl		SetWalk

	mov		r0,#MAIN_STREET
	mov		r1,#LIGHT_GREEN
	bl		SetLight

	mov		r0,#SIDE_STREET
	mov		r1,#LIGHT_RED
	bl		SetLight

	ldmfd	sp!,{r0-r1,pc}

ElapsedTime:
	stmfd	sp!,{r1-r2,lr}
	swi		SWI_GetTicks
	ldr		r1,=timer
	ldr		r2,[r1]
	sub		r0,r0,r2
	mov		r0,r0,lsr#10
	ldmfd	sp!,{r1-r2,pc}

ResetTimer:
	stmfd	sp!,{r0-r1,lr}
	swi		SWI_GetTicks
	ldr		r1,=timer
	str		r0,[r1]
	ldmfd	sp!,{r0-r1,pc}

CheckMainStreetButton:
	mov		r0,#MAIN_STREET
	swi		SWI_CheckXWalkButton
	mov		pc,lr

CheckSideStreetButton:
	mov		r0,#SIDE_STREET
	swi		SWI_CheckXWalkButton
	mov		pc,lr

.data
timer:				.word	0

	.end
