Pri	EQU	1
Seg	EQU	2
;comentario uno
Ter	EQU	3
;comentario dos
Cua	EQU	4
	ORG	255
Et	DS.B	5
Otra	EQU	0
	SWI
	DS.W	5
Qui	EQU	5
	SWI
Sex	EQU	8
Sep	SWI
Oct	EQU	8
	LDAA	3
	LDAA	Sep
	DS.B	3
	LDAA	3
Nov	EQU	9
	SWI
	DC.B	2
;comentario tres
Dec	EQU	10
Onc	EQU	11
Tre	EQU	13
	DS.B	1
	END