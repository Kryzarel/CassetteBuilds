using System;
using System.IO;

namespace CassetteBuilds.Code.Admin
{
	/// <summary>
	/// Parses and individual monster html page
	/// </summary>
	public static class MonsterHtmlParser
	{
		public static string Parse(in ReadOnlySpan<char> html, in ReadOnlySpan<char> baseUrl, in ReadOnlySpan<char> monster, TextWriter writer)
		{
			const StringComparison cmp = StringComparison.OrdinalIgnoreCase;

			ReadOnlySpan<char> table = html[html.IndexOf("All Compatible Moves", cmp)..];
			table = table[..table.IndexOf("</table>", cmp)];
			ParseTable(table, monster, writer);

			ReadOnlySpan<char> imageClass = html[html.IndexOf("class=\"infobox-image\"", cmp)..];
			ReadOnlySpan<char> imageLink = imageClass.GetBetween("src=\"", "\"");
			return baseUrl.ConcatToString(imageLink);
		}

		private static void ParseTable(ReadOnlySpan<char> table, in ReadOnlySpan<char> monster, TextWriter writer)
		{
			table = table.NextRow(out _); // Skip header row
			table = table.NextRow(out ReadOnlySpan<char> row);
			while (!row.IsEmpty)
			{
				writer.WriteLine();

				// Sticker
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
				}

				// Move
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					ReadOnlySpan<char> move = col.GetBetween(">", "</a>");
					writer.Write(monster);
					writer.Write(',');
					writer.Write(move);
				}

				table = table.NextRow(out row);
			}
		}
	}
}