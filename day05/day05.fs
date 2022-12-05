( -------------------------------------------------------------------- )
( Day 05 of Advent of Code 2022                                        )
( usage: gforth day05.fs filename                                      )


( DESCRIPTION of input and first part of problem:                      )

( The input is in two parts, separated by a blank line.                )

( The first part is a visual [space-sensitive] layout of up to         )
( `MAX-CRATES` crates as they start out on a supply ship.  The layout  )
( consists of 4-character columns of crates in the form `[X]` where    )
( `X` is a single upper-case letter.  The columns are bottom-aligned,  )
( meaning the the upper rows of the layout may include only spaces for )
( a column, or may not include the column at all.                      )
( The last line of the layout is the row of column numbers, starting   )
( at `1`, space-centered under each stack.                             )

( The second part is a list of move commands, indicating how many      )
( crates will be moved from what column to what column.  Crates are    )
( moved one at a time, so an instruction to "move 4 from 2 to 7" means )
( move the top crate from stack 2 to stack 7, repeating for a total of )
( 4 moves.                                                             )

( The first part of the problem asks you to replicate the moves that   )
( the crane would make and report the crates left at the top of each   )
( stack, in order starting from 1, as a single string.  For example,   )
( if the topmost crates were `[C]`, `[M]`, and `[Z]`, the solution     )
( would be the string `CMZ`.                                           )



( proposed solution for input parsing:                                 )

