\ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \
\ Day 01 of Advent of Code 2022
\ usage: gforth day01.fs filename

\ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \
\ Constants and Variables

\ Line buffer for reading input file
256 Constant max-line
Create line-buffer max-line 2 + allot

\ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \
\ Convenience word to read the next line from a file into `line-buffer`

: read-next-line ( fd -- length flag ior )
  line-buffer max-line rot read-line
;

\ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \
\ Read the elves' packs from the input file
\ Return the number of packs read and each pack's total calorie count

: read-packs ( fd -- calcnt-1 calcnt-2 .. calcnt-n n )
( fd -- )
  0 0
( fd n accum -- )
  begin
( calcnt-1 .. calcnt-n fd n accum -- )
    2 pick read-next-line throw
  while
( calcnt-1 .. calcnt-n fd n accum length -- )
    ?dup if
      #0. rot line-buffer swap >number
( calcnt-1 .. calcnt-n fd n accum calories 0 buf' 0 -- )
      2drop drop +
( calcnt-1 .. calcnt-n fd n accum' -- )
    else
( calcnt-1 .. calcnt-n fd n accum -- )
      rot rot 1 + 0
( calcnt-1 .. calcnt-n calcnt-n+1 fd n+1 -- )
    then
  repeat
( calcnt-1 .. calcnt-n fd n accum ? -- )
  drop rot drop
( calcnt-1 .. calcnt-n n accum -- )
  ?dup if
( calcnt-1 .. calcnt-n n accum>0 -- )
    swap 1 +
  then
;

\ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \
\ Given N packs of calories, return the top three packs, with the
\ largest at the top of the stack.

: find-largest-packs ( pack-1 .. pack-n n -- pack-3rd pack-2nd pack-1st )
  0 0 0
( pack-1 .. pack-n n pack-3rd pack-2nd pack-1st )
  3 roll
( pack-1 .. pack-n pack-3rd pack-2nd pack-1st n )
  begin
    ?dup
  while
    1 - 
( pack-1 .. pack-n pack-3rd pack-2nd pack-1st n-1 )
    4 roll
( pack-1 .. pack-n-1 pack-3rd pack-2nd pack-1st n-1 pack-n )
    dup 5 pick u> if
( pack-1 .. pack-n-1 pack-3rd pack-2nd pack-1st n-1 pack>3rd )
      4 roll drop
( pack-1 .. pack-n-1 pack-2nd pack-1st n-1 pack>3rd )
      dup 4 pick u> if
( pack-1 .. pack-n-1 pack-2nd pack-1st n-1 pack>2nd )
        dup 3 pick u> if
( pack-1 .. pack-n-1 pack-2nd pack-1st n-1 pack>1st )
          swap
( pack-1 .. pack-n-1 pack-2nd pack-1st pack>1st n-1 )
        else
          rot rot
( pack-1 .. pack-n-1 pack-2nd pack>2nd pack-1st n-1 )
        then
      else
( pack-1 .. pack-n-1 pack-2nd pack-1st n-1 pack>3rd )
        3 roll 3 roll 3 roll
( pack-1 .. pack-n-1 pack>3rd pack-2nd pack-1st n-1 )
      then
    else
( pack-1 .. pack-n-1 pack-3rd pack-2nd pack-1st n-1 pack<3rd )
      drop
( pack-1 .. pack-n-1 pack-3rd pack-2nd pack-1st n-1 )
    then
  repeat
( pack-3rd pack-2nd pack-1st )
;

\ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \ \

\ open the first command line argument as a file, read the elves' packs
\ from it, and close it

next-arg r/o open-file throw
( fd )
dup read-packs
( fd pack-1 pack-2 .. pack-n n )
dup 1 + roll close-file throw
( pack-1 pack-2 .. pack-n n )

\ find the largest packs

find-largest-packs
( pack-3rd pack-2nd pack-largest )

\ print the calorie counts for the largest, two largest, and
\ three largest packs

( pack-3rd pack-2nd pack-largest )
." The largest pack contains " dup . ." calories." cr
+
( pack-3rd pack-1st-and-2nd )
." The two largest packs contain " dup . ." calories." cr
+
( pack-1st-and-2nd-and-3rd )
." The three largest packs contain " . ." calories." cr
( -- )

bye
