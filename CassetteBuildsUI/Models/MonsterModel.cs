using CassetteBuilds.Code.Data;

namespace CassetteBuildsUI.Models
{
	public class MonsterModel
	{
		public Monster Monster;

		public int Index => Monster.Index;
		public int Number => Monster.Number;
		public string Name => Monster.Name;
		public string Type => Monster.Type;
		public int HP => Monster.HP;
		public int MeleeAttack => Monster.MeleeAttack;
		public int MeleeDefense => Monster.MeleeDefense;
		public int RangedAttack => Monster.RangedAttack;
		public int RangedDefense => Monster.RangedDefense;
		public int Speed => Monster.Speed;
		public string WikiLink => Monster.WikiLink;
	}
}