( 1. Read the layout:                                                  )
(    a. Read the next line.                                            )
(    b. Check the character at line[1].  If it's a digit, goto step e. )
(    c. Starting at position 1 and incrementing by 4 for as long as we )
(       do not exceed the length of the string, record the crate       )
(       letters [if any] at each stack number.  Keep track of the      )
(       total number of crates thus recorded.                          )
(    d. repeat back to step a.                                         )
(    e. at this point, the stack should look like this:                )
(       [ crate1 stk1 .. crateN stkN n -- ]                            )
(    f. find the last number in the input line to determine the total  )
(       number of stacks                                               )
(    g. allocate enough space in a `crates` heap array for one         )
(       character per total number of crates times total number of     )
(       stacks.                                                        )
(    h. allocate enough space in a second `stacks` heap array for one  )
(       word per stack.  Initialize this array with 0's.               )
(    i. loop through the crates on the stack in LIFO order, so that    )
(       the last crate read gets processed first.                      )
(    j. for each crate, record the crate letter in the crates array    )
(       at position stack# * #crates + stacks[stack#], and increment   )
(       stacks[stack#].                                                )

( 2. Read the move schedule:                                           )
(    a. Read the next line.                                            )
(    b. parse the line to find the number at the 2nd, 4th, and 6th     )
(       word.  Record those in that order on the stack, and keep track )
(       of the total number of moves thus recorded.                    )

( proposed solution for first part of problem:                         )

( 3. Identify which crates will be on top of each stack                )
(    a. Create duplicate of crates-array and stacks-array              )
(    b. Process each move                                              )
(    c. Loop through each stack, recording into `line-buffer` the      )
(       crate at the top of each stack.                                )
(    d. Return `line-buffer` and the # of stacks.                      )



( -------------------------------------------------------------------- )
( Constants and Variables                                              )

( there are duplicates among the letters, so just allow lots of crates )
256 Constant MAX-CRATES

( Line buffer for reading input file )
256 Constant MAX-LINE
Create line-buffer MAX-LINE 2 + allot

( for debugging purposes: )
99 maxdepth-.s !

( -------------------------------------------------------------------- )

( Convenience word to read the next line from a file into line-buffer  )

: read-next-line ( fd -- length flag ior )
  line-buffer MAX-LINE rot read-line
;

( Convenience function to dump the crate stacks horizontally )

: dump-stacks ( #stacks crates-array stacks-array -- #stacks crates-array stacks-array )
  0 begin
( #stacks crates-array stacks-array stack-idx )
    dup 4 pick <
  while
( #stacks crates-array stacks-array stack-idx )
    dup 1 + .
( #stacks crates-array stacks-array stack-idx )
    over over cells + @
( #stacks crates-array stacks-array stack-idx crates-in-stack )
    3 pick 2 pick MAX-CRATES * chars + swap type cr
( #stacks crates-array stacks-array stack-idx )
    1 +
( #stacks crates-array stacks-array stack-idx' )
  repeat
( #stacks crates-array stacks-array )
  drop
;

( Convenience function to dump the move commands )

: dump-moves ( #moves moves-array -- #moves moves-array )
( #moves moves-array -- )
  0 begin
( #moves moves-array move-idx -- )
    dup 3 pick <
  while
( #moves moves-array move-idx -- )
    dup 1+ dup .
( #moves moves-array move-idx move-idx' -- )
    swap 3 * cells 2 pick +
( #moves moves-array move-idx' move-ptr -- )
    ." move " dup @ . cell+
    ." from " dup @ . cell+
    ." to " @ . cr
  repeat
( #moves moves-array move-idx-- )
  drop
( #moves moves-array )
;

( -------------------------------------------------------------------- )
( Parse the input file and return the list of moves, the pointer to    )
( the array of crates, and the pointer to the array of stack sizes.    )

( 1. Read the layout:                                                  )
(    a. Read the next line.                                            )
(    b. Check the character at line[1].  If it's a digit, goto step e. )
(    c. Starting at position 1 and incrementing by 4 for as long as we )
(       do not exceed the length of the string, record the crate       )
(       letters [if any] at each stack number.  Keep track of the      )
(       total number of crates thus recorded.                          )
(    d. repeat back to step a.                                         )
(    e. at this point, the stack should look like this:                )
(       [ crate1 stk1 .. crateN stkN n -- ]                            )
(    f. find the last number in the input line to determine the total  )
(       number of stacks                                               )
(    g. allocate enough space in a `crates` heap array for             )
(       `MAX-CRATES` characters per stack.                             )
(    h. allocate enough space in a second `stacks` heap array for one  )
(       word per stack.  Initialize this array with 0's.               )
(    i. loop through the crates on the stack in LIFO order, so that    )
(       the last crate read gets processed first.                      )
(    j. for each crate, record the crate letter in the crates array    )
(       at position stack# * MAX-CRATES + stacks[stack#], and          )
(       increment stacks[stack#].                                      )

: parse-crate-layout ( fd -- #stacks crates-array stacks-array )
  0
( fd 0 -- )
  begin
( crate-1 stack-1 ... crate-N stack-N fd n -- )
    over read-next-line throw 0= throw
( crate-1 stack-1 ... crate-N stack-N fd n linelen -- )
    line-buffer 1 chars + c@ [char] 0 - dup 0 < swap 9 > or
  while
( crate-1 stack-1 ... crate-N stack-N fd n linelen -- )
    1
( crate-1 stack-1 ... crate-N stack-N fd n linelen lineidx -- )
    begin
      2dup >
    while
( crate-1 stack-1 ... crate-N stack-N fd n linelen lineidx -- )
      line-buffer over chars + c@ dup bl <> if
( crate-1 stack-1 ... crate-N stack-N fd n linelen lineidx crate -- )
        over 4 /
( crate-1 stack-1 ... crate-N stack-N fd n linelen lineidx crate stack# -- )
        5 roll 5 roll 1+
( crate-1 stack-1 ... crate-N stack-N linelen lineidx crate-N+1 stack-N+1 fd n+1 -- )
        5 roll 5 roll
( crate-1 stack-1 ... crate-N stack-N crate-N+1 stack-N+1 fd n+1 linelen lineidx -- )
      else
( crate-1 stack-1 ... crate-N stack-N fd n linelen lineidx space -- )
        drop
      then
( crate-1 stack-1 ... crate-N stack-N fd n linelen lineidx -- )
      4 +
( crate-1 stack-1 ... crate-N stack-N fd n linelen lineidx' -- )
    repeat
( crate-1 stack-1 ... crate-N stack-N fd n' linelen lineidx -- )
    drop drop
( crate-1 stack-1 ... crate-N stack-N fd n' -- )
  repeat
( crate-1 stack-1 ... crate-N stack-N fd n linelen -- )
( -- line-buffer contains the line of stack numbers -- )
  dup 1 - dup 4 mod -
( crate-1 stack-1 ... crate-N stack-N fd n linelen last-stack-idx -- )
  begin
    line-buffer over chars + c@ bl =
  while
    1+
  repeat
( crate-1 stack-1 ... crate-N stack-N fd n linelen last-stack-number-idx -- )
  #0. line-buffer 3 pick chars + 4 roll 4 roll - >number drop drop drop
( crate-1 stack-1 ... crate-N stack-N fd n #stacks -- )
  dup MAX-CRATES chars * allocate throw
( crate-1 stack-1 ... crate-N stack-N fd n #stacks crates-array -- )
  over cells allocate throw
( crate-1 stack-1 ... crate-N stack-N fd n #stacks crates-array stacks-array -- )
  dup 3 pick cells 0 fill ( -- fill stacks-array with 0 -- )
( crate-1 stack-1 ... crate-N stack-N fd n #stacks crates-array stacks-array -- )
  3 roll
( crate-1 stack-1 ... crate-N stack-N fd #stacks crates-array stacks-array n -- )
  begin ?dup while
( crate-1 stack-1 ... crate-N stack-N fd #stacks crates-array stacks-array n -- )
    6 roll 6 roll
( crate-1 stack-1 ... crate-N-1 stack-N-1 fd #stacks crates-array stacks-array n crate-N stack-N -- )
    3 pick over cells + @
( crate-1 stack-1 ... crate-N-1 stack-N-1 fd #stacks crates-array stacks-array n crate-N stack-N stack-N-idx -- )
    dup 1 + 5 pick 3 pick cells + ! ( store stack-N-idx + 1 back into stacks-array )
( crate-1 stack-1 ... crate-N-1 stack-N-1 fd #stacks crates-array stacks-array n crate-N stack-N stack-N-idx -- )
    swap MAX-CRATES * + chars 4 pick + c!
( crate-1 stack-1 ... crate-N-1 stack-N-1 fd #stacks crates-array stacks-array n -- )
    1-
( crate-1 stack-1 ... crate-N-1 stack-N-1 fd #stacks crates-array stacks-array n-1 -- )
  repeat
( fd #stacks crates-array stacks-array -- )
  3 roll read-next-line throw drop drop ( -- skip blank line -- )
( #stacks crates-array stacks-array -- )
;


( 2. Read the move schedule:                                           )
(    a. Read the next line.                                            )
(    b. parse the line to find the number at the 2nd, 4th, and 6th     )
(       word.  Record those in that order on the stack, and keep track )
(       of the total number of moves thus recorded.                    )
(    c. allocate an array for the moves and transcribe the moves into  )
(       that array, leaving the # moves and array ptr on the stack.    )

: parse-move-schedule ( fd -- #moves moves-array )
  0
  begin
( count1 src1 dst1 .. countN srcN dstN fd n )
    over read-next-line throw
( count1 src1 dst1 .. countN srcN dstN fd n length not-eof? )
  while
( count1 src1 dst1 .. countN srcN dstN fd n length )
    line-buffer swap
( count1 src1 dst1 .. countN srcN dstN fd n line-buffer length )
( -- skip past the first space -- )
    bl scan 1- swap char+ swap
( count1 src1 dst1 .. countN srcN dstN fd n line-buffer' length' )
( -- parse the # of crates to move -- )
    #0. 3 roll 3 roll >number rot drop
( count1 src1 dst1 .. countN srcN dstN fd n countN+1 line-buffer' length' )
( -- skip past the next space -- )
    1- swap char+ swap bl scan 1- swap char+ swap
( count1 src1 dst1 .. countN srcN dstN fd n countN+1 line-buffer' length' )
( -- parse the starting stack to move from -- )
    #0. 3 roll 3 roll >number rot drop
( count1 src1 dst1 .. countN srcN dstN fd n countN+1 srcN+1 line-buffer' length' )
( -- skip past the next space -- )
    1- swap char+ swap bl scan 1- swap char+ swap
( count1 src1 dst1 .. countN srcN dstN fd n countN+1 srcN+1 line-buffer' length' )
( -- parse the ending stack to move to -- )
    #0. 3 roll 3 roll >number drop drop drop
( count1 src1 dst1 .. countN srcN dstN fd n countN+1 srcN+1 dstN+1 )
    4 roll 4 roll 1+
( count1 src1 dst1 .. countN srcN dstN countN+1 srcN+1 dstN+1 fd n+1 )
  repeat
( count1 src1 dst1 .. countN srcN dstN fd n length )
  drop swap drop
( count1 src1 dst1 .. countN srcN dstN n )
  dup 3 * dup cells allocate throw swap
( count1 src1 dst1 .. countN srcN dstN n moves-array array-idx )
  begin ?dup while
( moveitem1 moveitem2 .. moveitemN*3 n moves-array array-idx )
    1-
( moveitem1 moveitem2 .. moveitemN*3 n moves-array array-idx' )
    over over cells +
( moveitem1 moveitem2 .. moveitemN*3 n moves-array array-idx' array-ptr )
    4 roll swap !
( moveitem1 moveitem2 .. n moves-array array-idx' )
  repeat
( #moves moves-array )
;

( Parse the input file, by opening the file, calling step 1 and 2 above, and closing the file. )

: parse-input-file ( filename length -- count1 src1 dst1 .. countN srcN dstN #stacks crates-array stacks-array n )
  r/o open-file throw
( fd -- )
  dup parse-crate-layout
( fd #stacks crates-array stacks-array -- )
  3 pick parse-move-schedule
( fd #stacks crates-array stacks-array #moves moves-array -- )
  5 roll close-file throw
( #stacks crates-array stacks-array #moves moves-array -- )
  4 roll 4 roll 4 roll
( #moves moves-array #stacks crates-array stacks-array -- )
;

( -------------------------------------------------------------------- )
( Process the moves [for CrateMover 9000]                              )

(       b1. Loop through each move, processing the moves:              )
(           b11. fetch the src stack, dst stack, and #crates from the  )
(                moves-array.                                          )
(           b12. fetch the stack sizes for src and dst stacks from the )
(                stacks-array.                                         )
(           b13. Loop until #crates is 0:                              )
(                b131. decrement the stack size for src and fetch the  )
(                      crate from that position in crates-array.       )
(                b132. store the crate into the dst position in        )
(                      crates-array and increment the stack size for   )
(                      dst.                                            )
(                b133. decrement #crates.                              )
(           b14. store the updated stack sizes for the src and dst     )
(                stacks back into the stacks-array.                    )

: process-moves-9000 ( #moves moves-array #stacks crates-array stacks-array -- )
  0
( #moves moves-array #stacks crates-array stacks-array move-idx -- )
  begin dup 6 pick < while
( #moves moves-array #stacks crates-array stacks-array move-idx -- )
    dup 3 * cells 5 pick +
( #moves moves-array #stacks crates-array stacks-array move-idx move-ptr -- )
    dup @ swap cell+ dup @ swap cell+ @
( #moves moves-array #stacks crates-array stacks-array move-idx crate-count src-stack dst-stack -- )
    4 pick 2 pick 1- cells + @
    5 pick 2 pick 1- cells + @
( #moves moves-array #stacks crates-array stacks-array move-idx crate-count src-stack dst-stack src-stack-size dst-stack-size -- )
    4 roll
( #moves moves-array #stacks crates-array stacks-array move-idx src-stack dst-stack src-stack-size dst-stack-size crate-count -- )
    begin ?dup while
( #moves moves-array #stacks crates-array stacks-array move-idx src-stack dst-stack src-stack-size dst-stack-size crate-count -- )
(                b131. decrement the stack size for src and fetch the  )
(                      crate from that position in crates-array.       )
      rot 1- 4 pick 1- MAX-CRATES * over + chars 8 pick + c@
( #moves moves-array #stacks crates-array stacks-array move-idx src-stack dst-stack dst-stack-size crate-count src-stack-size-1 src-crate -- )
(                b132. store the crate into the dst position in        )
(                      crates-array and increment the stack size for   )
(                      dst.                                            )
      3 roll swap 4 pick 1- MAX-CRATES * 2 pick + chars 9 pick + c! 1+
( #moves moves-array #stacks crates-array stacks-array move-idx src-stack dst-stack crate-count src-stack-size-1 dst-stack-size+1 -- )
(                b133. decrement #crates.                              )
( #moves moves-array #stacks crates-array stacks-array move-idx src-stack dst-stack src-stack-size-1 dst-stack-size+1 crate-count-1 -- )
      rot 1-
    repeat
( #moves moves-array #stacks crates-array stacks-array move-idx src-stack dst-stack src-stack-size dst-stack-size -- )
(           b14. store the updated stack sizes for the src and dst     )
(                stacks back into the stacks-array.                    )
    5 pick 3 roll 1- cells + !
( #moves moves-array #stacks crates-array stacks-array move-idx src-stack src-stack-size -- )
    3 pick rot 1- cells + !
( #moves moves-array #stacks crates-array stacks-array move-idx -- )
    1+
( #moves moves-array #stacks crates-array stacks-array move-idx' -- )
  repeat
( #moves moves-array #stacks crates-array stacks-array move-idx -- )
  drop drop drop drop drop drop
;

( -------------------------------------------------------------------- )
( Process the moves [for CrateMover 9001]                              )

(       b1. Loop through each move, processing the moves:              )
(           b11. fetch the src stack, dst stack, and #crates from the  )
(                moves-array.                                          )
(           b12. fetch the stack sizes for src and dst stacks from the )
(                stacks-array.                                         )
(           b13. decrement src stack size by #crates.                  )
(           b14. calculate position for src and dst stack.             )
(           b15. `move` the crates from src to dst.                    )
(           b16. increment dst stack size by #crates.                  )
(           b17. store the updated stack sizes for the src and dst     )
(                stacks back into the stacks-array.                    )

: process-moves-9001 ( #moves moves-array #stacks crates-array stacks-array -- )
  0
( #moves moves-array #stacks crates-array stacks-array move-idx -- )
  begin dup 6 pick < while
(           b11. fetch the src stack, dst stack, and #crates from the  )
(                moves-array.                                          )
( #moves moves-array #stacks crates-array stacks-array move-idx -- )
    dup 3 * cells 5 pick +
( #moves moves-array #stacks crates-array stacks-array move-idx move-ptr -- )
    dup @ swap cell+ dup @ swap cell+ @
(           b12. fetch the stack sizes for src and dst stacks from the )
(                stacks-array.                                         )
( #moves moves-array #stacks crates-array stacks-array move-idx crate-count src-stack dst-stack -- )
    4 pick 2 pick 1- cells + @
    5 pick 2 pick 1- cells + @
( #moves moves-array #stacks crates-array stacks-array move-idx crate-count src-stack dst-stack src-stack-size dst-stack-size -- )

(           b13. decrement src stack size by #crates.                  )
    swap 4 pick -
( #moves moves-array #stacks crates-array stacks-array move-idx crate-count src-stack dst-stack dst-stack-size src-stack-size' -- )

(           b14. calculate position for src and dst stack.             )
    7 pick 4 pick 1- MAX-CRATES * 2 pick + chars +
( #moves moves-array #stacks crates-array stacks-array move-idx crate-count src-stack dst-stack dst-stack-size src-stack-size' src-ptr -- )
    8 pick 4 pick 1- MAX-CRATES * 4 pick + chars +
( #moves moves-array #stacks crates-array stacks-array move-idx crate-count src-stack dst-stack dst-stack-size src-stack-size' src-ptr dst-ptr -- )

(           b15. `move` the crates from src to dst.                    )
    6 pick chars move
( #moves moves-array #stacks crates-array stacks-array move-idx crate-count src-stack dst-stack dst-stack-size src-stack-size' -- )

(           b16. increment dst stack size by #crates.                  )
    swap 4 pick +
( #moves moves-array #stacks crates-array stacks-array move-idx crate-count src-stack dst-stack src-stack-size' dst-stack-size' -- )

(           b17. store the updated stack sizes for the src and dst     )
(                stacks back into the stacks-array.                    )

( #moves moves-array #stacks crates-array stacks-array move-idx crate-count src-stack dst-stack src-stack-size' dst-stack-size' -- )
    6 pick 3 roll 1- cells + !
( #moves moves-array #stacks crates-array stacks-array move-idx crate-count src-stack src-stack-size' -- )
    4 pick rot 1- cells + !
( #moves moves-array #stacks crates-array stacks-array move-idx crate-count -- )
    drop
( #moves moves-array #stacks crates-array stacks-array move-idx -- )
    1+
( #moves moves-array #stacks crates-array stacks-array move-idx' -- )
  repeat
( #moves moves-array #stacks crates-array stacks-array move-idx -- )
  drop drop drop drop drop drop
;

( -------------------------------------------------------------------- )
( Transcribe the topmost crate of each stack into `line-buffer`.       )
( Return `line-buffer` and # of stacks.                                )

: transcribe-top-crates ( #stacks crates-array stacks-array -- line-buffer #stacks )
( #stacks crates-array stacks-array -- )
  0 begin
    dup 4 pick <
  while
( #stacks crates-array' stacks-array' stack-idx -- )
    over over cells + @
( #stacks crates-array' stacks-array' stack-idx stack-size -- )
    1- over MAX-CRATES * + chars 3 pick + c@
( #stacks crates-array' stacks-array' stack-idx crate -- )
    line-buffer 2 pick chars + c!
( #stacks crates-array' stacks-array' stack-idx -- )
    1+
( #stacks crates-array' stacks-array' stack-idx' -- )
  repeat
( #stacks crates-array' stacks-array' stack-idx -- )
  drop drop drop line-buffer swap
( line-buffer #stacks -- )
;

( -------------------------------------------------------------------- )
( Part 1 solution                                                      )

( 3. Identify which crates will be on top of each stack                )
(    a. Create duplicates of the crates-array and stacks-array.        )
(    b. Process the moves [via a separate function]:                   )
(       b1. Loop through each move, processing the moves:              )
(           b11. fetch the src stack, dst stack, and #crates from the  )
(                moves-array.                                          )
(           b12. fetch the stack sizes for src and dst stacks from the )
(                stacks-array.                                         )
(           b13. Loop until #crates is 0:                              )
(                b131. decrement the stack size for src and fetch the  )
(                      crate from that position in crates-array.       )
(                b132. store the crate into the dst position in        )
(                      crates-array and increment the stack size for   )
(                      dst.                                            )
(                b133. decrement #crates.                              )
(           b14. store the updated stack sizes for the src and dst     )
(                stacks back into the stacks-array.                    )
(    c. Loop through each stack, recording into `line-buffer` the      )
(       crate at the top of each stack.                                )
(    d. Return `line-buffer` and the # of stacks.                      )

: day05part1 ( #moves moves-array #stacks crates-array stacks-array -- addr u )
(    a. Create duplicates of the crates-array and stacks-array.        )
( #moves moves-array #stacks crates-array stacks-array -- )
( -- create duplicate of the crates-array )
  2 pick MAX-CRATES * chars dup allocate throw
( #moves moves-array #stacks crates-array stacks-array crates-array-size crates-array' -- )
  3 roll over 3 roll move
( -- create duplicate of the stacks-array )
  2 pick cells dup allocate throw
( #moves moves-array #stacks stacks-array crates-array' stacks-array-size stacks-array' -- )
  3 roll over 3 roll move
( #moves moves-array #stacks crates-array' stacks-array' -- )
(    b. Process the moves [via a separate function]:                   )
( -- use the process-moves word to process the arrays -- )
  4 roll 4 roll 4 pick 4 pick 4 pick
  process-moves-9000
( #stacks crates-array' stacks-array' -- )
  2 pick 2 pick 2 pick transcribe-top-crates
( #stacks crates-array' stacks-array' line-buffer #stacks -- )
( -- deallocate the duplicated crates-array and stacks-array )
  rot free throw rot free throw
( #stacks line-buffer #stacks -- )
  rot drop
( line-buffer #stacks -- )
;

: day05part2 ( #moves moves-array #stacks crates-array stacks-array -- addr u )
(    a. Create duplicates of the crates-array and stacks-array.        )
( #moves moves-array #stacks crates-array stacks-array -- )
( -- create duplicate of the crates-array )
  2 pick MAX-CRATES * chars dup allocate throw
( #moves moves-array #stacks crates-array stacks-array crates-array-size crates-array' -- )
  3 roll over 3 roll move
( -- create duplicate of the stacks-array )
  2 pick cells dup allocate throw
( #moves moves-array #stacks stacks-array crates-array' stacks-array-size stacks-array' -- )
  3 roll over 3 roll move
( #moves moves-array #stacks crates-array' stacks-array' -- )
(    b. Process the moves [via a separate function]:                   )
( -- use the process-moves word to process the arrays -- )
  4 roll 4 roll 4 pick 4 pick 4 pick
  process-moves-9001
( #stacks crates-array' stacks-array' -- )
  2 pick 2 pick 2 pick transcribe-top-crates
( #stacks crates-array' stacks-array' line-buffer #stacks -- )
( -- deallocate the duplicated crates-array and stacks-array )
  rot free throw rot free throw
( #stacks line-buffer #stacks -- )
  rot drop
( line-buffer #stacks -- )
;




( -------------------------------------------------------------------- )

( Main program )

next-arg parse-input-file

\ ." ( #moves moves-array #stacks crates-array stacks-array -- )" cr .s cr
\ dump-stacks
\ 4 pick 4 pick dump-moves drop drop

4 pick 4 pick 4 pick 4 pick 4 pick day05part1
." part 1: the crates on top of each stack are: " type cr

4 pick 4 pick 4 pick 4 pick 4 pick day05part2
." part 2: the crates on top of each stack are: " type cr

( clean up heap! )
free throw free throw drop free throw drop

bye
