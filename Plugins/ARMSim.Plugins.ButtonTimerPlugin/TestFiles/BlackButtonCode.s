
@the ids of the 2 black buttons and the black button id register
	.equ	RIGHT_BLACK_BUTTON,		0x01
	.equ	LEFT_BLACK_BUTTON,		0x02
	.equ	BLACKBUTTONID,			0x01d00000

	.global		BlackButton_Init
	.global		BLACKBUTTONID
	.global		RIGHT_BLACK_BUTTON
	.global		LEFT_BLACK_BUTTON
	
@Initialize black buttons. Do nothing for now	
BlackButton_Init:
	mov		pc,lr
		

	.end
