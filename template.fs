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

