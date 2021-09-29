\ SPDX-License-Identifier: MIT

: ASSERT ( n -- )
	0= IF
		."  FAILED
"
		." <" DEPTH 1 CELLS / . ." >  " .S CR EMIT
		ABORT
	THEN
;

: (DO) R> -ROT SWAP >R >R >R ;
: I R> R> DUP -ROT >R >R ;
: (LOOP)
	R> R> R>	( ret I len )
	SWAP 1+ SWAP    ( ret I+1 len )
	2DUP >=		( ret I+1 len C )
	SWAP >R		( ret I+1 C )
	-ROT		( I+1 ret C )
	>R >R		( C )
;

: DO IMMEDIATE
	' (DO) ,	\ compile (DO)
	HERE @		\ save location on the stack
;

: LOOP IMMEDIATE
	' (LOOP) ,	\ compile (LOOP)
	' 0BRANCH ,	\ compile 0BRANCH
	HERE @ -	\ calculate the offset from the address saved on the stack
	,		\ compile the offset here
	' RDROP DUP , , \ drop the loop control block
;

: SELFTEST ( N xN..x0 yN..y0 )
	DUP 0 DO
		DUP 1 + PICK	( yN N xN ... )
		ROT		( xN yN N )
		= ASSERT	( N xN+1 ... )
	LOOP
	0 DO
		DROP
	LOOP
;

: T{
	DEPTH
	R> SWAP >R >R
;

: ->
	DEPTH
	R> SWAP >R >R
;

: }T
	DEPTH
	R> R> R> ROT >R
	SWAP DUP ROT
	-
	-ROT -
	DUP ROT
	= ASSERT
	1 CELLS /
	DUP IF
		SELFTEST
	ELSE
		DROP
	THEN
;

CR EMIT

: OK ."  ok
"
;

." Test suite self tests ..."

( Check that a nop matches another nop... and that there is no stack leakage )
DEPTH CELL+
T{ -> }T
DEPTH = ASSERT

DEPTH CELL+
T{ -> }T
T{ 1002 1001 1000 -> 1002 1001 1000 }T
DEPTH = ASSERT

( Anti-tests:
  We can't run these yet but if we need to check that T{ }T correctly
  detect problems then we need to check these report errors. )
\ T{ 1 -> }T
\ T{ -> 1 }T
\ T{ 1 -> 0 }T
\ T{ 1 1 -> 1 0 }T
\ T{ 1 1 1 -> 1 0 1 }T

OK


." Stack primitive tests ..."

T{ 1 2 3 DROP -> 1 2     }T
T{ 1 2 3 SWAP -> 1 3 2   }T
T{ 1 2 3 DUP  -> 1 2 3 3 }T
T{ 1 2 3 OVER -> 1 2 3 2 }T
T{ 1 2 3 ROT  -> 2 3 1   }T
T{ 1 2 3 -ROT -> 3 1 2   }T

T{ 1 2 3 4 2DROP -> 1 2     }T
T{ 1 2 3 4 2SWAP -> 2 1 4 3 }T

T{ 1 2 3 ?DUP -> 1 2 3 3 }T
T{ 1 2 0 ?DUP -> 1 2 0   }T

T{ 1 2 3 1+   -> 1 2 4   }T
T{ 1 2 3 1-   -> 1 2 2   }T

T{ 0 CELL+ CELL- -> 0 }T
\ CELL+ should increment by with 4 (32-bit cells) or 8 (64-bit cells)
T{ 0 CELL+ DUP 8 = SWAP 8 = OR -> TRUE }T

OK

." Arithmetic tests ..."
T{ 1 2 + -> 3 }T
T{ 1 2 - -> -1 }T
T{ 9 9 * -> 81 }T
T{ 101 5 /MOD -> 1 20 }T
OK

." Comparison tests ..."
\ These are copied from jonesforth/test_comparison.f

T{  1  0 < -> FALSE }T
T{  0  1 < -> TRUE  }T
T{  1 -1 < -> FALSE }T
T{ -1  1 < -> TRUE  }T
T{ -1  0 < -> TRUE  }T
T{  0 -1 < -> FALSE }T

T{  1  0 > -> TRUE  }T
T{  0  1 > -> FALSE }T
T{  1 -1 > -> TRUE  }T
T{ -1  1 > -> FALSE }T
T{ -1  0 > -> FALSE }T
T{  0 -1 > -> TRUE  }T

T{  1  1 <= -> TRUE  }T
T{  0  0 <= -> TRUE  }T
T{ -1 -1 <= -> TRUE  }T
T{  1  0 <= -> FALSE }T
T{  0  1 <= -> TRUE  }T
T{  1 -1 <= -> FALSE }T
T{ -1  1 <= -> TRUE  }T
T{ -1  0 <= -> TRUE  }T
T{  0 -1 <= -> FALSE }T

T{  1  1 >= -> TRUE  }T
T{  0  0 >= -> TRUE  }T
T{ -1 -1 >= -> TRUE  }T
T{  1  0 >= -> TRUE  }T
T{  0  1 >= -> FALSE }T
T{  1 -1 >= -> TRUE  }T
T{ -1  1 >= -> FALSE }T
T{ -1  0 >= -> FALSE }T
T{  0 -1 >= -> TRUE  }T

