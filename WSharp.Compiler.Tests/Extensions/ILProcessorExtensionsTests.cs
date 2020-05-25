using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;
using System;
using System.Numerics;
using WSharp.Compiler.Extensions;

namespace WSharp.Compiler.Tests.Extensions
{
	public static class ILProcessorExtensionsTests
	{
		private static void UseProcessor(Action<ILProcessor> processor)
		{
			var assemblyName = Guid.NewGuid().ToString("N");
			var assemblyFileName = $"{assemblyName}.dll";

			using (var assembly = AssemblyDefinition.CreateAssembly(
				new AssemblyNameDefinition(assemblyName, new Version(1, 0, 0, 0)), assemblyFileName,
				ModuleKind.Dll))
			{
				var type = new TypeDefinition(string.Empty, Guid.NewGuid().ToString("N"), TypeAttributes.Public);
				assembly.MainModule.Types.Add(type);

				var method = new MethodDefinition(Guid.NewGuid().ToString("N"),
					MethodAttributes.Public | MethodAttributes.Static,
					type.Module.Assembly.MainModule.ImportReference(typeof(void)));

				type.Methods.Add(method);

				processor(method.Body.GetILProcessor());
			}
		}

		[Test]
		public static void EmitWhenValueIsLessThanIntMaxValue() =>
			ILProcessorExtensionsTests.UseProcessor(processor =>
			{
				var value = new BigInteger(444);
				processor.EmitBigInteger(value);

				Assert.Multiple(() =>
				{
					var instructions = processor.Body.Instructions;
					Assert.That(instructions.Count, Is.EqualTo(2));

					Assert.That(instructions[0].OpCode, Is.EqualTo(OpCodes.Ldc_I4));
					Assert.That(instructions[0].Operand, Is.EqualTo((int)value));

					Assert.That(instructions[^1].OpCode, Is.EqualTo(OpCodes.Newobj));
					Assert.That(((MemberReference)instructions[^1].Operand).FullName,
						Is.EqualTo("System.Void System.Numerics.BigInteger::.ctor(System.Int32)"));
				});
			});

		[Test]
		public static void EmitWhenValueIsGreaterThanIntMaxValue() =>
			ILProcessorExtensionsTests.UseProcessor(processor =>
			{
				var value = BigInteger.Parse("444444444444444");
				var valueData = value.ToByteArray();
				processor.EmitBigInteger(value);

				Assert.Multiple(() =>
				{
					var instructions = processor.Body.Instructions;
					Assert.That(instructions.Count, Is.EqualTo((3 * valueData.Length) + 2));

					Assert.That(instructions[0].OpCode, Is.EqualTo(OpCodes.Newarr));
					Assert.That(((MemberReference)instructions[0].Operand).FullName,
						Is.EqualTo("System.Byte"));

					Assert.That(instructions[^1].OpCode, Is.EqualTo(OpCodes.Newobj));
					Assert.That(((MemberReference)instructions[^1].Operand).FullName,
						Is.EqualTo("System.Void System.Numerics.BigInteger::.ctor(System.Byte[])"));
				});
			});
	}
}