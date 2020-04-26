1 again(exists(1)) defer(exists(3) || N(1) <= N(2) || N(7) > 99) 2#N(1),3,7;
2 again(exists(2)) defer(exists(3) || N(2) <= N(1) || N(7) > 99) 1#N(2),3,7;
3 defer(exists(5)) print(string(N(1)) + " " + string(N(2)));
4 defer(exists(5)) print("1");
5 4,-3,7;
6 defer(exists(4)) 3;
7 7;
8 defer(N(7) < 100) -1#N(1),-2#N(2),-7#100,-3;
9 defer(exists(3) || exists(6)) 1,3; 