( -------------------------------------------------------------------- )
( Day 02 of Advent of Code 2022                                        )
( usage: gforth day02.fs filename                                      )

( -------------------------------------------------------------------- )
( Constants and Variables                                              )

( Line buffer for reading input file )
256 Constant max-line
Create line-buffer max-line 2 + allot

( -------------------------------------------------------------------- )
( Convenience word to read the next line from a file into line-buffer  )

: read-next-line ( fd -- length flag ior )
  line-buffer max-line rot read-line
;

( -------------------------------------------------------------------- )
( Calculate the score for a round based on your opponent's choice and  )
( your choice, where 1 = rock, 2 = paper, and 3 = scissors.            )
( The score is calculated by adding your numeric choice to the outcome )
( of the round, where 0 = you lose, 3 = a draw, 6 = you win.           )
( The actual calculation is:                                           )
( [ [ 4 + your-choice - opponent-choice ] mod 3 ] * 3 + your-choice    )

: round-score ( opponent-choice your-choice -- score )
  swap over 4 + swap - 3 mod 3 * +
;

( -------------------------------------------------------------------- )
( Parse a strategy guide line from line-buffer                         )
( Return the opponent's choice as a number - 1 for rock, 2 for paper,  )
( 3 for scissors; your choice as a similar number; and your score for  )
( the round.                                                           )

( WARNING - currently assumes the input line consists of exactly one   )
( upper case A, B, or C for the opponent, followed by a single space,  )
( followed by an upper case X, Y, or Z for your move.                  ) 

: parse-strategy-line ( -- opponentChoice yourChoice yourScore )
  line-buffer c@ [char] A - 1 +
( opponentChoice )
  line-buffer 2 chars + c@ [char] X - 1 +
( opponentChoice yourChoice )
  2dup round-score
( opponentChoice yourChoice score )
;

( -------------------------------------------------------------------- )
( Parse the entire strategy guide and return choices and score for     )
( each line, followed by the number of lines.                          )

: parse-strategy-guide ( filenameAddr filenameLength -- opponentChoice1 yourChoice1 yourScore1 .. onC ycN ysN n )
( filenameAddr filenameLength )
  r/o open-file throw
( fileID )
  0
( fileID 0 )
  begin
( oc1 yc1 ys1 .. ocN ycN ysN fileID n )
    over read-next-line throw
( oc1 yc1 ys1 .. ocN ycN ysN fileID n linelen !eof? )
  while
( oc1 yc1 ys1 .. ocN ycN ysN fileID n linelen )
    drop parse-strategy-line
( oc1 yc1 ys1 .. ocN ycN ysN fileID n opponentChoiceN+1 yourChoiceN+1 yourScoreN+1 )
    4 roll 4 roll 1 +
( oc1 yc1 ys1 .. ocN ycN ysN opponentChoiceN+1 yourChoiceN+1 yourScoreN+1 fileID n+1 )
  repeat
( oc1 yc1 ys1 .. ocN ycN ysN fileID n linelen )
  drop swap close-file throw
( oc1 yc1 ys1 .. ocN ycN ysN n )
;

( -------------------------------------------------------------------- )
( day02part1 - given a number of rounds and the oc/yc/ys values for    )
( each round, return your total score, without otherwise affecting     )
( the stack.                                                           )

: day02part1 ( oc1 yc1 ys1 .. ocN ycN ysN n -- [...] n totalScore )
  0 over
( [...] n accumScore idx )
  begin
( [...] n accumScore idx )
    ?dup 0>
  while
( [...] n accumScore idx )
    dup 3 * pick rot + swap 1-
( [...] n accumScore idx'  )
  repeat
( [...] n totalScore )
;


( -------------------------------------------------------------------- )
( day02part2 - given a number of rounds and the original oc/yc/ys      )
( values for each round, recalculate each round based on `yc` actually )
( determining the result of the round and return the total score,      )
( without otherwise affecting the stack.                               )

( [ [ 4 + your-choice - opponent-choice ] mod 3 ] * 3 + your-choice    )

: day02part2 ( oc1 yc1 ys1 .. ocN ycN ysN n -- [...] n totalScore )
  0 over
( [...] n accumScore idx )
  begin
( [...] n accumScore idx )
    ?dup 0>
  while
( [...] n accumScore idx )
    dup 1+ 3 * dup pick swap 1- pick
( [...] n accumScore idx opponent-choice-X your-result-X )
    over + 3 mod 1+
( [...] n accumScore idx opponent-choice-X your-choice-X )
    round-score
( [...] n accumScore idx score-X )
   rot + swap
( [...] n accumScore' idx )
    1-
( [...] n accumScore' idx'  )
  repeat
( [...] n totalScore )
;

( -------------------------------------------------------------------- )

next-arg parse-strategy-guide
day02part1
." Your total score for part 1 is " . cr
day02part2
." Your total score for part 2 is " . cr

bye
