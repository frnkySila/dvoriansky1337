/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

grammar VisualBasic;

file
    : (stmt NEWLINE)* stmt? EOF
    ;

stmt
    : ( assignmentStmt
      | msgBoxStmt
      | declarationStmt)? WS?
    ;
       
assignmentStmt
    : ID WS? '=' WS? expr
    | arraySubscript WS? '=' WS? expr
    ;
      
arraySubscript
    : ID OPEN_BRACKET INTEGER_CONSTANT CLOSE_BRACKET
    ;

msgBoxStmt
    : MSGBOX WS expr
    ;

declarationStmt
    : DIM WS? ID WS? AS WS? typeName (WS? ',' WS? ID WS? AS WS? typeName)*
    ;

typeName
    : TYPE_SINGLE
    | TYPE_INTEGER
    | TYPE_DECIMAL
    ;

primaryExpr
    : '(' WS? expr WS? ')'
    | arraySubscript
    | ID
    | INTEGER_CONSTANT
    | FLOATING_POINT_CONSTANT
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
    | multiplicativeExpr WS? '*' WS? negationExpr
    | multiplicativeExpr WS? '/' WS? negationExpr
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
    | additiveExpr WS? '+' WS? moduloExpr
    | additiveExpr WS? '-' WS? moduloExpr
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

TYPE_SINGLE : S I N G L E;
TYPE_INTEGER : I N T E G E R;
TYPE_DECIMAL : D E C I M A L;

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

