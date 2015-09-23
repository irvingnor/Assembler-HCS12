;uso de directivas de reserva de espacio
;y su relacion con el archivo de codigo objeto
	ORG	$1000
	ADCA	[0,X]
	ADCA	[0,Y]
	ADCA	[0,SP]
	ADCA	[0,PC]
	SWI
	DS.B	$20
	ADCA	[0,X]
	ADCA	[0,Y]
	ADCA	[0,PC]
	FCC	"abcde"
	SWI
	DS.B	$1
	SWI
	DS.B	$1
	SWI
	SWI
	SWI
	SWI
	SWI
	DS.B	$1
	FCC	"abcde"
	END