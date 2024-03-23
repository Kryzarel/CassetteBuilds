using System;
using System.Collections.Generic;
using System.IO;

namespace CassetteBuilds.Code.Admin
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
			List<(string, string)> monsterNamesAndLinks = new(150);

			// Write header
			if (table.IsEmpty) return monsterNamesAndLinks;
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

				ReadOnlySpan<char> monsterLink;
				// Name & Link
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					monsterLink = col.GetBetween("href=\"", "\"");
					ReadOnlySpan<char> name = col.GetBetween("title=\"", "\"");
					writer.Write(name);
					writer.Write(',');

					monsterNamesAndLinks.Add((name.ToString(), baseUrl.ConcatToString(monsterLink)));
				}

				// Number
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
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
				writer.Write(monsterLink);
				table = table.NextRow(out row);
			}
			return monsterNamesAndLinks;
		}
	}
}