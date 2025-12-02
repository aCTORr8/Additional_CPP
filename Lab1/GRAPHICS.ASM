MODEL SMALL
STACK 256
DATASEG

CODESEG
start:
    MOV AX, @DATA
    MOV DS, AX

    MOV AX, 0013h     
    INT 10h           

    MOV CX, 160       
    MOV DX, 50        
    MOV AL, 1         
    MOV BH, 0         

    MOV CL, 100
    MOV CH, 0         

maljuvannja_liniji:
    MOV AH, 0Ch       
    INT 10h           

    INC DX            
    LOOP maljuvannja_liniji 

ex:
    MOV AH, 00h       
    INT 16h           

    MOV AX, 0003h    
    INT 10h

    MOV AH, 04Ch     
    INT 21h


END start
