using System;
using System.IO;
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
		public string ImagePath { get; }

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

			DisplayNumber = Number < 0 ? "???" : Number.ToString();
			ImagePath = Path.ChangeExtension(Path.Combine(AppContext.BaseDirectory, "Images", Name), ".png");
		}
	}
}