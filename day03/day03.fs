( -------------------------------------------------------------------- )
( Day 03 of Advent of Code 2022                                        )
( usage: gforth day03.fs filename                                      )

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
( Bubble-sort a string of characters into ASCII order                  )

: bubble-sort-string ( addr u -- )
  1
( addr u idx -- )
  begin
    over over >
  while
    2 pick over chars + dup 1 chars - c@ over c@
( addr u idx addr+idx char@idx-1 char@idx )
    2dup > if
      2 pick 1 chars - c! swap c!
( addr u idx )
      dup 1 > if
        1 -
      else
        1 +
      then
( addr u idx' )
    else
( addr u idx addr+idx char@idx-1 char@idx )
      drop drop drop
( addr u idx )
      1 +
( addr u idx' )
    then
  repeat
( addr u idx -- )
  drop drop drop
;

( -------------------------------------------------------------------- )
( Read all rucksacks, returning two strings per pack, each sorted in   )
( ASCII order.  NOTE - assumes each line read has an even length!      )

: read-all-rucksacks ( filenameAddr filenameLength -- rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n )
( filenameAddr filenameLength )
  r/o open-file throw
( fileID )
  0
( fileID 0 )
  begin
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n fileID n )
    over read-next-line throw
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n fileID n linelen !eof? )
  while
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n fileID n linelen )
    dup allocate throw
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n fileID n linelen rucksack-n+1-addr )
    line-buffer over 3 pick cmove
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n fileID n linelen rucksack-n+1-addr )
    swap 2 /
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n fileID n rucksack-n+1-a items-n+1 )
    2dup bubble-sort-string
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n fileID n rucksack-n+1-a items-n+1 )
    2dup chars + swap
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n fileID n rucksack-n+1-a rucksack-n+1-b items-n+1 )
    2dup bubble-sort-string
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n fileID n rucksack-n+1-a rucksack-n+1-b items-n+1 )
    4 roll 4 roll 1 +
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n rucksack-n+1-a rucksack-n+1-b items-n+1 fileID n+1 )
  repeat
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n fileID n linelen )
  drop swap close-file throw
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n )
;

( -------------------------------------------------------------------- )
( Return an item's priority: 1 through 26 for `a` through `z`;         )
( 27 through 52 for `A` through `Z`.                                   )

: item-priority ( item -- priority )
  dup [char] a < if
    [char] A - 27 +
  else
    [char] a - 1 +
  then
;

( -------------------------------------------------------------------- )
( Identify the first duplicate item between the two compartments of a  )
( rucksack.  Stops when the first duplicate item is found.  Returns 0  )
( if no duplicate is found.                                            )

: identify-duplicate-item ( rucksack-a rucksack-b items -- dup-item-or-0 )
  0 0 0
( rucksack-a rucksack-b items dup-item-or-0 idx-a idx-b )
  begin
    over 4 pick < over 5 pick < and 3 pick 0= and
  while
( rucksack-a rucksack-b items dup-item-or-0 idx-a idx-b )
    5 pick 2 pick chars + c@
( rucksack-a rucksack-b items dup-item-or-0 idx-a idx-b char@a )
    5 pick 2 pick chars + c@
( rucksack-a rucksack-b items dup-item-or-0 idx-a idx-b char@a char@b )
    over - ?dup if
( rucksack-a rucksack-b items dup-item-or-0 idx-a idx-b char@a char@b-char@a )
      swap drop 0> if
( @b is > @a )
( rucksack-a rucksack-b items dup-item-or-0 idx-a idx-b )
        swap 1 chars + swap
      else
( @b is < @a )
( rucksack-a rucksack-b items dup-item-or-0 idx-a idx-b )
        1 chars +
      then
( rucksack-a rucksack-b items dup-item-or-0 idx-a' idx-b' )
    else
( rucksack-a rucksack-b items dup-item-or-0 idx-a idx-b char@a&b )
      3 roll drop rot rot
( rucksack-a rucksack-b items char@a&b idx-a idx-b )
    then
( rucksack-a rucksack-b items dup-item-or-0' idx-a' idx-b' )
  repeat
( rucksack-a rucksack-b items dup-item-or-0 idx-a idx-b )
  2drop swap drop swap drop swap drop
( dup-item-or-0 )
;

( -------------------------------------------------------------------- )
( Identify the duplicate item in each rucksack and sum the priorities  )
( of those duplicate items.                                            )

: sum-duplicate-item-priorities ( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n -- [..rucksacks..] sum-of-duplicate-item-priorities )
  0 over 3 *
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n 0 offset )
  begin ?dup while
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum offset )
    dup 3 + dup pick swap dup pick swap 1 - pick
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum offset rucksack-X-a rucksack-X-b items-X )
    identify-duplicate-item item-priority
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum offset duplicate-item-priority )
    rot + swap
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum' offset )
    3 -
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum' offset' )
  repeat
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum )
;

