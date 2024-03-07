using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Kryz.Collections
{
	public ref struct NonAllocBuffer<T>
	{
		private int count;
		private T[]? array;
		private Span<T> span;
		private ArrayPool<T> arrayPool;

		public readonly Span<T> Span
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => span;
		}

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => count;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				if ((uint)value > (uint)span.Length)
					throw new IndexOutOfRangeException();
				count = value;
			}
		}

		public readonly int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => span.Length;
		}

		public T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => span[index];
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => span[index] = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public NonAllocBuffer(Span<T> span, int count = 0, ArrayPool<T> pool = null!)
		{
			this.count = count;
			array = null;
			this.span = span;
			arrayPool = pool ?? ArrayPool<T>.Shared;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public NonAllocBuffer(int minCapacity, int count = 0, ArrayPool<T> pool = null!)
		{
			this.count = count;
			arrayPool = pool ?? ArrayPool<T>.Shared;
			span = array = arrayPool.Rent(minCapacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T item)
		{
			EnsureCapacity(count + 1);
			span[count++] = item;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddRange(in ReadOnlySpan<T> values)
		{
			EnsureCapacity(count + values.Length);
			values.CopyTo(span[count..]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			count = 0;
			span.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacity(int capacity)
		{
			if (capacity > span.Length)
			{
				capacity = Math.Max(capacity, span.Length * 2);

				T[]? oldArray = array;
				Span<T> oldSpan = span[..count];
				arrayPool ??= ArrayPool<T>.Shared;
				span = array = arrayPool.Rent(capacity);
				oldSpan.CopyTo(span);

				if (oldArray != null)
				{
					arrayPool.Return(oldArray);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			if (array != null)
			{
				arrayPool.Return(array);
				array = null;
				span = default;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Span<T>.Enumerator GetEnumerator() => span[..count].GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Span<T> GetFilledSpan() => span[..count];
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly T[] ToArray() => span[..count].ToArray();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ReadOnlyNonAllocBuffer<T> AsReadOnly() => new(span[..count], array, arrayPool);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator NonAllocBuffer<T>(Span<T> value) => new(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Span<T>(NonAllocBuffer<T> value) => value.span;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ReadOnlySpan<T>(NonAllocBuffer<T> value) => value.span[..value.count];
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ReadOnlyNonAllocBuffer<T>(NonAllocBuffer<T> value) => value.AsReadOnly();
	}
}