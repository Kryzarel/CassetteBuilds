namespace Cassette_Builds
{
	public readonly struct Monster
	{
		public readonly int Number;
		public readonly string Name;
		public readonly string Type;
		public readonly int HP;
		public readonly int MeleeAttack;
		public readonly int MeleeDefense;
		public readonly int RangedAttack;
		public readonly int RangedDefense;
		public readonly int Speed;
		public readonly string Link;

		public Monster(int number, string name, string type, int hp, int meleeAttack, int meleeDefense, int rangedAttack, int rangedDefense, int speed, string link)
		{
			Number = number;
			Name = name;
			Type = type;
			HP = hp;
			MeleeAttack = meleeAttack;
			MeleeDefense = meleeDefense;
			RangedAttack = rangedAttack;
			RangedDefense = rangedDefense;
			Speed = speed;
			Link = link;
		}
	}
}