// Hello world
/*
1 print("Hello world from W#!");
*/

// String concatenation
1 print("2" + "3" + "5");

// 99 bottles of beer
/*
1 defer(E(4) || N(1) < N(2) || N(2) < N(3)) print(string(N(1)) + " bottles of beer on the wall, " + string(N(1)) + " bottles of beer,");
2 defer(E(4) || N(1) == N(2)) print("Take one down and pass it around,");
3 defer(E(4) || N(2) == N(3)) print(string(N(1)) + " bottles of beer on the wall.");
4 1#98,2#98,3#98;
*/

// Fibonacci sequence
/*
1 again(E(1)) defer(E(3) || N(1) <= N(2) || N(7) > 99) 2#N(1),3,7;
2 again(E(2)) defer(E(3) || N(2) <= N(1) || N(7) > 99) 1#N(2),3,7;
3 defer(E(5)) print(string(N(1)) + " " + string(N(2)));
4 defer(E(5)) print("1");
5 4,-3,7;
6 defer(E(4)) 3;
7 7;
8 defer(N(7) < 100) -1#N(1),-2#N(2),-7#100,-3;
9 defer(E(3) || E(6)) 1,3;
*/