T{  1  1 = -> TRUE  }T
T{  1  0 = -> FALSE }T
T{  0  0 = -> TRUE  }T
T{  1 -1 = -> FALSE }T
T{ -1 -1 = -> TRUE  }T

T{  1  1 <> -> FALSE }T
T{  1  0 <> -> TRUE  }T
T{  0  0 <> -> FALSE }T
T{  1 -1 <> -> TRUE  }T
T{ -1 -1 <> -> FALSE }T

T{  1 0= -> FALSE }T
T{  0 0= -> TRUE  }T
T{ -1 0= -> FALSE }T

T{  1 0<> -> TRUE  }T
T{  0 0<> -> FALSE }T
T{ -1 0<> -> TRUE  }T

T{  1 0< -> FALSE }T
T{  0 0< -> FALSE }T
T{ -1 0< -> TRUE  }T

T{  1 0> -> TRUE  }T
T{  0 0> -> FALSE }T
T{ -1 0> -> FALSE }T

T{  1 0<= -> FALSE }T
T{  0 0<= -> TRUE  }T
T{ -1 0<= -> TRUE  }T

T{  1 0>= -> TRUE  }T
T{  0 0>= -> TRUE  }T
T{ -1 0>= -> FALSE }T

OK


." Logic operations ..."
T{ 31 15 AND -> 15 }T
T{ 29 15 AND -> 13 }T
T{ 31 15 OR  -> 31 }T
T{ 29 15 OR  -> 31 }T
T{ 31 15 XOR -> 16 }T
T{ 29 15 XOR -> 18 }T
T{ -1 INVERT ->  0 }T
T{ -2 INVERT ->  1 }T
OK

." Memory accessors ..."
T{ 17 DSP@                   @ -> 17 17    }T
T{ 20 21 DSP@ CELL+          @ -> 20 21 20 }T
T{ 17 DSP@ 21 SWAP           ! -> 21       }T
T{ 20 21 DSP@ CELL+ 17 SWAP  ! -> 17 21    }T
T{ 17 DSP@ 3 SWAP           +! -> 20       }T
T{ 17 DSP@ 3 SWAP           -! -> 14       }T

\ Only works on little-endian platforms
T{ 260 DSP@                 C@ -> 260 4    }T
T{ 260 DSP@ 1+              C@ -> 260 1    }T

\ Only works on little-endian platforms
T{ 260 DSP@ 8 SWAP          C! -> 264      }T
T{ 260 DSP@ 1+ 2 SWAP       C! -> 516      }T

T{ 6 9 42 DSP@ DUP CELL+ 1 CELLS CMOVE -> 6 42 42 }T
T{ 6 9 42 0 DSP@ DUP CELL+ SWAP 3 CELLS CMOVE -> 6 6 9 42 }T
OK

." Built-in variables ..."
HERE @ \ Capture HERE before we define VARTEST:AT
: VARTEST:AT IMMEDIATE @ ;

T{ STATE @ -> 0 }T
T{ STATE ] VARTEST:AT [ -> 1 }T

T{ DUP HERE @ < -> TRUE }T

\ Check that the new LATEST is equal to the old HERE
T{ DUP LATEST @ = -> TRUE }T

\ This is a fairly weak check (it just checks that DSP points to a lower address than S0)
T{ S0 @ DSP@ > -> TRUE }T

T{ BASE @ -> 10 }T
T{ 20 16 BASE ! 20 A BASE ! -> 20 32 }T
T{ BASE @ -> 10 }T

DROP

OK

." Constant loading ..."
T{ R0 RSP@ > -> TRUE }T
T{ VERSION -> 47 }T
\ DOCOL gives the same value as extracting the codeword from a builtin
T{ DOCOL -> S" QUIT" FIND >CFA @ }T
T{ F_IMMED 0<> -> TRUE }T
T{ F_IMMED DUP 1- AND -> 0 }T
T{ F_HIDDEN 0<> -> TRUE }T
T{ F_HIDDEN DUP 1- AND -> 0 }T
T{ F_LENMASK 0<> -> TRUE }T
OK

." Return stack manipulation ..."
T{ 10 >R R> -> 10 }T
T{ 10 >R RSP@ @ R> -> 10 10 }T
T{ 10 >R 100 RSP@ ! R> -> 100 }T
T{ RSP@ DUP RSP! RSP@ = -> TRUE }T
\ Temporarily use HERE as the RSP so we can prove the store worked
T{ RSP@ HERE @ CELL+ RSP! 42 >R RSP! HERE @ @ -> 42 }T
T{ 2 >R 1 >R RDROP R> -> 2 }T
OK

." Data stack load/store ..."
T{ 1000 1001 DSP@ @ -> 1000 1001 1001 }T
T{ 1000 1001 DSP@ CELL+ DSP! -> 1000 }T
OK

