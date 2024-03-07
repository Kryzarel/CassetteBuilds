using System;
using System.Collections.Generic;

namespace Cassette_Builds.Code.Admin
{
	public static class SpeciesHtmlParser
	{
		public static List<Monster> Parse(ReadOnlySpan<char> html, string baseUrl)
		{
			StringComparison cmp = StringComparison.OrdinalIgnoreCase;
			html = html[html.IndexOf("id=\"List_of_species\"", cmp)..];
			int index = html.IndexOf("<table", cmp);

			ReadOnlySpan<char> table = html[index..html.IndexOf("</table>", cmp)];
			ReadOnlySpan<char> tableContent = table.NextRow(out _); // Skip header row
			return ParseTable(tableContent, baseUrl);
		}

		private static List<Monster> ParseTable(ReadOnlySpan<char> table, string baseUrl)
		{
			List<Monster> monsters = new(150);

			while (!table.IsEmpty)
			{
				table = table.NextRow(out ReadOnlySpan<char> row);
				if (row.IsEmpty) break;

				Monster monster = default;

				// Image Link
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					ReadOnlySpan<char> imageLink = col.GetBetween("src=\"", ".png", includeEnd: true);
					monster.ImageLink = new string(imageLink).Replace("/thumb", "");
				}

				// Name & Link
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					ReadOnlySpan<char> link = col.GetBetween("href=\"", "\"");
					ReadOnlySpan<char> name = col.GetBetween("title=\"", "\"");
					monster.WikiLink = baseUrl + new string(link);
					monster.Name = new string(name);
				}

				// Number
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					ReadOnlySpan<char> number = col[(col.IndexOf('#') + 1)..];
					if (!int.TryParse(number, out monster.Number))
					{
						monster.Number = -1;
					}
				}

				// Type
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					ReadOnlySpan<char> type = col.GetBetween("title=\"", "\"");
					monster.Type = new string(type);
				}

				// HP
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					monster.HP = int.Parse(col);
				}

				// MeleeAttack
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					monster.MeleeAttack = int.Parse(col);
				}

				// MeleeDefense
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					monster.MeleeDefense = int.Parse(col);
				}

				// RangedAttack
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					monster.RangedAttack = int.Parse(col);
				}

				// RangedDefense
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					monster.RangedDefense = int.Parse(col);
				}

				// Speed
				{
					row = row.NextCol(out ReadOnlySpan<char> col);
					monster.Speed = int.Parse(col);
				}

				monsters.Add(monster);
			}
			return monsters;
		}
	}
}