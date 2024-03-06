using System;
using System.Collections.Generic;

namespace Cassette_Builds.Code.Admin
{
	public static class MovesHtmlParser
	{
		public static Move[] Parse(ReadOnlySpan<char> html, string baseUrl)
		{
			StringComparison cmp = StringComparison.OrdinalIgnoreCase;
			html = html[html.IndexOf("id=\"List_of_moves\"", cmp)..];
			int index = html.IndexOf("<table", cmp);

			ReadOnlySpan<char> table = html[index..html.IndexOf("</table>", cmp)];
			ReadOnlySpan<char> tableContent = table.NextRow(out _); // Skip header row
			return ParseTable(tableContent, baseUrl);
		}

		private static Move[] ParseTable(ReadOnlySpan<char> table, string baseUrl)
		{
			List<Move> moves = new(150);

			while (!table.IsEmpty)
			{
				table = table.NextRow(out ReadOnlySpan<char> row);
				if (row.IsEmpty) break;

				Move move = default;

				// Name & Link
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					ReadOnlySpan<char> link = col.GetBetween("href=\"", "\"");
					ReadOnlySpan<char> name = col.GetBetween("title=\"", "\"");
					if (name.EndsWith(" (move)", StringComparison.OrdinalIgnoreCase))
					{
						name = name[..(name.Length - " (move)".Length)];
					}
					move.Link = baseUrl + new string(link);
					move.Name = new string(name);
				}

				// Type
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					ReadOnlySpan<char> type = col.GetBetween("title=\"", "\"");
					move.Type = new string(type);
				}

				// Category
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					move.Category = new string(col);
				}

				// Power
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					_ = int.TryParse(col, out move.Power);
				}

				// Accuracy
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					int index = col.IndexOf('%');
					if (index < 0 || !int.TryParse(col[..index], out move.Accuracy))
					{
						move.Accuracy = -1;
					}
				}

				// Cost
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					int index = col.IndexOf("AP");
					if (index < 0 || !int.TryParse(col[..index], out move.Cost))
					{
						move.Cost = -1;
					}
				}

				moves.Add(move);
			}
			return moves.ToArray();
		}
	}
}