namespace Cassette_Builds.Code.Admin
{
	public struct Monster
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
		public string WikiLink;
		public string ImageLink;

		public override readonly string ToString()
		{
			return $"{Number} | {Name} | {Type} | {HP} | {MeleeAttack} | {MeleeDefense} | {RangedAttack} | {RangedDefense} | {Speed} | {WikiLink} | {ImageLink}";
		}
	}
}