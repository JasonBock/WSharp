using Mono.Cecil.Cil;
using System;
using System.Linq;
using System.Numerics;
using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Extensions
{
	public static class ILProcessorExtensions
	{
		public static void EmitBox(this ILProcessor self, TypeSymbol? currentStackType)
		{
			if (self is null)
			{
				throw new ArgumentNullException(nameof(self));
			}

			if (currentStackType is { })
			{
				self.Emit(OpCodes.Box, self.Body.Method.Module.ImportReference(
					currentStackType == TypeSymbol.Boolean ? typeof(bool) : typeof(BigInteger)));
			}
		}

		public static void EmitBigInteger(this ILProcessor self, BigInteger value)
		{
			if (self is null)
			{
				throw new ArgumentNullException(nameof(self));
			}

			if (value.IsZero)
			{
				self.Emit(OpCodes.Call,
					self.Body.Method.Module.ImportReference(
						typeof(BigInteger).GetProperties().Single(_ => _.Name == nameof(BigInteger.Zero)).GetGetMethod()));
			}
			else if (value.IsOne)
			{
				self.Emit(OpCodes.Call,
					self.Body.Method.Module.ImportReference(
						typeof(BigInteger).GetProperties().Single(_ => _.Name == nameof(BigInteger.One)).GetGetMethod()));
			}
			else if (value <= new BigInteger(int.MaxValue))
			{
				self.Emit(OpCodes.Ldc_I4, (int)value);
				var ctor = self.Body.Method.Module.ImportReference(
					typeof(BigInteger).GetConstructor(new[] { typeof(int) }));
				self.Emit(OpCodes.Newobj, ctor);
			}
			else
			{
				self.Emit(OpCodes.Newarr, self.Body.Method.Module.ImportReference(typeof(byte)));

				var valueData = value.ToByteArray();

				for (var i = 0; i < valueData.Length; i++)
				{
					self.Emit(OpCodes.Ldc_I4, i);

					if (valueData[i] > sbyte.MaxValue)
					{
						self.Emit(OpCodes.Ldc_I4, (int)valueData[i]);
					}
					else
					{
						self.Emit(OpCodes.Ldc_I4_S, (sbyte)valueData[i]);
					}

					self.Emit(OpCodes.Stelem_I1);
				}

				var ctor = self.Body.Method.Module.ImportReference(
					typeof(BigInteger).GetConstructor(new[] { typeof(byte[]) }));
				self.Emit(OpCodes.Newobj, ctor);
			}
		}
	}
}