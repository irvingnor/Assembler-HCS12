EN1	EQU	$2F
;comentario UNO
	ORG	$033
EN2	LDAA	#82
EN3	EQU	%11
EN4	DS.B	15
EN5	TSTA
	DC.W	35555
EN6	FCB	25
	FCC	"H H"
	RMW	4
	END