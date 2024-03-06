using System;
using System.Collections.Generic;
using System.Linq;

namespace Cassette_Builds
{
	public static class SpeciesHtmlParser
	{
		private struct MutableMonster
		{
			public int Number;
			public string Name;
			public string Type;
			public int HP;
			public int MeleeAttack;
			public int MeleeDefense;
			public int RangedAttack;
			public int RangedDefense;
			public int Speed;
			public string Link;
			public string ImageLink;

			public readonly Monster ToMonster()
			{
				return new Monster(Number, Name, Type, HP, MeleeAttack, MeleeDefense, RangedAttack, RangedDefense, Speed, Link);
			}

			public override readonly string ToString()
			{
				return $"{Number} | {Name} | {Type} | {HP} | {MeleeAttack} | {MeleeDefense} | {RangedAttack} | {RangedDefense} | {Speed} | {Link} | {ImageLink}";
			}
		}

		public static Monster[] Parse(ReadOnlySpan<char> html, string baseUrl, out string[] imageLinks, out string[] imageNames)
		{
			StringComparison cmp = StringComparison.OrdinalIgnoreCase;
			html = html[html.IndexOf("id=\"List_of_species\"", cmp)..];
			int index = html.IndexOf("<table", cmp);

			ReadOnlySpan<char> table = html[index..html.IndexOf("</table>", cmp)];
			ReadOnlySpan<char> tableContent = table.NextRow(out _); // Skip header row
			return ParseTable(tableContent, baseUrl, out imageLinks, out imageNames);
		}

		private static Monster[] ParseTable(ReadOnlySpan<char> table, string baseUrl, out string[] imageLinks, out string[] imageNames)
		{
			List<MutableMonster> monsters = new(150);

			while (!table.IsEmpty)
			{
				table = table.NextRow(out ReadOnlySpan<char> row);
				if (row.IsEmpty) break;

				MutableMonster monster = default;

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
					monster.Link = baseUrl + new string(link);
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

			imageNames = monsters.Select(m => m.Name).ToArray();
			imageLinks = monsters.Select(m => m.ImageLink).ToArray();
			imageLinks[0] = "/images/d/d8/Magikrab.png"; // Manually set magikrab image that is hidden under "spoiler"
			return monsters.Select(m => m.ToMonster()).ToArray();
		}
	}
}