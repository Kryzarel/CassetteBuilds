using System;
using System.Runtime.CompilerServices;
using Kryz.Collections;

namespace Kryz.CSV
{
	public readonly ref struct CsvRow
	{
		public readonly int Index;
		public readonly int ColumnCount;
		public readonly ReadOnlySpan<char> Row;
		public readonly ReadOnlySpan<int> SeparatorIndexes;

		public ReadOnlySpan<char> this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Row.GetSection(index, SeparatorIndexes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CsvRow(int index, ReadOnlySpan<char> row, ReadOnlySpan<char> columnSeparators, Span<int> separatorCache)
		{
			Index = index;
			Row = row;
			ColumnCount = separatorCache.Length + 1;
			SeparatorIndexes = separatorCache;
			row.FindSeparators(columnSeparators, separatorCache);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ReadOnlySpan<char> GetColumn(int index)
		{
			return Row.GetSection(index, SeparatorIndexes);
		}
	}
}