using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace CassetteBuilds.Code.Models
{
	public class Monster
	{
		public int Index { get; }
		public int Number { get; }
		public string Name { get; }
		public string Type { get; }
		public int HP { get; }
		public int MeleeAttack { get; }
		public int MeleeDefense { get; }
		public int RangedAttack { get; }
		public int RangedDefense { get; }
		public int Speed { get; }
		public string WikiLink { get; }

		public string DisplayNumber { get; }
		public Bitmap Image { get; }

		public static Comparison<Monster?> NumberComparisonAsc { get; }
		public static Comparison<Monster?> NumberComparisonDes { get; }

		static Monster()
		{
			NumberComparisonAsc = CompareIndexes;
			NumberComparisonDes = (a, b) => CompareIndexes(b, a);
		}

		public Monster(int index, int number, string name, string type, int hp, int meleeAttack, int meleeDefense, int rangedAttack, int rangedDefense, int speed, string wikiLink)
		{
			Index = index;
			Number = number;
			Name = name;
			Type = type;
			HP = hp;
			MeleeAttack = meleeAttack;
			MeleeDefense = meleeDefense;
			RangedAttack = rangedAttack;
			RangedDefense = rangedDefense;
			Speed = speed;
			WikiLink = wikiLink;

			DisplayNumber = Number < 0 ? "#???" : string.Format("#{0:000}", Number);

			using Stream stream = AssetLoader.Open(new Uri($"avares://CassetteBuilds/Assets/Images/{Name}.png"));
			Image = new Bitmap(stream);
		}

		public override string ToString()
		{
			return $"{Index} | {Number} | {Name} | {Type} | {HP} | {MeleeAttack} | {MeleeDefense} | {RangedAttack} | {RangedDefense} | {Speed} | {WikiLink}";
		}

		public static int CompareIndexes(Monster? a, Monster? b)
		{
			int indexA = a?.Index ?? int.MinValue;
			int indexB = b?.Index ?? int.MinValue;
			return indexA.CompareTo(indexB);
		}
	}
}