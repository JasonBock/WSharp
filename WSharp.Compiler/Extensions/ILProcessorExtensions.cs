using Mono.Cecil.Cil;
using System;
using System.Numerics;
using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Extensions
{
	public static class ILProcessorExtensions
	{
		public static void EmitBox(this ILProcessor @this, TypeSymbol? currentStackType)
		{
			if (@this is null)
			{
				throw new ArgumentNullException(nameof(@this));
			}

			if (currentStackType is { })
			{
				@this.Emit(OpCodes.Box, @this.Body.Method.Module.ImportReference(
					currentStackType == TypeSymbol.Boolean ? typeof(bool) : typeof(BigInteger)));
			}
		}

		public static void EmitBigInteger(this ILProcessor @this, BigInteger value)
		{
			if (@this is null)
			{
				throw new ArgumentNullException(nameof(@this));
			}

			if (value <= new BigInteger(int.MaxValue) && value >= new BigInteger(int.MinValue))
			{
				@this.Emit(OpCodes.Ldc_I4, (int)value);
				var ctor = @this.Body.Method.Module.ImportReference(
					typeof(BigInteger).GetConstructor(new[] { typeof(int) }));
				@this.Emit(OpCodes.Newobj, ctor);
			}
			else
			{
				@this.Emit(OpCodes.Newarr, @this.Body.Method.Module.ImportReference(typeof(byte)));

				var valueData = value.ToByteArray();

				for(var i = 0; i < valueData.Length; i++)
				{
					@this.Emit(OpCodes.Ldc_I4, i);

					if(valueData[i] > sbyte.MaxValue)
					{
						@this.Emit(OpCodes.Ldc_I4, (int)valueData[i]);
					}
					else
					{
						@this.Emit(OpCodes.Ldc_I4_S, (sbyte)valueData[i]);
					}
					@this.Emit(OpCodes.Stelem_I1);
				}

				var ctor = @this.Body.Method.Module.ImportReference(
					typeof(BigInteger).GetConstructor(new[] { typeof(byte[]) }));
				@this.Emit(OpCodes.Newobj, ctor);
			}
		}
	}
}