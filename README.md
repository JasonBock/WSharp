# WSharp (pronounced "dub Sharp")
A Whenever compiler written in C#. You can learn more about the language [here](http://www.dangermouse.net/esoteric/whenever.html). Granted, the amount of documentation around the language is ... sparce, so a fair amount of what I've done is open to interpretation. But I've found Whenever to be a fun, weird programming language where any line can execute and yet you can still write programs like "99 bottles of beer on the wall".

If you want to run WSharp code, you can run the `WSharp` project. This will start in interactive mode, so you can type the `#runFile` command (including the .ws file you want to run), and it will run the code.

You can also build executables from WSharp code. Navigate to the `Samples\Playground` directory, and run `dotnet build`. This will (probably) give you an error, saying something like:

```
error : Expected file "obj\Debug\net5.0\refint\Playground.dll" does not exist. [C:\Users\jason\source\repos\WSharp\Samples\Playground\Playground.wsproj]
```

Ignore that :). Just look in the `bin\Debug\net7.0` directory (.NET 7.0 is the default runtime used in `Playground.wsproj`), and you should see `Playground.exe`.

If you want to debug the executable in Visual Studio, "open" the executable by saying `File -> Open -> Project/Solution`, and then select `Playground.exe`. Next, add `Playground.ws` to this project, and then change the `Debugger Type` project setting to `Managed`. If all is well, you should be able to set breakpoints in `Playground.ws` and when you launch the solution under the debugger, the breakpoints will be hit.