using System;
using System.IO;

namespace CassetteBuilds.Code.Admin
{
	/// <summary>
	/// Parses the table of all moves
	/// </summary>
	public static class MovesHtmlParser
	{
		public static void Parse(in ReadOnlySpan<char> html, in ReadOnlySpan<char> baseUrl, TextWriter writer)
		{
			const StringComparison cmp = StringComparison.OrdinalIgnoreCase;

			ReadOnlySpan<char> table = html[html.IndexOf("id=\"List_of_moves\"", cmp)..];
			table = table[table.IndexOf("<table", cmp)..table.IndexOf("</table>", cmp)];
			ParseTable(table, baseUrl, writer);
		}

		private static void ParseTable(ReadOnlySpan<char> table, in ReadOnlySpan<char> baseUrl, TextWriter writer)
		{
			// Write header
			if (table.IsEmpty) return;
			table = table.NextRow(out ReadOnlySpan<char> headerRow);
			while (!headerRow.IsEmpty)
			{
				headerRow = headerRow.NextHeaderCol(out ReadOnlySpan<char> col);
				writer.Write(col);
				writer.Write(',');
			}
			writer.Write("Wiki Link");

			table = table.NextRow(out ReadOnlySpan<char> row);
			while (!row.IsEmpty)
			{
				writer.WriteLine();

				ReadOnlySpan<char> wikiLink;
				// Name & Link
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					wikiLink = col.GetBetween("href=\"", "\"");
					col = col[col.IndexOf("title=\"")..];
					ReadOnlySpan<char> name = col.GetBetween(">", "</a>");
					writer.Write(name);
					writer.Write(',');
				}

				// Type
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					ReadOnlySpan<char> type = col.GetBetween("title=\"", "\"");
					writer.Write(type);
					writer.Write(',');
				}

				// Category
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					writer.Write(col);
					writer.Write(',');
				}

				// Power
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					_ = int.TryParse(col, out int number);
					writer.Write(number);
					writer.Write(',');
				}

				// Accuracy
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					writer.Write(col);
					writer.Write(',');
				}

				// Cost
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					writer.Write(col);
					writer.Write(',');
				}

				writer.Write(baseUrl);
				writer.Write(wikiLink);
				table = table.NextRow(out row);
			}
		}
	}
}