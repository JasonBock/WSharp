using Mono.Cecil.Cil;
using System.Numerics;
using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Extensions
{
	internal static class ILProcessorExtensions
	{
		internal static void EmitBox(this ILProcessor @this, TypeSymbol? currentStackType)
		{
			if(currentStackType is { })
			{
				@this.Emit(OpCodes.Box, @this.Body.Method.Module.ImportReference(
					currentStackType == TypeSymbol.Boolean ? typeof(bool) : typeof(BigInteger)));
			}
		}

		internal static void EmitBigInteger(this ILProcessor @this, BigInteger value)
		{
			if(value <= new BigInteger(int.MaxValue) && value >= new BigInteger(int.MinValue))
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
					@this.Emit(OpCodes.Ldc_I4_S, valueData[i]);
					@this.Emit(OpCodes.Stelem_I1);
				}

				var ctor = @this.Body.Method.Module.ImportReference(
					typeof(BigInteger).GetConstructor(new[] { typeof(byte[]) }));
				@this.Emit(OpCodes.Newobj, ctor);
			}
		}
	}
}
