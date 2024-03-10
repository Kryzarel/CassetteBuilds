using System;
using System.Runtime.CompilerServices;

namespace Cassette_Builds.Code.Database
{
	public static class ArrayMapper2D
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Get<T>(this in ReadOnlySpan<T> span, int width, int x, int y)
		{
			return span[GetIndex(width, x, y)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Set<T>(this in Span<T> span, int width, int x, int y, T value)
		{
			span[GetIndex(width, x, y)] = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Set<T>(this T[] array, int width, int x, int y, T value)
		{
			array[GetIndex(width, x, y)] = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T RefGet<T>(this T[] array, int width, int x, int y)
		{
			return ref array[GetIndex(width, x, y)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetIndex(int width, int x, int y)
		{
			return y * width + x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (int, int) GetIndex(int width, int i)
		{
			return (i / width, i % width);
		}
	}
}