." Input handling ..."
T{ KEY A -> 65 }T
T{ KEY A1000 -> 65 1000 }T
T{ WORD ABC SWAP DROP -> 3 }T
: WORDTEST WORD SWAP ROT + C@ ;
T{ 0 WORDTEST ABC -> 3 65 }T
T{ 2 WORDTEST ABC -> 3 67 }T
T{ S" 0" NUMBER -> 0 0 }T
T{ S" 10" NUMBER -> 10 0 }T
T{ S" -10" NUMBER -> -10 0 }T
T{ S" 20" 16 BASE ! NUMBER A BASE ! -> 32 0 }T
T{ S" 2A" 16 BASE ! NUMBER A BASE ! -> 42 0 }T
T{ S" 2147483647" NUMBER -> 2147483647 0 }T
T{ S" -2147483648" NUMBER -> -2147483648 0 }T
T{ S" 10A" NUMBER -> 10 1 }T
OK

." Dictionary lookups ..."
T{ S" +" FIND 0<> -> TRUE }T
T{ S" QUIT" FIND 0<> -> TRUE }T
T{ S" NONSENSE" FIND -> 0 }T
\ Compare codewords of primitive vs. builtin vs. normal
T{ S" +"    FIND >CFA @ DOCOL = -> FALSE }T
T{ S" QUIT" FIND >CFA @ DOCOL = -> TRUE }T
T{ S" T{"   FIND >CFA @ DOCOL = -> TRUE }T
T{ S" +" FIND >DFA -> S" +" FIND >CFA CELL+ }T
OK

(
  There are no tests for the following words. All are necessary for the
  test suite to operate and have already been tested (albeit without
  explicit error reporting when it breaks):

  :
  CREATE
  ,
  ;
  IMMEDIATE
  '
  BRANCH
  0BRANCH
  LIT
  LITSTRING
  INTERPRET

  In addition we already tests LATEST, [ and ] in earlier variable
  tests.
)

: HIDEME FALSE ;
: HIDEME TRUE ;
S" HIDEME" FIND
T{ HIDEME -> TRUE }T
HIDE HIDEME
T{ HIDEME -> FALSE }T
HIDDEN
T{ HIDEME -> TRUE }T

." Odds and ends ..."
T{ CHAR A -> 65 }T
T{ CHAR 9 CHAR 0 - -> 9 0 - }T
T{ 9 9 S" *" FIND >CFA EXECUTE -> 81 }T
OK

." Decompiler tools ..."
T{ S" HIDE" FIND >CFA CFA> -> S" HIDE" FIND  }T
T{ S" HIDE" FIND >DFA DFA> -> S" HIDE" FIND  }T
OK

." File read/write ..."
S" doesnotexist" MODE_R OPEN-FILE 0= ASSERT

VARIABLE OUTPUT_FILE
S" filetest.txt" MODE_W OPEN-FILE DUP OUTPUT_FILE ! 0<> ASSERT
S" CHUNK#1 " OUTPUT_FILE @ WRITE-FILE 0= ASSERT
S" CHUNK#2 " OUTPUT_FILE @ WRITE-FILE 0= ASSERT
S" CHUNK#3 " OUTPUT_FILE @ WRITE-FILE 0= ASSERT
OUTPUT_FILE @ CLOSE-FILE 0= ASSERT
FORGET OUTPUT_FILE

VARIABLE OUTPUT_FILE
S" filetest.txt" MODE_A OPEN-FILE DUP OUTPUT_FILE ! 0<> ASSERT
S" CHUNK#4 " OUTPUT_FILE @ WRITE-FILE 0= ASSERT
OUTPUT_FILE @ CLOSE-FILE 0= ASSERT
FORGET OUTPUT_FILE

VARIABLE INPUT_FILE
S" filetest.txt" MODE_R OPEN-FILE DUP INPUT_FILE ! 0<> ASSERT
HERE @ 8 INPUT_FILE @ READ-FILE 0= ASSERT
HERE @ 6 + C@ CHAR 1 = ASSERT
HERE @ 8 INPUT_FILE @ READ-FILE 0= ASSERT
HERE @ 6 + C@ CHAR 2 = ASSERT
HERE @ 8 INPUT_FILE @ READ-FILE 0= ASSERT
HERE @ 6 + C@ CHAR 3 = ASSERT
HERE @ 8 INPUT_FILE @ READ-FILE 0= ASSERT
HERE @ 6 + C@ CHAR 4 = ASSERT
INPUT_FILE @ CLOSE-FILE 0= ASSERT
FORGET INPUT_FILE

OK

." Memory allocation ..."
32 MALLOC DUP 0<> ASSERT
\ This is a crass "does it crash test"
DUP 31 + 31 SWAP C!
FREE
OK

\ Final test... let's make sure execution stops after we say BYE
DEPTH 0= ASSERT
BYE
0 ASSERT

\ These tests require eyeball/diff testing
\ WORDS
\ SEE TRUE
\ SEE WORDS
\ SEE HIDE
\ SEE >DFA
\ SEE +
\ SEE QUIT

\ HERE @ .
\ LATEST @ .
\ : DISPOSABLE ROT ROT ROT ;
\ WORDS
\ LATEST @ HERE !
\ LATEST @ @ LATEST !
\ HERE @ .
\ LATEST @ .
