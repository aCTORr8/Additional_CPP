MODEL SMALL
STACK 256
DATASEG
    real1   DD  3.5
    real2   DD  1.25
    real_sum DD ?   

CODESEG
start:
    MOV AX, @DATA
    MOV DS, AX

    FINIT

    FLD real1         

    FADD real2        

    FSTP real_sum     

    MOV AH, 4Ch
    INT 21h


END start
