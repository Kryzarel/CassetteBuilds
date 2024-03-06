using System;
using System.Collections.Generic;

namespace Cassette_Builds
{
	public static class MovesPerMonsterHtmlParser
	{
		public static void Parse(string htmlStr, string move, List<MoveMonsterPair> moveMonsterPairs)
		{
			StringComparison cmp = StringComparison.OrdinalIgnoreCase;
			ReadOnlySpan<char> html = htmlStr;
			html = html[html.IndexOf("id=\"Compatible_Species\"", cmp)..];
			int index = html.IndexOf("<table", cmp);
			if (index < 0) return;

			ReadOnlySpan<char> table = html[index..html.IndexOf("</table>", cmp)];
			ReadOnlySpan<char> remaining = table.NextRow(out _); // Skip header row
			Parse(remaining, move, moveMonsterPairs);
		}

		private static void Parse(ReadOnlySpan<char> remaining, string move, List<MoveMonsterPair> monsters)
		{
			while (!remaining.IsEmpty)
			{
				remaining = remaining.NextRow(out ReadOnlySpan<char> row);
				if (row.IsEmpty) break;

				// Image
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
				}

				// Name
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					ReadOnlySpan<char> name = col.GetBetween("title=\"", "\"");
					monsters.Add(new MoveMonsterPair(move, new string(name)));
				}
			}
		}
	}
}