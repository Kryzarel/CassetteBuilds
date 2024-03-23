using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CassetteBuilds.Code.Admin
{
	/// <summary>
	/// Parses the table of all moves
	/// </summary>
	public static class MovesHtmlParser
	{
		public static List<(string, string)> Parse(in ReadOnlySpan<char> html, in ReadOnlySpan<char> baseUrl, TextWriter writer)
		{
			const StringComparison cmp = StringComparison.OrdinalIgnoreCase;

			ReadOnlySpan<char> table = html[html.IndexOf("id=\"List_of_moves\"", cmp)..];
			table = table[table.IndexOf("<table", cmp)..table.IndexOf("</table>", cmp)];
			return ParseTable(table, baseUrl, writer);
		}

		private static List<(string, string)> ParseTable(ReadOnlySpan<char> table, in ReadOnlySpan<char> baseUrl, TextWriter writer)
		{
			HashSet<(string, string)> typeNamesAndLinks = new(20);

			// Write header
			if (table.IsEmpty) return null!;
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
					ReadOnlySpan<char> link = col.GetBetween("href=\"", "\"");
					ReadOnlySpan<char> type = col.GetBetween("title=\"", "\"");
					writer.Write(type);
					writer.Write(',');

					typeNamesAndLinks.Add((type.ToString(), baseUrl.ConcatToString(link)));
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
			return typeNamesAndLinks.ToList();
		}
	}
}