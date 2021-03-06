/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

grammar VisualBasic;

options {
    language = CSharp;
}

file
    : statements EOF
    ;

statements
    : (WS? stmt NEWLINE)* WS? stmt?
    ;

stmt
    : ( assignmentStmt
      | msgBoxStmt
      | declarationStmt
      | forLoopStmt
      | exitStmt)? WS?
    ;

assignmentStmt
    : ID WS? '=' WS? expr
    ;

msgBoxStmt
    : MSGBOX WS expr
    ;

exitStmt
    : EXIT WS FOR
    ;

declarationStmt
    : DIM WS? ID WS? AS WS? typeName (WS? ',' WS? ID WS? AS WS? typeName)*
    ;

forLoopStmt
    : FOR WS? ID WS? '=' WS? expr WS? TO WS? expr (WS? STEP WS? expr)? NEWLINE
      statements NEWLINE
      NEXT
    ;

typeName
    : TYPE_SINGLE
    | TYPE_INTEGER
    | TYPE_DECIMAL
    | TYPE_STRING
    ;

primaryExpr
    : '(' WS? concatExpr WS? ')'
    | ID
    | INTEGER_CONSTANT
    | FLOATING_POINT_CONSTANT
    | STRING_CONSTANT
    ;

exponentiationExpr
    : primaryExpr
    | exponentiationExpr WS? '^' WS? primaryExpr
    ;

negationExpr
    : exponentiationExpr
    | '-' WS? exponentiationExpr
    ;

multiplicativeExpr
    : negationExpr
    | multiplicativeExpr WS? ('*' | '/') WS? negationExpr
    ;

integerDivExpr
    : multiplicativeExpr
    | integerDivExpr WS? '\\' WS? multiplicativeExpr
    ;

moduloExpr
    : integerDivExpr
    | moduloExpr WS? OP_MODULO WS? integerDivExpr
    ;

additiveExpr
    : moduloExpr
    | additiveExpr WS? ('+' | '-') WS? moduloExpr
    ;

concatExpr
    : additiveExpr
    | concatExpr WS? '&' WS? additiveExpr
    ;

expr
    : concatExpr
    ;

NEWLINE : '\n' | '\r' | '\r\n';
WS : [' '\t]+;

OPEN_BRACKET : '[';
CLOSE_BRACKET : ']';

STRING_CONSTANT : '"' (~["])* '"';
PAREN : '"';

INTEGER_CONSTANT : MINUS? DIGIT+;
MINUS : '-';

FLOATING_POINT_CONSTANT : MINUS? DIGIT+ (DECIMAL_POINT DIGIT+)?;
DECIMAL_POINT : '.';

ASSIGN : '=';

OPEN_PAREN : '(';
CLOSE_PAREN : ')';

OP_EXPONENT : '^';
OP_TIMES : '*';
OP_DIVIDE : '/';
OP_INTDIVIDE : '\\';
OP_MODULO : M O D;
OP_PLUS : '+';

MSGBOX : M S G B O X;

DIM : D I M;
AS : A S;
COMMA : ',';

FOR : F O R;
TO : T O;
STEP : S T E P;
NEXT : N E X T;
EXIT : E X I T;

TYPE_SINGLE : S I N G L E;
TYPE_INTEGER : I N T E G E R;
TYPE_DECIMAL : D E C I M A L;
TYPE_STRING : S T R I N G;

ID : NONDIGIT (NONDIGIT | DIGIT)*;
NONDIGIT : [a-zA-Z_];
DIGIT : [0-9];

fragment A : 'a' | 'A';
fragment B : 'b' | 'B';
fragment C : 'c' | 'C';
fragment D : 'd' | 'D';
fragment E : 'e' | 'E';
fragment F : 'f' | 'F';
fragment G : 'g' | 'G';
fragment H : 'h' | 'H';
fragment I : 'i' | 'I';
fragment J : 'j' | 'J';
fragment K : 'k' | 'K';
fragment L : 'l' | 'L';
fragment M : 'm' | 'M';
fragment N : 'n' | 'N';
fragment O : 'o' | 'O';
fragment P : 'p' | 'P';
fragment Q : 'q' | 'Q';
fragment R : 'r' | 'R';
fragment S : 's' | 'S';
fragment T : 't' | 'T';
fragment U : 'u' | 'U';
fragment V : 'v' | 'V';
fragment W : 'w' | 'W';
fragment X : 'x' | 'X';
fragment Y : 'y' | 'Y';
fragment Z : 'z' | 'Z';

