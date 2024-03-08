using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Kryz.Collections
{
	public ref struct ReadOnlyNonAllocBuffer<T>
	{
		private T[]? array;
		private ReadOnlySpan<T> span;
		private readonly ArrayPool<T>? arrayPool;

		public readonly int Count;

		public readonly ReadOnlySpan<T> Span
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => span;
		}

		public readonly T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => span[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyNonAllocBuffer(ReadOnlySpan<T> span, T[]? array = null, ArrayPool<T>? arrayPool = null)
		{
			this.array = array;
			this.span = span;
			this.arrayPool = arrayPool ?? ArrayPool<T>.Shared;
			Count = span.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			if (array != null && arrayPool != null)
			{
				arrayPool.Return(array);
				array = null;
				span = default;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ReadOnlySpan<T>.Enumerator GetEnumerator() => span.GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly T[] ToArray() => span.ToArray();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ReadOnlySpan<T>(ReadOnlyNonAllocBuffer<T> value) => value.span;
	}
}