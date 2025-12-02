MODEL SMALL
STACK 256

DATASEG
    num1    DW  5       
    num2    DW  10      
    summand DW  2       
    result  DW  ?       
    msg     DB  'Integer calculation complete. Result (52) is in variable RESULT.$'

CODESEG
start:
    MOV AX, @DATA
    MOV DS, AX

    MOV AX, num1      
    MUL num2          

    MOV result, AX    

    MOV BX, summand     
    ADD result, BX      

    MOV AH, 09h       
    LEA DX, msg       
    INT 21h

    MOV AH, 4Ch
    INT 21h

END start