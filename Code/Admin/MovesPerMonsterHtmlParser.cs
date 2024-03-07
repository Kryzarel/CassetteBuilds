using System;
using System.Collections.Generic;

namespace Cassette_Builds.Code.Admin
{
	public static class MovesPerMonsterHtmlParser
	{
		public static void Parse(ReadOnlySpan<char> html, string move, List<MoveMonsterPair> moveMonsterPairs)
		{
			StringComparison cmp = StringComparison.OrdinalIgnoreCase;
			html = html[html.IndexOf("id=\"Compatible_Species\"", cmp)..];
			int index = html.IndexOf("<table", cmp);
			if (index < 0) return; // Some moves are not usable by any mosters (derp)

			ReadOnlySpan<char> table = html[index..html.IndexOf("</table>", cmp)];
			ReadOnlySpan<char> tableContent = table.NextRow(out _); // Skip header row
			ParseTable(tableContent, move, moveMonsterPairs);
		}

		private static void ParseTable(ReadOnlySpan<char> table, string move, List<MoveMonsterPair> moveMonsterPairs)
		{
			while (!table.IsEmpty)
			{
				table = table.NextRow(out ReadOnlySpan<char> row);
				if (row.IsEmpty) break;

				// Image
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
				}

				// Name
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					ReadOnlySpan<char> name = col.GetBetween("title=\"", "\"");
					moveMonsterPairs.Add(new MoveMonsterPair(move, new string(name)));
				}
			}
		}
	}
}