( -------------------------------------------------------------------- )
( Convenience function to add `item` to `line-buffer` at `offset` if   )
( `item` is not equal to `last-item`.  Return the updated `last-item`  )
( and `offset`.                                                        )

: add-unique-item-to-line-buffer ( last-item offset item -- item offset' )
( last-item offset item )
  rot over = if
( offset item )
    swap
( item offset )
  else
( offset item )
    dup 2 pick chars line-buffer + c!
( offset item )
    swap 1+
  then
( item offset' )
;

( -------------------------------------------------------------------- )
( Copy the unique items from a rucksack into `line-buffer` at the      )
( given offset.  Return the new length of `line-buffer`.               )
( Procedure:  Walk two indices through the two pockets of the rucksack )
( At each point, choose the lowest item of the two indices.  if that   )
( item is not the last item added to the linebuffer, then add it.      )
( increment the index chosen, or both indices if the two items were    )
( equal.                                                               )
( when one index is at the end, continue through the remaining index   )
( in the same manner.                                                  )

: copy-unique-items-into-line-buffer ( rucksack-a rucksack-b items offset -- length )
  0 0 0 3 roll
( rucksack-a rucksack-b items idx-a idx-b last-item offset )
  begin
( rucksack-a rucksack-b items idx-a idx-b last-item offset )
    3 pick 5 pick < 3 pick 6 pick < and
  while
( rucksack-a rucksack-b items idx-a idx-b last-item offset )
    6 pick 4 pick chars + c@ 6 pick 4 pick chars + c@
( rucksack-a rucksack-b items idx-a idx-b last-item offset item-a item-b )
    2dup < if
( rucksack-a rucksack-b items idx-a idx-b last-item offset item-a< item-b> )
      drop
( rucksack-a rucksack-b items idx-a idx-b last-item offset item-a )
      add-unique-item-to-line-buffer
( rucksack-a rucksack-b items idx-a idx-b last-item' offset' )
      3 roll 1+ 3 roll 3 roll 3 roll
( rucksack-a rucksack-b items idx-a' idx-b last-item' offset' )
    else
( rucksack-a rucksack-b items idx-a idx-b last-item offset item-a>= item-b<= )
      swap drop
( rucksack-a rucksack-b items idx-a idx-b last-item offset item-b )
      add-unique-item-to-line-buffer
( rucksack-a rucksack-b items idx-a idx-b last-item' offset' )
      rot 1+ rot rot
( rucksack-a rucksack-b items idx-a idx-b' last-item' offset' )
    then
( rucksack-a rucksack-b items idx-a' idx-b' last-item' offset' )
  repeat
( rucksack-a rucksack-b items idx-a idx-b last-item offset )
( -- at this point, either idx-a or idx-b will be equal to items -- )
( -- so it's safe to loop through both indepedently. -- )
( -- start with looping through idx-a -- )
  begin
( rucksack-a rucksack-b items idx-a idx-b last-item offset )
    3 pick 5 pick <
  while
( rucksack-a rucksack-b items idx-a idx-b last-item offset )
    6 pick 4 pick chars + c@
( rucksack-a rucksack-b items idx-a idx-b last-item offset item-a )
    add-unique-item-to-line-buffer
( rucksack-a rucksack-b items idx-a idx-b last-item' offset' )
    3 roll 1 + 3 roll 3 roll 3 roll
( rucksack-a rucksack-b items idx-a' idx-b last-item' offset' )
  repeat
( rucksack-a rucksack-b items idx-a idx-b last-item offset )
( -- and finish with looping through idx-b -- )
  begin
( rucksack-a rucksack-b items idx-a idx-b last-item offset )
    2 pick 5 pick <
  while
( rucksack-a rucksack-b items idx-a idx-b last-item offset )
    5 pick 3 pick chars + c@
( rucksack-a rucksack-b items idx-a idx-b last-item offset item-b )
    add-unique-item-to-line-buffer
( rucksack-a rucksack-b items idx-a idx-b last-item' offset' )
    rot 1 + rot rot
( rucksack-a rucksack-b items idx-a idx-b' last-item' offset' )
  repeat
( rucksack-a rucksack-b items idx-a idx-b last-item offset )
( -- drop the 6 items under the top item on the stack -- )
  swap drop swap drop swap drop swap drop swap drop swap drop
( offset )
;

( -------------------------------------------------------------------- )
( Identify the item that appears three times in `line-buffer` within   )
( the given length.                                                    )
( do this by: first, sort line-buffer.  second, scan [backwards]       )
( through line-buffer looking for anywhere @idx == @idx+2.             )

: identify-linebuffer-triplicate ( length -- triplicate-item )
( length )
  line-buffer over bubble-sort-string
( length )
  3 -
( idx )
  0 swap
( 0 idx )
  begin
( triplicate idx )
    dup 0>=
  while
    line-buffer over chars + c@
    line-buffer 2 pick 2 + chars + c@
( triplicate idx @idx @idx+2 )
    over = if
( triplicate idx @idx )
      swap drop swap drop -1
( triplicate' idx' )
    else
( triplicate idx @idx )
      drop 1-
    then
( triplicate' idx' )
  repeat
( triplicate -1 )
  drop
( triplicate )
;

( -------------------------------------------------------------------- )
( Identify the badges in each group of 3 rucksacks and sum their       )
( priorities.                                                          )
( Do this by recording the *unique* items from each rucksack back into )
( `line-buffer`, then sorting `line-buffer` and identifying which item )
( appears three times.                                                 )
( NOTE - this ASSUMES there are a multiple of 3 rucksacks!             )

: sum-badge-priorities ( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n -- [..rucksacks..] sum-of-badge-priorities )
  0 0 0 3 pick 3 *
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges rucksack-count linebuffer-offset rucksack-offset )
  begin ?dup while
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges rucksack-count linebuffer-offset rucksack-offset )
    dup 5 + dup pick swap dup pick swap 1 - pick
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges rucksack-count linebuffer-offset rucksack-offset rucksack-X-a rucksack-X-b items-X )
    4 roll copy-unique-items-into-line-buffer
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges rucksack-count rucksack-offset linebuffer-offset' )
    rot 1 +
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges rucksack-offset linebuffer-offset' rucksack-count' )
    dup 3 = if
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges rucksack-offset linebuffer-offset' 3 )
      drop
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges rucksack-offset linebuffer-offset' )
      identify-linebuffer-triplicate
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges rucksack-offset badge-item )
      item-priority
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges rucksack-offset badge-item-priority )
      rot +
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n rucksack-offset sum-of-badges' )
      0 0
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n rucksack-offset sum-of-badges' rucksack-count' linebuffer-offset' )
      3 roll
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges' rucksack-count' linebuffer-offset' rucksack-offset )
    else
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges rucksack-offset linebuffer-offset' rucksack-count' )
      swap rot
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges' rucksack-count' linebuffer-offset' rucksack-offset )
    then
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges' rucksack-count' linebuffer-offset' rucksack-offset )
    3 -
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges' rucksack-count' linebuffer-offset' rucksack-offset' )
  repeat
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges rucksack-count=0? linebuffer-offset=0? )
  drop drop
( rucksack-1-a rucksack-1-b items-1 ... rucksack-n-a rucksack-n-b items-n n sum-of-badges )
;

( -------------------------------------------------------------------- )

next-arg read-all-rucksacks
sum-duplicate-item-priorities
." The sum of of the priorities of the duplicate items is " . cr

sum-badge-priorities
." The sum of of the priorities of the badges is " . cr

bye
