using System;

namespace CassetteBuilds.Code.Admin
{
	public static class HtmlSpanParser
	{
		public static ReadOnlySpan<char> NextRow(this ReadOnlySpan<char> table, out ReadOnlySpan<char> row)
		{
			return table.Next(out row, "<tr", "</tr>");
		}

		public static ReadOnlySpan<char> NextCol(this ReadOnlySpan<char> row, out ReadOnlySpan<char> col)
		{
			return row.Next(out col, "<td", "</td>");
		}

		public static ReadOnlySpan<char> NextHeaderCol(this ReadOnlySpan<char> row, out ReadOnlySpan<char> col)
		{
			return row.Next(out col, "<th", "</th>");
		}

		public static ReadOnlySpan<char> Next(this ReadOnlySpan<char> span, out ReadOnlySpan<char> contents, ReadOnlySpan<char> start, ReadOnlySpan<char> end)
		{
			contents = span.GetBetween(start, end, includeStart: true, includeEnd: true);
			if (contents.IsEmpty) return default;

			ReadOnlySpan<char> remaining = span[contents.Length..];
			contents = contents[(contents.IndexOf('>') + 1)..(contents.Length - end.Length)];
			return remaining;
		}

		public static ReadOnlySpan<char> GetBetween(this ReadOnlySpan<char> span, ReadOnlySpan<char> start, ReadOnlySpan<char> end, bool includeStart = false, bool includeEnd = false)
		{
			int index = span.IndexOf(start, StringComparison.OrdinalIgnoreCase);
			if (index < 0) return default;
			span = span[(index + (includeStart ? 0 : start.Length))..];

			index = span.IndexOf(end, StringComparison.OrdinalIgnoreCase);
			if (index < 0) return default;
			return span[..(index + (includeEnd ? end.Length : 0))];
		}

		public static string ConcatToString(this in ReadOnlySpan<char> a, in ReadOnlySpan<char> b)
		{
			Span<char> result = stackalloc char[a.Length + b.Length];
			a.CopyTo(result);
			b.CopyTo(result[a.Length..]);
			return new string(result);
		}
	}
}