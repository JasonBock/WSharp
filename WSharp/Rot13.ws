﻿// Rot13.ws was copied from: https://raw.githubusercontent.com/megahallon/whenever/master/rot13.we
// Note that the original code used "put()", which is actually "print()"
1 2#integer(read()) - N(2);
2 2;
3 defer(exists(1) || N(2) > 90 || N(2) < 65) 5#65-N(5);
4 defer(exists(1) || N(2) > 122 || N(2) < 97) 5#97-N(5);
5 5;
6 defer(exists(3) && exists(4)) print(U(((N(2) - N(5) + 13) % 26) + N(5)));
7 defer(exists(1) || N(2) > 64 || N(2) == 0) print(U(N(2)));
8 again(exists(8)) defer(exists(6) && exists(7)) 1,-6#N(6)-1,-7#N(7)-1,-3#N(3)-1,-4#N(4)-1;
9 defer(exists(1) || N(2) != 0) -3,-4,-5#N(5),-6,-7,-8;
10 defer(exists(1) || exists(9)) print("\n");