using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WSharp.Runtime.Tests")]

// TODO: I need to take all the compilation "stuff" and move it into a WSharp.Compiler project.
// WSharp.Runtime will only contain things related to IExecutionEngineActions
// This way, I can build an EXE that uses the engine and also have my evaluator
// use the runtime as well. This gives me a lot of flexibility.
// This basically means WSharp.Playground gets renamed to WSharp.Interpreter