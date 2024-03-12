using System;
using System.Runtime.CompilerServices;

namespace Kryz.Collections
{
	public static class SpanSplitter
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TrySplit<T>(this ReadOnlySpan<T> span, out ReadOnlySpan<T> section, out ReadOnlySpan<T> remaining, out ReadOnlySpan<T> separator, ReadOnlySpan<T> separators, ReadOnlySpan<(int, int)> ranges = default, bool skipEmpty = false) where T : IEquatable<T>
		{
			remaining = span;
		Start:
			(int index, int size) = remaining.IndexOfSeparator(separators, ranges);
			if (index >= 0)
			{
				section = remaining[..index];
				separator = remaining.Slice(index, size);
				remaining = remaining[(index + size)..];
				if (section.IsEmpty && skipEmpty) goto Start;
				return true;
			}
			section = default;
			separator = default;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (int, int) IndexOfSeparator<T>(this in ReadOnlySpan<T> span, in ReadOnlySpan<T> separators, in ReadOnlySpan<(int, int)> ranges) where T : IEquatable<T>
		{
			int index = span.IndexOfAny(separators);
			if (index >= 0)
			{
				if (ranges.IsEmpty) return (index, 1);

				foreach ((int, int) item in ranges)
				{
					if (span.Length >= item.Item2 && span.Slice(index, item.Item2).SequenceEqual(separators.Slice(item.Item1, item.Item2)))
					{
						return (index, item.Item2);
					}
				}
			}
			return (-1, -1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountSeparators<T>(this in ReadOnlySpan<T> span, in ReadOnlySpan<T> separators) where T : IEquatable<T>
		{
			int index;
			int count = 0;
			ReadOnlySpan<T> remaining = span;
			while ((index = remaining.IndexOfAny(separators)) >= 0)
			{
				count++;
				remaining = remaining[(index + 1)..];
			}
			return count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void FindSeparators<T>(this in ReadOnlySpan<T> span, in ReadOnlySpan<T> separators, in Span<int> separatorCache) where T : IEquatable<T>
		{
			int index;
			int count = 0;
			int totalIndex = 0;
			ReadOnlySpan<T> remaining = span;
			while ((index = remaining.IndexOfAny(separators)) >= 0)
			{
				totalIndex += index;
				remaining = remaining[(index + 1)..];
				separatorCache[count++] = totalIndex++;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T> GetSection<T>(this ReadOnlySpan<T> span, int index, ReadOnlySpan<int> separatorCache)
		{
			int start = index > 0 ? separatorCache[index - 1] + 1 : 0;
			int end = index < separatorCache.Length ? separatorCache[index] : span.Length;
			return span[start..end];
		}
	}
}
