using System;
using System.Collections.Generic;
using System.Linq;

namespace Cassette_Builds
{
	public static class MovesHtmlParser
	{
		private struct MutableMove
		{
			public string Name;
			public string Type;
			public string Category;
			public int Power;
			public int Accuracy;
			public int Cost;
			public string Link;

			public readonly Move ToMove()
			{
				return new Move(Name, Type, Category, Power, Accuracy, Cost, Link);
			}

			public override readonly string ToString()
			{
				return $"{Name} | {Type} | {Category} | {Power} | {Accuracy} | {Cost} | {Link}";
			}
		}

		public static Move[] Parse(string htmlStr, string baseUrl)
		{
			StringComparison cmp = StringComparison.OrdinalIgnoreCase;
			ReadOnlySpan<char> html = htmlStr;
			ReadOnlySpan<char> table = html[html.IndexOf("<table", cmp)..html.IndexOf("</table>", cmp)];
			ReadOnlySpan<char> remaining = table.NextRow(out _); // Skip header row
			return Parse(remaining, baseUrl);
		}

		private static Move[] Parse(ReadOnlySpan<char> remaining, string baseUrl)
		{
			List<MutableMove> moves = new(150);

			while (!remaining.IsEmpty)
			{
				remaining = remaining.NextRow(out ReadOnlySpan<char> row);
				if (row.IsEmpty) break;

				MutableMove move = default;

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
			return moves.Select(m => m.ToMove()).ToArray();
		}
	}
}