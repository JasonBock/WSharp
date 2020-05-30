using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;
using System;
using System.Numerics;
using WSharp.Compiler.Extensions;
using WSharp.Compiler.Symbols;

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
		public static void EmitWhenProcessorIsNull() =>
			Assert.That(() => (null as ILProcessor)!.EmitBigInteger(BigInteger.Zero), Throws.TypeOf<ArgumentNullException>());

		[Test]
		public static void EmitWhenValueIsZero() =>
			ILProcessorExtensionsTests.UseProcessor(processor =>
			{
				var value = BigInteger.Zero;
				processor.EmitBigInteger(value);

				Assert.Multiple(() =>
				{
					var instructions = processor.Body.Instructions;
					Assert.That(instructions.Count, Is.EqualTo(1));

					Assert.That(instructions[0].OpCode, Is.EqualTo(OpCodes.Call));
					Assert.That(((MemberReference)instructions[0].Operand).FullName,
						Is.EqualTo("System.Numerics.BigInteger System.Numerics.BigInteger::get_Zero()"));
				});
			});

		[Test]
		public static void EmitWhenValueIsOne() =>
			ILProcessorExtensionsTests.UseProcessor(processor =>
			{
				var value = BigInteger.One;
				processor.EmitBigInteger(value);

				Assert.Multiple(() =>
				{
					var instructions = processor.Body.Instructions;
					Assert.That(instructions.Count, Is.EqualTo(1));

					Assert.That(instructions[0].OpCode, Is.EqualTo(OpCodes.Call));
					Assert.That(((MemberReference)instructions[0].Operand).FullName,
						Is.EqualTo("System.Numerics.BigInteger System.Numerics.BigInteger::get_One()"));
				});
			});

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

					for(var i = 0; i < valueData.Length; i++)
					{
						Assert.That(instructions[(3 * i) + 1].OpCode, Is.EqualTo(OpCodes.Ldc_I4));
						Assert.That(instructions[(3 * i) + 1].Operand, Is.EqualTo(i));

						if (valueData[i] > sbyte.MaxValue)
						{
							Assert.That(instructions[(3 * i) + 2].OpCode, Is.EqualTo(OpCodes.Ldc_I4));
							Assert.That(instructions[(3 * i) + 2].Operand, Is.EqualTo((int)valueData[i]));
						}
						else
						{
							Assert.That(instructions[(3 * i) + 2].OpCode, Is.EqualTo(OpCodes.Ldc_I4_S));
							Assert.That(instructions[(3 * i) + 2].Operand, Is.EqualTo((sbyte)valueData[i]));
						}

						Assert.That(instructions[(3 * i) + 3].OpCode, Is.EqualTo(OpCodes.Stelem_I1));
					}

					Assert.That(instructions[^1].OpCode, Is.EqualTo(OpCodes.Newobj));
					Assert.That(((MemberReference)instructions[^1].Operand).FullName,
						Is.EqualTo("System.Void System.Numerics.BigInteger::.ctor(System.Byte[])"));
				});
			});

		[Test]
		public static void EmitBoxWhenProcessorIsNull() =>
			Assert.That(() => (null as ILProcessor)!.EmitBox(null), Throws.TypeOf<ArgumentNullException>());

		[Test]
		public static void EmitBoxWhenSymbolIsNull() =>
			ILProcessorExtensionsTests.UseProcessor(processor =>
			{
				processor.EmitBox(null);

				Assert.Multiple(() =>
				{
					var instructions = processor.Body.Instructions;
					Assert.That(instructions.Count, Is.EqualTo(0));
				});
			});

		[Test]
		public static void EmitBoxWhenSymbolIsBoolean() =>
			ILProcessorExtensionsTests.UseProcessor(processor =>
			{
				processor.EmitBox(TypeSymbol.Boolean);

				Assert.Multiple(() =>
				{
					var instructions = processor.Body.Instructions;
					Assert.That(instructions.Count, Is.EqualTo(1));

					Assert.That(instructions[0].OpCode, Is.EqualTo(OpCodes.Box));
					Assert.That(((MemberReference)instructions[0].Operand).FullName,
						Is.EqualTo("System.Boolean"));
				});
			});

		[Test]
		public static void EmitBoxWhenSymbolIsNotBoolean() =>
			ILProcessorExtensionsTests.UseProcessor(processor =>
			{
				processor.EmitBox(TypeSymbol.Integer);

				Assert.Multiple(() =>
				{
					var instructions = processor.Body.Instructions;
					Assert.That(instructions.Count, Is.EqualTo(1));

					Assert.That(instructions[0].OpCode, Is.EqualTo(OpCodes.Box));
					Assert.That(((MemberReference)instructions[0].Operand).FullName,
						Is.EqualTo("System.Numerics.BigInteger"));
				});
			});
	}
}