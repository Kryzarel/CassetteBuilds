using System;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CassetteBuilds.Code.Logic;

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
		public Bitmap TypeImage { get; }

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

			using Stream stream = AssetLoader.Open(new Uri($"avares://CassetteBuilds/Assets/Images/Monsters/{Name}.png"));
			Image = new Bitmap(stream);

			TypeImage = TypeImageDatabase.GetImage(Type);
		}

		public override string ToString()
		{
			return $"{Index} | {Number} | {Name} | {Type} | {HP} | {MeleeAttack} | {MeleeDefense} | {RangedAttack} | {RangedDefense} | {Speed} | {WikiLink}";
		}

		public static int CompareIndexAsc(Monster? a, Monster? b) => (a?.Index ?? int.MinValue).CompareTo(b?.Index ?? int.MinValue);
		public static int CompareNameAsc(Monster? a, Monster? b) => (a?.Name ?? string.Empty).CompareTo(b?.Name ?? string.Empty);
		public static int CompareTypeAsc(Monster? a, Monster? b) => (a?.Type ?? string.Empty).CompareTo(b?.Type ?? string.Empty);

		public static int CompareIndexDes(Monster? a, Monster? b) => CompareIndexAsc(b, a);
		public static int CompareNameDes(Monster? a, Monster? b) => CompareNameAsc(b, a);
		public static int CompareTypeDes(Monster? a, Monster? b) => CompareTypeAsc(b, a);
	}
}