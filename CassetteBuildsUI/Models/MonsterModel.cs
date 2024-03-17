using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;
using CassetteBuilds.Code.Data;

namespace CassetteBuildsUI.Models
{
	public class MonsterModel
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

		public static IComparer NumberComparer { get; }

		static MonsterModel()
		{
			NumberComparer = Comparer<MonsterModel>.Create(NumberComparison);
		}

		public MonsterModel(Monster monster)
		{
			Index = monster.Index;
			Number = monster.Number;
			Name = monster.Name;
			Type = monster.Type;
			HP = monster.HP;
			MeleeAttack = monster.MeleeAttack;
			MeleeDefense = monster.MeleeDefense;
			RangedAttack = monster.RangedAttack;
			RangedDefense = monster.RangedDefense;
			Speed = monster.Speed;
			WikiLink = monster.WikiLink;

			DisplayNumber = Number < 0 ? "#???" : string.Format("#{0:000}", Number);
			string imagePath = Path.ChangeExtension(Path.Combine(AppContext.BaseDirectory, "Images", Name), ".png");
			using FileStream stream = new(imagePath, FileMode.Open);
			Image = new Bitmap(stream);
		}

		public static int NumberComparison(MonsterModel a, MonsterModel b)
		{
			if (a.Number < 0 || b.Number < 0)
			{
				return a.Index.CompareTo(b.Index);
			}
			return a.Number.CompareTo(b.Number);
		}
	}
}