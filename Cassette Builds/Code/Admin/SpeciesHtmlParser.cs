using System;
using System.Collections.Generic;
using System.IO;

namespace Cassette_Builds.Code.Admin
{
	/// <summary>
	/// Parses the table of all monsters from (or species, as the wiki calls them)
	/// </summary>
	public static class SpeciesHtmlParser
	{
		public static List<(string, string)> Parse(in ReadOnlySpan<char> html, in ReadOnlySpan<char> baseUrl, TextWriter writer)
		{
			const StringComparison cmp = StringComparison.OrdinalIgnoreCase;

			ReadOnlySpan<char> table = html[html.IndexOf("id=\"List_of_species\"", cmp)..];
			table = table[table.IndexOf("<table", cmp)..table.IndexOf("</table>", cmp)];
			return ParseTable(table, baseUrl, writer);
		}

		private static List<(string, string)> ParseTable(ReadOnlySpan<char> table, in ReadOnlySpan<char> baseUrl, TextWriter writer)
		{
			List<(string, string)> namesAndLinks = new(150);

			// Write header
			if (table.IsEmpty) return namesAndLinks;
			table = table.NextRow(out ReadOnlySpan<char> headerRow);
			headerRow = headerRow.NextHeaderCol(out _); // Skip image column
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

				// Image (skip)
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
				}

				ReadOnlySpan<char> wikiLink;
				// Name & Link
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					wikiLink = col.GetBetween("href=\"", "\"");
					ReadOnlySpan<char> name = col.GetBetween("title=\"", "\"");
					writer.Write(name);
					writer.Write(',');

					namesAndLinks.Add((name.ToString(), baseUrl.ConcatToString(wikiLink)));
				}

				// Number
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					// ReadOnlySpan<char> numberText = col[(col.IndexOf('#') + 1)..];
					// int number = int.TryParse(numberText, out int n) ? n : -1;
					writer.Write(col[(col.IndexOf('#') + 1)..]);
					writer.Write(',');
				}

				// Type
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					ReadOnlySpan<char> type = col.GetBetween("title=\"", "\"");
					writer.Write(type);
					writer.Write(',');
				}

				// HP, MeleeAttack, MeleeDefense, RangedAttack, RangedDefense, Speed
				for (int i = 0; i < 6; i++)
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					writer.Write(col);
					writer.Write(',');
				}

				writer.Write(baseUrl);
				writer.Write(wikiLink);
				table = table.NextRow(out row);
			}
			return namesAndLinks;
		}
	}
}