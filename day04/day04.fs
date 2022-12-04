( -------------------------------------------------------------------- )
( Day XX of Advent of Code 2022                                        )
( usage: gforth dayXX.fs filename                                      )

( -------------------------------------------------------------------- )
( Constants and Variables                                              )

( Line buffer for reading input file )
256 Constant max-line
Create line-buffer max-line 2 + allot

( for debugging purposes: )
99 maxdepth-.s !

( -------------------------------------------------------------------- )
( Convenience word to read the next line from a file into line-buffer  )

: read-next-line ( fd -- length flag ior )
  line-buffer max-line rot read-line
;

( -------------------------------------------------------------------- )
( Parse a line into pair of number ranges.  Each line follows the      )
( format `A-B,C-D`, where `A`, `B`, `C`, and `D` each consist of 1 or  )
( more digits and represent, respectively, the start and end of the    )
( first range and the start and end of the second range.               )
( NO ERROR CHECKING - assumes the above line format!                   )

: parse-number-range-pair ( length -- a b c d )
( length )
  #0. line-buffer 3 roll >number rot drop
( a buf' len' )
  #0. 3 roll 1 chars + 3 roll 1- >number rot drop
( a b buf' len' )
  #0. 3 roll 1 chars + 3 roll 1- >number rot drop
( a b c buf' len' )
  #0. 3 roll 1 chars + 3 roll 1- >number rot drop
( a b c d buf' len' )
  drop drop
;

( -------------------------------------------------------------------- )
( Read all section lists from the input file.                          )

: read-all-section-lists ( filename-addr filename-len -- a1 b1 c1 d1 .. aN bN cN dN n )
  r/o open-file throw
( fileID )
  0
( fileID 0 )
  begin
( a1 b1 c1 d1 .. aN bN cN dN fileID n )
    over read-next-line throw
( a1 b1 c1 d1 .. aN bN cN dN fileID n linelen !eof? )
  while
( a1 b1 c1 d1 .. aN bN cN dN fileID n linelen )
    parse-number-range-pair
( a1 b1 c1 d1 .. aN bN cN dN fileID n aN+1 bN+1 cN+1 dN+1 )
    5 roll 5 roll 1+
( a1 b1 c1 d1 .. aN bN cN dN aN+1 bN+1 cN+1 dN+1 fileID n+1 )
  repeat
( a1 b1 c1 d1 .. aN bN cN dN fileID n linelen @eof )
  drop swap
( a1 b1 c1 d1 .. aN bN cN dN n fileID )
  close-file throw
( a1 b1 c1 d1 .. aN bN cN dN n )
;

( -------------------------------------------------------------------- )
( Determine if two assignment ranges fully overlap.                    )

: fully-overlapping-assignment-ranges? ( start1 end1 start2 end2 -- f )
( start1 end1 start2 end2 )
  2 pick over <= swap 3 roll <=
( start1 start2 end1<=end2 end2<=end1 )
  3 pick 3 pick <= 3 roll 4 roll <=
( end1<=end2 end2<=end1 start1<=start2 start2<=start1 )
  3 roll and rot rot and or
( [start2<=start1&&end1<=end2]||[end2<=end1&&start1<=start2] )
;

( -------------------------------------------------------------------- )
( Determine if two assignment ranges partially overlap.                )
( given start1, end1, start2, and end2, this means any of these        )
( conditions is true:                                                  )
( * start2 <= start1 <= end2                                           )
( * start2 <= end1 <= end2                                             )
( * start1 <= start2 <= end1                                           )
( * start1 <= end2 <= end1                                             )

: partially-overlapping-assignment-ranges? ( start1 end1 start2 end2 -- f )
( start1 end1 start2 end2 )
  over 4 pick <= over 5 pick >= and
( start1 end1 start2 end2 start2<=start1<=end2? )
  2 pick 4 pick <= 4 pick 3 pick <= and or
( start1 end1 start2 end2 [start2<=start1<=end2?]||[start2<=end1<=end2?] )
  4 pick 3 pick <= 3 roll 4 pick <= and or
( start1 end1 end2 [start2<=start1<=end2?]||[start2<=end1<=end2?]||[start1<=start2<=end1?] )
  3 roll 2 pick <= 2 roll 3 roll <= and or
( [start2<=start1<=end2?]||[start2<=end1<=end2?]||[start1<=start2<=end1?]||[start1<=end2<=end1?] )
;


( -------------------------------------------------------------------- )
( matching-assignment-pairs: count how many assignment pairs return    )
( true from a given word.                                              )

: count-matching-assignment-pairs ( a1 b1 c1 d1 .. aN bN cN dN n XT -- [...] n #pairs )
( a1 b1 c1 d1 .. aN bN cN dN n XT )
  0 2 pick 4 *
( a1 b1 c1 d1 .. aN bN cN dN n XT #pairs offset )
  begin ?dup while
( a1 b1 c1 d1 .. aN bN cN dN n XT #pairs offset )
    dup 4 + dup pick over pick 2 pick pick 3 roll 1- pick
( a1 b1 c1 d1 .. aN bN cN dN n XT #pairs offset a b c d )
    6 pick execute if
( a1 b1 c1 d1 .. aN bN cN dN n XT #pairs offset )
      swap 1+ swap
    then
( a1 b1 c1 d1 .. aN bN cN dN n XT #pairs' offset )
    4 -
( a1 b1 c1 d1 .. aN bN cN dN n XT #pairs' offset' )
  repeat
( a1 b1 c1 d1 .. aN bN cN dN n XT #pairs )
  swap drop
( a1 b1 c1 d1 .. aN bN cN dN n #pairs )
;

( -------------------------------------------------------------------- )
( day04part1: Count how many assignment pairs fully overlap.           )

: day04part1 ( a1 b1 c1 d1 .. aN bN cN dN n -- [...] n #pairs )
  ['] fully-overlapping-assignment-ranges?
  count-matching-assignment-pairs
;

( -------------------------------------------------------------------- )
( day04part2: Count how many assignment pairs partially overlap.       )

: day04part2 ( a1 b1 c1 d1 .. aN bN cN dN n -- [...] n #pairs )
  ['] partially-overlapping-assignment-ranges?
  count-matching-assignment-pairs
;


( -------------------------------------------------------------------- )

next-arg read-all-section-lists

day04part1 ." There are " . ." pair(s) of fully overlapping section ranges." cr
day04part2 ." There are " . ." pair(s) of partially overlapping section ranges." cr

bye
